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
        private TabControl pagesTabControl;
        private TreeViewItem treeViewItemPages;

        public ObservableCollection<PageView> Pages { get; } = new ObservableCollection<PageView>();

        public PageView SelectedPage { get { if (((PageView)pagesTabControl.SelectedItem).Visible) return (PageView)pagesTabControl.SelectedItem; else return null; } }

        public void AddPage(PageView pageView)
        {
            Pages.Add(pageView);
            pageView.Focus();
            pagesTabControl.SelectedItem = pageView;
            treeViewItemPages.Items = Pages.Select(pw => pw.Name);
        }

        private void Page_DoubleTapped(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            string pageName = (string)((StackPanel)sender).DataContext;
            PageView pageView = Pages.Single(pw => pw.Name == pageName);
            pageView.Visible = true;
            pagesTabControl.SelectedItem = pageView;
        }

        private async void NewPage_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            PageWindow pageWindow = new PageWindow(null, this);
            PageView pageView = await pageWindow.ShowDialog<PageView>(ConfigTools.MainWindow);
            if(pageView != null)
            {
                pagesTabControl.SelectedItem = pageView;
            }
        }

        private async void EditPage_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            string pageName = (string)((MenuItem)sender).DataContext;
            PageView pageView = Pages.Single(pw => pw.Name == pageName);
            PageWindow pageWindow = new PageWindow(pageView, this);
            await pageWindow.ShowDialog(ConfigTools.MainWindow);
            treeViewItemPages.Items = Pages.Select(pw => pw.Name);
        }

        private async void RemovePage_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            string pageName = (string)((MenuItem)sender).DataContext;
            PageView pageView = Pages.Single(pw => pw.Name == pageName);
            var result = await MessageBoxManager.GetMessageBoxStandardWindow("Remove Page", $"Do you really want to remove page '{pageName}'?", ButtonEnum.YesNo, Icon.Warning).ShowDialog(ConfigTools.MainWindow);
            if(result == ButtonResult.Yes)
            {
                Pages.Remove(pageView);
            }
            treeViewItemPages.Items = Pages.Select(pw => pw.Name);
        }

        public ProjectView()
        {
            this.InitializeComponent();

            pagesTabControl = this.FindControl<TabControl>("pagesTabControl");
            treeViewItemPages = this.FindControl<TreeViewItem>("treeViewItemPages");

            DataContext = this;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
