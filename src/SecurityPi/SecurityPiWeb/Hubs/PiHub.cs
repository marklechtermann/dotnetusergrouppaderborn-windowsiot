using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SecurityPiWeb.Hubs
{
    using Microsoft.AspNet.SignalR;

    public class PiHub : Hub
    {
        public void Send(string message)
        {
            this.Clients.All.broadcastMessage(message);
        }
    }
}