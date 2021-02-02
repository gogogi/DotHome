using DotHome.RunningModel;
using DotHome.RunningModel.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace DotHome.Definitions.Tools
{
    public static class DefinitionsCreator
    {
        public static DefinitionsContainer CreateDefinitions(string dllsDirectory)
        {
            DefinitionsContainer definitionsContainer = new DefinitionsContainer();
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
                if(!type.IsAbstract && type.IsAssignableTo(typeof(ABlock)))
                {
                    BlockDefinition blockDefinition = new BlockDefinition();
                    blockDefinition.Type = type;
                    blockDefinition.Name = type.Name;
                    blockDefinition.Description = type.GetCustomAttribute<DescriptionAttribute>()?.Description;
                    blockDefinition.Color = type.GetCustomAttribute<ColorAttribute>()?.Color ?? Color.SlateGray;
                    Dictionary<ParameterDefinition, Type> parameterDeclaringTypesDictionary = new Dictionary<ParameterDefinition, Type>();
                    Dictionary<ParameterDefinition, int> parameterOriginalOrderDictionary = new Dictionary<ParameterDefinition, int>();
                    int i = 0;  // Counter for original order
                    foreach (PropertyInfo propertyInfo in type.GetProperties())
                    {
                        if(propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Input<>))
                        {
                            InputDefinition inputDefinition = new InputDefinition();
                            inputDefinition.Name = propertyInfo.Name;
                            inputDefinition.Type = propertyInfo.PropertyType.GetGenericArguments()[0];
                            inputDefinition.Description = propertyInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;
                            inputDefinition.Disablable = propertyInfo.GetCustomAttribute<DisablableAttribute>() != null;
                            inputDefinition.DefaultDisabled = propertyInfo.GetCustomAttribute<DisablableAttribute>()?.DefaultDisabled ?? false;
                            blockDefinition.Inputs.Add(inputDefinition);
                        }
                        else if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Output<>))
                        {
                            OutputDefinition outputDefinition = new OutputDefinition();
                            outputDefinition.Name = propertyInfo.Name;
                            outputDefinition.Type = propertyInfo.PropertyType.GetGenericArguments()[0];
                            outputDefinition.Description = propertyInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;
                            outputDefinition.Disablable = propertyInfo.GetCustomAttribute<DisablableAttribute>() != null;
                            outputDefinition.DefaultDisabled = propertyInfo.GetCustomAttribute<DisablableAttribute>()?.DefaultDisabled ?? false;
                            blockDefinition.Outputs.Add(outputDefinition);
                        }
                        else if(propertyInfo.GetCustomAttribute<ParameterAttribute>() != null)
                        {
                            ParameterDefinition parameterDefinition = new ParameterDefinition();
                            parameterDefinition.Name = propertyInfo.Name;
                            parameterDefinition.Type = propertyInfo.PropertyType;
                            parameterDefinition.ShowInBlock = propertyInfo.GetCustomAttribute<ParameterAttribute>().ShowInBlock;
                            parameterDefinition.Description = propertyInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;
                            parameterDefinition.DefaultValue = propertyInfo.GetValue(Activator.CreateInstance(type, Enumerable.Repeat<object>(null, type.GetConstructors().Single().GetParameters().Length).ToArray()));
                            blockDefinition.Parameters.Add(parameterDefinition);
                            parameterDeclaringTypesDictionary[parameterDefinition] = propertyInfo.DeclaringType;
                            parameterOriginalOrderDictionary[parameterDefinition] = i++; ;
                        }
                    }
                    blockDefinition.Parameters.Sort((a, b) => 
                    {
                        if (parameterDeclaringTypesDictionary[a].IsSubclassOf(parameterDeclaringTypesDictionary[b]))
                        {
                            return 1;
                        }
                        else if(parameterDeclaringTypesDictionary[b].IsSubclassOf(parameterDeclaringTypesDictionary[a]))
                        {
                            return -1;
                        }
                        return parameterOriginalOrderDictionary[a].CompareTo(parameterOriginalOrderDictionary[b]);
                    });

                    string categoryName = type.GetCustomAttribute<CategoryAttribute>()?.Category ?? "Default";
                    BlocksCategory blocksCategory = definitionsContainer.BlockCategories.SingleOrDefault(c => c.Name == categoryName);
                    if(blocksCategory == null)
                    {
                        blocksCategory = new BlocksCategory() { Name = categoryName };
                        definitionsContainer.BlockCategories.Add(blocksCategory);
                    }
                    blocksCategory.BlockDefinitions.Add(blockDefinition);
                }
            }
        }
    }
}
