namespace Draughts.Api.Draughts
{
    public class Piece
    {
        public PieceColour Colour { get; set; }
        public bool IsKing { get; set; }

        public Piece(PieceColour colour, bool isKing = false)
        {
            Colour = colour;
            IsKing = isKing;
        }

        public Piece Clone()
        {
            return new(Colour, IsKing);
        }
        
        public static Piece White()
            => new(PieceColour.White);
        
        public static Piece Black()
            => new(PieceColour.Black);
        
        public static Piece WhiteKing()
            => new(PieceColour.White, true);
        
        public static Piece BlackKing()
            => new(PieceColour.Black, true);
    }
}
