﻿using DotHome.Definitions;
using DotHome.ProgrammingModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DotHome.Config
{
    partial class MainWindow : INotifyPropertyChanged
    {
        private Project project;
        private string path;
        private Server server;

        private Project Project { get => project; set { project = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Project))); } }

        private string Path { get => path; set { path = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Path))); } }

        private Server Server { get => server; set { server = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Server))); } }

        public new event PropertyChangedEventHandler PropertyChanged;
    }
}
