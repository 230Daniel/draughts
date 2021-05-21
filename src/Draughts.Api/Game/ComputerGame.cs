using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Draughts.Api.Extensions;
using Draughts.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Draughts.Api.Game
{
    public class ComputerGame : IGame
    {
        public string GameCode { get; }
        public GameStatus GameStatus { get; set; }
        public List<User> Players { get; }
        public Board Board { get; }
        public GameCreateOptions Options { get; }

        int _turnNumber;
        List<(Position, Position)> _moves;
        int _currentMoveCount;
        
        IHubContext<GameHub> _hub;
        IClientProxy PlayerConnection => _hub.Clients.Clients(Players[0].ConnectionId);

        public ComputerGame(string gameCode, GameCreateOptions options, IHubContext<GameHub> hub)
        {
            GameCode = gameCode;
            Options = options;
            GameStatus = GameStatus.Waiting;
            Players = new();
            Board = new();
            _hub = hub;
            _moves = new();
        }

        public async Task AddPlayerAsync(User player)
        {
            player.OnDisconnected += OnPlayerDisconnected;
            Players.Add(player);
            
            GameStatus = GameStatus.Playing;
            await PlayerConnection.SendAsync("GameStarted", 0);
            await PlayerConnection.SendAsync("GameUpdated", 
                _turnNumber % 2, 
                Board, 
                Board.GetForcedMoves(PieceColour.White).AsTransportable(),
                _moves.TakeLast(_currentMoveCount + 1));
        }

        public async Task CancelAsync()
        {
            GameStatus = GameStatus.Canceled;
            await PlayerConnection.SendAsync("GameCanceled");
        }

        public async Task SubmitMove(User player, Position before, Position after)
        {
            if(player is not null && _turnNumber % 2 != 0) return;
            PieceColour pieceColour = player is null ? PieceColour.Black : PieceColour.White;

            MoveResult moveResult = Board.Move(before, after);
            if (moveResult.IsValid)
            {
                _moves.Add((before, after));
                _currentMoveCount++;
                
                Board.PromoteKings();
                Board.ApplyPossibleMoves();

                if (moveResult.IsFinished)
                    _turnNumber++;

                PieceColour nextPieceColour = moveResult.IsFinished ? pieceColour == PieceColour.White? PieceColour.Black : PieceColour.White : pieceColour;
                List<(Position, Position)> newForcedMoves = Board.GetForcedMoves(nextPieceColour);
                if (!moveResult.IsFinished) newForcedMoves.RemoveAll(x => x.Item1 != moveResult.PositionToMoveAgain);
                
                await PlayerConnection.SendAsync("GameUpdated", 
                    _turnNumber % 2, 
                    Board, 
                    newForcedMoves.AsTransportable(),
                    _moves.TakeLast(_currentMoveCount).AsTransportable());

                if (moveResult.IsFinished)
                    _currentMoveCount = 0;
                
                if (Board.GetIsWon(out PieceColour? winner))
                {
                    GameStatus = GameStatus.Ended;
                    await PlayerConnection.SendAsync("GameEnded", winner);
                    return;
                }

                if (player is not null && moveResult.IsFinished || player is null && !moveResult.IsFinished)
                {
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(1000);
                        List<(Position, Position)> possibleMoves = new();
                        foreach (Piece piece in Board.Pieces.Where(x => x.Colour == PieceColour.Black))
                            possibleMoves.AddRange(piece.PossibleMoves.Select(x => (piece.Position, x)));

                        Random random = new();
                        (Position, Position) move = possibleMoves[random.Next(0, possibleMoves.Count)];
                        _ = SubmitMove(null, move.Item1, move.Item2);
                    });
                }
            }
        }

        void OnPlayerDisconnected(object sender, EventArgs e)
        {
            _ = CancelAsync();
        }

        public ComputerGame()
                 {
            Players = new List<User>();
        }
    }
}
