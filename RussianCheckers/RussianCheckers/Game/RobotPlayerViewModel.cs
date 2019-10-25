using System.Collections.Generic;
using System.Linq;
using RussianCheckers.Strategy;

namespace RussianCheckers.Game
{
    public class RobotPlayerViewModel : PlayerViewModel
    {
        private readonly RobotPlayer _robotPlayer;
        private readonly RobotStrategy _robotStrategy;

        public RobotPlayerViewModel(RobotPlayer robotPlayer, RobotStrategy robotStrategy) : base(robotPlayer)
        {
            _robotPlayer = robotPlayer;
            _robotStrategy = robotStrategy;
        }

        public RobotPlayerViewModel(RobotPlayer robotPlayer) : this(robotPlayer, new DummyStrategy())
        {
        }

        public KeyValuePair<CheckerElementViewModel, CheckerElementViewModel> GetOptimalMove(GameViewModel gameViewModel)
        {
            var result = _robotStrategy.GetSuggestedMove(gameViewModel);
            return result;
        }

        public override PlayerViewModel Clone(DataProvider dataProvider)
        {
            return new RobotPlayerViewModel(_robotPlayer.Clone(dataProvider));
        }
    }
}