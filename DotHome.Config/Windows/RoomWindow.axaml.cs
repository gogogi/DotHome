using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DotHome.Config.Tools;
using DotHome.Model;
using MessageBox.Avalonia;
using System.Linq;

namespace DotHome.Config.Windows
{
    public class RoomWindow : Window
    {
        private string RoomName { get; set; }

        private Room room;

        public RoomWindow(Room room) : this()
        {
            this.room = room;
            if(room != null)
            {
                RoomName = room.Name;
            }
            else
            {
                RoomName = NextRoomName();
            }
            DataContext = this;
        }

        public RoomWindow()
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
            if (ConfigTools.MainWindow.Project.Rooms.Any(r => r != room && r.Name == RoomName))
            {
                await MessageBoxManager.GetMessageBoxStandardWindow("Error", "Room with this name already exists").ShowDialog(this);
                return;
            }
            if (room == null)
            {
                room = new Room() { Name = RoomName };
            }
            else
            {
                room.Name = RoomName;
            }
            Close(room);
        }

        private void ButtonCancel_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close();
        }

        private string NextRoomName()
        {
            int i = 1;
            while (true)
            {
                string s = $"Room{i}";
                if (ConfigTools.MainWindow.Project.Rooms.All(r => r.Name != s)) return s;
                i++;
            }
        }
    }
}
