using System.Collections.Generic;
using System.Linq;

namespace Checkers.Api.Models
{
    public class Board
    {
        public List<Piece> Pieces { get; set; }

        public Board()
        {
            Pieces = new List<Piece>
            {
                Piece.White(1, 7),
                Piece.White(3, 7),
                Piece.White(5, 7),
                Piece.White(7, 7),

                Piece.White(0, 6),
                Piece.White(2, 6),
                Piece.White(4, 6),
                Piece.White(6, 6),
                
                Piece.White(1, 5),
                Piece.White(3, 5),
                Piece.White(5, 5),
                Piece.White(7, 5),
                
                
                Piece.Black(0, 0),
                Piece.Black(2, 0),
                Piece.Black(4, 0),
                Piece.Black(6, 0),
                
                Piece.Black(1, 1),
                Piece.Black(3, 1),
                Piece.Black(5, 1),
                Piece.Black(7, 1),
                
                Piece.Black(0, 2),
                Piece.Black(2, 2),
                Piece.Black(4, 2),
                Piece.Black(6, 2)
            };
            
            PromoteKings();
            ApplyPossibleMoves();
        }

        public MoveResult Move(Position before, Position after)
        {
            Piece piece = Pieces.First(x => x.Position == before);
            Movement m = new(before, after);
            
            // Pieces must move diagonally
            if(!m.IsDiagonal)
                return MoveResult.Invalid();
            
            // Pieces must take other pieces whenever possible
            List<(Position, Position)> forcedMoves = GetForcedMoves(piece.Colour);
            if (forcedMoves.Count > 0 && !forcedMoves.Any(x => x.Item1 == before && x.Item2 == after))
                return MoveResult.Invalid();
            
            // Pieces can not be on top of one another
            if (Pieces.Any(x => x.Position == after))
                return MoveResult.Invalid();

            // Pieces can not move backwards unless they're a king
            if (m.Down && !piece.CanMoveDown || m.Up && !piece.CanMoveUp) 
                return MoveResult.Invalid();
            
            // Pieces can not move more than 2 squares
            if(m.Magnitude > 2)
                return MoveResult.Invalid();
            
            // Pieces must take an enemy piece while moving 2 squares
            if (m.Magnitude == 2)
            {
                Piece taken = Pieces.FirstOrDefault(x => x.Position == m.GetJumpedPosition());

                if (taken is null || taken.Colour == piece.Colour)
                    return MoveResult.Invalid();

                Pieces.Remove(taken);
                piece.Position = after;
                
                // Note the piece has not yet been promoted (if it should be),
                // ... so the turn will end if the piece is about to be promoted
                // ... and the promoted piece could take a piece.
                return GetPiecesThatCanBeTaken(piece).Any() ? MoveResult.MoveAgain(after) : MoveResult.FinishMove();
            }

            // The move passed all checks
            piece.Position = after;
            return MoveResult.FinishMove();
        }

        public List<(Position, Position)> GetForcedMoves(PieceColour colour)
        {
            List<(Position, Position)> forcedMoves = new();
            List<Piece> friendlyPieces = Pieces.Where(x => x.Colour == colour).ToList();

            foreach (Piece friendlyPiece in friendlyPieces)
            {
                List<Piece> takablePieces = GetPiecesThatCanBeTaken(friendlyPiece);
                forcedMoves.AddRange(
                    takablePieces.Select(takablePiece => 
                        Movement.ByTakingPiece(friendlyPiece.Position, takablePiece)
                            .AsPositions()));
            }

            forcedMoves.RemoveAll(x => !x.Item2.IsValid);
            return forcedMoves;
        }
        
        public List<Piece> GetPiecesThatCanBeTaken(Piece piece)
        {
            List<Piece> takablePieces = new();
            
            if (piece.CanMoveUp)
            {
                Position destination = (piece.Position.X - 2, piece.Position.Y - 2);
                if (destination.IsValid && Pieces.All(x => x.Position != destination))
                {
                    Piece takable = Pieces.FirstOrDefault(x =>
                        x.Colour != piece.Colour && x.Position == (piece.Position.X - 1, piece.Position.Y - 1));
                    if (takable is not null) takablePieces.Add(takable);
                }
                destination = (piece.Position.X + 2, piece.Position.Y - 2);
                if (destination.IsValid && Pieces.All(x => x.Position != destination))
                {
                    Piece takable = Pieces.FirstOrDefault(x =>
                        x.Colour != piece.Colour && x.Position == (piece.Position.X + 1, piece.Position.Y - 1));
                    if (takable is not null) takablePieces.Add(takable);
                }
            }
            if (piece.CanMoveDown)
            {
                Position destination = (piece.Position.X - 2, piece.Position.Y + 2);
                if (destination.IsValid && Pieces.All(x => x.Position != destination))
                {
                    Piece takable = Pieces.FirstOrDefault(x =>
                        x.Colour != piece.Colour && x.Position == (piece.Position.X - 1, piece.Position.Y + 1));
                    if (takable is not null) takablePieces.Add(takable);
                }
                destination = (piece.Position.X + 2, piece.Position.Y + 2);
                if (destination.IsValid && Pieces.All(x => x.Position != destination))
                {
                    Piece takable = Pieces.FirstOrDefault(x =>
                        x.Colour != piece.Colour && x.Position == (piece.Position.X + 1, piece.Position.Y + 1));
                    if (takable is not null) takablePieces.Add(takable);
                }
            }

            return takablePieces;
        }

        public List<Position> GetPossibleMoves(Piece piece)
        {
            List<(Position, Position)> forcedMoves = GetForcedMoves(piece.Colour);
            
            if (forcedMoves.Any()) return forcedMoves
                .Where(x => x.Item1 == piece.Position)
                .Select(x => x.Item2).ToList();

            List<Position> possibleMoves = new();

            // Use existing logic to find valid moves which take pieces
            List<Piece> takablePieces = GetPiecesThatCanBeTaken(piece);
            possibleMoves.AddRange(takablePieces.Select(x => Movement.ByTakingPiece(piece.Position, x).AsPositions().Item2));
            
            // Get positions for 1 square in the 4 diagonal directions
            if (piece.CanMoveUp)
            {
                Position newPosition = (piece.Position.X - 1, piece.Position.Y - 1);
                if (Pieces.All(x => x.Position != newPosition))
                    possibleMoves.Add(newPosition);
                
                newPosition = (piece.Position.X + 1, piece.Position.Y - 1);
                if (Pieces.All(x => x.Position != newPosition))
                    possibleMoves.Add(newPosition);
            }
            if (piece.CanMoveDown)
            {
                Position newPosition = (piece.Position.X - 1, piece.Position.Y + 1);
                if (Pieces.All(x => x.Position != newPosition))
                    possibleMoves.Add(newPosition);
                
                newPosition = (piece.Position.X + 1, piece.Position.Y + 1);
                if (Pieces.All(x => x.Position != newPosition))
                    possibleMoves.Add(newPosition);
            }

            possibleMoves.RemoveAll(x => !x.IsValid);
            return possibleMoves;
        }

        public bool GetIsWon(out PieceColour? winner)
        {
            // The game is won if all opposing pieces are eliminated
            PieceColour firstPieceColour = Pieces.First().Colour;
            if (Pieces.All(x => x.Colour == firstPieceColour))
            {
                winner = firstPieceColour;
                return true;
            }
            
            // The game is won if all opposing pieces are unable to move
            List<Piece> whitePieces = Pieces.Where(x => x.Colour == PieceColour.White).ToList();
            if (whitePieces.All(x => GetPossibleMoves(x).Count == 0))
            {
                winner = PieceColour.Black;
                return true;
            }
            List<Piece> blackPieces = Pieces.Where(x => x.Colour == PieceColour.Black).ToList();
            if (blackPieces.All(x => GetPossibleMoves(x).Count == 0))
            {
                winner = PieceColour.White;
                return true;
            }

            winner = null;
            return false;
        }
        
        public void ApplyPossibleMoves()
        {
            foreach (Piece piece in Pieces)
                piece.PossibleMoves = GetPossibleMoves(piece);
        }
        
        public void PromoteKings()
        {
            foreach (Piece pieceToPromote in Pieces.Where(ShouldPromote))
                pieceToPromote.IsKing = true;
        }

        bool ShouldPromote(Piece piece)
            => piece.Colour == PieceColour.White && piece.Position.Y == 0 || 
               piece.Colour == PieceColour.Black && piece.Position.Y == 7;
    }
}
