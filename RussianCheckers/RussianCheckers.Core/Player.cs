using System;
using System.Collections.Generic;
using System.Linq;

namespace RussianCheckers.Core
{
    public abstract  class Player
    {
        private readonly DataProvider _dataProvider;
        private readonly NeighborsCalculator _neighborsCalculator;
        private readonly PathCalculator _pathCalculator;
        public List<CheckerModel> PlayerPositions { get; private set; }
        //        public IEnumerable<LinkedList<CheckerModel>> AvailablePaths { get; private set; }
        public Action<List<CheckerModel>, List<CheckerModel>> NotificationAction;

        public bool IsMainPlayer { get; private set; }
        public Side Side { get; private set; }

        public Player(DataProvider dataProvider, Side side, bool isMainPlayer)
        {
            _dataProvider = dataProvider;
            _dataProvider.NotificationAction += NotificationFromDataAdapter;
            Side = side;
            IsMainPlayer = isMainPlayer;
            PlayerPositions = dataProvider.GetMySideCheckers(side);
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
                PlayerPositions.Remove(deletedModel);
                deletedFromForPlayer.Add(deletedModel);
            }

            NotificationAction?.Invoke(addedForPlayer, deletedFromForPlayer);
        }


        public void CalculateNeighbors()
        {
            _neighborsCalculator.CalculateNeighbors();
        }

        public IEnumerable<LinkedList<CheckerModel>> CalculateAvailablePaths()
        {
            return _pathCalculator.CalculateAvailablePaths();
        }

        public void MoveCheckerToNewPlace(int currentCol, int currentRow, int nextCol, int nextRow)
        {
            CheckerModel checker = this.PlayerPositions.Single(x => x.Column == currentCol && x.Row == currentRow);
            var availablePaths = CalculateAvailablePaths();
            var path = availablePaths.Where(x => x.Last.Value.Column == nextCol && x.Last.Value.Row == nextRow).OrderByDescending(x => x.Count).FirstOrDefault();
            if (ShouldConvertToQueenByPathDuringTaking(path))
            {
                checker.BecomeAQueen();
            }
            
            CheckerModel newPosition = _dataProvider.GetElementAtPosition(nextCol, nextRow);
            CheckerModel oldPositionedChecker = _dataProvider.GetElementAtPosition(currentCol, currentRow);
            if (_pathCalculator.IsMoveToucheBoard(newPosition))
            {
                oldPositionedChecker.BecomeAQueen();
            }

            _dataProvider.StartTrackChanges();

            List<CheckerModel> itemsToTake = GetToTakeCheckers(availablePaths, nextCol, nextRow, checker);

            foreach (CheckerModel checkerElement in itemsToTake)
            {
                var element = new CheckerModel(checkerElement.Column, checkerElement.Row, PieceType.Checker, Side.Empty);
                _dataProvider.MoveCheckerToNewPosition(element, checkerElement.Column, checkerElement.Row);

            }
            _dataProvider.MoveCheckerToNewPosition(oldPositionedChecker, nextCol, nextRow);

            _dataProvider.StopTrackChanges();

//            return resultTuple;

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


        public void RemoveCheckers(List<CheckerModel> models)
        {
            foreach (var checkerModel in models)
            {
                PlayerPositions.Remove(checkerModel);
            }
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
        public RobotPlayer(DataProvider dataProvider, Side side) : base(dataProvider, side, false)
        {
        }

        public RobotPlayer Clone(DataProvider dataProvider)
        {
            return new RobotPlayer(dataProvider, Side);
        }
    }
}