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
        private Dictionary<string, RoomInfo> roomInfo;

        public SignalRHub(Dictionary<string, ConnectionInfo> info, Dictionary<string, RoomInfo> leader)
        {
            this.connectinInfo = info;
            this.roomInfo = leader;
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

            if(roomInfo.TryGetValue(roomID,out var room))
            {
                await Clients.Group(roomID).SendAsync("NewLeader", room.leaderStream);
            }
            else
            {
                roomInfo.Add(roomID, new RoomInfo { leaderStream = "", roomName = roomID });
            }

            

            await Clients.Group(roomID).SendAsync("UserList", extractRoomList(roomID));

            await Clients.Group(roomID).SendAsync("UserConnected", peerID, username);
        }

        public async Task SetLeader(string streamID)
        {
            //System.Diagnostics.Debug.WriteLine(streamID);
            if (connectinInfo.TryGetValue(Context.ConnectionId, out var conn))
            {
                if (roomInfo.TryGetValue(conn.roomID, out var info))
                {
                    info.leaderStream = streamID;
                    await Clients.Group(conn.roomID).SendAsync("NewLeader", info.leaderStream);
                    roomInfo[conn.roomID] = info;
                }
                
            }
        }

        private Dictionary<string, string> extractRoomList(string room)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();

            connectinInfo.ToList().ForEach(delegate (KeyValuePair<string, ConnectionInfo> conn)
            {
                if (conn.Value.roomID == room)
                {
                    if (ret.ContainsKey(conn.Value.username))
                    {
                        ret.Remove(conn.Value.username);
                    }
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
