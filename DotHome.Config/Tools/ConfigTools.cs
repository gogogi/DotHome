using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using DotHome.Config.Views;
using DotHome.Definitions.Tools;
using DotHome.ProgrammingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotHome.Config.Tools
{
    public static class ConfigTools
    {
        public static double MinOrDefault<T>(this IEnumerable<T> collection, Func<T, double> selector) => collection.Count() == 0 ? default : collection.Min(selector);
        public static double MaxOrDefault<T>(this IEnumerable<T> collection, Func<T, double> selector) => collection.Count() == 0 ? default : collection.Max(selector);

        public static Window MainWindow => ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow;

        public static Project NewProject()
        {
            Project project = new Project() { Definitions = DefinitionsCreator.CreateDefinitions(AppConfig.Configuration["AssembliesPath"]) };
            project.Pages.Add(new Page() { Name = "Page1", Width = 1000, Height = 1000 });
            project.Pages.Add(new Page() { Name = "Page2", Width = 1000, Height = 1000 });

            for(int i = 0; i < 200; i++)
            {
                //project.Pages[0].Blocks.Add(new RefSink() { Reference = "Ahoj", X = 40 * (i % 20), Y = 30 * (i / 20) });
            }

            for (int i = 0; i < 200; i++)
            {
                //project.Pages[0].Blocks.Add(new RefSource() { Reference = "Ahoj", X = 40 * (i % 20), Y = 500 + 30 * (i / 20) });
            }

            return project;
        }

        public static T ParentOfType<T>(this IControl control) where T : IControl
        {
            var parent = control.Parent;
            while(true)
            {
                if (parent == null) return default;
                if (parent is T t) return t;
                parent = parent.Parent;
            }
        }
    }
}
