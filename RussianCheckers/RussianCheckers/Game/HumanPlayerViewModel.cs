using System.Collections.Generic;
using System.Collections.ObjectModel;
using RussianCheckers.Core;
using RussianCheckers.Game;

namespace RussianCheckers
{
    public class HumanPlayerViewModel : PlayerViewModel
    {
        private readonly MainPlayer _player;

        public HumanPlayerViewModel(MainPlayer player, List<CheckerElementViewModel> emptyCheckerViewModelsAsPossible) : base(player,emptyCheckerViewModelsAsPossible)
        {
            _player = player;
//            PlayerPositions = new ObservableCollection<CheckerElement>(GetTestSchema1(side));
//            PlayerPositions = new ObservableCollection<CheckerElement>(GetTestSchema2(side));
        }
//
//        private List<CheckerElement> GetTestSchema1(Side side)
//        {
//            return new List<CheckerElement>()
//            {
//                new CheckerElement(4,2,PieceType.Checker,side),
////                new CheckerElement(0,2,PieceType.Checker,side) 
//            };
//        }
//        private List<CheckerElement> GetTestSchema2(Side side)
//        {
//            return new List<CheckerElement>()
//            {
//                new CheckerElement(3,3,PieceType.Checker,side),
//                new CheckerElement(3,5,PieceType.Checker,side),
//                new CheckerElement(1,3,PieceType.Checker,side),
//                new CheckerElement(4,2,PieceType.Checker,side),
////                new CheckerElement(0,2,PieceType.Checker,side)
//            };
//        }

//        public override PlayerViewModel Clone(DataProvider dataProvider)
//        {
//            return new HumanPlayerViewModel(_player.Clone(dataProvider));
//        }
        public override PlayerViewModel Clone(DataProvider dataProvider)
        {
            throw new System.NotImplementedException();
        }
    }
}