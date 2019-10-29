using System.Collections.Generic;
using System.Linq;
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
        private Side _nextMoveSide;

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

        public Side NextMoveSide
        {
            get { return _nextMoveSide; }
            private set
            {
                _nextMoveSide = value;
                NextMovePlayer = MainPlayer.Side == _nextMoveSide ? (Player)MainPlayer : RobotPlayer;

            }
        }

        public Player NextMovePlayer { get; private set; }

        public bool IsGameFinished { get; private set; }

        public IEnumerable<KeyValuePair<CheckerModel, CheckerModel>> GetAllAvailableMoves()
        {
            return NextMovePlayer.GetLegalMovements();
        }

        private bool CheckGameStatus()
        {

            var gameStatusChecker = new GameStatusChecker(_dataProvider, MainPlayer, RobotPlayer);
            Side winnerSide = gameStatusChecker.GetGameStatus();
            if (winnerSide != Side.None)
            {
                _winnerSide = winnerSide;
                IsGameFinished = true;
                return true;
            }
            return false;
        }

        public Side GetWinnerSide()
        {
            return _winnerSide;
        }

        public void ChangeTurn()
        {
            NextMoveSide = NextMoveSide == Side.White ? Side.Black : Side.White;
        }

        public Game CreateGame()
        {
            DataProvider newDataProvider = _dataProvider.Clone();
            MainPlayer newPlayerOne = MainPlayer.Clone(newDataProvider);
            RobotPlayer newViewPlayerTwo = RobotPlayer.Clone(newDataProvider);
            EmptyUserPlayer  newEmptyCellsPlayer = EmptyCellsAsPlayer.Clone(newDataProvider);
            var newGameModel = new Game(newPlayerOne, newViewPlayerTwo, newEmptyCellsPlayer, newDataProvider);
            newGameModel.NextMoveSide = NextMoveSide;
            newGameModel.ReCalculateWithRespectToOrder(newGameModel.NextMovePlayer.IsMainPlayer);
            return newGameModel;
        }


        public void MoveChecker(CheckerModel fromPlace, CheckerModel toPlace)
        {
            CheckerModel foundChecker = NextMovePlayer.PlayerPositions.SingleOrDefault(x => x.Column == fromPlace.Column && x.Row == fromPlace.Row);
            CheckerModel toPosition = EmptyCellsAsPlayer.PlayerPositions.SingleOrDefault(x => x.Column == toPlace.Column && x.Row == toPlace.Row);
            if (toPlace.Side == fromPlace.Side)
            {
                toPosition = NextMovePlayer.PlayerPositions.SingleOrDefault(x => x.Column == toPlace.Column && x.Row == toPlace.Row);
            }
            int currentCol = foundChecker.Column;
            int currentRow = foundChecker.Row;
            int nextCol = toPosition.Column;
            int nextRow = toPosition.Row;
            NextMovePlayer.MoveCheckerToNewPlace(currentCol, currentRow, nextCol, nextRow);
            ReCalculateWithRespectToOrder(NextMovePlayer.IsMainPlayer);
            bool isFinished = CheckGameStatus();
            if (!isFinished)
            {
                ChangeTurn();
            }
        }


        public Player GetPlayer(bool isMain)
        {
            if (isMain)
            {
                return MainPlayer;
            }

            return RobotPlayer;

        }

        public int GetSimpleCheckersCount(bool isForMainPlayer)
        {
            Player player = GetPlayer(isForMainPlayer);
            return player.GetSimpleCheckersCount();
        }

        public int GetQueensCount(bool isForMainPlayer)
        {
            Player player = GetPlayer(isForMainPlayer);
            return player.GetQueensCount();
        }
    }
}