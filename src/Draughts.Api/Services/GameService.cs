using System;
using System.Collections.Generic;
using System.Linq;
using Draughts.Api.Hubs;
using Draughts.Api.Draughts;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace Draughts.Api.Services
{
    public interface IGameService
    {
        IGame CreateGame(GameCreateOptions options);
        IGame GetGame(string gameCode);
    }

    public class GameService : IGameService
    {
        IHubContext<GameHub> _hub;
        List<IGame> _games;
        
        public GameService(IHubContext<GameHub> hub)
        {
            _hub = hub;
            _games = new();
        }

        public IGame CreateGame(GameCreateOptions options)
        {
            string gameCode = GetGameCode();
            IGame game;
            
            if(options.Opponent == Opponent.Player)
                game = new TwoPlayerGame (gameCode, options);
            else if (options.Opponent == Opponent.Computer)
                game = new ComputerGame(gameCode, options);
            else
                return null;
            
            _games.Add(game);

            return game;
        }

        public IGame GetGame(string gameCode)
        {
            return _games.FirstOrDefault(x => x.GameCode == gameCode);
        }

        string GetGameCode()
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
