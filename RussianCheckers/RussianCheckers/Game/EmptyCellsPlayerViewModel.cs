using System.Collections.Generic;
using RussianCheckers.Core;

namespace RussianCheckers.Game
{
    public class EmptyCellsPlayerViewModel : PlayerViewModel
    {

        public EmptyCellsPlayerViewModel(EmptyUserPlayer emptyUserPlayer) : base(emptyUserPlayer, new List<CheckerElementViewModel>())
        {
        }
    }
}