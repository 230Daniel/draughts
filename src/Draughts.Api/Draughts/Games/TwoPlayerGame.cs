using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Draughts.Api.Draughts.Players;
using Draughts.Api.Extensions;
using Draughts.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Draughts.Api.Draughts
{
    public class TwoPlayerGame : IGame
    {
        public string GameCode { get; }
        public GameStatus GameStatus { get; set; }
        public Board Board { get; }
        public GameCreateOptions Options { get; }

        IPlayer _player1;
        IPlayer _player2;
        
        List<(Position, Position)> _previousMove;

        public TwoPlayerGame(string gameCode, GameCreateOptions options)
        {
            GameCode = gameCode;
            Options = options;
            GameStatus = GameStatus.Waiting;
            Board = new();
            _previousMove = new();
        }

        public async Task AddPlayerAsync(IPlayer player)
        {
            player.OnMoveSubmitted += OnMoveSubmitted;
            player.OnDisconnected += OnPlayerDisconnected;
            
            if (_player1 is null)
            {
                _player1 = player;
                _player1.PieceColour = Options.Side switch
                {
                    Side.Random => (PieceColour) new Random().Next(0, 2),
                    Side.White => PieceColour.White,
                    Side.Black => PieceColour.Black,
                    _ => PieceColour.White
                };
                await _player1.SendWaitingForOpponentAsync();
            }
            else if (_player2 is null)
            {
                _player2 = player;
                _player2.PieceColour = _player1.PieceColour.Opposite();
                GameStatus = GameStatus.Playing;
                await SendGameStartedAsync();
                await SendGameUpdatedAsync();
            }
        }

        public async Task OnMoveSubmitted(IPlayer player, Position before, Position after)
        {
            MoveResult moveResult = Board.MovePiece(before, after);
            if (moveResult.IsValid)
            {
                _previousMove.Add((before, after));

                await SendGameUpdatedAsync();

                if (moveResult.IsFinished)
                    _previousMove.Clear();
                
                if (Board.GetIsWon(out PieceColour? winner) && winner.HasValue)
                {
                    GameStatus = GameStatus.Ended;
                    await SendGameEndedAsync(winner.Value);
                }
            }
        }
        
        Task SendGameStartedAsync()
            => Task.WhenAll(new List<Task>
            {
                _player1.SendGameStartedAsync(_player1.PieceColour),
                _player2.SendGameStartedAsync(_player2.PieceColour)
            });

        Task SendGameUpdatedAsync()
            => Task.WhenAll(new List<Task>
            {
                _player1.SendGameUpdatedAsync(
                    Board.ColourToMove,
                    Board,
                    Board.GetPossibleMoves(Board.ColourToMove),
                    _previousMove),
                
                _player2.SendGameUpdatedAsync(
                    Board.ColourToMove,
                    Board,
                    Board.GetPossibleMoves(Board.ColourToMove),
                    _previousMove)
            });
        
        Task SendGameEndedAsync(PieceColour winner)
            => Task.WhenAll(new List<Task>
            {
                _player1.SendGameEndedAsync(_player1.PieceColour == winner),
                _player2.SendGameEndedAsync(_player2.PieceColour == winner)
            });

        async Task OnPlayerDisconnected(IPlayer player)
        {
            if (player.Equals(_player1) && _player2 is not null)
                await _player2.SendGameCanceledAsync();
            
            if (player.Equals(_player2) && _player1 is not null)
                await _player1.SendGameCanceledAsync();
        }

        public TwoPlayerGame() { }
    }
}
