using System.Threading.Tasks;
using Draughts.Api.Draughts.Players;

namespace Draughts.Api.Draughts
{
    public interface IGame
    {
        string GameCode { get; }
        GameStatus GameStatus { get; set; }
        Board Board { get; }
        GameCreateOptions Options { get; }

        Task AddPlayerAsync(IPlayer player);
    }
}
