﻿using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FMInatorul.Hubs
{
	public class GameHub : Hub
	{
		private static Dictionary<string, List<string>> rooms = new Dictionary<string, List<string>>();

		// Adds the current connection to the specified room.
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

		// Removes the current connection from the specified room.
		public async Task LeaveRoom(string roomName)
		{
			if (rooms.ContainsKey(roomName))
			{
				rooms[roomName].Remove(Context.ConnectionId);
				await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
				await Clients.Group(roomName).SendAsync("PlayerLeft", Context.ConnectionId);
			}
		}

		// Handles the event when a connection is disconnected.
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
