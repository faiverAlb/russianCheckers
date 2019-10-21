using System.Collections.Generic;
using System.Linq;
using RussianCheckers.Strategy;

namespace RussianCheckers.Game
{
    public class RobotPlayer : PlayerViewModel
    {
        private readonly RobotStrategy _robotStrategy;

        public RobotPlayer(Side side, DataProvider dataProvider, RobotStrategy robotStrategy) : base(side, dataProvider,false)
        {
            _robotStrategy = robotStrategy;
        }

        public RobotPlayer(Side side, DataProvider dataProvider) : this(side, dataProvider, new DummyStrategy())
        {
        }

        public KeyValuePair<CheckerElement, CheckerElement> GetOptimalMove()
        {
            var result = _robotStrategy.GetSuggestedMove(AvailablePaths.ToList(), PlayerPositions);
            return result;
        }
    }
}