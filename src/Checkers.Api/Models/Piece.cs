namespace Checkers.Api.Models
{
    public class Piece
    {
        public PieceColour Colour { get; set; }
        public Position Position { get; set; }
        public bool IsKing { get; set; }
        public bool CanMoveUp => Colour == PieceColour.Black || IsKing; 
        public bool CanMoveDown => Colour == PieceColour.White || IsKing;

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