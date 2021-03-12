using DotHome.Definitions;
using DotHome.ProgrammingModel;
using DotHome.ProgrammingModel.Tools;
using DotHome.RunningModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DotHome.Config.Tools
{
    public class BlockContainerSerializer
    {
        private static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            TypeNameHandling = TypeNameHandling.Auto, 
            Converters = { new BlockJsonConverter(), new DefinitionJsonConverter() }
        };

        public static string SerializeContainer(BlockContainer container)
        {
            return JsonConvert.SerializeObject(container, jsonSerializerSettings);
        }

        public static BlockContainer TryDeserializeContainer(string text, Project project)
        {
            try
            {
                jsonSerializerSettings.Context = new StreamingContext(StreamingContextStates.All, project.Definitions);
                var container = JsonConvert.DeserializeObject<BlockContainer>(text, jsonSerializerSettings);

                // Now we need to attach rooms, categories and users to correct instances of our project
                foreach(var block in container.Blocks)
                {
                    foreach(var parameter in block.Parameters)
                    {
                        if(parameter.Definition.Type == typeof(Room) && parameter.Value != null)
                        {
                            parameter.Value = project.Rooms.Single(r => r.Name == ((Room)parameter.Value).Name);
                        }
                        else if (parameter.Definition.Type == typeof(Category) && parameter.Value != null)
                        {
                            parameter.Value = project.Categories.Single(c => c.Name == ((Category)parameter.Value).Name);
                        }
                        else if (parameter.Definition.Type == typeof(List<User>))
                        {
                            parameter.Value = new List<User>(project.Users.Where(u => ((List<User>)parameter.Value).Select(u => u.Name).Contains(u.Name)));
                        }
                    }
                }
                return container;
            }
            catch
            {
                return null;
            }
        }
    }
}
