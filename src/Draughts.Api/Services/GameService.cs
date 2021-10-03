using System;
using System.Collections.Generic;
using System.Linq;
using Draughts.Api.Draughts;

namespace Draughts.Api.Services
{
    public interface IGameService
    {
        IGame CreateGame(GameCreateOptions options);
        IGame GetGame(string gameCode);
    }

    public class GameService : IGameService
    {
        private List<IGame> _games;
        
        public GameService()
        {
            _games = new();
        }

        public IGame CreateGame(GameCreateOptions options)
        {
            var gameCode = GetGameCode();
            var game = options.Opponent switch
            {
                Opponent.Player => new TwoPlayerGame(gameCode, options) as IGame,
                Opponent.Computer => new ComputerGame(gameCode, options),
                _ => throw new Exception("Invalid opponent")
            };

            _games.Add(game);
            return game;
        }

        public IGame GetGame(string gameCode)
        {
            return _games.FirstOrDefault(x => x.GameCode == gameCode);
        }

        private string GetGameCode()
        {
            Random random = new();
            const string chars = "abcdefghijklmnopqrstuvwxyz";
            string[] forbidden = { "create" };

            string code = null;
            while (code is null || _games.Any(x => x.GameCode == code) || forbidden.Contains(code))
            {
                code = new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
            }

            return code;
        }
    }
}
