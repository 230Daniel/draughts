using System;

namespace Draughts.Api.Draughts.Players.Engines
{
    public class MiniMaxEngine
    {
        private int _maxDepth;
        public PieceColour MyPieceColour;
        
        public MiniMaxEngine(int maxDepth)
        {
            _maxDepth = maxDepth;
        }

        public Move FindBestMove(Board board)
        {
            var maxEval = int.MinValue;
            Move bestMove = null;
            foreach (var move in board.GetPossibleMoves())
            {
                var eval = RateMove(board, move, _maxDepth, int.MinValue, int.MaxValue, true);
                if (maxEval == int.MinValue || eval > maxEval)
                {
                    bestMove = move;
                    maxEval = eval;
                }
            }

            Console.WriteLine(maxEval);
            return bestMove;
        }

        private int MiniMax(Board board, int depth, int alpha, int beta, bool maximisingPlayer)
        {
            if (board.GetIsWon(out var winner)) return winner == MyPieceColour ? int.MaxValue - (_maxDepth - depth) : int.MinValue + (_maxDepth - depth);
            if (depth == 0) return GetScore(board);
            
            if (maximisingPlayer)
            {
                var maxEval = int.MinValue;
                foreach (var move in board.GetPossibleMoves())
                {
                    var eval = RateMove(board, move, depth, alpha, beta, true);
                    maxEval = Math.Max(maxEval, eval);
                    
                    alpha = Math.Max(alpha, eval);
                    if (beta <= alpha) break;
                }
                return maxEval;
            }
            
            var minEval = int.MaxValue;
            foreach (var move in board.GetPossibleMoves())
            {
                var eval = RateMove(board, move, depth, alpha, beta, false);
                minEval = Math.Min(minEval, eval);
                
                beta = Math.Min(beta, eval);
                if (beta <= alpha) break;
            }
            return minEval;
        }

        private int RateMove(Board board, Move move, int depth, int alpha, int beta, bool maximisingPlayer)
        {
            var newBoard = board.Clone();
            var moveResult = newBoard.MovePiece(move);
            
            while (!moveResult.IsFinished)
            {
                var defaultEval = maximisingPlayer ? int.MinValue : int.MaxValue;
                var bestEval = defaultEval;
                Move bestMove = null;
                var moves = newBoard.GetPossibleMoves();
                foreach (var newMove in moves)
                {
                    var eval = RateMove(newBoard, newMove, depth, alpha, beta, maximisingPlayer);
                    if (bestEval == defaultEval || (maximisingPlayer && eval > bestEval) || (!maximisingPlayer && eval < bestEval))
                    {
                        bestMove = newMove;
                        bestEval = eval;
                    }
                }
                moveResult = newBoard.MovePiece(bestMove);
            }
            
            return MiniMax(newBoard, depth - 1, alpha, beta, !maximisingPlayer);
        }
        
        private int GetScore(Board board)
        {
            var totalPieceScore = 0;
            var pieceAdvancementScore = 0;
            var kingScore = 0;
            var homeRowScore = 0;
            
            for (var x = 0; x < 8; x++)
            {
                for (var y = 0; y < 8; y++)
                {
                    var tile = board.Tiles[x, y];
                    if (!tile.IsOccupied)
                        continue;

                    totalPieceScore++;
                    var advancement = tile.Piece.Colour == PieceColour.Black ? y : 7 - y;
                    
                    if (tile.Piece.Colour == MyPieceColour)
                    {
                        if (tile.Piece.IsKing) kingScore += 10;
                        else pieceAdvancementScore += (advancement + 3);

                        if (advancement == 0 && 
                            (tile.Piece.Colour == PieceColour.Black && (x is 2 or 6) 
                             || tile.Piece.Colour == PieceColour.White && (x is 1 or 5))) 
                            homeRowScore += 5;
                    }
                    else
                    {
                        if (tile.Piece.IsKing) kingScore -= 10;
                        else pieceAdvancementScore -= (advancement + 3);
                    }
                }
            }

            return totalPieceScore + pieceAdvancementScore + kingScore + homeRowScore;
        }
    }
}
