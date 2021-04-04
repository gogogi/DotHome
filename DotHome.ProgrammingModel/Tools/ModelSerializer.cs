using DotHome.Definitions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DotHome.ProgrammingModel.Tools
{
    /// <summary>
    /// Helper class for serializing and deserializing <see cref="IProgrammingModelObject"/>s using <see cref="Newtonsoft.Json"/>
    /// </summary>
    public static class ModelSerializer
    {
        /// <summary>
        /// Json settings used for serialization/deserialization
        /// </summary>
        private static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            PreserveReferencesHandling = PreserveReferencesHandling.All, // this is the most important thing, it preserves the structure of programming model
            TypeNameHandling = TypeNameHandling.All,
            Converters = { new BlockJsonConverter(), new DefinitionJsonConverter() }
        };

        /// <summary>
        /// Gives json string representing the <paramref name="obj"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize(IProgrammingModelObject obj)
        {
            return JsonConvert.SerializeObject(obj, jsonSerializerSettings);
        }

        /// <summary>
        /// Reads <see cref="IProgrammingModelObject"/> from json <paramref name="text"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <param name="definitionsContainer"></param>
        /// <returns></returns>
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
