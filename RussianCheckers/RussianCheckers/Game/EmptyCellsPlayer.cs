using System.Collections.Generic;

namespace RussianCheckers.Game
{
    public class EmptyCellsPlayer : PlayerViewModel
    {
        public EmptyCellsPlayer(Side side, DataProvider dataProvider) : base(side, dataProvider, false)
        {
        }

        public void AddNewEmptyElements(List<CheckerElementViewModel> itemsTakeByOtherUser)
        {
            foreach (CheckerElementViewModel checkerElement in itemsTakeByOtherUser)
            {
                CheckerElementViewModel elementViewModel = _dataProvider.GetElementAtPosition(checkerElement.Column, checkerElement.Row);
                PlayerPositions.Add(elementViewModel);
            }

        }

        public override PlayerViewModel Clone(DataProvider dataProvider)
        {
            return new EmptyCellsPlayer(this.Side, dataProvider);
        }
    }
}