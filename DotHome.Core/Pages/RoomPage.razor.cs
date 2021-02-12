using DotHome.RunningModel;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotHome.Core.Pages
{
    public partial class RoomPage
    {
        [Parameter]
        public string RoomName { get; set; }

        private Room room;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
        }
    }
}
