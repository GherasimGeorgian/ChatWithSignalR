using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ChatWithSignalR.Controllers;

namespace ChatWithSignalR.Hubs
{
    public class ChatHub : Hub
    {
        public string GetConnectionId() => Context.ConnectionId;

      

        static long counter = 0;

        

      

        public override async Task OnConnectedAsync()
        {
            
            counter += 1;
           
          //  await Clients.Client.SendAsync("ReceiveMail", counter.ToString());
            await Clients.Client(GetConnectionId()).SendAsync("initSignal", counter.ToString() + " " + GetConnectionId());
            
            await base.OnConnectedAsync();
        }

        public virtual async Task OnDisconnectedAsync(bool stopCalled)
        {
            counter -= 1;

            //  await Clients.All.SendAsync("ReceiveMail", counter.ToString());
            await Clients.Client(GetConnectionId()).SendAsync("initSignal", counter.ToString() + " " + GetConnectionId());

            await OnDisconnectedAsync(stopCalled);
        }

    }
}
