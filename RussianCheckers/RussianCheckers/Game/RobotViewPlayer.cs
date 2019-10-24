using System.Collections.Generic;
using System.Linq;
using RussianCheckers.Strategy;

namespace RussianCheckers.Game
{
    public class RobotViewPlayer : PlayerViewModel
    {
        private readonly RobotStrategy _robotStrategy;

        public RobotViewPlayer(Side side, DataProvider dataProvider, RobotStrategy robotStrategy) : base(side, dataProvider,false)
        {
            _robotStrategy = robotStrategy;
        }

        public RobotViewPlayer(Side side, DataProvider dataProvider) : this(side, dataProvider, new DummyStrategy())
        {
        }

        public KeyValuePair<CheckerElementViewModel, CheckerElementViewModel> GetOptimalMove(GameViewModel gameViewModel)
        {
            var result = _robotStrategy.GetSuggestedMove(gameViewModel);
            return result;
        }

        public override PlayerViewModel Clone(DataProvider dataProvider)
        {
            return new RobotViewPlayer(this.Side, dataProvider);
        }
    }
}