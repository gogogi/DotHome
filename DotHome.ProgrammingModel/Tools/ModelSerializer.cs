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

        public static string SerializeProject(Project project)
        {
            return JsonConvert.SerializeObject(project, jsonSerializerSettings);
        }

        public static Project DeserializeProject(string text, DefinitionsContainer definitionsContainer)
        {
            jsonSerializerSettings.Context = new StreamingContext(StreamingContextStates.All, definitionsContainer);
            var project = JsonConvert.DeserializeObject<Project>(text, jsonSerializerSettings);
            project.Definitions = definitionsContainer;
            return project;
        }
    }
}
