using DotHome.RunningModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DotHome.Config
{
    partial class MainWindow : INotifyPropertyChanged
    {
        // We are not using Fody, because it causes issues with classes inherited from controls

        private Project project;
        private string path;
        private Server server;

        public Project Project { get => project; set { project = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Project))); } }

        public string Path { get => path; set { path = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Path))); } }

        public Server Server { get => server; set { server = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Server))); } }

        public new event PropertyChangedEventHandler PropertyChanged;
    }
}
