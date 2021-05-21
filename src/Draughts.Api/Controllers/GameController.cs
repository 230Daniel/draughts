using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Draughts.Api.Game;
using Draughts.Api.Services;

namespace Draughts.Api.Controllers
{
    public class GameController : Controller
    {
        IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost("game/create")]
        public IActionResult CreateGame([Required] [FromBody] GameCreateOptions options)
        {
            IGame game = _gameService.CreateGame(options);
            return new JsonResult(game.GameCode);
        }

        [HttpGet("game/{gameCode}")]
        public IActionResult Game([Required] string gameCode)
        {
            IGame game = _gameService.GetGame(gameCode);
            if (game is null)
                return new NotFoundResult();
            return new JsonResult(game);
        }
    }
}
