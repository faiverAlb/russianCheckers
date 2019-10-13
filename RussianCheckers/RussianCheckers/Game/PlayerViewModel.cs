using System;
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

        private void SetPossibleMovements(CheckerElement initialChecker, bool haveOtherSideNeighbor)
        {
            List<CheckerElement> oneDirectionEmptyMoves = GetSimpleEmptyMoves(initialChecker);
            if (!haveOtherSideNeighbor)
            {
                initialChecker.SetPossibleMovementElements(oneDirectionEmptyMoves);
                return;
            }

            Stack<CheckerElement> stack = new Stack<CheckerElement>();
            var currentPath = new LinkedList<CheckerElement>();
            stack.Push(initialChecker);

            var possibleMovements = new List<CheckerElement>();
            Side initialCheckerSide = initialChecker.Side;
            List<CheckerElement> visited = new List<CheckerElement>();
            var toVisitByChecker = new Dictionary<CheckerElement, List<CheckerElement>>();
            while (stack.Count > 0)
            {
                CheckerElement playerChecker = stack.Pop();
                currentPath.AddLast(playerChecker);
                visited.Add(playerChecker);
//                BuildPathToCurrentChecker(playerChecker, initialChecker, parentsList, pathsDict);
                
                foreach (CheckerElement otherSideNeighbor in playerChecker.Neighbors.Where(x => x.Side != Side.Empty && x.Side != initialCheckerSide))
                {
                    if (!toVisitByChecker.ContainsKey(playerChecker))
                    {
                        toVisitByChecker.Add(playerChecker, new List<CheckerElement>());
                    }
                    
                    CheckerElement positionAfterNextChecker = GetNextElementInDiagonal(playerChecker, otherSideNeighbor);
                    if (positionAfterNextChecker != null 
                        && positionAfterNextChecker.Side == Side.Empty
                        )
                    {
                        if (!visited.Contains(positionAfterNextChecker))
                        {
                            stack.Push(positionAfterNextChecker);
                            possibleMovements.Add(positionAfterNextChecker);
                            toVisitByChecker[playerChecker].Add(positionAfterNextChecker);

                        }
                        else
                        {
                            toVisitByChecker[playerChecker].Remove(positionAfterNextChecker);
                            var test = 123;
                        }

//                        parentsList.Add(new Tuple<CheckerElement, CheckerElement>(playerChecker, positionAfterNextChecker));
                    }
                }
            }

            if (!possibleMovements.Any())
            {
                initialChecker.SetPossibleMovementElements(oneDirectionEmptyMoves);
                return;
            }

            initialChecker.SetPossibleMovementElements(possibleMovements);
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

                            var test = 123; 

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

        private void BuildPathToCurrentChecker(CheckerElement playerChecker, CheckerElement initialChecker, List<Tuple<CheckerElement, CheckerElement>> parentsList, Dictionary<string, LinkedList<CheckerElement>> pathsDict)
        {

            Tuple<CheckerElement, CheckerElement> cur = parentsList.LastOrDefault(x => x.Item2 == playerChecker);
            if (cur == null)
            {
                return;
            }

            var pathToElements = new LinkedList<CheckerElement>();
            pathToElements.AddFirst(playerChecker);
            string res = $"({playerChecker.Column},{playerChecker.Row})";
            while (cur.Item1 != initialChecker)
            {
                cur = parentsList.Last(x => x.Item2 == cur.Item1);
                res = $"({cur.Item2.Column},{cur.Item2.Row}) - {res}";
                pathToElements.AddFirst(cur.Item2);
            }
            res = $"({cur.Item1.Column},{cur.Item1.Row}) - {res}";
            pathToElements.AddFirst(cur.Item1);


            if (pathsDict.ContainsKey(res))
            {
                return;
            }

            pathsDict.Add(res, pathToElements);

        }

        private void SetPossibleMovementsUsingQueue(CheckerElement initialChecker, bool haveOtherSideNeighbor)
        {
            List<CheckerElement> oneDirectionEmptyMoves = GetSimpleEmptyMoves(initialChecker);
            if (!haveOtherSideNeighbor)
            {
                initialChecker.SetPossibleMovementElements(oneDirectionEmptyMoves);
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
                initialChecker.SetPossibleMovementElements(oneDirectionEmptyMoves);
                return;
            }

            initialChecker.SetPossibleMovementElements(possibleMovements);
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