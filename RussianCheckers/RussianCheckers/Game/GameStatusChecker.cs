using RussianCheckers.Game.GameInfrastructure;

namespace RussianCheckers.Game
{
    internal class GameStatusChecker
    {
        public GameStatus GetGameStatus()
        {
            return GameStatus.BlackWin;
        }
    }
}