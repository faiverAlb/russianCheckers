using System.Collections.Generic;
using System.Linq;
using RussianCheckers.Core;

namespace RussianCheckers.Game
{
    public class EmptyCellsPlayerViewModel : PlayerViewModel
    {
        private readonly EmptyUserPlayer _emptyUserPlayer;

        public EmptyCellsPlayerViewModel(EmptyUserPlayer emptyUserPlayer) : base(emptyUserPlayer, new List<CheckerElementViewModel>())
        {
            _emptyUserPlayer = emptyUserPlayer;
        }

        public override PlayerViewModel Clone(DataProvider dataProvider)
        {
            return new EmptyCellsPlayerViewModel(_emptyUserPlayer.Clone(dataProvider));
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

        public void UpdateModelValue(CheckerElementViewModel currentValue, int playerCol, int playerRow)
        {
//            CheckerModel toUpdate = _emptyUserPlayer.PlayerPositions.Single(x => x.Column == currentValue.Column && x.Row == currentValue.Row);
////            toUpdate.SetNewPosition(playerCol, playerRow);
        }
    }
}