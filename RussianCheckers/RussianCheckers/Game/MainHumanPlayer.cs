using System.Collections.Generic;
using System.Collections.ObjectModel;
using RussianCheckers.Game;

namespace RussianCheckers
{
    public class MainHumanPlayer : PlayerViewModel
    {
        public MainHumanPlayer(Side side, DataProvider dataProvider):base(side, dataProvider, true)
        {
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

        public override PlayerViewModel Clone(DataProvider dataProvider)
        {
            return new MainHumanPlayer(this.Side, dataProvider);
        }
    }
}