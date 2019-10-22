using System.Collections.Generic;

namespace RussianCheckers.Game
{
    public class EmptyCellsPlayer : PlayerViewModel
    {
        public EmptyCellsPlayer(Side side, DataProvider dataProvider) : base(side, dataProvider, false)
        {
        }

        public void AddNewEmptyElements(List<CheckerElement> itemsTakeByOtherUser)
        {
            foreach (CheckerElement checkerElement in itemsTakeByOtherUser)
            {
                CheckerElement element = _dataProvider.GetElementAtPosition(checkerElement.Column, checkerElement.Row);
                PlayerPositions.Add(element);
            }

        }

        public override PlayerViewModel Clone(DataProvider dataProvider)
        {
            return new EmptyCellsPlayer(this.Side, dataProvider);
        }
    }
}