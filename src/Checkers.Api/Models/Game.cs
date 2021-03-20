using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using WSTest.Api.Hubs;

namespace WSTest.Api.Models
{
    public class Game
    {
        public string GameCode { get; set; }
        public GameStatus GameStatus { get; private set; }
        public List<User> Players { get; }
        public Board Board { get; }

        int _turnNumber;
        User NextPlayer => Players[_turnNumber % Players.Count];

        IHubContext<GameHub> _hub;
        IClientProxy PlayersConnection => _hub.Clients.Clients(Players.Select(x => x.ConnectionId));
        IClientProxy Player1Connection => _hub.Clients.Clients(Players[0].ConnectionId);
        IClientProxy Player2Connection => _hub.Clients.Clients(Players[1].ConnectionId);

        public Game(string gameCode, IHubContext<GameHub> hub)
        {
            GameCode = gameCode;
            GameStatus = GameStatus.Waiting;
            Players = new List<User>();
            Board = new Board();
            _hub = hub;
        }

        public async Task AddPlayerAsync(User player)
        {
            Players.Add(player);
            if (Players.Count == 2)
            {
                GameStatus = GameStatus.Playing;
                await Player1Connection.SendAsync("GameStarted", 0);
                await Player2Connection.SendAsync("GameStarted", 1);
                await PlayersConnection.SendAsync("BoardUpdated", Board);
                await PlayersConnection.SendAsync("TurnChanged", 0);
            }

            player.OnDisconnected += OnPlayerDisconnected;
        }

        public async Task CancelAsync()
        {
            GameStatus = GameStatus.Canceled;
            await PlayersConnection.SendAsync("GameCanceled");
        }

        public async Task TakeTurn(User player)
        {
            if(NextPlayer != player) return;
            
            Board.IncrementNumber();
            await PlayersConnection.SendAsync("BoardUpdated", Board);

            _turnNumber++;
            await PlayersConnection.SendAsync("TurnChanged", _turnNumber % 2);
        }

        void OnPlayerDisconnected(object sender, EventArgs e)
        {
            _ = CancelAsync();
        }

        public Game()
        {
            Players = new List<User>();
        }
    }

    public enum GameStatus
    {
        Waiting,
        Playing,
        Canceled,
        Ended
    }
}
