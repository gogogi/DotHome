using DotHome.Definitions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotHome.ProgrammingModel.Tools
{
    public class BlockJsonConverter : JsonConverter<Block>
    {
        public override bool CanWrite => false;

        public override Block ReadJson(JsonReader reader, Type objectType, Block existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            serializer.Converters.Remove(this);
            Block block = serializer.Deserialize<Block>(reader);
            serializer.Converters.Add(this);
            foreach (InputDefinition inputDefinition in block.Definition.Inputs)
            {
                var input = block.Inputs.SingleOrDefault(i => i.Definition.Name == inputDefinition.Name);
                if (input != null)
                {
                    input.Definition = inputDefinition;
                }
                else
                {
                    //block.Inputs.Add(new Input(inputDe) { Definition = inputDefinition, Disabled = inputDefinition.DefaultDisabled });
                }
            }
            foreach (OutputDefinition outputDefinition in block.Definition.Outputs)
            {
                var output = block.Outputs.SingleOrDefault(o => o.Definition.Name == outputDefinition.Name);
                if (output != null)
                {
                    output.Definition = outputDefinition;
                }
                else
                {
                    //block.Outputs.Add(new Output() { Definition = outputDefinition, Disabled = outputDefinition.DefaultDisabled });
                }
            }
            foreach (ParameterDefinition parameterDefinition in block.Definition.Parameters)
            {
                var parameter = block.Parameters.SingleOrDefault(p => p.Definition.Name == parameterDefinition.Name);
                if (parameter != null)
                {
                    parameter.Definition = parameterDefinition;
                }
                else
                {
                    //block.Parameters.Add(new Parameter() { Definition = parameterDefinition, Value = parameterDefinition.DefaultValue });
                }
            }
            return block;
        }

        public override void WriteJson(JsonWriter writer, Block value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
