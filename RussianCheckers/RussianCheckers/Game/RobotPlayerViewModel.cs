using System.Collections.Generic;
using System.Linq;
using RussianCheckers.Core;
using RussianCheckers.Core.Strategy;

namespace RussianCheckers.Game
{
    public class RobotPlayerViewModel : PlayerViewModel
    {
        private readonly RobotPlayer _robotPlayer;

        public override PlayerViewModel Clone(DataProvider dataProvider)
        {
            throw new System.NotImplementedException();
        }

        public RobotPlayerViewModel(RobotPlayer robotPlayer, List<CheckerElementViewModel> emptyCheckerViewModelsAsPossible) : base(robotPlayer, emptyCheckerViewModelsAsPossible)
        {
            _robotPlayer = robotPlayer;
        }

        public KeyValuePair<CheckerModel, CheckerModel> GetOptimalMove(GameViewModel gameViewModel)
        {
            KeyValuePair<CheckerModel, CheckerModel> result = _robotPlayer.GetOptimalMove(gameViewModel.Game);
            return result;
        }
    }
}