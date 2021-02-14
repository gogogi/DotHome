using DotHome.Core.Services;
using DotHome.RunningModel;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotHome.Core.Pages
{
    public partial class Navbar
    {
        [Inject]
        public IProgramCore ProgramCore { get; set; }

        private List<Room> rooms;
        private List<Category> categories;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            rooms = ProgramCore.Rooms;
            categories = ProgramCore.Categories;
        }
    }
}
