using DotHome.RunningModel;
using DotHome.RunningModel.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
            foreach(string dll in Directory.GetFiles(dllsDirectory, "*.dll"))
            {
                Assembly assembly = Assembly.LoadFile(dll);
                AddBlocksFromAssembly(definitionsContainer, assembly);
            }
            return definitionsContainer;
        }

        private static void AddBlocksFromAssembly(DefinitionsContainer definitionsContainer, Assembly assembly)
        {
            foreach(Type type in assembly.GetTypes())
            {
                if(!type.IsAbstract && type.IsSubclassOf(typeof(ABlock)))
                {
                    BlockDefinition blockDefinition = new BlockDefinition();
                    blockDefinition.Name = type.Name;
                    blockDefinition.Description = type.GetCustomAttribute<DescriptionAttribute>()?.Description;
                    foreach(PropertyInfo propertyInfo in type.GetProperties())
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
                        else if(propertyInfo.GetCustomAttributes<ParameterAttribute>() != null)
                        {
                            ParameterDefinition parameterDefinition = new ParameterDefinition();
                            parameterDefinition.Name = propertyInfo.Name;
                            parameterDefinition.Type = propertyInfo.PropertyType;
                            parameterDefinition.Description = propertyInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;
                            parameterDefinition.DefaultValue = propertyInfo.GetValue(Activator.CreateInstance(type));
                            blockDefinition.Parameters.Add(parameterDefinition);
                        }
                    }
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
