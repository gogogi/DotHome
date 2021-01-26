using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DotHome.Config.Tools;
using MessageBox.Avalonia;
using MessageBox.Avalonia.Enums;

namespace DotHome.Config.Windows
{
    public class ConnectWindow : Window
    {
        private string Host { get; set; } = AppConfig.Configuration["Host"];
        private string Username { get; set; } = AppConfig.Configuration["Username"];
        private string Password { get; set; } = AppConfig.Configuration["Password"];
        private bool RememberPassword { get; set; } = AppConfig.Configuration["Password"] != null;


        public ConnectWindow()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            DataContext = this;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void ButtonOk_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var server = await Server.Connect(Host, Username, Password);
            if(server == null)
            {
                await MessageBoxManager.GetMessageBoxStandardWindow("Connect", "Could not connect", ButtonEnum.Ok, MessageBox.Avalonia.Enums.Icon.Error).ShowDialog(this);
            }
            else
            {
                AppConfig.Configuration["Host"] = Host;
                AppConfig.Configuration["Username"] = Username;
                if (RememberPassword)
                {
                    AppConfig.Configuration["Password"] = Password;
                }
                Close(server);
            }
        }

        private void ButtonCancel_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close();
        }
    }
}
