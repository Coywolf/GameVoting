using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameVoting.Models.DatabaseModels;
using GameVoting.Models.ViewModels;
using Microsoft.AspNet.SignalR;

namespace GameVoting.Hubs
{
    public class EventHub : Hub
    {
        public override System.Threading.Tasks.Task OnConnected()
        {
            Groups.Add(Context.ConnectionId, Context.Request.QueryString["eventId"]);

            return base.OnConnected();
        }
    }
}