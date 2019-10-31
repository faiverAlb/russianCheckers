using System;
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

        public void ReCalculateNeighborsAndPaths()
        {
            EmptyCellsAsPlayer.CalculateNeighbors();
            MainPlayer.CalculateNeighbors();
            RobotPlayer.CalculateNeighbors();
            
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

        private void ChangeTurn()
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
            newGameModel.ReCalculateNeighborsAndPaths();
            return newGameModel;
        }


        public HistoryMove MoveChecker(CheckerModel fromPlace, CheckerModel toPlace)
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
            HistoryMove historyMove = NextMovePlayer.MoveCheckerToNewPlace(currentCol, currentRow, nextCol, nextRow);
            ReCalculateNeighborsAndPaths();
            bool isFinished = CheckGameStatus();
            if (!isFinished)
            {
                ChangeTurn();
            }

            return historyMove;
        }
        public void RevertMove(HistoryMove historyMove)
        {
            RevertMove(historyMove.MovedFromTo.Value, historyMove.MovedFromTo.Key, historyMove.IsConvertedToQueen);
            foreach (KeyValuePair<CheckerModel, CheckerModel> deletedInfoPair in historyMove.DeletedList)
            {
                CheckerModel recurrectedChecker = deletedInfoPair.Key;
                Player playerToAddChecker = GetPlayerByCheckerSide(recurrectedChecker.Side);
                playerToAddChecker.AddNewChecker(recurrectedChecker, deletedInfoPair.Key.Column, deletedInfoPair.Key.Row);
            }

            ReCalculateNeighborsAndPaths();
            bool isFinished = CheckGameStatus();
            if (!isFinished)
            {
                ChangeTurn();
            }

        }

        private Player GetPlayerByCheckerSide(Side checkerSide)
        {
            return MainPlayer.Side == checkerSide ? (Player) MainPlayer : RobotPlayer;
        }

        private void RevertMove(CheckerModel @from, CheckerModel to, bool historyMoveIsConvertedToQueen)
        {
            KeyValuePair<Player, CheckerModel> fromPlacePair = FindChecker(from);
            KeyValuePair<Player, CheckerModel> toPlace = FindChecker(to);
            int currentCol = fromPlacePair.Value.Column;
            int currentRow = fromPlacePair.Value.Row;
            int nextCol = toPlace.Value.Column;
            int nextRow = toPlace.Value.Row;
            fromPlacePair.Key.MoveCheckerToNewPlace(currentCol, currentRow, nextCol, nextRow, historyMoveIsConvertedToQueen);
        }


        private KeyValuePair<Player, CheckerModel> FindChecker(CheckerModel fromPlace)
        {
            int column = fromPlace.Column;
            int row = fromPlace.Row;
            var findChecker = MainPlayer.PlayerPositions.SingleOrDefault(x => x.Column == column && x.Row == row);
            if (findChecker != null)
                return new KeyValuePair<Player, CheckerModel>(MainPlayer, findChecker);
            findChecker = RobotPlayer.PlayerPositions.SingleOrDefault(x => x.Column == column && x.Row == row);
            if (findChecker != null)
                return new KeyValuePair<Player, CheckerModel>(RobotPlayer, findChecker);
            findChecker = EmptyCellsAsPlayer.PlayerPositions.SingleOrDefault(x => x.Column == column && x.Row == row);
            if (findChecker == null)
            {
                throw new Exception($"Can't find checker at position ({column},{row}) in game view model: ");
            }

            return new KeyValuePair<Player, CheckerModel>(EmptyCellsAsPlayer, findChecker);

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