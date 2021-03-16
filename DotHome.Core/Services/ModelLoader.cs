using DotHome.Definitions;
using DotHome.Definitions.Tools;
using DotHome.ProgrammingModel;
using DotHome.ProgrammingModel.Tools;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DotHome.Core.Services
{
    public class ModelLoader
    {
        private IConfiguration configuration;

        public ModelLoader(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public DefinitionsContainer LoadDefinitions()
        {
            var defs = DefinitionsCreator.CreateDefinitions(configuration["AssembliesPath"]);
            return defs;
        }

        public Project LoadProgrammingModel()
        {
            if (File.Exists(configuration["ProjectPath"]))
            {
                return ModelSerializer.Deserialize<Project>(File.ReadAllText(configuration["ProjectPath"]), LoadDefinitions());
            }
            else
            {
                return new Project();
            }
        }
    }
}