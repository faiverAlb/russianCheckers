using System.Collections.Generic;
using System.Collections.ObjectModel;
using RussianCheckers.Game;

namespace RussianCheckers
{
    public class MainHumanPlayer : PlayerViewModel
    {
        public MainHumanPlayer(Side side):base(side, true)
        {
            PlayerPositions = new ObservableCollection<CheckerElement>(GetInitialPositions(side));
            PlayerPositions = new ObservableCollection<CheckerElement>(GetTestSchema(side));
        }

        private List<CheckerElement> GetTestSchema(Side side)
        {
            return new List<CheckerElement>(){new CheckerElement(4,2,PieceType.Checker,side)};
        }

        private List<CheckerElement> GetInitialPositions(Side side)
        {
            var positions = new List<CheckerElement>();
            for (int col = 0; col < 8; col++)
            {
                for (int row = 0; row < 3; row++)
                {
                    if (row % 2 == 1 && col % 2 == 1)
                    {
                        positions.Add(new CheckerElement(col, row, PieceType.Checker, side));
                        continue;
                    }
                    if (row % 2 == 0 && col % 2 == 0)
                    {
                        positions.Add(new CheckerElement(col, row, PieceType.Checker, side));
                    }
                }
            }
            return positions;
        }
    }
}