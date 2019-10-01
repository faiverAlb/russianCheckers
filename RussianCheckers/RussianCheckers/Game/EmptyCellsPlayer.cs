using System.Collections.Generic;
using System.Collections.ObjectModel;
using RussianCheckers.Game;

namespace RussianCheckers
{
    public class EmptyCellsPlayer : PlayerViewModel
    {
        public EmptyCellsPlayer(Side side) : base(side)
        {
            PlayerPositions = new ObservableCollection<CheckerElement>(GetInitialPositions(side));

        }

        private List<CheckerElement> GetInitialPositions(Side side)
        {
            var positions = new List<CheckerElement>();
            for (int col = 0; col < 8; col++)
            {
                for (int row = 3; row < 5; row++)
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
                }
            }
            return positions;
            
        }
    }
}