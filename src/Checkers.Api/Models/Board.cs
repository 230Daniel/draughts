using System;
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
