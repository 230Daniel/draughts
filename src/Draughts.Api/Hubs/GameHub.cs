using System.Threading.Tasks;
using Draughts.Api.Game;
using Draughts.Api.Services;
using Microsoft.AspNetCore.SignalR;

namespace Draughts.Api.Hubs
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
        public async Task<IGame> JoinGameAsync(string gameCode)
        {
            User user = _userService.GetOrCreateUser(Context);
            IGame game = _gameService.GetGame(gameCode);
            if(game is null || game.GameStatus != GameStatus.Waiting) 
                return null;

            await game.AddPlayerAsync(user);
            return game;
        }

        [HubMethodName("CancelGame")]
        public async Task CancelGameAsync()
        {
            User user = _userService.GetOrCreateUser(Context);
            IGame game = _gameService.GetCurrentUserGame(user);
            await game.CancelAsync();
        }

        [HubMethodName("SubmitMove")]
        public async Task SubmitMove(int[] current, int[] destination)
        {
            User user = _userService.GetOrCreateUser(Context);
            IGame game = _gameService.GetCurrentUserGame(user);
            await game.SubmitMove(user, current, destination);
        }
    }
}
