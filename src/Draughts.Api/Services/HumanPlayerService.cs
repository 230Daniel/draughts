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
        private IHubContext<GameHub> _hub;
        private ConcurrentDictionary<string, HumanPlayer> _players;

        public HumanPlayerService(IHubContext<GameHub> hub)
        {
            _hub = hub;
            _players = new();
        }
        
        public bool TryGetPlayer(string connectionId, out HumanPlayer player)
            => _players.TryGetValue(connectionId, out player);

        public HumanPlayer GetOrCreatePlayer(HubCallerContext context)
            => TryGetPlayer(context.ConnectionId, out var player) ? 
                player : 
                CreatePlayer(context);

        private HumanPlayer CreatePlayer(HubCallerContext context)
        {
            HumanPlayer player = new(_hub, context.ConnectionId);
            context.ConnectionAborted.Register(() => player.Disconnect());
            player.OnDisconnected += OnPlayerDisconnectedAsync;
            _players.TryAdd(context.ConnectionId, player);
            return player;
        }

        private Task OnPlayerDisconnectedAsync(IPlayer player)
        {
            _players.TryRemove(player.Id, out _);
            return Task.CompletedTask;
        }
    }
}
