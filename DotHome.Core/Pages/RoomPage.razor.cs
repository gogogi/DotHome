﻿using DotHome.Core.Services;
using DotHome.RunningModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
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
        private List<AVisualBlock> visualBlocks;

        [Inject]
        public IProgramCore ProgramCore { get; set; }

        [Inject]
        public IHttpContextAccessor HttpContextAccessor { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            room = ProgramCore.Rooms.SingleOrDefault(r => r.Name == RoomName);
            string username = HttpContextAccessor.HttpContext.User.Identity.Name;
            User user = ProgramCore.Users.SingleOrDefault(u => u.Name == username);
            if(room != null && user != null)
            {
                visualBlocks = ProgramCore.VisualBlocks.Where(b => b.AllowedUsers.Contains(user) && b.Room == room).ToList();
            }
            else
            {
                visualBlocks = null;
            }
        }
    }
}
