using DotHome.Definitions;
using DotHome.Definitions.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DotHome.ProgrammingModel
{
    public class Project : INotifyPropertyChanged
    {
        public ObservableCollection<Page> Pages { get; set; } = new ObservableCollection<Page>();

        [JsonIgnore]
        public DefinitionsContainer Definitions { get; set; }

        [JsonIgnore]
        public Page SelectedPage { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Project()
        {
            string path = @"C:\Users\Vojta\Desktop\Bakalarka\Assemblies";
            if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
            {
                path = "/mnt/shared/Assemblies";
            }
            Definitions = DefinitionsCreator.CreateDefinitions(path);
        }
    }
}
