using System;

namespace Draughts.Api.Draughts
{
    public class Movement
    {
        Position _before;
        
        public int ChangeInX { get; private set; }
        public int ChangeInY { get; private set; }
        public bool IsDiagonal { get; } = true;
        public Direction Direction
        {
            get
            {
                Direction value = Direction.None;
                if (ChangeInX < 0) value |= Direction.Left;
                if (ChangeInX > 0) value |= Direction.Right;
                if (ChangeInY < 0) value |= Direction.Up;
                if (ChangeInY > 0) value |= Direction.Down;
                return value;
            }
        }
        public int Magnitude => Math.Abs(ChangeInX);
        public bool Left => Direction.HasFlag(Direction.Left);
        public bool Right => Direction.HasFlag(Direction.Right);
        public bool Up => Direction.HasFlag(Direction.Up);
        public bool Down => Direction.HasFlag(Direction.Down);

        public (Position, Position) AsPositions()
        {
            return (_before, _before + this);
        }

        public Position GetJumpedPosition()
        {
            Movement movement = (Movement) MemberwiseClone();
            if (movement.Magnitude != 2) throw new Exception("The movement does not jump a position.");
            movement.ChangeInX /= 2;
            movement.ChangeInY /= 2;
            return _before + movement;
        }

        public static Movement ByTakingPiece(Position before, Piece piece)
        {
            Movement movement = new Movement(before, piece.Position);
            if (movement.Magnitude != 1) throw new Exception("The piece can not be taken.");
            movement.ChangeInX *= 2;
            movement.ChangeInY *= 2;
            return movement;
        }

        public Movement(Position before, Position after)
        {
            _before = before;
            ChangeInX = after.X - before.X;
            ChangeInY = after.Y - before.Y;
            if (Math.Abs(ChangeInX) != Math.Abs(ChangeInY))
                IsDiagonal = false;
        }
    }
    
    [Flags]
    public enum Direction
    {
        None = 0,
        Up = 1 << 0,
        Down = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3
    }
}