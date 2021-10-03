using System;
using System.Collections.Generic;
using System.Linq;
using Draughts.Api.Extensions;

namespace Draughts.Api.Draughts
{
    public class Board
    {
        private static readonly string[] DefaultBoard =
        {
            "b b b b ",
            " b b b b",
            "b b b b ",
            "        ",
            "        ",
            " w w w w",
            "w w w w ",
            " w w w w"
        };

        /*private static readonly string[] DebugBoard =
        {
            "        ",
            "        ",
            "        ",
            "        ",
            "    B   ",
            "        ",
            "      W ",
            "        "
        };*/
        
        public Tile[,] Tiles { get; }
        public PieceColour ColourToMove { get; private set; }
        public Position PositionToMoveAgain { get; private set; }
        public PieceColour ColourJustMoved { get; private set; }

        public Board()
        {
            Tiles = new Tile[8, 8];
            ColourToMove = PieceColour.White;
            PositionToMoveAgain = null;

            var board = DefaultBoard;
            
            for (var y = 0; y < 8; y++)
            {
                var row = board[y];
                for (var x = 0; x < 8; x++)
                {
                    Tiles[x, y] = row[x] switch
                    {
                        ' ' => Tile.Empty,
                        'w' => Tile.WhitePiece,
                        'b' => Tile.BlackPiece,
                        'W' => Tile.WhiteKing,
                        'B' => Tile.BlackKing,
                        _ => Tile.Empty
                    };
                }
            }
            
            PromoteKings();
        }

        public MoveResult MovePiece(Move move)
        {
            // Get the piece in the specified position
            var tile = Tiles[move.Origin.X, move.Origin.Y];
            if (!tile.IsOccupied || tile.Piece.Colour != ColourToMove)
                return MoveResult.Invalid();
            var piece = tile.Piece;

            // If the move jumps a piece and can jump another, award another move
            if (move.IsJumping)
            {
                Tiles[move.Destination.X, move.Destination.Y].Piece = piece;
                Tiles[move.Origin.X, move.Origin.Y].Piece = null;
                Tiles[move.Jumped.X, move.Jumped.Y].Piece = null;
                
                PositionToMoveAgain = move.Destination;
                var possibleExtraMoves = GetPossibleMoves(move.Destination);
                ColourJustMoved = piece.Colour;
                PromoteKings();
                
                if (possibleExtraMoves.All(x => !x.IsJumping))
                {
                    PositionToMoveAgain = null;
                    ColourToMove = ColourToMove.Opposite();
                    return MoveResult.FinishMove();
                }

                PositionToMoveAgain = move.Destination;
                return MoveResult.MoveAgain(move.Destination);
            }
            
            Tiles[move.Destination.X, move.Destination.Y].Piece = piece;
            Tiles[move.Origin.X, move.Origin.Y].Piece = null;
            
            PositionToMoveAgain = null;
            ColourJustMoved = piece.Colour;
            PromoteKings();
            
            ColourToMove = ColourToMove.Opposite();
            return MoveResult.FinishMove();
        }

        public MoveResult MovePiece(Position origin, Position destination)
        {
            var possibleMoves = GetPossibleMoves(origin);
            var move = possibleMoves.FirstOrDefault(x => x.Destination == destination);
            return move is null
                ? MoveResult.Invalid()
                : MovePiece(move);
        }

        public List<Move> GetPossibleMoves(Position origin)
        {
            List<Move> possibleMoves = new();
            
            // If a different piece has to move again return an empty list
            if (PositionToMoveAgain is not null && origin != PositionToMoveAgain)
                return possibleMoves;
            
            // Get the piece in the specified position
            var tile = Tiles[origin.X, origin.Y];
            if (!tile.IsOccupied) throw new InvalidOperationException("That position is not occupied");
            var piece = tile.Piece;

            // Depending on piece colour and king status, set valid Y movements
            var possibleYMovements = piece.IsKing switch
            {
                true => new[] {1, -1, 2, -2},
                false when piece.Colour == PieceColour.White => new[] {-1, -2},
                false when piece.Colour == PieceColour.Black => new[] {1, 2},
                _ => throw new InvalidOperationException("The piece in that position has invalid properties")
            };
            
            var jumpingMoveAvailable = false;
            
            // Check to see if moving diagonally with each Y movement is valid
            foreach (var movement in possibleYMovements)
            {
                // We can move by this value in both negative and positive X
                foreach (var xDirection in new[] {-1, 1})
                {
                    var destX = origin.X + (movement * xDirection);
                    var destY = origin.Y + movement;
                
                    if (destX is < 0 or > 7 || destY is < 0 or > 7)
                        continue;
                
                    var destinationTile = Tiles[destX, destY];
                    if (destinationTile.IsOccupied)
                        continue;

                    // If the move is a jump, check that the piece jumped can be jumped
                    if (movement % 2 == 0)
                    {
                        var jumpedX = origin.X + ((movement / 2) * xDirection);
                        var jumpedY = origin.Y + (movement / 2);

                        var jumpedTile = Tiles[jumpedX, jumpedY];
                        if (!jumpedTile.IsOccupied || jumpedTile.Piece.Colour == piece.Colour)
                            continue;

                        jumpingMoveAvailable = true;
                        possibleMoves.Add(Move.Jumping(origin, (destX, destY), (jumpedX, jumpedY)));
                    }
                    else
                        possibleMoves.Add(Move.Simple(origin, (destX, destY)));
                }
            }

            if (jumpingMoveAvailable)
                possibleMoves.RemoveAll(x => !x.IsJumping);

            return possibleMoves;
        }

        public List<Move> GetPossibleMoves(PieceColour pieceColour)
        {
            List<Move> possibleMoves = new();
            var jumpingMoveAvailable = false;

            if (PositionToMoveAgain is not null)
            {
                var tile = Tiles[PositionToMoveAgain.X, PositionToMoveAgain.Y];
                if (!tile.IsOccupied || tile.Piece.Colour != pieceColour)
                    return possibleMoves;
                
                return GetPossibleMoves(PositionToMoveAgain);
            }
            
            for (var x = 0; x < 8; x++)
            {
                for (var y = 0; y < 8; y++)
                {
                    var tile = Tiles[x, y];
                    if (!tile.IsOccupied || tile.Piece.Colour != pieceColour)
                        continue;

                    var moves = GetPossibleMoves((x, y));
                    if (!jumpingMoveAvailable && moves.Any(m => m.IsJumping))
                        jumpingMoveAvailable = true;
                    possibleMoves.AddRange(moves);
                }
            }
            
            if (jumpingMoveAvailable)
                possibleMoves.RemoveAll(x => !x.IsJumping);
            return possibleMoves;
        }

        public bool GetIsWon(out PieceColour? winner)
        {
            var allPiecesEliminated = true;
            foreach (var tile in Tiles)
            {
                if (tile.IsOccupied && tile.Piece.Colour != ColourJustMoved)
                {
                    allPiecesEliminated = false;
                    break;
                }
            }

            if (allPiecesEliminated)
            {
                winner = ColourJustMoved;
                return true;
            }
            
            // If position to move again is not null we know there is a possible move
            // If we did this check anyway we would find the opponent has no way to move the position to move again
            if (PositionToMoveAgain is null)
            {
                var nextPlayerCanMove = false;
                
                for (var x = 0; x < 8; x++)
                {
                    for (var y = 0; y < 8; y++)
                    {
                        var tile = Tiles[x, y];
                        if (!tile.IsOccupied || tile.Piece.Colour == ColourJustMoved)
                            continue;

                        var moves = GetPossibleMoves((x, y));
                        if (moves.Any())
                        {
                            nextPlayerCanMove = true;
                            break;
                        }
                    }
                }
                
                if (!nextPlayerCanMove)
                {
                    winner = ColourJustMoved;
                    return true;
                }
            }
            
            winner = null;
            return false;
        }

        private void PromoteKings()
        {
            foreach (var y in new[]{0, 7})
            {
                var opposingPieceColour = y switch
                {
                    0 => PieceColour.White,
                    7 => PieceColour.Black,
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                for (var x = 0; x < 8; x++)
                {
                    var tile = Tiles[x, y];
                    if (!tile.IsOccupied || tile.Piece.Colour != opposingPieceColour)
                        continue;

                    tile.Piece.IsKing = true;
                }
            }
        }

        public string GetDebugString()
        {
            var debug = "";
            for (var y = 0; y < 8; y++)
            {
                for (var x = 0; x < 8; x++)
                {
                    var tile = Tiles[x, y];
                    debug += Tiles[x, y].IsOccupied switch
                    {
                        false => " ",
                        true when tile.Piece.Colour == PieceColour.White && tile.Piece.IsKing => "W",
                        true when tile.Piece.Colour == PieceColour.White && !tile.Piece.IsKing => "w",
                        true when tile.Piece.Colour == PieceColour.Black && tile.Piece.IsKing => "B",
                        true when tile.Piece.Colour == PieceColour.Black && !tile.Piece.IsKing => "b",
                        _ => throw new ArgumentOutOfRangeException()
                    };
                }
            }

            return debug;
        }
    }
}
