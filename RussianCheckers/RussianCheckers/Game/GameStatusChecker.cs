using RussianCheckers.Game.GameInfrastructure;

namespace RussianCheckers.Game
{
    internal class GameStatusChecker
    {
        private readonly Side[,] _data;

        public GameStatusChecker(Side[,] data)
        {
            _data = data;
        }

        public GameStatus GetGameStatus()
        {
            //TODO: to do :)
            return GameStatus.InProgress;
        }
    }
}