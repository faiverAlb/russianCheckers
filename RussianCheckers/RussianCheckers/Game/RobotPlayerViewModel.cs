using System.Collections.Generic;
using System.Threading;
using RussianCheckers.Core;

namespace RussianCheckers.Game
{
    public class RobotPlayerViewModel : PlayerViewModel
    {
        private readonly RobotPlayer _robotPlayer;
        public RobotPlayerViewModel(RobotPlayer robotPlayer, List<CheckerElementViewModel> emptyCheckerViewModelsAsPossible) : base(robotPlayer, emptyCheckerViewModelsAsPossible)
        {
            _robotPlayer = robotPlayer;
        }

        public KeyValuePair<CheckerModel, CheckerModel> GetOptimalMove(GameViewModel gameViewModel,CancellationToken token)
        {
            KeyValuePair<CheckerModel, CheckerModel> result = _robotPlayer.GetOptimalMove(gameViewModel.Game, token);
            return result;
        }
    }
}