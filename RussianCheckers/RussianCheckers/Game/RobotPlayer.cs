using System.Collections.Generic;
using System.Collections.ObjectModel;
using RussianCheckers.Game;

namespace RussianCheckers
{
    public class RobotPlayer: PlayerViewModel
    {
        public RobotPlayer(Side side, DataProvider dataProvider) : base(side, dataProvider, false)
        {
//            PlayerPositions = new ObservableCollection<CheckerElement>(GetInitialPositions(side));
//            PlayerPositions = new ObservableCollection<CheckerElement>(GetTestSchema1(side));
//            PlayerPositions = new ObservableCollection<CheckerElement>(GetTestSchema2(side));
        }
//
//        private List<CheckerElement> GetTestSchema1(Side side)
//        {
//            return new List<CheckerElement>()
//            {
//                new CheckerElement(3,3,PieceType.Checker,side),
//                new CheckerElement(5,3,PieceType.Checker,side),
//                new CheckerElement(3,5,PieceType.Checker,side),
//                new CheckerElement(5,5,PieceType.Checker,side),
////                new CheckerElement(5,1,PieceType.Checker,side),
//                new CheckerElement(1,5,PieceType.Checker,side),
//            };
//        }
//        private List<CheckerElement> GetTestSchema2(Side side)
//        {
//            return new List<CheckerElement>()
//            {
//                new CheckerElement(2,6,PieceType.Checker,side),
//                new CheckerElement(4,6,PieceType.Checker,side),
//                new CheckerElement(1,7,PieceType.Checker,side),
//                new CheckerElement(3,7,PieceType.Checker,side),
//                new CheckerElement(5,7,PieceType.Checker,side),
//            };
//        }


    }
}