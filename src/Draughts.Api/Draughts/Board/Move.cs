namespace Draughts.Api.Draughts
{
    public class Move
    {
        public Position Origin { get; }
        public Position Destination { get; }
        public bool IsJumping => Jumped is not null;
        public Position Jumped { get; }

        public Move(Position origin, Position destination)
        {
            Origin = origin;
            Destination = destination;
        }

        public Move(Position origin, Position destination, Position jumped)
        {
            Origin = origin;
            Destination = destination;
            Jumped = jumped;
        }

        public static Move Simple(Position origin, Position destination) => new Move(origin, destination);
        public static Move Jumping(Position origin, Position destination, Position jumped) => new Move(origin, destination, jumped);
    }
}