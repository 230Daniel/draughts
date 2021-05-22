using System;
using System.Collections.Generic;
using Draughts.Api.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Draughts.Api.Draughts.Players.Engines
{
    public class StockfishEngine
    {
        int _maxDepth;
        Random _random;
        public PieceColour DesiredPieceColour;
        
        public StockfishEngine(int maxDepth)
        {
            _maxDepth = maxDepth;
            _random = new();
        }
        
        public Move FindBestMove(Board board)
        {
            List<Move> moves = board.GetPossibleMoves(DesiredPieceColour);
            if (moves.Count == 1) return moves[0];
            if (moves.Count == 0) return null;
            Max(board, _maxDepth, int.MinValue, int.MaxValue, out var bestMove);
            return bestMove;
        }

        int Max(Board board, int depth, int alpha, int beta, out Move bestMove)
        {
            int bestScore = int.MinValue;
            bestMove = null;
            
            if (board.GetIsWon(out PieceColour? winner) && winner.HasValue)
                return winner.Value == DesiredPieceColour ? int.MaxValue : int.MinValue;

            if (depth == 0)
                return GetScore(board);

            List<Move> moves = board.GetPossibleMoves(board.ColourToMove);
            List<Move> bestMoves = new();

            foreach (Move move in moves)
            {
                Board newBoard = board.Copy();
                MoveResult moveResult = newBoard.MovePiece(move);

                while (!moveResult.IsFinished)
                {
                    Max(newBoard, depth, alpha, beta, out var extraMove);
                    if (extraMove is null) break;
                    moveResult = newBoard.MovePiece(extraMove);
                }

                int score = Min(newBoard, depth - 1, alpha, beta, out _);
                
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMoves.Clear();
                    bestMoves.Add(move);
                }
                else if(score == bestScore)
                    bestMoves.Add(move);
                
                alpha = Math.Max(alpha, score);
                if (alpha >= beta)
                {
                    break;
                }
            }

            if (bestMoves.Count > 0) bestMove = bestMoves[_random.Next(0, bestMoves.Count)];
            else if (moves.Count != 0) bestMove = moves[_random.Next(0, moves.Count)];
            return bestScore;
        }

        int Min(Board board, int depth, int alpha, int beta, out Move bestMove)
        {
            int bestScore = int.MaxValue;
            bestMove = null;
            
            if (board.GetIsWon(out PieceColour? winner) && winner.HasValue)
                return winner.Value == DesiredPieceColour ? int.MaxValue : int.MinValue;

            if (depth == 0)
                return GetScore(board);

            List<Move> moves = board.GetPossibleMoves(board.ColourToMove);
            List<Move> bestMoves = new();
            
            foreach (Move move in moves)
            {
                Board newBoard = board.Copy();
                MoveResult moveResult = newBoard.MovePiece(move);

                while (!moveResult.IsFinished)
                {
                    Min(newBoard, depth, alpha, beta, out var extraMove);
                    if (extraMove is null) break;
                    moveResult = newBoard.MovePiece(extraMove);
                }

                int score = Max(newBoard, depth - 1, alpha, beta, out _);

                if (score < bestScore)
                {
                    bestScore = score;
                    bestMoves.Clear();
                    bestMoves.Add(move);
                }
                else if(score == bestScore)
                    bestMoves.Add(move);
                
                beta = Math.Min(beta, score);
                if (beta <= alpha)
                {
                    break;
                }
            }

            if (bestMoves.Count > 0) bestMove = bestMoves[_random.Next(0, bestMoves.Count)];
            else if (moves.Count != 0) bestMove = moves[_random.Next(0, moves.Count)];
            return bestScore;
        }

        int GetScore(Board board)
        {
            int pieceScore = 0;
            int positionScore = 0;

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Tile tile = board.Tiles[x, y];
                    if (!tile.IsOccupied)
                        continue;

                    if (tile.Piece.Colour == DesiredPieceColour)
                    {
                        pieceScore += 10;
                        positionScore += GetPositionScore(tile, y);
                    }
                    else
                    {
                        pieceScore -= 10;
                        positionScore -= GetPositionScore(tile, y);
                    }
                }
            }

            return pieceScore + positionScore;
        }

        int GetPositionScore(Tile tile, int y)
        {
            if (tile.Piece.IsKing) return 10;
            if (tile.Piece.Colour == PieceColour.White && y == 7) return 15;
            if (tile.Piece.Colour == PieceColour.Black && y == 0) return 15;
            if (tile.Piece.Colour == PieceColour.White) return 7 - y;
            return y;
        }
    }
}