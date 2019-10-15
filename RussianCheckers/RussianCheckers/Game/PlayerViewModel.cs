using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RussianCheckers.Game
{
    public abstract class PlayerViewModel: ObservableObject
    {
        public bool IsMainPLayer { get; private set; }
        public readonly Side Side;
        public  ObservableCollection<CheckerElement> PlayerPositions { get; protected set; }
        public List<LinkedList<CheckerElement>> AvailablePaths { get;private set; }

        protected PlayerViewModel(Side side, bool isMainPLayer)
        {
            Side = side;
            IsMainPLayer = isMainPLayer;
            AvailablePaths = new List<LinkedList<CheckerElement>>();
        }


        public List<CheckerElement> MoveCheckerToNewPlace(CheckerElement checker, int column, int row)
        {
            CheckerElement foundChecker = PlayerPositions.Single(x => x == checker);
            List<CheckerElement> itemsToTake = TakeCheckers(AvailablePaths, column, row, checker);

            foundChecker.SetNewPosition(column, row);
            foundChecker.DeSelectPossibleMovement();
            return itemsToTake;
        }

        private List<CheckerElement> TakeCheckers(List<LinkedList<CheckerElement>> availablePaths, int column, int row, CheckerElement checker)
        {
            if (!availablePaths.Any())
            {
                return new List<CheckerElement>();
            }

            LinkedList<CheckerElement> neededPath = availablePaths.FirstOrDefault(x => x.Last.Value.Column == column && x.Last.Value.Row == row);
            if (neededPath == null)
            {
                return new List<CheckerElement>();
            }

            var itemsToRemove = new List<CheckerElement>(neededPath.Where(x => x.Side != Side.Empty && x.Side != checker.Side));
            return itemsToRemove;
        }

        public void CalculateNeighbors(CheckerElement[,] currentData)
        {
            foreach (CheckerElement playerPosition in PlayerPositions)
            {
                CheckerElement checkerElement = currentData[playerPosition.Column, playerPosition.Row];
                List<CheckerElement> neighbors = new List<CheckerElement>();
                if (checkerElement.Column - 1 >= 0)
                {
                    if (checkerElement.Row - 1 >= 0)
                    {
                        var element = currentData[checkerElement.Column - 1, checkerElement.Row - 1];
                        neighbors.Add(element);
                    }

                    if (checkerElement.Row + 1 < 8)
                    {
                        var element = currentData[checkerElement.Column - 1, checkerElement.Row + 1];
                        neighbors.Add(element);
                    }
                }

                if (checkerElement.Column + 1 < 8)
                {
                    if (checkerElement.Row - 1 >= 0)
                    {
                        var element = currentData[checkerElement.Column + 1, checkerElement.Row - 1];
                        neighbors.Add(element);
                    }

                    if (checkerElement.Row + 1 < 8)
                    {
                        var element = currentData[checkerElement.Column + 1, checkerElement.Row + 1];
                        neighbors.Add(element);
                    }
                }

                playerPosition.SetNeighbors(neighbors);
               
            }
        }

        public void CalculateAvailablePaths()
        {
            AvailablePaths.Clear();
            foreach (CheckerElement playerPosition in PlayerPositions)
            {
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
                foreach (LinkedList<CheckerElement> max in maxValues)
                {
                    if (max.Count == 1)
                    {
                        possibleMovements.AddRange(GetSimpleEmptyMoves(max.Last.Value));
                    }
                    else
                    {
                        possibleMovements.Add(max.Last.Value);
                        if (AvailablePaths.Count > 0 && AvailablePaths.Max(x => x.Count) > max.Count)
                        {
                            continue;
                        }

                        AvailablePaths.Add(max);
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

                    var cycle = new LinkedList<CheckerElement>();
                    if (path.Contains(positionAfterNextChecker)) // Cycle here
                    {
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

                        int len = index - indexOfChecker;
                        if (len > 3)
                        {

                            foreach (CheckerElement checkerElement in positionAfterNextChecker.Neighbors.Where(x => x.Side != Side.Empty))
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

                            path.AddLast(otherSideNeighbor);
                            SetPossibleMovementsRecursive(positionAfterNextChecker, path, visited, checkerSide, paths, cycle);
                            path.RemoveLast();
                        }
                    }

                    bool notContainsInCycle = !cycle.Contains(positionAfterNextChecker);
//                    bool inVisitedArray = !visited.Contains(positionAfterNextChecker);
//                    bool isVisitedInPast = !IsVisitedAsPartOfSomePath(path, positionAfterNextChecker, paths); 

                    if (/*(inVisitedArray || isVisitedInPast) && */notContainsInCycle)
                    {
                        path.AddLast(otherSideNeighbor);
                        SetPossibleMovementsRecursive(positionAfterNextChecker, path, visited, checkerSide, paths);
                        path.RemoveLast();
                    }

                }
            }

            path.RemoveLast();
        }

        private bool IsVisitedAsPartOfSomePath(LinkedList<CheckerElement> currentPath, CheckerElement positionAfterNextChecker, List<LinkedList<CheckerElement>> paths)
        {
            foreach (LinkedList<CheckerElement> historyPath in paths)
            {
                var tempPath =new LinkedList<CheckerElement>(currentPath);
                tempPath.AddLast(positionAfterNextChecker);
                bool isPathsEquals = CompareLists(tempPath, historyPath);
                if (isPathsEquals)
                {
                    return true;
                }

            }

            return false;
        }

        private bool CompareLists(LinkedList<CheckerElement> tempPath, LinkedList<CheckerElement> historyPath)
        {
            if (tempPath.Count != historyPath.Count)
            {
                return false;
            }

            var currentTempHeader = tempPath.First;
            var historyHeader = historyPath.First;
            while (currentTempHeader != null)
            {
                if (currentTempHeader.Value != historyHeader.Value)
                {
                    return false;
                }
                currentTempHeader = currentTempHeader.Next;
                historyHeader = historyHeader.Next;
            }

            return true;
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

        public void RemoveCheckers(List<CheckerElement> itemsTakeByOtherUser)
        {
            foreach (var checkerElement in itemsTakeByOtherUser)
            {
                PlayerPositions.Remove(checkerElement);
            }
        }
    }
}