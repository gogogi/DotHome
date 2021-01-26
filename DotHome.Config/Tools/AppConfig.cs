using Microsoft.Extensions.Configuration;
using RWConfiguration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DotHome.Config.Tools
{
    public static class AppConfig
    {
        public static IConfiguration Configuration { get; }
        static AppConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .Add(new RWConfigurationSource("rwsettings.json"));
                //.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

             Configuration = builder.Build();
        }

        //public static string Get(string key) => configuration[key];

        //public static string Get(params string[] keys)
        //{
        //    IConfiguration c = Configuration;
        //    foreach(string key in keys.SkipLast(1))
        //    {
        //        c = c.GetSection(key);
        //    }
        //    return c[keys.Last()];
        //}
    }
}
