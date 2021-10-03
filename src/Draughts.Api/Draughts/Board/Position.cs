using System;

namespace Draughts.Api.Draughts
{
    public class Position : IEquatable<Position>
    {
        public int X { get; }
        public int Y { get; }

        private Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int[] AsTransportable() => this;

        public static implicit operator Position((int, int) value)
            => new(value.Item1, value.Item2);
        
        public static implicit operator (int, int)(Position value)
            => (value.X, value.Y);

        public static implicit operator Position(int[] value)
            => new(value[0], value[1]);
        
        public static implicit operator int[](Position value)
            => new[]{value.X, value.Y};
        
        public static bool operator ==(Position left, Position right)
            => left.Equals(right);

        public static bool operator !=(Position left, Position right)
            => !left.Equals(right);

        public bool Equals(Position other)
        {
            return X == other?.X && Y == other?.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is Position other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
}
