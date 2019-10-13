using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RussianCheckers.Game;

namespace RussianCheckers
{
    public abstract class PlayerViewModel: ObservableObject
    {
        public bool IsMainPLayer { get; private set; }
        public readonly Side Side;
        public  ObservableCollection<CheckerElement> PlayerPositions { get; protected set; }

        protected PlayerViewModel(Side side, bool isMainPLayer)
        {
            Side = side;
            IsMainPLayer = isMainPLayer;
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
                if (checkerElement.Column - 1 >= 0)
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
                        haveOtherSideNeighbor = haveOtherSideNeighbor ||
                                                ((element.Side != playerPosition.Side) && element.Side != Side.Empty);
                        neighbors.Add(element);
                    }
                }

                if (checkerElement.Column + 1 < 8)
                {
                    if (checkerElement.Row - 1 >= 0)
                    {
                        var element = currentData[checkerElement.Column + 1, checkerElement.Row - 1];
                        haveOtherSideNeighbor = haveOtherSideNeighbor ||
                                                ((element.Side != playerPosition.Side) && element.Side != Side.Empty);
                        neighbors.Add(element);
                    }

                    if (checkerElement.Row + 1 < 8)
                    {
                        var element = currentData[checkerElement.Column + 1, checkerElement.Row + 1];
                        haveOtherSideNeighbor = haveOtherSideNeighbor ||
                                                ((element.Side != playerPosition.Side) && element.Side != Side.Empty);
                        neighbors.Add(element);
                    }
                }

                playerPosition.SetNeighbors(neighbors);
                if (playerPosition.Side == Side.Empty)
                {
                    continue;
                }

                var currentPath = new LinkedList<CheckerElement>();
                Side initialCheckerSide = playerPosition.Side;
                List<CheckerElement> visited = new List<CheckerElement>();
                List<LinkedList<CheckerElement>> paths = new List<LinkedList<CheckerElement>>();
                SetPossibleMovementsRecursive(playerPosition, currentPath, visited, initialCheckerSide, paths);
                var possibleMovements = new List<CheckerElement>();
                IGrouping<int, LinkedList<CheckerElement>> maxValues = paths.GroupBy(x => x.Count).OrderByDescending(x => x.Key).FirstOrDefault();
                foreach (var max in maxValues)
                {
                    if (max.Count == 1)
                    {
                        possibleMovements.AddRange(GetSimpleEmptyMoves(max.Last.Value));
                    }
                    else
                    {
                        possibleMovements.Add(max.Last.Value);
                    }
                }

                playerPosition.SetPossibleMovementElements(possibleMovements);
            }
        }

        private void SetPossibleMovementsRecursive(CheckerElement currentChecker
            , LinkedList<CheckerElement> path
            , List<CheckerElement> visited
            , Side checkerSide
            , List<LinkedList<CheckerElement>> paths
            , LinkedList<CheckerElement> outerCycle = null)
        {
            path.AddLast(currentChecker);
            paths.Add(new LinkedList<CheckerElement>(path));
            visited.Add(currentChecker);
            foreach (CheckerElement otherSideNeighbor in currentChecker.Neighbors.Where(x => x.Side != Side.Empty && x.Side != checkerSide))
            {
                CheckerElement positionAfterNextChecker = GetNextElementInDiagonal(currentChecker, otherSideNeighbor);
                if (positionAfterNextChecker != null 
                    && (   positionAfterNextChecker.Side == Side.Empty 
                        || path.Contains(positionAfterNextChecker)))
                {
                    if (outerCycle != null && outerCycle.Contains(positionAfterNextChecker))
                    {
                        continue;
                    }
                    if (path.Contains(positionAfterNextChecker)) // Cycle here
                    {
                        var cycle = new LinkedList<CheckerElement>();
                        int indexOfChecker = 0;
                        int index = 0;
                        foreach (var checkerElement in path)
                        {
                            cycle.AddLast(checkerElement);
                            if (checkerElement == positionAfterNextChecker)
                            {
                                indexOfChecker = index;
                            }

                            index++;
                        }

                        var len = index - indexOfChecker;
                        if (len > 2)
                        {

                            foreach (CheckerElement checkerElement in positionAfterNextChecker.Neighbors)
                            {
                                CheckerElement tempToDelete = GetNextElementInDiagonal(positionAfterNextChecker, checkerElement);
                                CheckerElement firstToNotDelete = path.Last.Value;
                                CheckerElement secondToNotDelete = path.Find(positionAfterNextChecker).Next.Value;
                                if (tempToDelete != null 
                                    && (tempToDelete.Side == Side.Empty) 
                                    && tempToDelete != firstToNotDelete 
                                    && tempToDelete != secondToNotDelete)
                                {
                                    visited.Remove(tempToDelete);
                                }

                            }
                            SetPossibleMovementsRecursive(positionAfterNextChecker, path, visited, checkerSide, paths, cycle);
                        }
                    }
                    if (!visited.Contains(positionAfterNextChecker))
                    {
                        SetPossibleMovementsRecursive(positionAfterNextChecker, path, visited, checkerSide, paths);
                    }

                }
            }

            path.RemoveLast();
        }

        private List<CheckerElement> GetSimpleEmptyMoves(CheckerElement initialChecker)
        {
            List<CheckerElement> moves = initialChecker.Neighbors.Where(x => x.Side == Side.Empty).ToList();
            if (IsMainPLayer)
            {
                return moves.Where(x => x.Row > initialChecker.Row).ToList();
            }
            return moves.Where(x => x.Row < initialChecker.Row).ToList();

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