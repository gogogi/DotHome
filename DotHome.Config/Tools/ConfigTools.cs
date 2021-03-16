using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.VisualTree;
using DotHome.Config.Views;
using DotHome.Definitions.Tools;
using DotHome.ProgrammingModel;
using DotHome.RunningModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotHome.Config.Tools
{
    public static class ConfigTools
    {
        public static IEnumerable<DeviceValueType> DeviceValueTypes { get; } = Enum.GetValues(typeof(DeviceValueType)).Cast<DeviceValueType>();
        public static double MinOrDefault<T>(this IEnumerable<T> collection, Func<T, double> selector) => collection.Count() == 0 ? default : collection.Min(selector);
        public static double MaxOrDefault<T>(this IEnumerable<T> collection, Func<T, double> selector) => collection.Count() == 0 ? default : collection.Max(selector);

        public static MainWindow MainWindow => (MainWindow)((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow;

        public static Project NewProject()
        {
            Project project = new Project() { Definitions = DefinitionsCreator.CreateDefinitions(AppConfig.Configuration["AssembliesPath"]) };
            project.Pages.Add(new Page() { Name = "Page1", Width = 1000, Height = 1000 });
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

        public static T ChildOfType<T>(this IControl control) where T : IControl
        {
            IEnumerable<IVisual> children = control.VisualChildren;
            while (true)
            {
                if (children == null || children.Count() == 0) return default;
                foreach (var child in children) if (child is T t) return t;
                children = children.SelectMany(c => c.VisualChildren);
            }
        }
    }
}
