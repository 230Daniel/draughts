using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Draughts.Api.Draughts.Players.Engines;
using Draughts.Api.Extensions;
using Microsoft.Extensions.Logging;

namespace Draughts.Api.Draughts.Players
{
    public class StockfishPlayer : IPlayer
    {
        StockfishEngine _engine;
        
        public string Id => "Bot Stockfish";
        public PieceColour PieceColour { get; set; }
        
        public event MoveSubmittedHandler OnMoveSubmitted;
        public event DisconnectedEventHandler OnDisconnected;

        public StockfishPlayer()
        {
            _engine = new(4);
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
                    Task delay = Task.Delay(750);
                    
                    (Position, Position) bestMove = _engine.FindBestMove(board, pieceColour);

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