using System.Collections.Generic;
using System.Threading.Tasks;
using Draughts.Api.Draughts.Players.Engines;

namespace Draughts.Api.Draughts.Players
{
    public class RandomPlayer : IPlayer
    {
        private RandomEngine _engine;
        
        public string Id => "Bot Random";
        public PieceColour PieceColour { get; set; }
        
        public event MoveSubmittedHandler OnMoveSubmitted;
        public event DisconnectedEventHandler OnDisconnected;

        public RandomPlayer()
        {
            _engine = new();
        }
        
        public Task SendGameStartedAsync(PieceColour pieceColour)
            => Task.CompletedTask;

        public Task SendGameUpdatedAsync(PieceColour pieceColour, Board board, List<Move> possibleMoves, List<(Position, Position)> previousMove)
        {
            _ = Task.Run(async () =>
            {
                if (pieceColour != PieceColour) return;
                await Task.Delay(1000);

                var bestMove = _engine.FindRandomMove(board);
                await OnMoveSubmitted.Invoke(this, bestMove.Item1, bestMove.Item2);
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
