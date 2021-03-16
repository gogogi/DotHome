using DotHome.Definitions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DotHome.ProgrammingModel.Tools
{
    public static class ModelSerializer
    {
        private static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            TypeNameHandling = TypeNameHandling.All,
            Converters = { new BlockJsonConverter(), new DefinitionJsonConverter() }
        };

        public static string Serialize(IProgrammingModelObject obj)
        {
            return JsonConvert.SerializeObject(obj, jsonSerializerSettings);
        }
        public static T Deserialize<T>(string text, DefinitionsContainer definitionsContainer) where T : IProgrammingModelObject
        {
            jsonSerializerSettings.Context = new StreamingContext(StreamingContextStates.All, definitionsContainer);
            var obj = JsonConvert.DeserializeObject<T>(text, jsonSerializerSettings);
            if(obj is Project p)
            {
                p.Definitions = definitionsContainer;
            }
            return obj;
        }
    }
}
