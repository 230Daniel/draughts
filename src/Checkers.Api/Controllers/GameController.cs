using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WSTest.Api.Models;
using WSTest.Api.Services;

namespace WSTest.Api.Controllers
{
    public class GameController : Controller
    {
        IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet("game/create")]
        public IActionResult CreateGame()
        {
            Game game = _gameService.CreateGame();
            return new JsonResult(game.GameCode);
        }

        [HttpGet("game/{gameCode}")]
        public IActionResult Game([Required] string gameCode)
        {
            Game game = _gameService.GetGame(gameCode);
            if (game is null)
                return new NotFoundResult();
            return new JsonResult(game);
        }
    }
}
