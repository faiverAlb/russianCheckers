using System.Collections.Generic;
using System.Collections.ObjectModel;
using RussianCheckers.Game;

namespace RussianCheckers
{
    public class RobotPlayer: PlayerViewModel
    {
        public RobotPlayer(Side side) : base(side)
        {
            PlayerPositions = new ObservableCollection<CheckerElement>(GetInitialPositions(side));
        }

        private List<CheckerElement> GetInitialPositions(Side side)
        {
            var positions = new List<CheckerElement>();
            for (int col = 0; col < 8; col++)
            {
                for (int row = 5; row < 8; row++)
                {
                    if (row % 2 == 1 && col % 2 == 1)
                    {
                        positions.Add(new CheckerElement(col, row, PieceType.Checker, side));
                        continue;
                    }
                    if (row % 2 == 0 && col % 2 == 0)
                    {
                        positions.Add(new CheckerElement(col, row, PieceType.Checker, side));
                        continue;
                    }
                    //positions.Add(new CheckerElement(col, row, PieceType.Checker, Side.Empty));

                }
            }
            return positions;

        }

    }
}