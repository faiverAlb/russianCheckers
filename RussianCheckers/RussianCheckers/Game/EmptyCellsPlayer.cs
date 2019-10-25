using System.Collections.Generic;
using RussianCheckers.Core;

namespace RussianCheckers.Game
{
    public class EmptyCellsPlayer : PlayerViewModel
    {
        private readonly EmptyUserPlayer _emptyUserPlayer;

        public EmptyCellsPlayer(EmptyUserPlayer emptyUserPlayer) : base(emptyUserPlayer)
        {
            _emptyUserPlayer = emptyUserPlayer;
        }

//        public void AddNewEmptyElements(List<CheckerElementViewModel> itemsTakeByOtherUser)
//        {
//            foreach (CheckerElementViewModel checkerElement in itemsTakeByOtherUser)
//            {
//                CheckerElementViewModel elementViewModel = _dataProvider.GetElementAtPosition(checkerElement.Column, checkerElement.Row);
//                PlayerPositions.Add(elementViewModel);
//            }
//
//        }

        public override PlayerViewModel Clone(DataProvider dataProvider)
        {
            return new EmptyCellsPlayer(_emptyUserPlayer.Clone(dataProvider));
        }
    }
}