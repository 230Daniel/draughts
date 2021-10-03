namespace Draughts.Api.Draughts
{
    public class GameCreateOptions
    {
        public Opponent Opponent { get; set; }
        public Algorithm Algorithm { get; set; }
        public int Depth { get; set; }
        public Side Side { get; set; }
        public Variant Variant { get; set; }
    }

    public enum Side
    {
        Random,
        White,
        Black
    }

    public enum Opponent
    {
        Player,
        Computer
    }

    public enum Algorithm
    {
        MiniMax,
        RandomMoves
    }

    public enum Variant
    {
        EnglishDraughts
    }
}
