using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DotHome.ProgrammingModel
{
    public class Project : INotifyPropertyChanged
    {
        public ObservableCollection<Page> Pages { get; set; } = new ObservableCollection<Page>();

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
