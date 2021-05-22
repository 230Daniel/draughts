using System.Collections.Generic;

namespace Draughts.Api.Draughts
{
    public class Piece
    {
        public PieceColour Colour { get; set; }
        public bool IsKing { get; set; }

        public Piece(PieceColour colour)
        {
            Colour = colour;
        }

        public static Piece White()
            => new(PieceColour.White);
        
        public static Piece Black()
            => new(PieceColour.Black);
    }
}