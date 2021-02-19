using DotHome.RunningModel;
using DotHome.RunningModel.Attributes;
using DotHome.RunningModel.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DotHome.Definitions.Tools
{
    public static class DefinitionsCreator
    {
        public static DefinitionsContainer CreateDefinitions(string dllsDirectory)
        {
            DefinitionsContainer definitionsContainer = new DefinitionsContainer();

            List<Assembly> loadedAssemblies = new List<Assembly>();
            Queue<Assembly> queue = new Queue<Assembly>(Enumerable.Repeat(Assembly.GetEntryAssembly(), 1));
            while(queue.TryDequeue(out Assembly a))
            {
                loadedAssemblies.Add(a);
                AddBlocksFromAssembly(definitionsContainer, a);
                foreach (AssemblyName aa in a.GetReferencedAssemblies())
                {
                    Assembly aaa = Assembly.Load(aa);
                    if(!loadedAssemblies.Contains(aaa))
                        queue.Enqueue(Assembly.Load(aa));
                }
            }

            foreach (string dll in Directory.GetFiles(dllsDirectory, "*.dll"))
            {
                Assembly assembly = Assembly.Load(File.ReadAllBytes(dll));
                AddBlocksFromAssembly(definitionsContainer, assembly);
            }
            definitionsContainer.BlockCategories.Sort((a, b) => a.Name.CompareTo(b.Name));
            return definitionsContainer;
        }

        private static void AddBlocksFromAssembly(DefinitionsContainer definitionsContainer, Assembly assembly)
        {
            foreach(Type type in assembly.GetTypes())
            {
                if (!type.IsAbstract
                    && typeof(ABlock).IsAssignableFrom(type)
                    && type.GetConstructors().Count() == 1)
                {
                    if (type.IsGenericType)
                    {
                        if (type.GetGenericArguments().Length == 1)
                        {
                            foreach (Type t in RunningModelTools.SupportedTypes)
                            {
                                AddBlockFromType(definitionsContainer, type.MakeGenericType(t));
                            }
                        }
                    }
                    else
                    {
                        AddBlockFromType(definitionsContainer, type);
                    }
                }
            }
        }

        private static void AddBlockFromType(DefinitionsContainer definitionsContainer, Type type)
        {
            BlockDefinition blockDefinition = GetBlockFromType(type);
            if(blockDefinition != null)
            {
                string categoryName = type.GetCustomAttribute<CategoryAttribute>()?.Category ?? "Default";
                BlocksCategory blocksCategory = definitionsContainer.BlockCategories.SingleOrDefault(c => c.Name == categoryName);
                if (blocksCategory == null)
                {
                    blocksCategory = new BlocksCategory() { Name = categoryName };
                    definitionsContainer.BlockCategories.Add(blocksCategory);
                }
                blocksCategory.BlockDefinitions.Add(blockDefinition);
            }
        }

        public static BlockDefinition GetBlockFromType(Type type)
        {
            BlockDefinition blockDefinition = new BlockDefinition();
            blockDefinition.Type = type;
            if(type.IsGenericType)
            {
                int index = type.Name.IndexOf('`');
                if (index > 0) blockDefinition.Name = type.Name.Remove(index);
                blockDefinition.Name += $"<{type.GetGenericArguments().First().Name}>";
            }
            else blockDefinition.Name = type.Name;
            blockDefinition.Description = type.GetCustomAttribute<DescriptionAttribute>()?.Description;
            blockDefinition.Color = type.GetCustomAttribute<ColorAttribute>()?.Color ?? Color.SlateGray;
            Dictionary<ParameterDefinition, Type> parameterDeclaringTypesDictionary = new Dictionary<ParameterDefinition, Type>();
            Dictionary<ParameterDefinition, int> parameterOriginalOrderDictionary = new Dictionary<ParameterDefinition, int>();
            int i = 0;  // Counter for original order
            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Input<>))
                {
                    InputDefinition inputDefinition = new InputDefinition();
                    inputDefinition.Name = propertyInfo.Name;
                    inputDefinition.Type = propertyInfo.PropertyType.GetGenericArguments().Single();
                    if (!RunningModelTools.SupportedTypes.Contains(inputDefinition.Type)) return null;
                    inputDefinition.Description = propertyInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;
                    inputDefinition.Disablable = propertyInfo.GetCustomAttribute<DisablableAttribute>() != null;
                    inputDefinition.DefaultDisabled = propertyInfo.GetCustomAttribute<DisablableAttribute>()?.DefaultDisabled ?? false;
                    blockDefinition.Inputs.Add(inputDefinition);
                }
                else if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Output<>))
                {
                    OutputDefinition outputDefinition = new OutputDefinition();
                    outputDefinition.Name = propertyInfo.Name;
                    outputDefinition.Type = propertyInfo.PropertyType.GetGenericArguments().Single();
                    if (!RunningModelTools.SupportedTypes.Contains(outputDefinition.Type)) return null;
                    outputDefinition.Description = propertyInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;
                    outputDefinition.Disablable = propertyInfo.GetCustomAttribute<DisablableAttribute>() != null;
                    outputDefinition.DefaultDisabled = propertyInfo.GetCustomAttribute<DisablableAttribute>()?.DefaultDisabled ?? false;
                    blockDefinition.Outputs.Add(outputDefinition);
                }
                else if (propertyInfo.GetCustomAttribute<BlockParameterAttribute>() != null)
                {
                    ParameterDefinition parameterDefinition = new ParameterDefinition();
                    parameterDefinition.PropertyInfo = propertyInfo;
                    parameterDefinition.Name = propertyInfo.Name;
                    parameterDefinition.Type = propertyInfo.PropertyType;
                    parameterDefinition.ShowInBlock = propertyInfo.GetCustomAttribute<BlockParameterAttribute>().ShowInBlock;
                    parameterDefinition.Description = propertyInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;
                    parameterDefinition.DefaultValue = propertyInfo.GetValue(Activator.CreateInstance(type, Enumerable.Repeat<object>(null, type.GetConstructors().Single().GetParameters().Length).ToArray()));
                    parameterDefinition.ValidationAttributes.AddRange(propertyInfo.GetCustomAttributes<ValidationAttribute>(true));
                    blockDefinition.Parameters.Add(parameterDefinition);
                    parameterDeclaringTypesDictionary[parameterDefinition] = propertyInfo.DeclaringType;
                    parameterOriginalOrderDictionary[parameterDefinition] = i++; ;
                }
            }

            blockDefinition.Parameters.Sort((a, b) =>
            {
                if (parameterDeclaringTypesDictionary[a].IsSubclassOf(parameterDeclaringTypesDictionary[b])) return 1;
                else if (parameterDeclaringTypesDictionary[b].IsSubclassOf(parameterDeclaringTypesDictionary[a])) return -1;
                else return parameterOriginalOrderDictionary[a].CompareTo(parameterOriginalOrderDictionary[b]);
            });

            return blockDefinition;
        }
    }
}
