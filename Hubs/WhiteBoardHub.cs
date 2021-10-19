using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;

namespace PeerJS.Hubs
{
    public class WhiteBoardHub : Hub
    {
        private Dictionary<string, string> _SocketToRoom;

        public WhiteBoardHub(Dictionary<string, string> socektToRoom)
        {
            this._SocketToRoom = socektToRoom;
        }
        public async Task JoinRoom(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            _SocketToRoom.Add(Context.ConnectionId, roomName);
        }

        public async Task Drawing(string data)
        {
            if(_SocketToRoom.TryGetValue(Context.ConnectionId, out var room))
            {
                await Clients.Group(room).SendAsync("Drawing", data);
            }
        }
        public async Task Rectangle(string data)
        {
            if (_SocketToRoom.TryGetValue(Context.ConnectionId, out var room))
            {
                await Clients.Group(room).SendAsync("Rectangle", data);
            }
        }
        public async Task Linedraw(string data)
        {
            if (_SocketToRoom.TryGetValue(Context.ConnectionId, out var room))
            {
                await Clients.Group(room).SendAsync("Linedraw", data);
            }
        }

        public async Task Eraser(string data)
        {
            if (_SocketToRoom.TryGetValue(Context.ConnectionId, out var room))
            {
                await Clients.Group(room).SendAsync("Eraser", data);
            }
        }

        public async Task Ellipsedraw(string data)
        {
            if (_SocketToRoom.TryGetValue(Context.ConnectionId, out var room))
            {
                await Clients.Group(room).SendAsync("Ellipsedraw", data);
            }
        }

        public async Task Textdraw(string data)
        {
            if (_SocketToRoom.TryGetValue(Context.ConnectionId, out var room))
            {
                await Clients.Group(room).SendAsync("Textdraw", data);
            }
        }

        public async Task Notedraw(string data)
        {
            if (_SocketToRoom.TryGetValue(Context.ConnectionId, out var room))
            {
                await Clients.Group(room).SendAsync("Notedraw", data);
            }
        }

        public async Task CopyCanvas(string data)
        {
            if (_SocketToRoom.TryGetValue(Context.ConnectionId, out var room))
            {
                await Clients.Group(room).SendAsync("CopyCanvas", data);
            }
        }

        public async Task Clearboard(string data)
        {
            if (_SocketToRoom.TryGetValue(Context.ConnectionId, out var room))
            {
                await Clients.Group(room).SendAsync("Clearboard", data);
            }
        }

    }
}
