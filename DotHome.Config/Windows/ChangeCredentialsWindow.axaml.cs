using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MessageBox.Avalonia;
using MessageBox.Avalonia.Enums;

namespace DotHome.Config.Windows
{
    public class ChangeCredentialsWindow : Window
    {
        private string OldUsername { get; set; }
        private string OldPassword { get; set; }
        private string NewUsername { get; set; }
        private string NewPassword { get; set; }
        private string NewPasswordAgain { get; set; }

        private Server server;

        public ChangeCredentialsWindow(Server server) : this()
        {
            this.server = server;
            DataContext = this;
        }

        public ChangeCredentialsWindow()
        {
            this.InitializeComponent();
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
            if(NewPassword == NewPasswordAgain)
            {
                if(await server.ChangeCredentials(OldUsername, OldPassword, NewUsername, NewPassword))
                {
                    Close();
                }
                else
                {
                    await MessageBoxManager.GetMessageBoxStandardWindow("Change Credentials", "Failed to change credentials", ButtonEnum.Ok, MessageBox.Avalonia.Enums.Icon.Error).ShowDialog(this);
                }
            }
            else
            {
                await MessageBoxManager.GetMessageBoxStandardWindow("Change Credentials", "Paswords are not equal", ButtonEnum.Ok, MessageBox.Avalonia.Enums.Icon.Error).ShowDialog(this);
            }
        }

        private void ButtonCancel_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close();
        }
    }
}
