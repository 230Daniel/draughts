using System.Threading.Tasks;
using Checkers.Api.Models;
using Checkers.Api.Services;
using Microsoft.AspNetCore.SignalR;

namespace Checkers.Api.Hubs
{
    public class GameHub : Hub
    {
        IGameService _gameService;
        IUserService _userService;

        public GameHub(IGameService gameService, IUserService userService)
        {
            _gameService = gameService;
            _userService = userService;
        }

        [HubMethodName("JoinGame")]
        public async Task<bool> JoinGameAsync(string gameCode)
        {
            User user = _userService.GetOrCreateUser(Context);
            Game game = _gameService.GetGame(gameCode);
            if(game is null || game.GameStatus != GameStatus.Waiting) 
                return false;

            await game.AddPlayerAsync(user);
            return true;
        }

        [HubMethodName("CancelGame")]
        public async Task CancelGameAsync()
        {
            User user = _userService.GetOrCreateUser(Context);
            Game game = _gameService.GetCurrentUserGame(user);
            await game.CancelAsync();
        }

        [HubMethodName("SubmitMove")]
        public async Task SubmitMove(int[] current, int[] destination)
        {
            User user = _userService.GetOrCreateUser(Context);
            Game game = _gameService.GetCurrentUserGame(user);
            await game.SubmitMove(user, current, destination);
        }
    }
}
