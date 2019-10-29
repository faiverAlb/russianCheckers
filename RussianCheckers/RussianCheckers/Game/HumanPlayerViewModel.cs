using System.Collections.Generic;
using RussianCheckers.Core;

namespace RussianCheckers.Game
{
    public class HumanPlayerViewModel : PlayerViewModel
    {
        public HumanPlayerViewModel(MainPlayer player, List<CheckerElementViewModel> emptyCheckerViewModelsAsPossible) :base(player, emptyCheckerViewModelsAsPossible)
        {
        }
    }
}