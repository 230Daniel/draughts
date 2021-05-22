using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Draughts.Api.Extensions;

namespace Draughts.Api.Draughts
{
    public class Board
    {
        const string DefaultBoard = 
            "B B B B B " +
            " B B B B B" +
            "B B B B B " +
            "          " +
            "          " +
            " W W W W W" +
            "W W W W W " +
            " W W W W W";
        
        const string DebugBoard = 
            "          " +
            "          " +
            "          " +
            "          " +
            "          " +
            "          " +
            "          " +
            " W        ";
        
        Tile[,] Tiles { get; }

        public Board()
        {
            Tiles = new Tile[8, 8];
            char[] board = DebugBoard.ToCharArray();
            
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    Tiles[x, y] = board[y + x] switch
                    {
                        ' ' => Tile.Empty,
                        'W' => Tile.WhitePiece,
                        'B' => Tile.BlackPiece,
                        _ => Tile.Empty
                    };
                }
            }
            
            PromoteKings();
        }

        public MoveResult MovePiece(Position origin, Position destination)
        {
            // Get the piece in the specified position
            Tile tile = Tiles[origin.X, origin.Y];
            if (!tile.IsOccupied)
                return MoveResult.Invalid();
            Piece piece = tile.Piece;

            // Check that the move is possible
            List<Move> possibleMoves = GetPossibleMoves(origin);
            Move move = possibleMoves.FirstOrDefault(x => x.Destination == destination);
            if(move is null)
                return MoveResult.Invalid();

            // If the move jumps a piece and can jump another, award another move
            if (move.IsJumping)
            {
                Tiles[destination.X, destination.Y].Piece = piece;
                Tiles[origin.X, origin.Y].Piece = null;
                Tiles[move.Jumped.X, move.Jumped.Y] = null;

                List<Move> possibleExtraMoves = GetPossibleMoves(destination);
                PromoteKings();
                return possibleExtraMoves.Any(x => x.IsJumping) ? 
                    MoveResult.MoveAgain(destination) : 
                    MoveResult.FinishMove();
            }
            
            Tiles[destination.X, destination.Y].Piece = piece;
            Tiles[origin.X, origin.Y].Piece = null;
            PromoteKings();
            return MoveResult.FinishMove();
        }

        public List<Move> GetPossibleMoves(Position origin)
        {
            // Get the piece in the specified position
            Tile tile = Tiles[origin.X, origin.Y];
            if (!tile.IsOccupied) throw new InvalidOperationException("That position is not occupied");
            Piece piece = tile.Piece;

            // Depending on piece colour and king status, set valid Y movements
            int[] possibleYMovements = piece.IsKing switch
            {
                true => new[] {1, -1, 2, -2},
                false when piece.Colour == PieceColour.White => new[] {-1, -2},
                false when piece.Colour == PieceColour.Black => new[] {1, 2},
                _ => throw new InvalidOperationException("The piece in that position has invalid properties")
            };

            List<Move> possibleMoves = new();
            bool jumpingMoveAvailable = false;
            
            // Check to see if moving diagonally with each Y movement is valid
            foreach (int movement in possibleYMovements)
            {
                // We can move by this Y value in both negative and positive X
                foreach (int xDirection in new[] {-1, 1})
                {
                    int destX = origin.X + (movement * xDirection);
                    int destY = origin.Y + movement;
                
                    if (destX is < 0 or > 7 || destY is < 0 or > 7)
                        continue;
                
                    Tile destinationTile = Tiles[destX, destY];
                    if (destinationTile.IsOccupied)
                        continue;

                    // If the move is a jump, check that the piece jumped can be jumped
                    if (movement % 2 == 0)
                    {
                        int jumpedX = origin.X + ((movement / 2) * xDirection);
                        int jumpedY = origin.Y + (movement / 2);

                        Tile jumpedTile = Tiles[jumpedX, jumpedY];
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
            bool jumpingMoveAvailable = false;

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Tile tile = Tiles[x, y];
                    if (!tile.IsOccupied || tile.Piece.Colour != pieceColour)
                        continue;

                    List<Move> moves = GetPossibleMoves((x, y));
                    if (!jumpingMoveAvailable && moves.Any(m => m.IsJumping))
                        jumpingMoveAvailable = true;
                    possibleMoves.AddRange(moves);
                }
            }
            
            if (jumpingMoveAvailable)
                possibleMoves.RemoveAll(x => !x.IsJumping);
            return possibleMoves;
        }

        public bool GetIsWon(PieceColour justPlayed, out PieceColour? winner)
        {
            bool allPiecesEliminated = true;
            foreach (Tile tile in Tiles)
            {
                if (tile.IsOccupied && tile.Piece.Colour != justPlayed)
                {
                    allPiecesEliminated = false;
                    break;
                }
            }

            if (allPiecesEliminated)
            {
                winner = justPlayed;
                return true;
            }

            bool nextPlayerCanMove = false;

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Tile tile = Tiles[x, y];
                    if (!tile.IsOccupied || tile.Piece.Colour == justPlayed)
                        continue;

                    List<Move> moves = GetPossibleMoves((x, y));
                    if (moves.Any())
                    {
                        nextPlayerCanMove = true;
                        break;
                    }
                }
            }

            if (!nextPlayerCanMove)
            {
                winner = justPlayed;
                return true;
            }

            winner = null;
            return false;
        }

        void PromoteKings()
        {
            foreach (int y in new[]{0, 7})
            {
                PieceColour opposingPieceColour = y switch
                {
                    0 => PieceColour.White,
                    7 => PieceColour.Black
                };
                
                for (int x = 0; x < 8; x++)
                {
                    Tile tile = Tiles[x, y];
                    if (!tile.IsOccupied || tile.Piece.Colour != opposingPieceColour)
                        continue;

                    tile.Piece.IsKing = true;
                }
            }
        }
    }
}
