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
            foundChecker.DeSelectPossibleMovement();
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
                //if (playerPosition.Side == Side.Empty)
                {
                 //   continue;
                }
                SetPossibleMovements(playerPosition, haveOtherSideNeighbor);
            }
        }

        private void SetPossibleMovements(CheckerElement initialChecker, bool haveOtherSideNeighbor)
        {
            if (!haveOtherSideNeighbor)
            {
                initialChecker.SetPossibleMovementElements(initialChecker.Neighbors.Where(x => x.Side == Side.Empty).ToList());
                return;
            }

            Queue<CheckerElement> processingQueue = new Queue<CheckerElement>();
            processingQueue.Enqueue(initialChecker);
            

            var possibleMovements = new List<CheckerElement>();
            int localMaximum = 0;
            int currentMaxLevel = 0;
            List<CheckerElement> passedCheckers = new List<CheckerElement>();
            Side initialCheckerSide = initialChecker.Side;
            while (processingQueue.Any())
            {
                CheckerElement playerChecker = processingQueue.Dequeue();
                passedCheckers.Add(playerChecker);
                foreach (CheckerElement otherSideNeighbor in 
                    playerChecker.Neighbors.Where(x => x.Side != Side.Empty && x.Side != initialCheckerSide))
                {
                    
                    CheckerElement positionAfterNextChecker = GetNextElementInDiagonal(playerChecker, otherSideNeighbor);
                    if (passedCheckers.Contains(positionAfterNextChecker))
                    {
                        continue;
                    }
                    if (positionAfterNextChecker != null 
                        && positionAfterNextChecker.Side == Side.Empty)
                    {
                        if (localMaximum < currentMaxLevel)
                        {
                            localMaximum = currentMaxLevel;
                            possibleMovements.Clear();
                        }
                        processingQueue.Enqueue(positionAfterNextChecker);
                        possibleMovements.Add(positionAfterNextChecker);
                    }
                }

                currentMaxLevel++;
            }
            

            if (!possibleMovements.Any())
            {
                initialChecker.SetPossibleMovementElements(initialChecker.Neighbors.Where(x => x.Side == Side.Empty).ToList());
                return;
            }

            initialChecker.SetPossibleMovementElements(possibleMovements);
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