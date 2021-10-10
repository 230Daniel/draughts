namespace Draughts.Api.Draughts
{
    public class Tile
    {
        public Piece Piece { get; set; }
        public bool IsOccupied => Piece is not null;

        public Tile() { }

        public Tile(Piece piece)
        {
            Piece = piece;
        }

        public Tile Clone()
        {
            return new()
            {
                Piece = Piece?.Clone()
            };
        }
        
        public static Tile Empty => new ();
        public static Tile WhitePiece => new(Piece.White());
        public static Tile BlackPiece => new(Piece.Black());
        public static Tile WhiteKing => new(Piece.WhiteKing());
        public static Tile BlackKing => new(Piece.BlackKing());
    }
}
