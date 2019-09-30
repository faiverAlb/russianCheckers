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

            foreach (var element in foundChecker.PossibleMovementElements)
            {
                element.IsSelected = false;
            }
        }

        public void CalculateNeighbors(CheckerElement[,] currentData)
        {
            foreach (CheckerElement playerPosition in PlayerPositions)
            {
                bool haveOtherSideNeighbor = false;
                CheckerElement checkerElement = currentData[playerPosition.Column, playerPosition.Row];
                List<CheckerElement> neighbors = new List<CheckerElement>();
                if (checkerElement.Column -1 >= 0)
                {
                    if (checkerElement.Row - 1 >= 0)
                    {
                        var element = currentData[checkerElement.Column - 1, checkerElement.Row - 1];
                        haveOtherSideNeighbor = (element.Side != playerPosition.Side) && element.Side != Side.Empty;
                        neighbors.Add(element);
                    }
                    if (checkerElement.Row + 1 < 8)
                    {
                        var element = currentData[checkerElement.Column - 1, checkerElement.Row + 1];
                        haveOtherSideNeighbor = haveOtherSideNeighbor || ((element.Side != playerPosition.Side) && element.Side != Side.Empty);
                        neighbors.Add(element);
                    }
                }

                if (checkerElement.Column + 1 < 8)
                {
                    if (checkerElement.Row - 1 >= 0)
                    {
                        var element = currentData[checkerElement.Column + 1, checkerElement.Row - 1];
                        haveOtherSideNeighbor = haveOtherSideNeighbor || ((element.Side != playerPosition.Side) && element.Side != Side.Empty);
                        neighbors.Add(element);
                    }
                    if (checkerElement.Row + 1 < 8)
                    {
                        var element = currentData[checkerElement.Column + 1, checkerElement.Row + 1];
                        haveOtherSideNeighbor = haveOtherSideNeighbor || ((element.Side != playerPosition.Side) && element.Side != Side.Empty);
                        neighbors.Add(element);
                    }
                }

                playerPosition.SetNeighbors(neighbors);
                SetPossibleMovements(playerPosition, haveOtherSideNeighbor);
            }
        }

        private void SetPossibleMovements(CheckerElement playerChecker, bool haveOtherSideNeighbor)
        {
            if (!haveOtherSideNeighbor)
            {
                playerChecker.SetPossibleMovementElements(playerChecker.Neighbors.Where(x => x.Side == Side.Empty).ToList());
                return;
            }

            List<CheckerElement> possibleMovements = new List<CheckerElement>();

            foreach (CheckerElement otherSideNeighbor in playerChecker.Neighbors.Where(x => x.Side != Side.Empty && x.Side !=playerChecker.Side))
            {
                CheckerElement positionAfterNextChecker = GetNextElementInDiagonal(playerChecker, otherSideNeighbor);
                if (positionAfterNextChecker != null && positionAfterNextChecker.Side == Side.Empty)
                {
                    possibleMovements.Add(positionAfterNextChecker);
                }
            }

            if (!possibleMovements.Any())
            {
                playerChecker.SetPossibleMovementElements(playerChecker.Neighbors.Where(x => x.Side == Side.Empty).ToList());
                return;
            }

            playerChecker.SetPossibleMovementElements(possibleMovements);
        }

        private CheckerElement GetNextElementInDiagonal(CheckerElement playerChecker, CheckerElement otherSideNeighbor)
        {
            if (playerChecker.Column - otherSideNeighbor.Column > 0)
            {
                if (playerChecker.Row -  otherSideNeighbor.Row > 0)
                {
                    return otherSideNeighbor.Neighbors.SingleOrDefault(x => x.Column == playerChecker.Column - 2 && x.Row == playerChecker.Row - 2);
                }
                else
                {
                    return otherSideNeighbor.Neighbors.SingleOrDefault(x => x.Column == playerChecker.Column - 2 && x.Row == playerChecker.Row + 2);
                }
            }
            else
            {
                if (playerChecker.Row - otherSideNeighbor.Row > 0)
                {
                    return otherSideNeighbor.Neighbors.SingleOrDefault(x => x.Column == playerChecker.Column + 2 && x.Row == playerChecker.Row - 2);
                }
                else
                {
                    return otherSideNeighbor.Neighbors.SingleOrDefault(x => x.Column == playerChecker.Column + 2 && x.Row == playerChecker.Row + 2);
                }

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