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
        
        public (Position, Position) FindBestMove(Board board, PieceColour pieceColour)
        {
            List<(Position, Position)> moves = new();
            foreach (Piece piece in board.Pieces.Where(x => x.Colour == pieceColour))
                moves.AddRange(piece.PossibleMoves.Select(x => (piece.Position, x)));

            return moves[_random.Next(0, moves.Count)];
        }
    }
}