using System.Collections.Generic;
using System.Threading;

namespace RussianCheckers.Core.Strategy
{
    public abstract class RobotStrategy
    {
        public abstract KeyValuePair<CheckerModel, CheckerModel> GetSuggestedMove(Game initialGameViewModel, CancellationToken token);
    }
}