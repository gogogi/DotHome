using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DotHome.Config.Tools;
using DotHome.RunningModel;
using MessageBox.Avalonia;
using System.Linq;

namespace DotHome.Config.Windows
{
    public class UserWindow : Window
    {
        private string Username { get; set; }

        private string Password { get; set; }

        private User user;

        public UserWindow(User user) : this()
        {
            this.user = user;
            if(user != null)
            {
                Username = user.Name;
                Password = user.Password;
            }
            else
            {
                Username = NextUserName();
            }
            DataContext = this;
        }

        public UserWindow()
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

        private async void ButtonOk_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (ConfigTools.MainWindow.Project.Users.Any(u => u != user && u.Name == Username))
            {
                await MessageBoxManager.GetMessageBoxStandardWindow("Error", "User with this name already exists").ShowDialog(this);
                return;
            }
            if(user == null)
            {
                user = new User() { Name = Username, Password = Password };
            }
            else
            {
                user.Name = Username;
                user.Password = Password;
            }
            Close(user);
        }

        private void ButtonCancel_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close();
        }

        private string NextUserName()
        {
            int i = 1;
            while (true)
            {
                string s = $"User{i}";
                if (ConfigTools.MainWindow.Project.Users.All(p => p.Name != s)) return s;
                i++;
            }
        }
    }
}
