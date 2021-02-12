using DotHome.RunningModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotHome.Core.Shared
{
    public partial class MainLayout
    {
        private bool sidebarExpanded;
        private List<Room> rooms = new List<Room>() { new Room() { Name = "Kuchyne" }, new Room() { Name = "Obyvak" } };
        private List<Category> categories = new List<Category>() { new Category() { Name = "Cat1" }, new Category() { Name = "Cat2" } };
    }
}
