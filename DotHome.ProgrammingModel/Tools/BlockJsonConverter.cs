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
            List<Input> oldInputs = new List<Input>();
            List<Output> oldOutputs = new List<Output>();
            List<Parameter> oldParameters = new List<Parameter>();
            foreach (Input input in block.Inputs)
            {
                input.Definition = block.Definition.Inputs.SingleOrDefault(id => id.Name == input.Definition.Name);
                if(input.Definition == null)
                {
                    oldInputs.Add(input);
                }
            }

            foreach (Output output in block.Outputs)
            {
                output.Definition = block.Definition.Outputs.SingleOrDefault(od => od.Name == output.Definition.Name);
                if (output.Definition == null)
                {
                    oldOutputs.Add(output);
                }
            }

            foreach (Parameter parameter in block.Parameters)
            {
                parameter.Definition = block.Definition.Parameters.SingleOrDefault(pd => pd.Name == parameter.Definition.Name);
                if (parameter.Definition == null)
                {
                    oldParameters.Add(parameter);
                }
                else
                {
                    parameter.Value = Convert.ChangeType(parameter.Value, parameter.Definition.Type); // Type can be wrong which can cause issues later (Int32 vs Int64...)
                }
            }

            // Remove what disappeared in new version of dll
            foreach (Input i in oldInputs) block.Inputs.Remove(i);
            foreach (Output o in oldOutputs) block.Outputs.Remove(o);
            foreach (Parameter p in oldParameters) block.Parameters.Remove(p);

            // Add what appeared in new version of dll
            foreach (InputDefinition id in block.Definition.Inputs.Where(id2 => !block.Inputs.Any(i => i.Definition == id2)).ToArray()) block.Inputs.Add(new Input(id));
            foreach (OutputDefinition od in block.Definition.Outputs.Where(od2 => !block.Outputs.Any(o => o.Definition == od2)).ToArray()) block.Outputs.Add(new Output(od));
            foreach (ParameterDefinition pd in block.Definition.Parameters.Where(pd2 => !block.Parameters.Any(p => p.Definition == pd2)).ToArray()) block.Parameters.Add(new Parameter(pd));

            return block;
        }

        public override void WriteJson(JsonWriter writer, Block value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
