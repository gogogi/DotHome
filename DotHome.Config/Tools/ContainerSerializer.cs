using DotHome.Definitions;
using DotHome.ProgrammingModel.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DotHome.Config.Tools
{
    public class ContainerSerializer
    {
        private static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            TypeNameHandling = TypeNameHandling.Auto, Converters = { new BlockJsonConverter(), new DefinitionJsonConverter() }
        };

        public static string SerializeContainer(BlockContainer container)
        {
            return JsonConvert.SerializeObject(container, jsonSerializerSettings);
        }

        public static BlockContainer DeserializeContainer(string text, DefinitionsContainer definitionsContainer)
        {
            jsonSerializerSettings.Context = new StreamingContext(StreamingContextStates.All, definitionsContainer);
            var v = JsonConvert.DeserializeObject<BlockContainer>(text, jsonSerializerSettings);
            return v;
        }
    }
}
