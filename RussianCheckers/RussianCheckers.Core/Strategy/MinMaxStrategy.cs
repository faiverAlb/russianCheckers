using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RussianCheckers.Core.Strategy
{
    public class MinMaxStrategy:RobotStrategy
    {
        readonly int _searchDepth;

        public MinMaxStrategy()
        {
            _searchDepth = 8;
        }

        public override KeyValuePair<CheckerModel, CheckerModel> GetSuggestedMove(Game initialGame)
        {
            var resultMove =  new KeyValuePair<CheckerModel, CheckerModel>();
            
            const int minValue = int.MaxValue;
            int maxValue = int.MinValue;
            IEnumerable<KeyValuePair<CheckerModel, CheckerModel>> allAvailableMoves =
                initialGame.GetAllAvailableMoves().ToList();
            var dict = new ConcurrentDictionary<int, KeyValuePair<CheckerModel, CheckerModel>>();
            if (allAvailableMoves.Count() == 1)
            {
                return allAvailableMoves.Single();
            }
//            foreach (KeyValuePair<CheckerModel, CheckerModel> availableMove in allAvailableMoves)
            Parallel.ForEach(allAvailableMoves, (availableMove) =>
            {
                { 
                    Game newGameModel = initialGame.CreateGame();
                    newGameModel.MoveChecker(availableMove.Key, availableMove.Value);
                    int curValue = MinMove(initialGame, newGameModel, 1, maxValue, minValue);

                    dict.AddOrUpdate(curValue, availableMove,(i, pair) => availableMove);
//                    if ((curValue > maxValue) || (resultMove.Value == null))
//                    {
//                        maxValue = curValue;
//                        resultMove = availableMove;
//                    }
                }
            });
            resultMove = dict.OrderByDescending(x => x.Key).First().Value;
            return resultMove;
        }
        private int MinMove(Game initialGameViewModel, Game curGameModel, int depth, int alpha, int beta)
        {
            if (DoCutOff(initialGameViewModel, curGameModel, depth))
            {
                var minMoveStrength = -DoCalculateStrength(initialGameViewModel, curGameModel);
                return minMoveStrength;
            }

            IEnumerable<KeyValuePair<CheckerModel, CheckerModel>> allAvailableMoves = curGameModel.GetAllAvailableMoves();
            foreach (var availableMove in allAvailableMoves)
            {
                Game newGameModel = curGameModel.CreateGame();
                newGameModel.MoveChecker(availableMove.Key, availableMove.Value);
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

        private int DoCalculateStrength(Game initialGameViewModel, Game curGameModel)
        {
            if (curGameModel.IsGameFinished  && curGameModel.NextMovePlayer.IsMainPlayer)
            {
                return int.MinValue;
            }
            if (curGameModel.IsGameFinished  && !curGameModel.NextMovePlayer.IsMainPlayer)
            {
                return int.MaxValue;
            }
         
            return CalculateStrength(initialGameViewModel, curGameModel);
        }

        private int CalculateStrength(Game initGame, Game curGame)
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


            Player currentPlayer = curGame.NextMovePlayer.IsMainPlayer ? curGame.GetPlayer(true) : curGame.GetPlayer(false);
            if (currentPlayer.IsMainPlayer)
            {
                foreach (var checkerElement in currentPlayer.PlayerPositions)
                {
                    strength -= CalculatePieceStrength(checkerElement,currentPlayer.IsMainPlayer);
                }
            }
            else
            {
                foreach (var checkerElement in currentPlayer.PlayerPositions)
                {
                    strength += CalculatePieceStrength(checkerElement, currentPlayer.IsMainPlayer);
                }

            }
            return strength;
        }

        int CalculatePieceStrength(CheckerModel piece, bool isMainPlayer)
        {
            int strength = 0;

            // Heuristic: Stronger simply because another piece is present
            strength += 1;

            if (piece.Type == PieceType.Checker)
            {
                if (piece.IsAtInitialPosition && (piece.Row == 7 || piece.Row == 0))
                {
                    strength += 2;
                }

                if (piece.Neighbors.Any(x => x.Side == piece.Side))
                {
                    strength += 1;
                }

                // It good to become queen
                if (piece.PossibleMovementElements.Any(x => (isMainPlayer && x.Row == 7) || (!isMainPlayer && x.Row == 0)))
                {
                    strength += 15;
                }
            }
            else
            {
                //  Stronger if queen is on board
                strength += 19;
            }

            return strength;
        }


        private bool DoCutOff(Game initialGameViewModel, Game curGameModel, int depth)
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

            return false;


        }


        private int MaxMove(Game initialGameViewModel, Game curGameModel, int depth, int alpha, int beta)
        {
            // Check algorithm limits..end prematurely, but with an educated approximation
            if (DoCutOff(initialGameViewModel, curGameModel, depth))
            {
                return DoCalculateStrength(initialGameViewModel, curGameModel);
            }

            IEnumerable<KeyValuePair<CheckerModel, CheckerModel>> allAvailableMoves = curGameModel.GetAllAvailableMoves();
            foreach (var availableMove in allAvailableMoves)
            {
                Game newGameModel = curGameModel.CreateGame();
                newGameModel.MoveChecker(availableMove.Key, availableMove.Value);
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