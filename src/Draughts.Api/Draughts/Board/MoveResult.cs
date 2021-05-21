namespace Draughts.Api.Draughts
{
    public class MoveResult
    {
        public bool IsValid { get; private set; }
        public bool IsFinished { get; private set; }
        public Position PositionToMoveAgain { get; private set; }

        public static MoveResult FinishMove() => new() { IsValid = true, IsFinished = true };
        public static MoveResult MoveAgain(Position position) => new() { IsValid = true, IsFinished = false, PositionToMoveAgain = position };
        public static MoveResult Invalid() => new() { IsValid = false, IsFinished = false };
    }
}