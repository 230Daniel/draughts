using System.Collections.Concurrent;
using System.Threading.Tasks;
using Draughts.Api.Draughts.Players;
using Draughts.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Draughts.Api.Services
{
    public interface IHumanPlayerService
    {
        bool TryGetPlayer(string connectionId, out HumanPlayer player);
        HumanPlayer GetOrCreatePlayer(HubCallerContext context);
    }

    public class HumanPlayerService : IHumanPlayerService
    {
        IHubContext<GameHub> _hub;
        ConcurrentDictionary<string, HumanPlayer> _players;

        public HumanPlayerService(IHubContext<GameHub> hub)
        {
            _hub = hub;
            _players = new();
        }
        
        public bool TryGetPlayer(string connectionId, out HumanPlayer player)
            => _players.TryGetValue(connectionId, out player);

        public HumanPlayer GetOrCreatePlayer(HubCallerContext context)
            => TryGetPlayer(context.ConnectionId, out HumanPlayer player) ? 
                player : 
                CreatePlayer(context);

        HumanPlayer CreatePlayer(HubCallerContext context)
        {
            HumanPlayer player = new(_hub, context.ConnectionId);
            context.ConnectionAborted.Register(() => player.Disconnect());
            player.OnDisconnected += OnPlayerDisconnectedAsync;
            _players.TryAdd(context.ConnectionId, player);
            return player;
        }

        Task OnPlayerDisconnectedAsync(IPlayer player)
        {
            _players.TryRemove(player.Id, out _);
            return Task.CompletedTask;
        }
    }
}
