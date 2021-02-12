using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DotHome.Config.Tools;
using DotHome.Config.Windows;
using DotHome.Model;
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
        private Dictionary<Page, PageView> pageViewsDictionary = new Dictionary<Page, PageView>();

        public Project Project => (Project)DataContext;

        public PageView SelectedPageView => Project?.SelectedPage == null ? null : pageViewsDictionary?[Project.SelectedPage];

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
            PageWindow pageWindow = new PageWindow(null);
            Page page = await pageWindow.ShowDialog<Page>(ConfigTools.MainWindow);
            if(page != null)
            {
                Project.Pages.Add(page);
                pagesTabControl.SelectedItem = page;
            }
        }

        private async void EditPage_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Page page = (Page)((IControl)sender).DataContext;
            await new PageWindow(page).ShowDialog(ConfigTools.MainWindow);
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

        private async void NewUser_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            User user = await new UserWindow(null).ShowDialog<User>(ConfigTools.MainWindow);
            if (user != null)
            {
                Project.Users.Add(user);
            }
        }

        private async void EditUser_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            User user = (User)((IControl)sender).DataContext;
            await new UserWindow(user).ShowDialog(ConfigTools.MainWindow);
        }

        private async void RemoveUser_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            User user = (User)((IControl)sender).DataContext;
            var result = await MessageBoxManager.GetMessageBoxStandardWindow("Remove User", $"Do you really want to remove user '{user.Name}'?", ButtonEnum.YesNo, Icon.Warning).ShowDialog(ConfigTools.MainWindow);
            if (result == ButtonResult.Yes)
            {
                Project.Users.Remove(user);
            }
        }

        private async void NewRoom_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Room room = await new RoomWindow(null).ShowDialog<Room>(ConfigTools.MainWindow);
            if (room != null)
            {
                Project.Rooms.Add(room);
            }
        }

        private async void EditRoom_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Room room = (Room)((IControl)sender).DataContext;
            await new RoomWindow(room).ShowDialog(ConfigTools.MainWindow);
        }

        private async void RemoveRoom_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Room room = (Room)((IControl)sender).DataContext;
            var result = await MessageBoxManager.GetMessageBoxStandardWindow("Remove Room", $"Do you really want to remove room '{room.Name}'?", ButtonEnum.YesNo, Icon.Warning).ShowDialog(ConfigTools.MainWindow);
            if (result == ButtonResult.Yes)
            {
                Project.Rooms.Remove(room);
            }
        }

        private async void NewCategory_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Category category = await new CategoryWindow(null).ShowDialog<Category>(ConfigTools.MainWindow);
            if (category != null)
            {
                Project.Categories.Add(category);
            }
        }

        private async void EditCategory_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Category category = (Category)((IControl)sender).DataContext;
            await new CategoryWindow(category).ShowDialog(ConfigTools.MainWindow);
        }

        private async void RemoveCategory_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Category category = (Category)((IControl)sender).DataContext;
            var result = await MessageBoxManager.GetMessageBoxStandardWindow("Remove Category", $"Do you really want to remove category '{category.Name}'?", ButtonEnum.YesNo, Icon.Warning).ShowDialog(ConfigTools.MainWindow);
            if (result == ButtonResult.Yes)
            {
                Project.Categories.Remove(category);
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

        private void Page_AttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            var pageView = (PageView)sender;
            pageViewsDictionary[pageView.Page] = pageView;
            Command.ForceChanges();
        }

        private void Page_DetachedFromVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            var pageView = (PageView)sender;
            pageViewsDictionary.Remove(pageView.Page);
            Command.ForceChanges();
        }

        private void Page_DataContextChanged(object sender, System.EventArgs e)
        {
            var pageView = (PageView)sender;
            if(pageView.DataContext != null) pageViewsDictionary[pageView.Page] = pageView;
            Command.ForceChanges();
        }
    }
}
