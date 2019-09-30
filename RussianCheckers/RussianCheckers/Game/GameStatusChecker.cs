using RussianCheckers.Game.GameInfrastructure;

namespace RussianCheckers.Game
{
    internal class GameStatusChecker
    {
        private readonly CheckerElement[,] _data;

        public GameStatusChecker(CheckerElement[,] data)
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