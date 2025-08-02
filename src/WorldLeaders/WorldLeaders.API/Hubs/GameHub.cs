using Microsoft.AspNetCore.SignalR;
using WorldLeaders.Shared.DTOs;

namespace WorldLeaders.API.Hubs;

/// <summary>
/// SignalR hub for real-time game updates in the World Leaders educational game
/// </summary>
public class GameHub : Hub
{
    /// <summary>
    /// Join a game session group for real-time updates
    /// </summary>
    /// <param name="playerId">The unique player identifier</param>
    public async Task JoinGameSession(string playerId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"player_{playerId}");
        await Clients.Caller.SendAsync("JoinedGameSession", playerId);
    }

    /// <summary>
    /// Leave a game session group
    /// </summary>
    /// <param name="playerId">The unique player identifier</param>
    public async Task LeaveGameSession(string playerId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"player_{playerId}");
        await Clients.Caller.SendAsync("LeftGameSession", playerId);
    }

    /// <summary>
    /// Broadcast game state updates to a specific player
    /// </summary>
    /// <param name="playerId">The player to update</param>
    /// <param name="update">The game state update</param>
    public async Task SendGameStateUpdate(string playerId, GameStateUpdate update)
    {
        await Clients.Group($"player_{playerId}").SendAsync("GameStateUpdated", update);
    }

    /// <summary>
    /// Send AI agent response to a specific player
    /// </summary>
    /// <param name="playerId">The player to update</param>
    /// <param name="response">The AI agent response</param>
    public async Task SendAIResponse(string playerId, AIAgentResponse response)
    {
        await Clients.Group($"player_{playerId}").SendAsync("AIResponseReceived", response);
    }

    /// <summary>
    /// Notify player of territory acquisition
    /// </summary>
    /// <param name="playerId">The player who acquired the territory</param>
    /// <param name="territory">The acquired territory information</param>
    public async Task NotifyTerritoryAcquired(string playerId, TerritoryDto territory)
    {
        await Clients.Group($"player_{playerId}").SendAsync("TerritoryAcquired", territory);
    }

    /// <summary>
    /// Handle connection events for logging and monitoring
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        // Log connection for monitoring child safety
    }

    /// <summary>
    /// Handle disconnection events
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        // Clean up any player groups and log disconnection
    }
}
