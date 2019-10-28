using System.Collections.Generic;
using RussianCheckers.Game;

namespace RussianCheckers.Core
{
    public class Game
    {
        public MainPlayer MainPlayer { get; }
        public RobotPlayer RobotPlayer { get; }
        public  EmptyUserPlayer EmptyCellsAsPlayer { get; }
        private readonly DataProvider _dataProvider;
        private  Side _winnerSide = Side.None;
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

        public IEnumerable<KeyValuePair<CheckerModel, CheckerModel>> GetAllAvailableMoves()
        {
            return NextMovePlayer.GetLegalMovements();
        }

        public void CheckGameStatus()
        {

            var gameStatusChecker = new GameStatusChecker(_dataProvider, MainPlayer, RobotPlayer);
            Side winnerSide = gameStatusChecker.GetGameStatus();
            if (winnerSide != Side.None)
            {
                _winnerSide = winnerSide;
            }
            
        }

        public Side GetWinnerSide()
        {
            return _winnerSide;
        }
    }
}