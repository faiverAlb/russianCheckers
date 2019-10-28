using System.Collections.Generic;

namespace RussianCheckers.Core.Strategy
{
    public abstract class RobotStrategy
    {
        public abstract KeyValuePair<CheckerModel, CheckerModel> GetSuggestedMove(Game initialGameViewModel);
    }
}