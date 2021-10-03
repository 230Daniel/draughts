using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Draughts.Api.Draughts.Players.Engines;

namespace Draughts.Api.Draughts.Players
{
    public class StockfishPlayer : IPlayer
    {
        private StockfishEngine _engine;
        
        public string Id => "Bot Stockfish";
        public PieceColour PieceColour { get; set; }
        
        public event MoveSubmittedHandler OnMoveSubmitted;
        public event DisconnectedEventHandler OnDisconnected;

        public StockfishPlayer(int maxDepth)
        {
            _engine = new(maxDepth);
        }

        public Task SendGameStartedAsync(PieceColour pieceColour)
        {
            _engine.DesiredPieceColour = pieceColour;
            return Task.CompletedTask;
        }

        public Task SendGameUpdatedAsync(PieceColour pieceColour, Board board, List<Move> possibleMoves, List<(Position, Position)> previousMove)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    if (pieceColour != PieceColour) return;
                    var delay = Task.Delay(750);
                    
                    var bestMove = _engine.FindBestMove(board);
                    if (bestMove is null) return;
                    await delay;
                    await OnMoveSubmitted.Invoke(this, bestMove.Origin, bestMove.Destination);
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