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
    public class ComputerGame : IGame
    {
        public string GameCode { get; }
        public GameStatus GameStatus { get; set; }
        public Board Board { get; }
        public GameCreateOptions Options { get; }

        IPlayer _player1;
        IPlayer _player2;

        int _turnNumber;
        List<(Position, Position)> _moves;
        int _currentMoveCount;

        public ComputerGame(string gameCode, GameCreateOptions options)
        {
            GameCode = gameCode;
            Options = options;
            GameStatus = GameStatus.Waiting;
            Board = new();
            _moves = new();
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

                _player2 = Options.Algorithm switch
                {
                    Algorithm.RandomMoves => new RandomPlayer(),
                    Algorithm.Stockfish => new StockfishPlayer(),
                    _ => _player2
                };
                
                _player2.OnMoveSubmitted += OnMoveSubmitted;
                _player2.OnDisconnected += OnPlayerDisconnected;
                
                _player2.PieceColour = _player1.PieceColour.Opposite();
                GameStatus = GameStatus.Playing;
                
                await SendGameStartedAsync();
                await SendGameUpdatedAsync();
            }
        }

        public async Task OnMoveSubmitted(IPlayer player, Position before, Position after)
        {
            if (player.PieceColour != (PieceColour) (_turnNumber % 2)) return;

            MoveResult moveResult = Board.MovePiece(before, after);
            if (moveResult.IsValid)
            {
                _moves.Add((before, after));
                _currentMoveCount++;

                if (moveResult.IsFinished)
                    _turnNumber++;

                PieceColour nextPieceColour =
                    moveResult.IsFinished ? player.PieceColour.Opposite() : player.PieceColour;
                List<(Position, Position)> newForcedMoves = Board.GetForcedMoves(nextPieceColour);
                if (!moveResult.IsFinished) newForcedMoves.RemoveAll(x => x.Item1 != moveResult.PositionToMoveAgain);

                if (moveResult.IsFinished)
                    _currentMoveCount = 0;
                
                await SendGameUpdatedAsync(newForcedMoves);

                if (Board.GetIsWon(player.PieceColour, out PieceColour winner))
                {
                    GameStatus = GameStatus.Ended;
                    await SendGameEndedAsync(winner);
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
                    (PieceColour) (_turnNumber % 2),
                    Board,
                    Board.GetPossibleMoves(PieceColour.White),
                    _moves.TakeLast(_currentMoveCount + 1).ToList()),

                _player2.SendGameUpdatedAsync(
                    (PieceColour) (_turnNumber % 2),
                    Board,
                    Board.GetPossibleMoves(PieceColour.White),
                    _moves.TakeLast(_currentMoveCount + 1).ToList())
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

        public ComputerGame() { }
    }
}
