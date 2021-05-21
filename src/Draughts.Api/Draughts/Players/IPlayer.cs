using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Draughts.Api.Draughts.Players
{
    public interface IPlayer
    {
        string Id { get; }
        PieceColour PieceColour { get; set; }

        event MoveSubmittedHandler OnMoveSubmitted;
        event DisconnectedEventHandler OnDisconnected;
        
        Task SendGameStartedAsync(PieceColour pieceColour);
        Task SendGameUpdatedAsync(PieceColour pieceColour, Board board, List<(Position, Position)> forcedMoves, List<(Position, Position)> previousMove);
        Task SendGameCanceledAsync();
        Task SendWaitingForOpponentAsync();
        Task SendGameEndedAsync(bool won);
    }
    
    public delegate Task MoveSubmittedHandler(IPlayer player, Position before, Position after);
    public delegate Task DisconnectedEventHandler(IPlayer player);

    public static class PlayerExtensions
    {
        public static bool Equals(this IPlayer player, IPlayer other)
            => player?.Id == other?.Id;
    }
    
}