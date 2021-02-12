using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.Model
{
    public class Project : INotifyPropertyChanged
    {
        private int period;
        private Page selectedPage;

        public int Period { get => period; set => SetAndRaise(ref period, value, nameof(Period)); }

        public Page SelectedPage { get => selectedPage; set => SetAndRaise(ref selectedPage, value, nameof(SelectedPage)); }

        public ObservableCollection<Page> Pages { get; set; } = new ObservableCollection<Page>();

        public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>();

        public ObservableCollection<Room> Rooms { get; set; } = new ObservableCollection<Room>();

        public ObservableCollection<Category> Categories { get; set; } = new ObservableCollection<Category>();

        public event PropertyChangedEventHandler PropertyChanged;

        private void SetAndRaise<T>(ref T field, T value, string name)
        {
            if (!Equals(field, value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
