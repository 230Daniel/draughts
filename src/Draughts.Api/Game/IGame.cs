using System.Collections.Generic;
using System.Threading.Tasks;

namespace Draughts.Api.Game
{
    public interface IGame
    {
        string GameCode { get; }
        GameStatus GameStatus { get; set; }
        List<User> Players { get; }
        Board Board { get; }
        GameCreateOptions Options { get; }

        Task AddPlayerAsync(User player);
        Task CancelAsync();
        Task SubmitMove(User player, Position before, Position after);
    }
}
