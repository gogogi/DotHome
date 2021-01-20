using DotHome.Definitions;
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
        //private Page selectedPage;

        private Project Project { get => project; set { project = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Project))); } }

        private string Path { get => path; set { path = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Path))); } }

        //private Page SelectedPage { get => selectedPage; set { selectedPage = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedPage))); } }

        public new event PropertyChangedEventHandler PropertyChanged;
    }
}
