using RussianCheckers.Game.GameInfrastructure;

namespace RussianCheckers.Game
{
    internal class GameStatusChecker
    {
        private readonly DataProvider _dataProvider;

        public GameStatusChecker(DataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public GameStatus GetGameStatus()
        {
            //TODO: to do :)
            return GameStatus.InProgress;
        }
    }
}