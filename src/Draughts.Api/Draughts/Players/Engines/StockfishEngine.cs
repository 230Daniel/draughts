using System;
using System.Collections.Generic;
using Draughts.Api.Extensions;

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
            int score = Max(board, _maxDepth, int.MinValue, int.MaxValue, out var bestMove);
            Console.WriteLine(score);
            return bestMove;
        }

        int Max(Board board, int depth, int alpha, int beta, out Move bestMove)
        {
            int bestScore = int.MinValue;
            bestMove = null;
            
            if (board.GetIsWon(out PieceColour? winner) && winner.HasValue)
                return winner.Value == DesiredPieceColour ? int.MaxValue - (_maxDepth - depth) : int.MinValue + (_maxDepth - depth);

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
                    Max(newBoard, depth - 1, alpha, beta, out var extraMove);
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
                return winner.Value == DesiredPieceColour ? int.MaxValue - (_maxDepth - depth) : int.MinValue + (_maxDepth - depth);

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
                    Min(newBoard, depth - 1, alpha, beta, out var extraMove);
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
            int totalPieces = 0;
            int pieceAdvantage = 0;
            int homeRowPieces = 0;

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Tile tile = board.Tiles[x, y];
                    if (!tile.IsOccupied)
                        continue;

                    totalPieces++;
                    
                    if (tile.Piece.Colour == DesiredPieceColour)
                    {
                        pieceAdvantage += tile.Piece.IsKing ? 3 : 1;
                        
                        // If on home row
                        if (tile.Piece.Colour == PieceColour.White && y == 7 ||
                            tile.Piece.Colour == PieceColour.Black && y == 0)
                        {
                            homeRowPieces++;
                        }
                    }
                    else
                    {
                        pieceAdvantage -= tile.Piece.IsKing ? 3 : 1;
                    }
                }
            }

            // Having a 1-piece advantage when there are 3 pieces left
            // is much more significant than having a 1-piece advantage when there are 23 left
            // This means that towards the end of the game, taking a piece becomes more significant than other factors
            int percentagePieceAdvantage = (int) Math.Ceiling(((double) pieceAdvantage / totalPieces) * 100);

            int pieceAdvantageScore = percentagePieceAdvantage;
            int homeRowPiecesScore = Math.Max(0, (homeRowPieces - 2) * 10);

            return pieceAdvantageScore + homeRowPiecesScore;
        }
    }
}