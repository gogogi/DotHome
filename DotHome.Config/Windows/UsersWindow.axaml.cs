using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DotHome.Config.Tools;
using DotHome.Model;
using System.Collections.Generic;
using System.Linq;

namespace DotHome.Config.Windows
{
    public class UsersWindow : Window
    {
        private List<User> users;
        private List<TableEntry> TableEntries { get; }

        public UsersWindow(List<User> users) : this()
        {
            this.users = users;
            TableEntries = ConfigTools.MainWindow.Project.Users.Select(u => new TableEntry() { User = u, Selected = users.Contains(u) }).ToList();
            DataContext = this;
        }

        public UsersWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Ok_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            users.Clear();
            users.AddRange(TableEntries.Where(te => te.Selected).Select(te => te.User));
            Close();
        }

        private void Cancel_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close();
        }

        private class TableEntry
        {
            public User User { get; set; }

            public bool Selected { get; set; }
        }

        private void DataGrid_CollectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Do not allow user to select anything in grids
            DataGrid dataGrid = (DataGrid)sender;
            dataGrid.SelectedItems.Clear();
        }
    }
}
