using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using PeerJS.Models;

namespace PeerJS.Hubs
{
    public class SignalRHub : Hub
    {

        private Dictionary<string, ConnectionInfo> connectinInfo;

        public SignalRHub(Dictionary<string, ConnectionInfo> info)
        {
            this.connectinInfo = info;
        }

        public async Task JoinRoom(string roomID, string peerID)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomID);

            connectinInfo.Add(Context.ConnectionId, new ConnectionInfo
            {
                peerID = peerID,
                roomID = roomID
            });

            await Clients.Group(roomID).SendAsync("UserConnected", peerID);
        }

        public async Task Message(string message)
        {
            if (connectinInfo.TryGetValue(Context.ConnectionId, out var conn))
            {
                await Clients.Group(conn.roomID).SendAsync("createMessage", message);
            }
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (connectinInfo.TryGetValue(Context.ConnectionId, out var conn))
            {
                
                    Clients.Group(conn.roomID).SendAsync("UserDisconnected", conn.peerID);
                
            }


            return base.OnDisconnectedAsync(exception);
        }
    }
}
