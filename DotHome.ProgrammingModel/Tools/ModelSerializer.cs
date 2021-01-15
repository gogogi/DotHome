using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.ProgrammingModel.Tools
{
    public static class ModelSerializer
    {
        private static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            TypeNameHandling = TypeNameHandling.Auto
        };

        public static string SerializeProject(Project project)
        {
            return JsonConvert.SerializeObject(project, jsonSerializerSettings);
        }

        public static Project DeserializeProject(string text)
        {
            return JsonConvert.DeserializeObject<Project>(text, jsonSerializerSettings);
        }
    }
}
