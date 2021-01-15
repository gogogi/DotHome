using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.Config.Tools
{
    public class ContainerSerializer
    {
        private static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            TypeNameHandling = TypeNameHandling.Auto
        };

        public static string SerializeContainer(BlockContainer container)
        {
            return JsonConvert.SerializeObject(container, jsonSerializerSettings);
        }

        public static BlockContainer DeserializeProject(string text)
        {
            return JsonConvert.DeserializeObject<BlockContainer>(text, jsonSerializerSettings);
        }
    }
}
