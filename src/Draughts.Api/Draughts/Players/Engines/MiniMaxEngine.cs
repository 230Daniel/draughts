using System;
using Draughts.Api.Extensions;

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
            foreach (var move in board.GetPossibleMoves(board.ColourToMove))
            {
                var newBoard = board.Copy();
                var moveResult = newBoard.MovePiece(move);
                while (!moveResult.IsFinished) moveResult = newBoard.MovePiece(newBoard.GetPossibleMoves(board.ColourToMove)[0]);
                var eval = MiniMax(newBoard, _maxDepth, int.MinValue, int.MaxValue, false);
                if (eval > maxEval)
                {
                    bestMove = move;
                    maxEval = eval;
                }
            }

            return bestMove;
        }

        
        public int MiniMax(Board board, int depth, int alpha, int beta, bool maximisingPlayer)
        {
            if (board.GetIsWon(out var winner)) return winner == MyPieceColour ? int.MaxValue : int.MinValue;
            if (depth == 0) return GetScore(board);

            if (maximisingPlayer)
            {
                var maxEval = int.MinValue;
                foreach (var move in board.GetPossibleMoves(board.ColourToMove))
                {
                    var newBoard = board.Copy();
                    var moveResult = newBoard.MovePiece(move);
                    while (!moveResult.IsFinished) moveResult = newBoard.MovePiece(newBoard.GetPossibleMoves(board.ColourToMove)[0]);
                    var eval = MiniMax(newBoard, depth - 1, alpha, beta, false);
                    maxEval = Math.Max(maxEval, eval);
                    
                    alpha = Math.Max(alpha, eval);
                    if (beta <= alpha) break;
                }
                return maxEval;
            }
            
            var minEval = int.MaxValue;
            foreach (var move in board.GetPossibleMoves(board.ColourToMove))
            {
                var newBoard = board.Copy();
                var moveResult = newBoard.MovePiece(move);
                while (!moveResult.IsFinished) moveResult = newBoard.MovePiece(newBoard.GetPossibleMoves(board.ColourToMove)[0]);
                var eval = MiniMax(newBoard, depth - 1, alpha, beta, true);
                minEval = Math.Min(minEval, eval);
                    
                beta = Math.Min(beta, eval);
                if (beta <= alpha) break;
            }
            return minEval;
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
                    
                    if (tile.Piece.Colour == MyPieceColour)
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
            var homeRowPiecesScore = homeRowPieces > 2 ? 10 : 0;

            return pieceAdvantageScore + homeRowPiecesScore;
        }
    }
}
