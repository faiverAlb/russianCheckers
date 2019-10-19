using RussianCheckers.Game.GameInfrastructure;

namespace RussianCheckers.Game
{
    internal class GameStatusChecker
    {
        private readonly DataProvider _dataProvider;
        private readonly PlayerViewModel _playerOne;
        private readonly PlayerViewModel _playerTwo;

        public GameStatusChecker(DataProvider dataProvider, PlayerViewModel playerOne, PlayerViewModel playerTwo)
        {
            _dataProvider = dataProvider;
            _playerOne = playerOne;
            _playerTwo = playerTwo;
        }

        public GameStatus GetGameStatus()
        {
            if (_playerOne.PlayerPositions.Count == 0)
            {
                if (_playerOne.Side == Side.White)
                {
                    return GameStatus.BlackWin;
                }

                return GameStatus.WhiteWin;
            }

            if (_playerTwo.PlayerPositions.Count == 0)
            {
                if (_playerTwo.Side == Side.White)
                {
                    return GameStatus.BlackWin;
                }

                return GameStatus.WhiteWin;
            }

            if (_playerOne.GetPossibleMovementsCount() == 0)
            {
                if (_playerOne.Side == Side.White)
                {
                    return GameStatus.BlackWin;
                }

                return GameStatus.WhiteWin;
            }
            if (_playerTwo.GetPossibleMovementsCount() == 0)
            {
                if (_playerTwo.Side == Side.White)
                {
                    return GameStatus.BlackWin;
                }

                return GameStatus.WhiteWin;
            }

            return GameStatus.InProgress;
        }
    }
}