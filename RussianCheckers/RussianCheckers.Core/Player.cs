using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using RussianCheckers.Core.Strategy;

namespace RussianCheckers.Core
{
    public abstract  class Player
    {
        private readonly DataProvider _dataProvider;
        private readonly NeighborsCalculator _neighborsCalculator;
        private readonly PathCalculator _pathCalculator;
        public List<CheckerModel> PlayerPositions { get; }
        public Action<List<CheckerModel>, List<CheckerModel>, List<CheckerModel>> NotificationAction;

        public bool IsMainPlayer { get; }
        public Side Side { get; }

        public Player(DataProvider dataProvider, Side side, bool isMainPlayer)
        {
            _dataProvider = dataProvider;
            _dataProvider.NotificationAction += NotificationFromDataAdapter;
            Side = side;
            IsMainPlayer = isMainPlayer;
            PlayerPositions = dataProvider.GetSideCheckers(side);
            _neighborsCalculator = new NeighborsCalculator(dataProvider, PlayerPositions);
            _pathCalculator = new PathCalculator(_dataProvider, PlayerPositions, IsMainPlayer);
            
        }

        private void NotificationFromDataAdapter(List<CheckerModel> added, List<CheckerModel> deleted, List<CheckerModel> modified)
        {
            var addedForPlayer = new List<CheckerModel>();
            foreach (var addedModel in added.Where(x => x.Side == Side))
            {
                PlayerPositions.Add(addedModel);
                addedForPlayer.Add(addedModel);
            }

            var deletedFromForPlayer = new List<CheckerModel>();
            foreach (var deletedModel in deleted.Where(x => x.Side == Side))
            {
                CheckerModel toRemove = PlayerPositions.Single(x => x.Column == deletedModel.Column && x.Row == deletedModel.Row);
                PlayerPositions.Remove(toRemove);
                deletedFromForPlayer.Add(toRemove);
            }

            modified = modified.Where(x => x.Side == Side).ToList();

            NotificationAction?.Invoke(addedForPlayer, deletedFromForPlayer, modified);
        }

        public int GetPossibleMovementsCount()
        {
            return PlayerPositions.Sum(position => position.PossibleMovementElements.Count);
        }


        public void CalculateNeighbors()
        {
            Dictionary<CheckerModel, List<CheckerModel>> playerDictionary = _neighborsCalculator.CalculateNeighbors();
            foreach (KeyValuePair<CheckerModel, List<CheckerModel>> playerCheckerNeighbors in playerDictionary)
            {
                playerCheckerNeighbors.Key.SetNeighbors(playerCheckerNeighbors.Value);
            }
        }

        public IEnumerable<LinkedList<CheckerModel>> CalculateAvailablePaths()
        {
            return _pathCalculator.CalculateAvailablePaths();
        }

        public HistoryMove MoveCheckerToNewPlace(int currentCol
            , int currentRow
            , int nextCol
            , int nextRow
            , bool convertBackToChecker = false)
        {
            CheckerModel checker = PlayerPositions.Single(x => x.Column == currentCol && x.Row == currentRow);
            IEnumerable<LinkedList<CheckerModel>> availablePaths = CalculateAvailablePaths();
            LinkedList<CheckerModel> path = availablePaths.Where(x => x.Last.Value.Column == nextCol && x.Last.Value.Row == nextRow).OrderByDescending(x => x.Count).FirstOrDefault();
            bool isConvertedToQueen = false;
            if (ShouldConvertToQueenByPathDuringTaking(path))
            {
                checker.BecomeAQueen();
                isConvertedToQueen = true;
            }
            CheckerModel newPosition = _dataProvider.GetElementAtPosition(nextCol, nextRow);
            CheckerModel oldPositionedChecker = _dataProvider.GetElementAtPosition(currentCol, currentRow);
            if (convertBackToChecker)
            {
                oldPositionedChecker.DowngradeToChecker();
            }
            if (_pathCalculator.IsMoveToucheBoard(newPosition))
            {
                oldPositionedChecker.BecomeAQueen();
                isConvertedToQueen = true;
            }
            _dataProvider.StartTrackChanges();
            List<CheckerModel> itemsToTake = GetToTakeCheckers(availablePaths, nextCol, nextRow, checker);
            var historyMove = new HistoryMove(isConvertedToQueen);
            
            foreach (CheckerModel checkerElement in itemsToTake)
            {
                var element = new CheckerModel(checkerElement.Column, checkerElement.Row, PieceType.Checker, Side.Empty);
                historyMove.DeletedList.Add(new KeyValuePair<CheckerModel,CheckerModel>(checkerElement.Clone(), element.Clone()));
                _dataProvider.MoveCheckerToNewPosition(element, checkerElement.Column, checkerElement.Row);
            }
            historyMove.MovedFromTo = new KeyValuePair<CheckerModel,CheckerModel>(oldPositionedChecker.Clone(), newPosition.Clone());
            _dataProvider.MoveCheckerToNewPosition(oldPositionedChecker, nextCol, nextRow);
            _dataProvider.StopTrackChanges();

            return historyMove;
        }

        private List<CheckerModel> GetToTakeCheckers(IEnumerable<LinkedList<CheckerModel>> availablePaths, int column, int row, CheckerModel checker)
        {
            if (!availablePaths.Any())
            {
                return new List<CheckerModel>();
            }

            LinkedList<CheckerModel> neededPath = availablePaths.Where(x => x.Last.Value.Column == column && x.Last.Value.Row == row).OrderByDescending(x => x.Count).FirstOrDefault();
            if (neededPath == null)
            {
                return new List<CheckerModel>();
            }

            var itemsToRemove = new List<CheckerModel>(neededPath.Where(x => x.Side != Side.Empty && x.Side != checker.Side));
            return itemsToRemove;
        }


        private bool ShouldConvertToQueenByPathDuringTaking(LinkedList<CheckerModel> path)
        {
            if (path == null)
            {
                return false;
            }
            foreach (CheckerModel checkerElement in path)
            {
                if (_pathCalculator.IsMoveToucheBoard(checkerElement))
                {
                    return true;
                }
            }

            return false;
        }


        public IEnumerable<KeyValuePair<CheckerModel, CheckerModel>> GetLegalMovements()
        {
            IEnumerable<LinkedList<CheckerModel>> availablePaths = CalculateAvailablePaths();
            if (availablePaths.Any())
            {
                var keyValuePairs = availablePaths.Select(x =>
                    new KeyValuePair<CheckerModel, CheckerModel>(x.First.Value, x.Last.Value));
                return keyValuePairs;
            }

            var resultList = new List<KeyValuePair<CheckerModel, CheckerModel>>();
            foreach (var playerPosition in PlayerPositions)
            {
                resultList.AddRange(playerPosition.PossibleMovementElements.Select(
                    playerPositionPossibleMovementElement =>
                        new KeyValuePair<CheckerModel, CheckerModel>(playerPosition,
                            playerPositionPossibleMovementElement)));
            }

            return resultList;
        }

        public int GetSimpleCheckersCount()
        {
            int counter = PlayerPositions.Count(playerPosition => playerPosition.Type == PieceType.Checker);
            return counter;
        }

        public int GetQueensCount()
        {
            int counter = PlayerPositions.Count(playerPosition => playerPosition.Type == PieceType.Queen);
            return counter;

        }

        public void AddNewChecker(CheckerModel resurrectedChecker
                                            , int currentEmptyColumn
                                            , int currentEmptyRow)
        {
            _dataProvider.StartTrackChanges();
            _dataProvider.AddNewChecker(resurrectedChecker, currentEmptyColumn,currentEmptyRow);
            _dataProvider.StopTrackChanges();

        }
    }


    public class MainPlayer : Player
    {
        public MainPlayer(DataProvider dataProvider, Side side) : base(dataProvider, side,true)
        {
        }

        public MainPlayer Clone(DataProvider dataProvider)
        {
            return new MainPlayer(dataProvider, Side);
        }
    }
    public class EmptyUserPlayer : Player
    {
        public EmptyUserPlayer(DataProvider dataProvider) : base(dataProvider, Side.Empty, false)
        {
        }
        public EmptyUserPlayer Clone(DataProvider dataProvider)
        {
            return new EmptyUserPlayer(dataProvider);
        }

    }

    public class RobotPlayer : Player
    {
        private readonly RobotStrategy _robotStrategy;

        public RobotPlayer(DataProvider dataProvider, Side side) : base(dataProvider, side, false)
        {
        }
        public RobotPlayer(DataProvider dataProvider, Side side, RobotStrategy robotStrategy) : base(dataProvider, side, false)
        {
            _robotStrategy = robotStrategy;
        }

        public KeyValuePair<CheckerModel, CheckerModel> GetOptimalMove(Game game, CancellationToken token)
        {
            var result = _robotStrategy.GetSuggestedMove(game,token);
            return result;
        }


        public RobotPlayer Clone(DataProvider dataProvider)
        {
            return new RobotPlayer(dataProvider, Side);
        }
    }
}