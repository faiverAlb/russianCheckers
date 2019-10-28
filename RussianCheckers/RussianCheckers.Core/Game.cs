﻿namespace RussianCheckers.Core
{
    public class Game
    {
        public MainPlayer MainPlayer { get; }
        public RobotPlayer RobotPlayer { get; }
        public  EmptyUserPlayer EmptyCellsAsPlayer { get; }
        private readonly DataProvider _dataProvider;

        public Game(MainPlayer mainPlayer
            , RobotPlayer robotPlayer
            , EmptyUserPlayer emptyCellsAsPlayer
            , DataProvider dataProvider)
        {
            MainPlayer = mainPlayer;
            RobotPlayer = robotPlayer;
            EmptyCellsAsPlayer = emptyCellsAsPlayer;
            _dataProvider = dataProvider;

            NextMoveSide = Side.White;


        }

        public void ReCalculateWithRespectToOrder(bool isMainPlayerMove)
        {
            EmptyCellsAsPlayer.CalculateNeighbors();

            if (isMainPlayerMove)
            {
                MainPlayer.CalculateNeighbors();
                RobotPlayer.CalculateNeighbors();
            }
            else
            {
                RobotPlayer.CalculateNeighbors();
                MainPlayer.CalculateNeighbors();
            }

            MainPlayer.CalculateAvailablePaths();
            RobotPlayer.CalculateAvailablePaths();
        }
        public Side NextMoveSide { get; set; }
        public bool IsGameFinished { get; private set; }
    }
}