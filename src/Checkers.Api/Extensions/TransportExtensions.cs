using System.Collections.Generic;
using System.Linq;
using Checkers.Api.Models;

namespace Checkers.Api.Extensions
{
    public static class TransportExtensions
    {
        public static int[][][] AsTransportable(this IEnumerable<(Position, Position)> moves)
        {
            return moves.Select(x => new []{x.Item1.AsTransportable(), x.Item2.AsTransportable()}).ToArray();
        }
    }
}