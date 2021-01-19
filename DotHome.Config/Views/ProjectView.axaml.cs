using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DotHome.Config.Tools;
using DotHome.Config.Windows;
using DotHome.ProgrammingModel;
using MessageBox.Avalonia;
using MessageBox.Avalonia.Enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace DotHome.Config.Views
{
    public class ProjectView : UserControl
    {
        private Project Project => (Project)DataContext;

        private TabControl pagesTabControl;

        private void PageClose_Clicked(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Page page = (Page)((IControl)sender).DataContext;
            page.Visible = false;
        }

        private void Page_DoubleTapped(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Page page = (Page)((IControl)sender).DataContext;
            page.Visible = true;
            pagesTabControl.SelectedItem = page;
        }

        private async void NewPage_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            PageWindow pageWindow = new PageWindow(null, Project);
            Page page = await pageWindow.ShowDialog<Page>(ConfigTools.MainWindow);
            if(page != null)
            {
                pagesTabControl.SelectedItem = page;
            }
        }

        private async void EditPage_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Page page = (Page)((IControl)sender).DataContext;
            PageWindow pageWindow = new PageWindow(page, Project);
            await pageWindow.ShowDialog(ConfigTools.MainWindow);
        }

        private async void RemovePage_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Page page = (Page)((IControl)sender).DataContext;
            var result = await MessageBoxManager.GetMessageBoxStandardWindow("Remove Page", $"Do you really want to remove page '{page.Name}'?", ButtonEnum.YesNo, Icon.Warning).ShowDialog(ConfigTools.MainWindow);
            if(result == ButtonResult.Yes)
            {
                Project.Pages.Remove(page);
            }
        }

        public ProjectView()
        {
            this.InitializeComponent();

            pagesTabControl = this.FindControl<TabControl>("pagesTabControl");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
