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

            // Inputs, outputs and parameters now do have only empty definitions with names

            foreach (Input input in block.Inputs)
            {
                input.Definition = block.Definition.Inputs.Single(id => id.Name == input.Definition.Name);
            }

            foreach (Output output in block.Outputs)
            {
                output.Definition = block.Definition.Outputs.Single(od => od.Name == output.Definition.Name);
            }

            foreach (Parameter parameter in block.Parameters)
            {
                parameter.Definition = block.Definition.Parameters.Single(pd => pd.Name == parameter.Definition.Name);
                parameter.Value = Convert.ChangeType(parameter.Value, parameter.Definition.Type); // Type can be wrong which can cause issues later (Int32 vs Int64...)
            }
            return block;
        }

        public override void WriteJson(JsonWriter writer, Block value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
