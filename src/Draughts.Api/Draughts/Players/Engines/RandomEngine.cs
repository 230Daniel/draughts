﻿using System;

namespace Draughts.Api.Draughts.Players.Engines
{
    public class RandomEngine
    {
        private Random _random;
        
        public RandomEngine()
        {
            _random = new();
        }
        
        public (Position, Position) FindRandomMove(Board board, PieceColour pieceColour)
        {
            var moves = board.GetPossibleMoves(pieceColour);
            var move = moves[_random.Next(0, moves.Count)];
            return (move.Origin, move.Destination);
        }
    }
}
