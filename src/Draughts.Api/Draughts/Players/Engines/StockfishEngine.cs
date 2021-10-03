using System;
using System.Collections.Generic;
using Draughts.Api.Extensions;

namespace Draughts.Api.Draughts.Players.Engines
{
    public class StockfishEngine
    {
        private int _maxDepth;
        private Random _random;
        public PieceColour DesiredPieceColour;
        
        public StockfishEngine(int maxDepth)
        {
            _maxDepth = maxDepth;
            _random = new();
        }
        
        public Move FindBestMove(Board board)
        {
            var moves = board.GetPossibleMoves(DesiredPieceColour);
            if (moves.Count == 1) return moves[0];
            if (moves.Count == 0) return null;
            var score = Max(board, _maxDepth, int.MinValue, int.MaxValue, out var bestMove);
            Console.WriteLine(score);
            return bestMove;
        }

        private int Max(Board board, int depth, int alpha, int beta, out Move bestMove)
        {
            var bestScore = int.MinValue;
            bestMove = null;
            
            if (board.GetIsWon(out var winner) && winner.HasValue)
                return winner.Value == DesiredPieceColour ? int.MaxValue - (_maxDepth - depth) : int.MinValue + (_maxDepth - depth);

            if (depth == 0)
                return GetScore(board);

            var moves = board.GetPossibleMoves(board.ColourToMove);
            List<Move> bestMoves = new();

            foreach (var move in moves)
            {
                var newBoard = board.Copy();
                var moveResult = newBoard.MovePiece(move);

                while (!moveResult.IsFinished)
                {
                    Max(newBoard, depth - 1, alpha, beta, out var extraMove);
                    if (extraMove is null) break;
                    moveResult = newBoard.MovePiece(extraMove);
                }

                var score = Min(newBoard, depth - 1, alpha, beta, out _);
                
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

        private int Min(Board board, int depth, int alpha, int beta, out Move bestMove)
        {
            var bestScore = int.MaxValue;
            bestMove = null;
            
            if (board.GetIsWon(out var winner) && winner.HasValue)
                return winner.Value == DesiredPieceColour ? int.MaxValue - (_maxDepth - depth) : int.MinValue + (_maxDepth - depth);

            if (depth == 0)
                return GetScore(board);

            var moves = board.GetPossibleMoves(board.ColourToMove);
            List<Move> bestMoves = new();
            
            foreach (var move in moves)
            {
                var newBoard = board.Copy();
                var moveResult = newBoard.MovePiece(move);

                while (!moveResult.IsFinished)
                {
                    Min(newBoard, depth - 1, alpha, beta, out var extraMove);
                    if (extraMove is null) break;
                    moveResult = newBoard.MovePiece(extraMove);
                }

                var score = Max(newBoard, depth - 1, alpha, beta, out _);

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

        private int GetScore(Board board)
        {
            var totalPieces = 0;
            var pieceAdvantage = 0;
            var homeRowPieces = 0;

            for (var x = 0; x < 8; x++)
            {
                for (var y = 0; y < 8; y++)
                {
                    var tile = board.Tiles[x, y];
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
            var percentagePieceAdvantage = (int) Math.Ceiling(((double) pieceAdvantage / totalPieces) * 100);

            var pieceAdvantageScore = percentagePieceAdvantage;
            var homeRowPiecesScore = Math.Max(0, (homeRowPieces - 2) * 10);

            return pieceAdvantageScore + homeRowPiecesScore;
        }
    }
}