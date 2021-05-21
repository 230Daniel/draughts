using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Draughts.Api.Extensions;
using Draughts.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Draughts.Api.Draughts.Players
{
    public class HumanPlayer : IPlayer
    {
        IClientProxy _connection;

        public string Id { get; }
        public PieceColour PieceColour { get; set; }

        public event MoveSubmittedHandler OnMoveSubmitted;
        public event DisconnectedEventHandler OnDisconnected;
        
        public HumanPlayer(IHubContext<GameHub> hub, string connectionId)
        {
            _connection = hub.Clients.Client(connectionId);
            Id = connectionId;
        }
        
        public void SubmitMove(Position before, Position after)
        {
            OnMoveSubmitted.Invoke(this, before, after);
        }

        public void Disconnect()
        {
            OnDisconnected.Invoke(this);
        }

        public Task SendGameStartedAsync(PieceColour pieceColour)
            => _connection.SendAsync("GameStarted", pieceColour);

        public Task SendGameUpdatedAsync(
            PieceColour pieceColour, 
            Board board, 
            List<(Position, Position)> forcedMoves, 
            List<(Position, Position)> previousMove)
            => _connection.SendAsync("GameUpdated", 
                pieceColour, 
                board, 
                forcedMoves.AsTransportable(), 
                previousMove.AsTransportable());

        public Task SendGameCanceledAsync()
            => _connection.SendAsync("GameCanceled");

        public Task SendWaitingForOpponentAsync()
            => _connection.SendAsync("WaitingForOpponent");

        public Task SendGameEndedAsync(bool won)
            => _connection.SendAsync("GameEnded", won);
    }
}