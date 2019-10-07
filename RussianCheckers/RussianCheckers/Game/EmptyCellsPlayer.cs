using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using RussianCheckers.Game;

namespace RussianCheckers
{
    public class EmptyCellsPlayer : PlayerViewModel
    {
        public EmptyCellsPlayer(Side emptySide, PlayerViewModel playerOne, PlayerViewModel playerTwo) :base(emptySide, false)
        {
            PlayerPositions = new ObservableCollection<CheckerElement>(GetInitialPositions(emptySide, playerOne, playerTwo));
        }

        private List<CheckerElement> GetInitialPositions(Side side, PlayerViewModel playerOne, PlayerViewModel playerTwo)
        {
            var positions = new List<CheckerElement>();
            var allPositions = playerOne.PlayerPositions.ToList();
            allPositions.AddRange(playerTwo.PlayerPositions.ToList());
            
            List<int> colsForPlayer = playerOne.PlayerPositions.Select(x => x.Column).Distinct().ToList();
            colsForPlayer.AddRange(playerTwo.PlayerPositions.Select(x => x.Column).Distinct());

            for (int col = 0; col < 8; col++)
            {
                for (int row = 0; row < 8; row++)
                {
                    if (allPositions.Any(x => x.Row == row && x.Column == col))
                    {
                        continue;
                    }
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