using System.Collections.Generic;
using RussianCheckers.Core;

namespace RussianCheckers.Game
{
    public class EmptyCellsPlayer : PlayerViewModel
    {
        private readonly EmptyUserPlayer _emptyUserPlayer;

        public EmptyCellsPlayer(EmptyUserPlayer emptyUserPlayer) : base(emptyUserPlayer, new List<CheckerElementViewModel>())
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

        public void AddNewEmptyElements(List<CheckerModel> itemsTaken)
        {
            
            foreach (CheckerModel checkerElement in itemsTaken)
            {
                var model = new CheckerModel(checkerElement.Column, checkerElement.Row, checkerElement.Type, Side.Empty);
                _emptyUserPlayer.PlayerPositions.Add(model);
                CheckerElementViewModel elementViewModel = new CheckerElementViewModel(model,new List<CheckerElementViewModel>());
                PlayerPositions.Add(elementViewModel);
            }
        }
    }
}