using System;
using System.Collections.Generic;

namespace RussianCheckers.Game
{
    public abstract  class Player
    {
        private readonly DataProvider _dataProvider;
        private readonly NeighborsCalculator _neighborsCalculator;
        private readonly PathCalculator _pathCalculator;
        public List<CheckerModel> PlayerPositions { get; private set; }
        public List<LinkedList<CheckerModel>> AvailablePaths { get; private set; }

        public bool IsMainPlayer { get; private set; }
        public Side Side { get; private set; }

        public Player(DataProvider dataProvider, Side side, bool isMainPlayer)
        {
            _dataProvider = dataProvider;
            Side = side;
            IsMainPlayer = isMainPlayer;
            PlayerPositions = dataProvider.GetMySideCheckers(side);
            _neighborsCalculator = new NeighborsCalculator(dataProvider, PlayerPositions);
            _pathCalculator = new PathCalculator(_dataProvider, PlayerPositions, IsMainPlayer);
        }


        public void CalculateNeighbors()
        {
            _neighborsCalculator.CalculateNeighbors();
        }

        public void CalculateAvailablePaths()
        {
            AvailablePaths = _pathCalculator.CalculateAvailablePaths();
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