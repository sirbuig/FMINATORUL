using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FMInatorul.Hubs
{
    public class GameHub : Hub
    {
        private static Dictionary<string, List<string>> rooms = new Dictionary<string, List<string>>();

        public async Task JoinRoom(string roomName)
        {
            if (!rooms.ContainsKey(roomName))
            {
                rooms[roomName] = new List<string>();
            }
            rooms[roomName].Add(Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).SendAsync("PlayerJoined", Context.ConnectionId);
        }

        public async Task LeaveRoom(string roomName)
        {
            if (rooms.ContainsKey(roomName))
            {
                rooms[roomName].Remove(Context.ConnectionId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
                await Clients.Group(roomName).SendAsync("PlayerLeft", Context.ConnectionId);
            }
        }

        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            foreach (var room in rooms)
            {
                if (room.Value.Contains(Context.ConnectionId))
                {
                    room.Value.Remove(Context.ConnectionId);
                    await Clients.Group(room.Key).SendAsync("PlayerLeft", Context.ConnectionId);
                }
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
