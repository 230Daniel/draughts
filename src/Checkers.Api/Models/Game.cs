using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Checkers.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Checkers.Api.Models
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
        IClientProxy NextPlayerConnection => _hub.Clients.Clients(NextPlayer.ConnectionId);

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
                await PlayersConnection.SendAsync("setForcedMoves", ConvertMovesToTransportable(Board.GetForcedMoves(PieceColour.White)));
            }

            player.OnDisconnected += OnPlayerDisconnected;
        }

        public async Task CancelAsync()
        {
            GameStatus = GameStatus.Canceled;
            await PlayersConnection.SendAsync("GameCanceled");
        }

        public async Task SubmitMove(User player, Position before, Position after)
        {
            if(NextPlayer != player) return;
            PieceColour pieceColour = Players.IndexOf(player) == 0 ? PieceColour.White : PieceColour.Black;

            MoveResult moveResult = Board.Move(before, after);
            if (moveResult.IsValid)
            {
                Board.PromoteKings();
                Board.ApplyPossibleMoves();
                await PlayersConnection.SendAsync("BoardUpdated", Board);

                if (Board.GetIsWon(out PieceColour? winner))
                {
                    GameStatus = GameStatus.Ended;
                    await PlayersConnection.SendAsync("GameEnded", winner);
                    return;
                }
                
                if (moveResult.IsFinished)
                {
                    Console.WriteLine($"{DateTime.Now} turn change");
                    _turnNumber++;
                    await PlayersConnection.SendAsync("TurnChanged", _turnNumber % 2);
                }

                PieceColour nextPieceColour = moveResult.IsFinished ? pieceColour == PieceColour.White? PieceColour.Black : PieceColour.White : pieceColour;
                List<(Position, Position)> newForcedMoves = Board.GetForcedMoves(nextPieceColour);
                if (!moveResult.IsFinished) newForcedMoves.RemoveAll(x => x.Item1 != moveResult.PositionToMoveAgain);
                
                await PlayersConnection.SendAsync("setForcedMoves", ConvertMovesToTransportable(newForcedMoves));
            }
        }

        void OnPlayerDisconnected(object sender, EventArgs e)
        {
            _ = CancelAsync();
        }

        public Game()
        {
            Players = new List<User>();
        }

        static int[][][] ConvertMovesToTransportable(IEnumerable<(Position, Position)> moves)
        {
            return moves.Select(x => new []{x.Item1.AsTransportable(), x.Item2.AsTransportable()}).ToArray();
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
