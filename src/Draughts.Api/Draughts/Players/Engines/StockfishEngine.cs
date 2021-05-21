using System;
using System.Collections.Generic;
using System.Linq;
using Draughts.Api.Extensions;

namespace Draughts.Api.Draughts.Players.Engines
{
    public class StockfishEngine
    {
        int _maxDepth;
        Random _random;
        PieceColour _desiredPieceColour;
        
        public StockfishEngine(int maxDepth)
        {
            _maxDepth = maxDepth;
            _random = new();
        }
        
        public (Position, Position) FindBestMove(Board board, PieceColour pieceColour)
        {
            _desiredPieceColour = pieceColour;
            int score = Max(board, pieceColour, null, _maxDepth, int.MinValue, int.MaxValue, out var bestMove);
            return bestMove;
        }

        int Max(Board board, PieceColour colourToPlay, MoveResult previousMoveResult, int depth, int alpha, int beta, out (Position, Position) bestMove)
        {
            int bestScore = int.MinValue;
            bestMove = (null, null);
            
            if (board.GetIsWon(colourToPlay.Opposite(), out PieceColour winner))
            {
                return winner == _desiredPieceColour ? int.MaxValue : int.MinValue;
            }

            if (depth == 0)
            {
                return GetScore(board);
            }
            
            List<(Position, Position)> moves = GetMoves(board, colourToPlay, previousMoveResult);
            List<(Position, Position)> bestMoves = new();

            foreach ((Position, Position) move in moves)
            {
                Board newBoard = board.Copy();
                MoveResult moveResult = newBoard.Move(move.Item1, move.Item2);
                newBoard.PromoteKings();
                newBoard.ApplyPossibleMoves();

                while (!moveResult.IsFinished)
                {
                    Max(newBoard, colourToPlay, moveResult, depth, alpha, beta, out var extraMove);
                    if (extraMove.Item1 is null) break;
                    moveResult = newBoard.Move(extraMove.Item1, extraMove.Item2);
                    newBoard.PromoteKings();
                    newBoard.ApplyPossibleMoves();
                }
                
                PieceColour nextColourToPlay = colourToPlay.Opposite();
                
                int score = Min(newBoard, nextColourToPlay, null, depth - 1, alpha, beta, out _);
                
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

        int Min(Board board, PieceColour colourToPlay, MoveResult previousMoveResult, int depth, int alpha, int beta, out (Position, Position) bestMove)
        {
            int bestScore = int.MaxValue;
            bestMove = (null, null);
            
            if (board.GetIsWon(colourToPlay.Opposite(), out PieceColour winner))
            {
                return winner == _desiredPieceColour ? int.MaxValue : int.MinValue;
            }

            if (depth == 0)
            {
                return GetScore(board);
            }
            
            List<(Position, Position)> moves = GetMoves(board, colourToPlay, previousMoveResult);
            List<(Position, Position)> bestMoves = new();
            
            foreach ((Position, Position) move in moves)
            {
                Board newBoard = board.Copy();
                MoveResult moveResult = newBoard.Move(move.Item1, move.Item2);
                newBoard.PromoteKings();
                newBoard.ApplyPossibleMoves();
                
                while (!moveResult.IsFinished)
                {
                    Max(newBoard, colourToPlay, moveResult, depth, alpha, beta, out var extraMove);
                    if (extraMove.Item1 is null) break;
                    moveResult = newBoard.Move(extraMove.Item1, extraMove.Item2);
                    newBoard.PromoteKings();
                    newBoard.ApplyPossibleMoves();
                }
                
                PieceColour nextColourToPlay = colourToPlay.Opposite();
                
                int score = Max(newBoard, nextColourToPlay, null, depth - 1, alpha, beta, out _);

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

        List<(Position, Position)> GetMoves(Board board, PieceColour pieceColour, MoveResult previousMoveResult)
        {
            List<(Position, Position)> moves = new();
            foreach (Piece piece in board.Pieces.Where(x => x.Colour == pieceColour))
                moves.AddRange(piece.PossibleMoves.Select(x => (piece.Position, x)));

            if (previousMoveResult?.PositionToMoveAgain is not null)
            {
                moves.RemoveAll(x => x.Item1 != previousMoveResult.PositionToMoveAgain);
            }
            
            return moves;
        }
        
        int GetScore(Board board)
        {
            int pieceScore = board.Pieces.Count(x => x.Colour == _desiredPieceColour) - board.Pieces.Count(x => x.Colour != _desiredPieceColour);
            int kingScore = board.Pieces.Count(x => x.Colour == _desiredPieceColour && x.IsKing) - board.Pieces.Count(x => x.Colour != _desiredPieceColour && x.IsKing);
            int possibleTakesScore = board.Pieces.Where(x => x.Colour == _desiredPieceColour).Sum(x => board.GetPiecesThatCanBeTaken(x).Count);
            int trappedPieceScore = board.Pieces.Count(x => x.Colour == _desiredPieceColour.Opposite() && x.PossibleMoves.Count == 0) - board.Pieces.Count(x => x.Colour == _desiredPieceColour && x.PossibleMoves.Count == 0);

            return pieceScore * 10 + kingScore * 5 + possibleTakesScore * 5 + trappedPieceScore;
        }
    }
}