using DotHome.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotHome.Core.Hubs
{
    [Authorize(Roles = "Administrator")]
    public class DebuggingHub : Hub
    {
        private IProgramCore programCore;

        public DebuggingHub(IProgramCore programCore)
        {
            this.programCore = programCore;
        }

        public override Task OnConnectedAsync()
        {
            programCore.StartDebugging();
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            programCore.StopDebugging();
            return base.OnDisconnectedAsync(exception);
        }

        public void Pause()
        {
            programCore.Pause();
        }

        public void Continue()
        {
            programCore.Continue();
        }

        public void Step()
        {
            programCore.Step();
        }
    }
}