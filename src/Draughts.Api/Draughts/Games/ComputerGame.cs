using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Draughts.Api.Draughts.Players;
using Draughts.Api.Extensions;

namespace Draughts.Api.Draughts
{
    public class ComputerGame : IGame
    {
        public string GameCode { get; }
        public GameStatus GameStatus { get; set; }
        public Board Board { get; }
        public GameCreateOptions Options { get; }

        private IPlayer _player1;
        private IPlayer _player2;

        private List<(Position, Position)> _previousMove;

        public ComputerGame(string gameCode, GameCreateOptions options)
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

                _player2 = Options.Algorithm switch
                {
                    Algorithm.RandomMoves => new RandomPlayer(),
                    Algorithm.MiniMax => new MiniMaxPlayer(Options.Depth),
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
            var moveResult = Board.MovePiece(before, after);
            if (moveResult.IsValid)
            {
                _previousMove.Add((before, after));

                await SendGameUpdatedAsync();
                
                if (moveResult.IsFinished)
                    _previousMove.Clear();

                if (Board.GetIsWon(out var winner) && winner.HasValue)
                {
                    GameStatus = GameStatus.Ended;
                    await SendGameEndedAsync(winner.Value);
                }
            }
        }

        private Task SendGameStartedAsync()
            => Task.WhenAll(new List<Task>
            {
                _player1.SendGameStartedAsync(_player1.PieceColour),
                _player2.SendGameStartedAsync(_player2.PieceColour)
            });

        private Task SendGameUpdatedAsync()
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

        private Task SendGameEndedAsync(PieceColour winner)
            => Task.WhenAll(new List<Task>
            {
                _player1.SendGameEndedAsync(_player1.PieceColour == winner),
                _player2.SendGameEndedAsync(_player2.PieceColour == winner)
            });

        private async Task OnPlayerDisconnected(IPlayer player)
        {
            if (player.Equals(_player1) && _player2 is not null)
                await _player2.SendGameCanceledAsync();

            if (player.Equals(_player2) && _player1 is not null)
                await _player1.SendGameCanceledAsync();
        }

        public ComputerGame() { }
    }
}
