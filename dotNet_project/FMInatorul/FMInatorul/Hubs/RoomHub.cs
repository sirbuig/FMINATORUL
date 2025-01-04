using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

public class RoomHub : Hub
{
    public async Task JoinRoomGroup(string roomCode)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);

        var userName = Context.User?.Identity?.Name ?? $"Connection {Context.ConnectionId}";

        await Clients.Group(roomCode).SendAsync("UserJoined", userName);
    }

    public async Task LeaveRoomGroup(string roomCode)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomCode);

        var userName = Context.User?.Identity?.Name ?? $"Connection {Context.ConnectionId}";
        await Clients.Group(roomCode).SendAsync("UserLeft", userName);
    }
}
