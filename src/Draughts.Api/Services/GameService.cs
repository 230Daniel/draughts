using System;
using System.Collections.Generic;
using System.Linq;
using Draughts.Api.Hubs;
using Draughts.Api.Models;
using Microsoft.AspNetCore.SignalR;

namespace Draughts.Api.Services
{
    public interface IGameService
    {
        Game CreateGame();
        Game GetGame(string gameCode);
        Game GetCurrentUserGame(User user);
    }

    public class GameService : IGameService
    {
        IHubContext<GameHub> _hub;
        List<Game> _games;
        
        public GameService(IHubContext<GameHub> hub)
        {
            _hub = hub;
            _games = new List<Game>();
        }

        public Game CreateGame()
        {
            string gameCode = GetGameCode();
            Game game = new Game(gameCode, _hub);
            _games.Add(game);

            return game;
        }

        public Game GetGame(string gameCode)
        {
            return _games.FirstOrDefault(x => x.GameCode == gameCode);
        }

        public Game GetCurrentUserGame(User user)
        {
            return _games.FirstOrDefault(x => x.Players.Any(y => y == user));
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
