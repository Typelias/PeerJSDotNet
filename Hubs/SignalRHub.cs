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

        public async Task JoinRoom(string roomID, string peerID, string username)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomID);

            connectinInfo.Add(Context.ConnectionId, new ConnectionInfo
            {
                peerID = peerID,
                roomID = roomID,
                username = username
            });

            await Clients.Group(roomID).SendAsync("UserList", extractRoomList(roomID));

            await Clients.Group(roomID).SendAsync("UserConnected", peerID);
        }

        private Dictionary<string, string> extractRoomList(string room)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();

            connectinInfo.ToList().ForEach(delegate (KeyValuePair<string, ConnectionInfo> conn)
            {
                if (conn.Value.roomID == room)
                {
                    ret.Add(conn.Value.username, conn.Key);
                }
            });

            return ret;

        }


        public async Task Message(string message)
        {
            if (connectinInfo.TryGetValue(Context.ConnectionId, out var conn))
            {
                await Clients.Group(conn.roomID).SendAsync("createMessage", message, conn.username);
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
