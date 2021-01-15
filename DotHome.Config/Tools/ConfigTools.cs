using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using DotHome.Config.Views;
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

        public static ProjectView NewProjectView()
        {
            ProjectView projectView = new ProjectView();
            projectView.AddPage(new PageView() { Name = "Page1", Width = 1000, Height = 1000 });
            projectView.AddPage(new PageView() { Name = "Page2", Width = 1000, Height = 1000 });
            return projectView;
        }

        public static IControl ParentOfType<T>(this IControl control)
        {
            var parent = control.Parent;
            while(true)
            {
                if (parent == null) return null;
                if (parent is T) return parent;
                parent = parent.Parent;
            }
        }
    }
}
