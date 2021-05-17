using System;
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
                Piece.White(7, 7),
                Piece.Black(6, 6),
                Piece.Black(4, 4),
                Piece.Black(1, 1)
                /*Piece.White(1, 7),
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
                Piece.Black(6, 2)*/
            };
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

            return forcedMoves;
        }
        
        public List<Piece> GetPiecesThatCanBeTaken(Piece piece)
        {
            bool canMoveDown = true;
            bool canMoveUp = true;
                
            if (piece.Colour == PieceColour.White) canMoveDown = piece.IsKing;
            else canMoveUp = piece.IsKing;

            List<Piece> takablePieces = new();
            
            if (canMoveUp)
            {
                if (Pieces.All(x => x.Position != (piece.Position.X - 2, piece.Position.Y - 2)))
                {
                    Piece takable = Pieces.FirstOrDefault(x =>
                        x.Colour != piece.Colour && x.Position == (piece.Position.X - 1, piece.Position.Y - 1));
                    if (takable is not null) takablePieces.Add(takable);
                }
                if (Pieces.All(x => x.Position != (piece.Position.X + 2, piece.Position.Y - 2)))
                {
                    Piece takable = Pieces.FirstOrDefault(x =>
                        x.Colour != piece.Colour && x.Position == (piece.Position.X + 1, piece.Position.Y - 1));
                    if (takable is not null) takablePieces.Add(takable);
                }
            }

            if (canMoveDown)
            {
                if (Pieces.All(x => x.Position != (piece.Position.X - 2, piece.Position.Y + 2)))
                {
                    Piece takable = Pieces.FirstOrDefault(x =>
                        x.Colour != piece.Colour && x.Position == (piece.Position.X - 1, piece.Position.Y + 1));
                    if (takable is not null) takablePieces.Add(takable);
                }
                if (Pieces.All(x => x.Position != (piece.Position.X + 2, piece.Position.Y + 2)))
                {
                    Piece takable = Pieces.FirstOrDefault(x =>
                        x.Colour != piece.Colour && x.Position == (piece.Position.X + 1, piece.Position.Y + 1));
                    if (takable is not null) takablePieces.Add(takable);
                }
            }

            return takablePieces;
        }

        public void PromoteKings()
        {
            foreach (Piece pieceToPromote in Pieces.Where(x => x.Colour == PieceColour.White && x.Position.Y == 0 || x.Colour == PieceColour.Black && x.Position.Y == 7))
                pieceToPromote.IsKing = true;
        }
    }
}
