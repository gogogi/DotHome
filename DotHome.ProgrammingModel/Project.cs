using DotHome.Definitions;
using DotHome.Definitions.Tools;
using DotHome.RunningModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace DotHome.ProgrammingModel
{
    public class Project : INotifyPropertyChanged
    {
        public int ProgramPeriod { get; set; } = 100;

        public ObservableCollection<Page> Pages { get; } = new ObservableCollection<Page>();

        public ObservableCollection<User> Users { get; } = new ObservableCollection<User>();

        public ObservableCollection<Room> Rooms { get; } = new ObservableCollection<Room>();

        public ObservableCollection<Category> Categories { get; } = new ObservableCollection<Category>();

        [JsonIgnore]
        public DefinitionsContainer Definitions { get; set; }

        [JsonIgnore]
        public Page SelectedPage { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Project()
        {
            Users.CollectionChanged += Users_CollectionChanged;
            Rooms.CollectionChanged += Rooms_CollectionChanged;
            Categories.CollectionChanged += Categories_CollectionChanged;
        }

        private void Categories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Page p in Pages)
                {
                    foreach (Block b in p.Blocks.OfType<Block>())
                    {
                        if (typeof(VisualBlock).IsAssignableFrom(b.Definition.Type))
                        {
                            Parameter parameter = b.Parameters.Single(p => p.Definition.Name == nameof(VisualBlock.Category));
                            if (e.OldItems.Contains(parameter.Value)) parameter.Value = null;
                        }
                    }
                }
            }
        }

        private void Rooms_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Page p in Pages)
                {
                    foreach (Block b in p.Blocks.OfType<Block>())
                    {
                        if (typeof(VisualBlock).IsAssignableFrom(b.Definition.Type))
                        {
                            Parameter parameter = b.Parameters.Single(p => p.Definition.Name == nameof(VisualBlock.Room));
                            if (e.OldItems.Contains(parameter.Value)) parameter.Value = null;
                        }
                    }
                }
            }
        }

        private void Users_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach(Page p in Pages)
                {
                    foreach(Block b in p.Blocks.OfType<Block>())
                    {
                        if(typeof(VisualBlock).IsAssignableFrom(b.Definition.Type))
                        {
                            List<User> allowedUsers = (List<User>)b.Parameters.Single(p => p.Definition.Name == nameof(VisualBlock.AllowedUsers)).Value;
                            allowedUsers.RemoveAll(u => e.OldItems.Contains(u));
                        }
                    }
                }
            }
        }
    }
}
