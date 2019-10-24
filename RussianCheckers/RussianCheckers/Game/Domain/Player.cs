using System;

namespace RussianCheckers.Game
{
    public abstract  class Player
    {
        private NeighborsCalculator _neighborsCalculator;

        public Player()
        {
            _neighborsCalculator = new NeighborsCalculator();
        }
        public void CalculateNeighbors()
        {
            _neighborsCalculator.CalculateNeighbors();
        }

        public void CalculateAvailablePaths()
        {
            throw new NotImplementedException();
        }


    }


    public class Game
    {
        private readonly MainPlayer _mainPlayer;
        private readonly RobotPlayer _robotPlayer;
        private readonly EmptyUserPlayer _emptyCellsAsPlayer;
        private readonly DataProvider _dataProvider;

        public Game(MainPlayer mainPlayer
            , RobotPlayer robotPlayer
            , EmptyUserPlayer emptyCellsAsPlayer
            , DataProvider dataProvider)
        {
            _mainPlayer = mainPlayer;
            _robotPlayer = robotPlayer;
            _emptyCellsAsPlayer = emptyCellsAsPlayer;
            _dataProvider = dataProvider;

            NextMoveSide = Side.White;


            _emptyCellsAsPlayer.CalculateNeighbors();
            _mainPlayer.CalculateNeighbors();
            _robotPlayer.CalculateNeighbors();

            _mainPlayer.CalculateAvailablePaths();
            _robotPlayer.CalculateAvailablePaths();
        }

        public Side NextMoveSide { get; set; }
    }

    public class MainPlayer : Player
    {
    }
    public class EmptyUserPlayer : Player
    {
    }

    public class RobotPlayer : Player
    {

    }
}