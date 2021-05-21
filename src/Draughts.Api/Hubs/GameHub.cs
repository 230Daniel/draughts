using System.Threading.Tasks;
using Draughts.Api.Draughts;
using Draughts.Api.Draughts.Players;
using Draughts.Api.Services;
using Microsoft.AspNetCore.SignalR;

namespace Draughts.Api.Hubs
{
    public class GameHub : Hub
    {
        IGameService _gameService;
        IHumanPlayerService _humanPlayerService;

        public GameHub(IGameService gameService, IHumanPlayerService humanPlayerService)
        {
            _gameService = gameService;
            _humanPlayerService = humanPlayerService;
        }

        [HubMethodName("JoinGame")]
        public async Task<IGame> JoinGameAsync(string gameCode)
        {
            IPlayer player = _humanPlayerService.GetOrCreatePlayer(Context);
            
            IGame game = _gameService.GetGame(gameCode);
            if(game is null || game.GameStatus != GameStatus.Waiting) 
                return null;

            await game.AddPlayerAsync(player);
            return game;
        }

        [HubMethodName("SubmitMove")]
        public void SubmitMove(int[] before, int[] after)
        {
            if(!_humanPlayerService.TryGetPlayer(Context.ConnectionId, out HumanPlayer player))
                return;

            player.SubmitMove(before, after);
        }
    }
}
