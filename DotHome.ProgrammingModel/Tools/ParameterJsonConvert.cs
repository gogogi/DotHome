using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.ProgrammingModel.Tools
{
    class ParameterJsonConvert : JsonConverter<Parameter>
    {
        public override bool CanWrite => false;
        public override Parameter ReadJson(JsonReader reader, Type objectType, Parameter existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            serializer.Converters.Remove(this);
            Parameter parameter = serializer.Deserialize<Parameter>(reader);
            serializer.Converters.Add(this);
            return parameter;
        }

        public override void WriteJson(JsonWriter writer, Parameter value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
