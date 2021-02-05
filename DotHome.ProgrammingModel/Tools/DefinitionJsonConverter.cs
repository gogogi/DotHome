using DotHome.Definitions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DotHome.ProgrammingModel.Tools
{
    public class DefinitionJsonConverter : JsonConverter<ADefinition>
    {
        public override ADefinition ReadJson(JsonReader reader, Type objectType, ADefinition existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            DefinitionsContainer definitions = (DefinitionsContainer)serializer.Context.Context;
            if (objectType == typeof(BlockDefinition))
            {
                return GetBlockDefinitionByFullName(definitions, (string)reader.Value);
            }
            // For now just create dummy IO definitions
            else if (objectType == typeof(InputDefinition))
            {
                return new InputDefinition() { Name = (string)reader.Value };
            }
            else if (objectType == typeof(OutputDefinition))
            {
                return new OutputDefinition() { Name = (string)reader.Value };
            }
            else if (objectType == typeof(ParameterDefinition))
            {
                return new ParameterDefinition() { Name = (string)reader.Value };
            }
            else
            {
                return null;
            }
        }

        public override void WriteJson(JsonWriter writer, ADefinition value, JsonSerializer serializer)
        {
            if (value is BlockDefinition bd) writer.WriteValue(bd.Type.FullName);
            else writer.WriteValue(value.Name);
        }

        public static BlockDefinition GetBlockByName(DefinitionsContainer definitionsContainer, string name)
        {
            foreach (BlocksCategory category in definitionsContainer.BlockCategories)
            {
                foreach (BlockDefinition blockDefinition in category.BlockDefinitions)
                {
                    if (blockDefinition.Name == name)
                    {
                        return blockDefinition;
                    }
                }
            }
            return null;
        }

        public static BlockDefinition GetBlockDefinitionByFullName(DefinitionsContainer definitionsContainer, string name)
        {
            foreach (BlocksCategory category in definitionsContainer.BlockCategories)
            {
                foreach (BlockDefinition blockDefinition in category.BlockDefinitions)
                {
                    if (blockDefinition.Type.FullName == name)
                    {
                        return blockDefinition;
                    }
                }
            }
            return null;
        }

        public static BlockDefinition GetBlockByType(DefinitionsContainer definitionsContainer, Type type)
        {
            foreach (BlocksCategory category in definitionsContainer.BlockCategories)
            {
                foreach (BlockDefinition blockDefinition in category.BlockDefinitions)
                {
                    if (blockDefinition.Type == type)
                    {
                        return blockDefinition;
                    }
                }
            }
            return null;
        }
    }
}
