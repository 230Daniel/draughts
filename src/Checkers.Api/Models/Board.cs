﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Checkers.Api.Models
{
    public class Board
    {
        public List<Piece> Pieces { get; set; }

        public Board()
        {
            Pieces = new List<Piece>
            {
                Piece.White(1, 7),
                Piece.White(3, 7),
                Piece.White(5, 7),
                Piece.White(7, 7),

                Piece.White(0, 6),
                Piece.White(2, 6),
                Piece.White(4, 6),
                Piece.White(6, 6),
                
                Piece.White(1, 5),
                Piece.White(3, 5),
                Piece.White(5, 5),
                Piece.White(7, 5),
                
                
                Piece.Black(0, 0),
                Piece.Black(2, 0),
                Piece.Black(4, 0),
                Piece.Black(6, 0),
                
                Piece.Black(1, 1),
                Piece.Black(3, 1),
                Piece.Black(5, 1),
                Piece.Black(7, 1),
                
                Piece.Black(0, 2),
                Piece.Black(2, 2),
                Piece.Black(4, 2),
                Piece.Black(6, 2)
            };
        }

        public MoveResult Move(Position before, Position after)
        {
            Piece piece = Pieces.First(x => x.Position == before);
            
            // Can not move piece onto another piece
            if (Pieces.Any(x => x.Position == after))
                return MoveResult.Invalid;
            
            // Piece could move diagonally 2 squares with an opponent piece underneath

            bool left = after.X == before.X - 2;
            bool right = after.X == before.X + 2;
            bool up = after.Y == before.Y - 2;
            bool down = after.Y == before.Y + 2;

            if (left || right || up || down)
            {
                Piece taken = null;

                if (left && up)
                    taken = Pieces.FirstOrDefault(x => x.Position == (before.X - 1, before.Y - 1));
                if (left && down)
                    taken = Pieces.FirstOrDefault(x => x.Position == (before.X - 1, before.Y + 1));
                if (right && up)
                    taken = Pieces.FirstOrDefault(x => x.Position == (before.X + 1, before.Y - 1));
                if (right && down)
                    taken = Pieces.FirstOrDefault(x => x.Position == (before.X + 1, before.Y + 1));
                
                if (taken is null || taken.Colour == piece.Colour)
                    return MoveResult.Invalid;
                
                Pieces.Remove(taken);
                piece.Position = after;
                
                return MoveResult.Free;
            }
            
            // Otherwise pieces must move diagonally one square
            
            left = after.X == before.X - 1;
            right = after.X == before.X + 1;
            up = after.Y == before.Y - 1;
            down = after.Y == before.Y + 1;

            if (!(left && up || left && down || right && up || right && down))
                return MoveResult.Invalid;
            
            piece.Position = after;
            return MoveResult.Valid;
        }
    }

    public enum MoveResult
    {
        Invalid,
        Valid,
        Free
    }

    public class Piece
    {
        public PieceColour Colour { get; set; }
        public Position Position { get; set; }
        public bool IsKing { get; set; }

        public Piece(PieceColour colour, int x, int y)
        {
            Colour = colour;
            Position = (x, y);
        }

        public static Piece White(int x, int y)
            => new(PieceColour.White, x, y);
        
        public static Piece Black(int x, int y)
            => new(PieceColour.Black, x, y);
    }

    public enum PieceColour
    {
        White,
        Black
    }
}
