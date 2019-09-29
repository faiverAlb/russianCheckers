using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RussianCheckers.Game;

namespace RussianCheckers
{
    public abstract class PlayerViewModel: ObservableObject
    {
        public readonly Side Side;
        public  ObservableCollection<CheckerElement> PlayerPositions { get; protected set; }

        protected PlayerViewModel(Side side)
        {
            Side = side;
        }

        public void MoveCheckerToNewPlace(CheckerElement checker, int column, int row)
        {
            CheckerElement foundChecker = PlayerPositions.Single(x => x == checker);
            foundChecker.SetNewPosition(column, row);
        }

        public void SetPossibleMovementElements(List<CheckerElement> allEmptyElements)
        {
            foreach (CheckerElement playerChecker in PlayerPositions)
            {
                var emptyPositionsToCheck = new Queue<CheckerElement>(allEmptyElements);
                var checkerPossibleMoves = new List<CheckerElement>();

                while (emptyPositionsToCheck.Any())
                {
                    CheckerElement emptyElement = emptyPositionsToCheck.Dequeue();
                    if (playerChecker.Column - 1 == emptyElement.Column && playerChecker.Row - 1 == emptyElement.Row)
                    {
                        checkerPossibleMoves.Add(emptyElement);
                        continue;
                    }

                    if (playerChecker.Column - 1 == emptyElement.Column && playerChecker.Row + 1 == emptyElement.Row)
                    {
                        checkerPossibleMoves.Add(emptyElement);
                        continue;
                    }

                    if (playerChecker.Column + 1 == emptyElement.Column && playerChecker.Row - 1 == emptyElement.Row)
                    {
                        checkerPossibleMoves.Add(emptyElement);
                        continue;
                    }

                    if (playerChecker.Column + 1 == emptyElement.Column && playerChecker.Row + 1 == emptyElement.Row)
                    {
                        checkerPossibleMoves.Add(emptyElement);
                        continue;
                    }
                }

//                foreach (CheckerElement emptyElement in allEmptyElements)
//                {
//                    if (playerChecker.Type == PieceType.Checker)
//                    {
//                        //TODO: Calculate positions for simple type
//                    }
//                    else
//                    {
//                        //TODO: Calculate positions for queen type
//                    }
//                }
                playerChecker.SetPossibleMovementElements(checkerPossibleMoves);
            }
        }
    }

    public class MainHumanPlayer : PlayerViewModel
    {
        public MainHumanPlayer(Side side):base(side)
        {
            PlayerPositions = new ObservableCollection<CheckerElement>(GetInitialPositions(side));
        }

        private List<CheckerElement> GetInitialPositions(Side side)
        {
            var positions = new List<CheckerElement>();
            for (int col = 1; col <= 8; col++)
            {
                for (int row = 1; row <= 3; row++)
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

    public class RobotPlayer: PlayerViewModel
    {
        public RobotPlayer(Side side) : base(side)
        {
            PlayerPositions = new ObservableCollection<CheckerElement>(GetInitialPositions(side));

        }

        private List<CheckerElement> GetInitialPositions(Side side)
        {
            var positions = new List<CheckerElement>();
            for (int col = 1; col <= 8; col++)
            {
                for (int row = 6; row <= 8; row++)
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