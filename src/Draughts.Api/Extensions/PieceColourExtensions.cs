using Draughts.Api.Draughts;

namespace Draughts.Api.Extensions
{
    public static class PieceColourExtensions
    {
        public static PieceColour Opposite(this PieceColour pieceColour)
            => pieceColour == PieceColour.White ? PieceColour.Black : PieceColour.White;
    }
}