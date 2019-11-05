using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RussianCheckers.Core.Strategy
{
    public class MinMaxStrategy:RobotStrategy
    {
        int _searchDepth;

        public override KeyValuePair<CheckerModel, CheckerModel> GetSuggestedMove(Game initialGame, CancellationToken token)
        {
            _searchDepth = 1;
            int beta = int.MaxValue;
            int alpha = int.MinValue;
            IEnumerable<KeyValuePair<CheckerModel, CheckerModel>> allAvailableMoves = initialGame.GetAllAvailableMoves().ToList();
            var dict = new ConcurrentDictionary<int, KeyValuePair<CheckerModel, CheckerModel>>();
            if (allAvailableMoves.Count() == 1)
            {
                return allAvailableMoves.Single();
            }
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                Parallel.ForEach(allAvailableMoves, new ParallelOptions(), (availableMove, state) =>
//                foreach (var availableMove in allAvailableMoves)
                    {
                        {
                            Game newGameModel = initialGame.CreateGame();
                            newGameModel.MoveChecker(availableMove.Key, availableMove.Value);
                            int curValue = MinMove(initialGame, newGameModel, 1, alpha, beta, token);

                            dict.AddOrUpdate(curValue, availableMove, (i, pair) => availableMove);
                        }
                    }
                );

                _searchDepth++;
            }
            //            );
            KeyValuePair<CheckerModel, CheckerModel> resultMove = dict.OrderByDescending(x => x.Key).First().Value;
            return resultMove;
        }
        private int MinMove(Game initialGameViewModel, Game curGameModel, int depth, int alpha, int beta, CancellationToken token)
        {
            int calculatedStrength = DoCalculateStrength(initialGameViewModel, curGameModel);
            if (ShouldStopTheProcess(initialGameViewModel, curGameModel, depth) || token.IsCancellationRequested)
            {
                return calculatedStrength;
            }

            int best = int.MaxValue;
            IEnumerable<KeyValuePair<CheckerModel, CheckerModel>> allAvailableMoves = curGameModel.GetAllAvailableMoves();
            foreach (KeyValuePair<CheckerModel, CheckerModel> availableMove in allAvailableMoves)
            {
                Game newGameModel = curGameModel.CreateGame();
                newGameModel.MoveChecker(availableMove.Key, availableMove.Value);
                int value = MaxMove(initialGameViewModel, newGameModel, depth + 1, alpha, beta, token);
                best = Math.Min(value, best);
                beta = Math.Min(beta, best);

                if (alpha >= beta)
                {
                    break;
                }
            }
            return best;
        }

        private int MaxMove(Game initialGameViewModel, Game curGameModel, int depth, int alpha, int beta,
            CancellationToken token)
        {
            int calculatedStrength = DoCalculateStrength(initialGameViewModel, curGameModel);
            if (ShouldStopTheProcess(initialGameViewModel, curGameModel, depth) || token.IsCancellationRequested)
            {
                return calculatedStrength;
            }

            IEnumerable<KeyValuePair<CheckerModel, CheckerModel>> allAvailableMoves = curGameModel.GetAllAvailableMoves();
            int best = int.MinValue;
            
            foreach (KeyValuePair<CheckerModel, CheckerModel> availableMove in allAvailableMoves)
            {
                Game newGameModel = curGameModel.CreateGame();
                newGameModel.MoveChecker(availableMove.Key, availableMove.Value);
                int value = MinMove(initialGameViewModel, newGameModel, depth + 1, alpha, beta, token);
                best = Math.Max(best, value);
                alpha = Math.Max(alpha, best);
                if (alpha >= beta)
                {
                    break;
                }
            }

            return best;

        }


        private int DoCalculateStrength(Game initGame, Game curGame)
        {
            if (curGame.IsGameFinished  && curGame.NextMovePlayer.IsMainPlayer)
            {
                return int.MinValue;
            }
            if (curGame.IsGameFinished  && !curGame.NextMovePlayer.IsMainPlayer)
            {
                return int.MaxValue;
            }
            int strength = 0;

            strength += curGame.GetSimpleCheckersCount(false);
            strength += 10 * curGame.GetQueensCount(false);

            int checkersToBeTakenFromMainPlayer = initGame.GetSimpleCheckersCount(true) - curGame.GetSimpleCheckersCount(true);
            strength += 2 * checkersToBeTakenFromMainPlayer;
            int queensToBeTakenFromMainPlayer = Math.Abs(initGame.GetQueensCount(true) - curGame.GetQueensCount(true));
            strength += 5 * queensToBeTakenFromMainPlayer;

            strength -= curGame.GetSimpleCheckersCount(true);
            strength -= 10 * curGame.GetQueensCount(true);

            int checkersToBeTakenFromRobotPlayer = initGame.GetSimpleCheckersCount(false) - curGame.GetSimpleCheckersCount(false);
            strength -= 2 * checkersToBeTakenFromRobotPlayer;
            int queensToBeTakenFromRobotPlayer = Math.Abs(initGame.GetQueensCount(false) - curGame.GetQueensCount(false));
            strength -= 5 * queensToBeTakenFromRobotPlayer;


            Player currentPlayer = curGame.NextMovePlayer.IsMainPlayer ? curGame.GetPlayer(true) : curGame.GetPlayer(false);
            if (currentPlayer.IsMainPlayer)
            {
                foreach (var checkerElement in currentPlayer.PlayerPositions)
                {
                    strength -= CalculatePieceStrength(checkerElement, currentPlayer.IsMainPlayer);
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
            int strength = 1;

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
                // Central positions are good
                if (piece.PossibleMovementElements.Any(x => (x.Column == 2 && x.Row == 4) 
                                                            || (x.Column == 3 && x.Row == 3) 
                                                            || (x.Column == 4 && x.Row == 4)
                                                            || (x.Column == 5 && x.Row == 3)))
                {
                    strength += 3;
                }


                // It's good to become a queen
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

            bool isGoldCheckerForMainPlayer = isMainPlayer && piece.IsAtInitialPosition && piece.Column == 4 && piece.Row == 0;
            bool isGoldCheckerForSecondPlayer = !isMainPlayer && piece.IsAtInitialPosition && piece.Column == 3 && piece.Row == 7;
            if (isGoldCheckerForMainPlayer || isGoldCheckerForSecondPlayer)
            {
                strength += 5;
            }
            return strength;
        }


        private bool ShouldStopTheProcess(Game initialGameViewModel, Game curGameModel, int depth)
        {
            if ((curGameModel.IsGameFinished) || (initialGameViewModel.IsGameFinished))
            {
                return true;
            }
            if (depth >= _searchDepth)
            {
                return true;
            }

            return false;


        }


    }
}