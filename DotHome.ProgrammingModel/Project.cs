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

    }
}
