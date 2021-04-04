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
    /// <summary>
    /// Container for the entire Config project. This entitiy is serialized and saved into a file and uploaded onto Server. 
    /// There it is converted to <see cref="RunningModel"/> with true <see cref="RunningModel.Block"/>s an run.
    /// </summary>
    public class Project : INotifyPropertyChanged, IProgrammingModelObject
    {
        /// <summary>
        /// Main program loop execution period
        /// </summary>
        public int ProgramPeriod { get; set; } = 100;

        /// <summary>
        /// Pages are helper organisation units in Config GUI
        /// </summary>
        public ObservableCollection<Page> Pages { get; } = new ObservableCollection<Page>();

        /// <summary>
        /// Users which have access to some of <see cref="VisualBlock"/>s in Visualisation GUI
        /// </summary>
        public ObservableCollection<User> Users { get; } = new ObservableCollection<User>();

        /// <summary>
        /// Rooms of house (Kitchen, Living room, Garden...)
        /// </summary>
        public ObservableCollection<Room> Rooms { get; } = new ObservableCollection<Room>();

        /// <summary>
        /// Categories of automation (Heating, Light, Ventilation...)
        /// </summary>
        public ObservableCollection<Category> Categories { get; } = new ObservableCollection<Category>();

        /// <summary>
        /// Definitions container stores metadata of available <see cref="RunningModel.Block"/>s so that we know how to interpret our <see cref="Block"/>s
        /// </summary>
        [JsonIgnore]
        public DefinitionsContainer Definitions { get; set; }

        /// <summary>
        /// Selected page in Config GUI. It only acts as a binding object.
        /// </summary>
        [JsonIgnore]
        public Page SelectedPage { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Project()
        {
            Users.CollectionChanged += Users_CollectionChanged;
            Rooms.CollectionChanged += Rooms_CollectionChanged;
            Categories.CollectionChanged += Categories_CollectionChanged;
        }

        /// <summary>
        /// When <see cref="Category"/> is removed, we have to remove all references to it (<see cref="VisualBlock.Category"/>)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// When <see cref="Room"/> is removed, we have to remove all references to it (<see cref="VisualBlock.Room"/>)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// When <see cref="User"/> is removed, we have to remove all references to it (<see cref="AuthenticatedBlock.AllowedUsers"/>)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Users_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach(Page p in Pages)
                {
                    foreach(Block b in p.Blocks.OfType<Block>())
                    {
                        if(typeof(AuthenticatedBlock).IsAssignableFrom(b.Definition.Type))
                        {
                            List<User> allowedUsers = (List<User>)b.Parameters.Single(p => p.Definition.Name == nameof(AuthenticatedBlock.AllowedUsers)).Value;
                            allowedUsers.RemoveAll(u => e.OldItems.Contains(u));
                        }
                    }
                }
            }
        }
    }
}
