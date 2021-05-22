using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Api.Draughts.Players.Engines
{
    public class RandomEngine
    {
        Random _random;
        
        public RandomEngine()
        {
            _random = new();
        }
        
        public (Position, Position) FindRandomMove(Board board, PieceColour pieceColour)
        {
            List<Move> moves = board.GetPossibleMoves(pieceColour);
            Move move = moves[_random.Next(0, moves.Count)];
            return (move.Origin, move.Destination);
        }
    }
}