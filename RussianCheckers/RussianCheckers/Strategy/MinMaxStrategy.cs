using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RussianCheckers.Game;

namespace RussianCheckers.Strategy
{
    public class MinMaxStrategy:RobotStrategy
    {
        readonly int _searchDepth;

        public MinMaxStrategy()
        {
            _searchDepth = 5;
        }

        //TODO: Move to library 
        public override KeyValuePair<CheckerElement, CheckerElement> GetSuggestedMove(GameViewModel initialGameViewModel)
        {
            var resultMove =  new KeyValuePair<CheckerElement, CheckerElement>();
            
            const int minValue = int.MaxValue;
            int maxValue = int.MinValue;
            IEnumerable<KeyValuePair<CheckerElement, CheckerElement>> allAvailableMoves = initialGameViewModel.GetAllAvailableMoves().ToList();
            foreach (KeyValuePair<CheckerElement, CheckerElement> availableMove in allAvailableMoves)
            {
                GameViewModel newGameModel = initialGameViewModel.CreateGame();
                newGameModel.MoveChecker(availableMove.Key, availableMove.Value);
                int curValue =  MinMove(initialGameViewModel, newGameModel, 1, maxValue, minValue);
                if ((curValue > maxValue) || (resultMove.Value == null))
                {
                    maxValue = curValue;
                    resultMove = availableMove;
                }
            }
            return resultMove;
        }

        private int MinMove(GameViewModel initialGameViewModel, GameViewModel curGameModel, int depth, int alpha, int beta)
        {

            if (DoCutOff(initialGameViewModel, curGameModel, depth))
                return -DoCalculateStrength(initialGameViewModel, curGameModel);
            
            foreach (var availableMove in curGameModel.GetAllAvailableMoves())
            {
                GameViewModel newGameModel = curGameModel.CreateGame();
                newGameModel.MoveChecker(availableMove.Key, availableMove.Value);
                //                if (!nextGameState.MovePiece(nextMoveState))
                //                    continue;
                int value = MaxMove(initialGameViewModel, newGameModel, depth + 1, alpha, beta);
                if (value < beta)
                {
                    // Get new min value
                    beta = value;
                }

                if (beta < alpha)
                {
                    // Return min value with pruning
                    return alpha;
                }

            }

            return beta;
        }

        private int DoCalculateStrength(GameViewModel initialGameViewModel, GameViewModel curGameModel)
        {
            if (curGameModel.IsGameFinished  && curGameModel.NextMovePlayer.IsMainPLayer)
            {
                return int.MinValue;
            }
            if (curGameModel.IsGameFinished  && !curGameModel.NextMovePlayer.IsMainPLayer)
            {
                return int.MaxValue;
            }
         
            return CalculateStrength(initialGameViewModel, curGameModel);
        }

        private int CalculateStrength(GameViewModel initGame, GameViewModel curGame)
        {
            int strength = 0;
            
            strength += 3 * curGame.GetSimpleCheckersCount(false);
            strength += 10 * curGame.GetQueensCount(false);

            // Heuristic: Stronger if opponent was jumped
            strength += 2 * ((initGame.GetSimpleCheckersCount(true) - curGame.GetSimpleCheckersCount(true)));
            strength += 5 * (initGame.GetQueensCount(true) - curGame.GetQueensCount(true));

            // Weakness Heuristics
            // --------------------

            // Heuristic: Weaker for each opponent pawn and king the player still has on the board
            strength -= 3 * curGame.GetSimpleCheckersCount(true);
            strength -= 10 * curGame.GetQueensCount(true);

            // Heuristic: Weaker if player was jumped
            strength -= 2 * ((initGame.GetSimpleCheckersCount(false) - curGame.GetSimpleCheckersCount(false)));
            strength -= 5 * (initGame.GetQueensCount(false) - curGame.GetQueensCount(false));

            // Return the difference of strengths
            return strength;
        }

        private bool DoCutOff(GameViewModel initialGameViewModel, GameViewModel curGameModel, int depth)
        {
            if ((curGameModel.IsGameFinished) || (initialGameViewModel.IsGameFinished))
            {
                return true;
            }
            int curSearchDepth = _searchDepth;
            //            if (increasingSearchDepth)
            //            {
            //                int totalPieces = CheckersGame.PiecesPerPlayer * CheckersGame.PlayerCount;
            //                //int removed = (CheckersGame.PiecesPerPlayer*CheckersGame.PlayerCount) - curGame.GetRemainingCount();
            //                //int factor = (int)Math.Log(removed, 3);
            //                int factor = (int)Math.Log(curGame.GetRemainingCount(), 3), mfactor = (int)Math.Log(totalPieces, 3);
            //                curSearchDepth += (mfactor - factor);
            //            }
            if ((depth >= 0) && (depth >= curSearchDepth))
                return true;

            return CutOff(initialGameViewModel, curGameModel, depth);


        }
        protected virtual bool CutOff(GameViewModel initGame, GameViewModel curGame, int depth)
        {
            return false;
        }


        private int MaxMove(GameViewModel initialGameViewModel, GameViewModel curGameModel, int depth, int alpha, int beta)
        {
            // Check algorithm limits..end prematurely, but with an educated approximation
            if (DoCutOff(initialGameViewModel, curGameModel, depth))
            {
                return DoCalculateStrength(initialGameViewModel, curGameModel);
            }

            var allAvailableMoves = curGameModel.GetAllAvailableMoves();
            foreach (var availableMove in allAvailableMoves)
            {
                GameViewModel newGameModel = curGameModel.CreateGame();
                newGameModel.MoveChecker(availableMove.Key, availableMove.Value);
                //                if (!nextGameState.MovePiece(nextMoveState))
                //                    continue;
                int value = MinMove(initialGameViewModel, newGameModel, depth + 1, alpha, beta);
                if (value > alpha)
                {
                    // Get new max value
                    alpha = value;
                }

                if (alpha > beta)
                {
                    // Return max value with pruning
                    return beta;
                }

            }

            return alpha;

        }
    }
}