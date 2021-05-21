using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Draughts.Api.Draughts.Players.Engines;
using Microsoft.Extensions.Logging;

namespace Draughts.Api.Draughts.Players
{
    public class StockfishPlayer : IPlayer
    {
        StockfishEngine _engine;
        
        public string Id => "Bot Random";
        public PieceColour PieceColour { get; set; }
        
        public event MoveSubmittedHandler OnMoveSubmitted;
        public event DisconnectedEventHandler OnDisconnected;

        public StockfishPlayer()
        {
            _engine = new(3);
        }
        
        public Task SendGameStartedAsync(PieceColour pieceColour)
            => Task.CompletedTask;

        public Task SendGameUpdatedAsync(PieceColour pieceColour, Board board, List<(Position, Position)> forcedMoves, List<(Position, Position)> previousMove)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    if (pieceColour != PieceColour) return;
                    Task delay = Task.Delay(1000);
                    
                    (Position, Position) bestMove = _engine.FindBestMove(board, pieceColour);
                    Console.WriteLine($"Making move {((int, int)) bestMove.Item1} -> {((int, int)) bestMove.Item2}");

                    await delay;
                    await OnMoveSubmitted.Invoke(this, bestMove.Item1, bestMove.Item2);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });
            return Task.CompletedTask;
        }

        public Task SendGameCanceledAsync()
            => Task.CompletedTask;

        public Task SendWaitingForOpponentAsync()
            => Task.CompletedTask;

        public Task SendGameEndedAsync(bool won)
            => Task.CompletedTask;
    }
}