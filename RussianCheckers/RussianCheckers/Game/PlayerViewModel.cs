using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RussianCheckers.Game
{
    public abstract class PlayerViewModel: ObservableObject
    {
        public bool IsMainPLayer { get; private set; }
        public readonly Side Side;
        protected readonly DataProvider _dataProvider;
        public  ObservableCollection<CheckerElement> PlayerPositions { get; protected set; }
        public List<LinkedList<CheckerElement>> AvailablePaths { get;private set; }

        protected PlayerViewModel(Side side, DataProvider dataProvider, bool isMainPLayer)
        {
            List<CheckerElement> mySideCheckers = dataProvider.GetMySideCheckers(side);
            PlayerPositions = new ObservableCollection<CheckerElement>(mySideCheckers);
            Side = side;
            _dataProvider = dataProvider;
            IsMainPLayer = isMainPLayer;
            AvailablePaths = new List<LinkedList<CheckerElement>>();
        }


        public List<CheckerElement> MoveCheckerToNewPlace(CheckerElement checker, int nextCol, int nextRow)
        {
            int currentCol = checker.Column;
            int currentRow = checker.Row;


            CheckerElement newPosition = _dataProvider.GetElementAtPosition(nextCol, nextRow);

            
            CheckerElement oldPositionedChecker = _dataProvider.GetElementAtPosition(currentCol, currentRow);
            bool shouldTransferToQueen = newPosition.Row == 7;
            if (shouldTransferToQueen)
            {
                oldPositionedChecker.Type = PieceType.Queen;
            }
            _dataProvider.MoveCheckerToNewPosition(oldPositionedChecker, nextCol, nextRow);

            CheckerElement existingPlayerChecker = PlayerPositions.Single(x => x == checker);
            List<CheckerElement> itemsToTake = TakeCheckers(AvailablePaths, nextCol, nextRow, checker);
            existingPlayerChecker.SetNewPosition(nextCol, nextRow);

            foreach (CheckerElement checkerElement in itemsToTake)
            {
                var element = new CheckerElement(checkerElement.Column, checkerElement.Row, PieceType.Checker, Side.Empty);
                _dataProvider.MoveCheckerToNewPosition(element, checkerElement.Column, checkerElement.Row);
                
            }
            newPosition.SetNewPosition(currentCol, currentRow);
            _dataProvider.MoveCheckerToNewPosition(newPosition, currentCol, currentRow);
            
            existingPlayerChecker.DeSelectPossibleMovement();
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

        public void CalculateNeighbors()
        {
            foreach (CheckerElement playerPosition in PlayerPositions)
            {
                CheckerElement checkerElement = _dataProvider.GetElementAtPosition(playerPosition.Column, playerPosition.Row);
                List<CheckerElement> neighbors = new List<CheckerElement>();
                if (checkerElement.Type == PieceType.Checker)
                {
                    neighbors = GetNeighborsForChecker(checkerElement);
                }
                else
                {
                    neighbors = GetNeighborsForQueen(checkerElement);
                }



                playerPosition.SetNeighbors(neighbors);
               
            }
        }

        private List<CheckerElement> GetNeighborsForQueen(CheckerElement checkerElement)
        {
            var neighbors = new List<CheckerElement>();

            int checkerRowUp = checkerElement.Row;
            int checkerRowDown = checkerElement.Row;
            bool skipUpDiagonal = false;
            bool skipDownDiagonal = false;
            for (int col = checkerElement.Column - 1; col >= 0; col--)
            {
                if (checkerRowUp + 1 < 8 && !skipUpDiagonal)
                {
                    var element = _dataProvider.GetElementAtPosition(col, checkerRowUp + 1);
                    if (element.Side != Side.Empty)
                    {
                        skipUpDiagonal = true;
                    }

                    neighbors.Add(element);
                    checkerRowUp++;
                }

                if (checkerRowDown - 1 >= 0 && !skipDownDiagonal)
                {
                    var element = _dataProvider.GetElementAtPosition(col, checkerRowDown - 1);
                    if (element.Side != Side.Empty)
                    {
                        skipDownDiagonal = true;
                    }

                    neighbors.Add(element);
                    checkerRowDown--;

                }
            }

            checkerRowUp = checkerElement.Row;
            checkerRowDown = checkerElement.Row;
            skipUpDiagonal = false;
            skipDownDiagonal = false;
            for (int col = checkerElement.Column + 1; col < 8; col++)
            {
                if (checkerRowUp + 1 < 8 && !skipUpDiagonal)
                {
                    var element = _dataProvider.GetElementAtPosition(col, checkerRowUp + 1);
                    if (element.Side != Side.Empty)
                    {
                        skipUpDiagonal = true;
                    }

                    neighbors.Add(element);
                    checkerRowUp++;
                }

                if (checkerRowDown - 1 >= 0 && !skipDownDiagonal)
                {
                    var element = _dataProvider.GetElementAtPosition(col, checkerRowDown - 1);
                    if (element.Side != Side.Empty)
                    {
                        skipDownDiagonal = true;
                    }

                    neighbors.Add(element);
                    checkerRowDown--;
                }
            }

            return neighbors;
        }

        private List<CheckerElement> GetNeighborsForChecker(CheckerElement checkerElement)
        {
            var neighbors = new List<CheckerElement>();
            if (checkerElement.Column - 1 >= 0)
            {
                if (checkerElement.Row - 1 >= 0)
                {
                    var element = _dataProvider.GetElementAtPosition(checkerElement.Column - 1, checkerElement.Row - 1);
                    neighbors.Add(element);
                }

                if (checkerElement.Row + 1 < 8)
                {
                    var element = _dataProvider.GetElementAtPosition(checkerElement.Column - 1, checkerElement.Row + 1);
                    neighbors.Add(element);
                }
            }
            if (checkerElement.Column + 1 < 8)
            {
                if (checkerElement.Row - 1 >= 0)
                {
                    var element = _dataProvider.GetElementAtPosition(checkerElement.Column + 1, checkerElement.Row - 1);
                    neighbors.Add(element);
                }

                if (checkerElement.Row + 1 < 8)
                {
                    var element = _dataProvider.GetElementAtPosition(checkerElement.Column + 1, checkerElement.Row + 1);
                    neighbors.Add(element);
                }
            }
            return neighbors;
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

                List<LinkedList<CheckerElement>> paths = GetPossiblePaths(playerPosition);
                var possibleMovements = new List<CheckerElement>();
                IGrouping<int, LinkedList<CheckerElement>> maxValues = paths.GroupBy(x => x.Count).OrderByDescending(x => x.Key).FirstOrDefault();
                foreach (LinkedList<CheckerElement> max in maxValues)
                {
                    if (max.Count == 1)
                    {
                        if (playerPosition.Type == PieceType.Checker)
                        {
                            possibleMovements.AddRange(GetSimpleEmptyMoves(max.Last.Value));
                        }
                        else
                        {
                            possibleMovements.AddRange(GetSimpleEmptyMovesForQueen(playerPosition));

                        }
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

        private IEnumerable<CheckerElement> GetSimpleEmptyMovesForQueen(CheckerElement queen)
        {
            return queen.Neighbors;
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


        public List<LinkedList<CheckerElement>> GetPossiblePaths(CheckerElement playerPosition)
        {
            var paths = new List<LinkedList<CheckerElement>>();
            if (playerPosition.Type == PieceType.Checker)
            {
                SetPossibleMovementsRecursive(playerPosition, new LinkedList<CheckerElement>(), new List<CheckerElement>(),playerPosition.Side, paths);
            }
            else
            {
                if (playerPosition.Neighbors.All(x => x.Side == Side.Empty))
                {
                    foreach (var neighbor in playerPosition.Neighbors)
                    {
                        paths.Add(new LinkedList<CheckerElement>(new List<CheckerElement>(){ neighbor }));
                    }
                }
                else
                {
                    SetPossibleMovementsForQueenRecursive(playerPosition, new LinkedList<CheckerElement>(), new List<CheckerElement>(),playerPosition.Side, paths);
                }

            }
            return paths;
        }

        private void SetPossibleMovementsForQueenRecursive(CheckerElement currentChecker
            , LinkedList<CheckerElement> path
            , List<CheckerElement> visited
            , Side checkerSide
            , List<LinkedList<CheckerElement>> paths
            , LinkedList<CheckerElement> outerCycle = null)
        {
            path.AddLast(currentChecker);
            paths.Add(new LinkedList<CheckerElement>(path));
            visited.Add(currentChecker);
            
            var otherSideNeighbors = currentChecker.Neighbors.Where(x => x.Side != Side.Empty && x.Side != checkerSide);
            foreach (CheckerElement otherSideNeighbor in otherSideNeighbors)
            {
                var elementsAfterOpponent = GetNextElementsInDiagonal(currentChecker, otherSideNeighbor);
                CheckerElement positionAfterNextChecker = GetNextElementInDiagonal(currentChecker, otherSideNeighbor);
                if (positionAfterNextChecker != null
                    && (positionAfterNextChecker.Side == Side.Empty
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
            var otherSideNeighbors = currentChecker.Neighbors.Where(x => x.Side != Side.Empty && x.Side != checkerSide);

            foreach (CheckerElement otherSideNeighbor in otherSideNeighbors)
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


        public CheckerElement GetNextElementInDiagonal(CheckerElement playerChecker, CheckerElement otherSideNeighbor)
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


        public Queue<CheckerElement> GetAllElementsInDiagonalFromCurrent(CheckerElement checker, Diagonal diagonal)
        {
            switch (diagonal)
            {
                case Diagonal.LeftDown:
                    return GetAllElementsInLeftDownDiagonal(checker);
                case Diagonal.LeftUp:
                    return GetAllElementsInLeftUpDiagonal(checker);
                case Diagonal.RightUp:
                    return GetAllElementsInRightUpDiagonal(checker);
                case Diagonal.RightDown:
                    return GetAllElementsInRightDownDiagonal(checker);
                default:
                    throw new ArgumentOutOfRangeException(nameof(diagonal), diagonal, null);
            }
        }

        private Queue<CheckerElement> GetAllElementsInLeftDownDiagonal(CheckerElement checker)
        {
            int checkerRowDown = checker.Row;
            var elements = new Queue<CheckerElement>();
            for (int col = checker.Column - 1; col >= 0; col--)
            {
                if (checkerRowDown - 1 >= 0)
                {
                    var element = _dataProvider.GetElementAtPosition(col, checkerRowDown - 1);
                    elements.Enqueue(element);
                    checkerRowDown--;
                }
            }
            return elements;
        }

        private Queue<CheckerElement> GetAllElementsInRightUpDiagonal(CheckerElement checker)
        {
            var checkerRowUp = checker.Row;
            var elements = new Queue<CheckerElement>();
            for (int col = checker.Column + 1; col < 8; col++)
            {
                if (checkerRowUp + 1 < 8)
                {
                    var element = _dataProvider.GetElementAtPosition(col, checkerRowUp + 1);

                    elements.Enqueue(element);
                    checkerRowUp++;
                }
            }

            return elements;
        }
        private Queue<CheckerElement> GetAllElementsInRightDownDiagonal(CheckerElement checker)
        {
            var checkerRowDown = checker.Row;
            var elements = new Queue<CheckerElement>();
            for (int col = checker.Column + 1; col < 8; col++)
            {
                if (checkerRowDown - 1 >= 0 )
                {
                    var element = _dataProvider.GetElementAtPosition(col, checkerRowDown - 1);
                    elements.Enqueue(element);
                    checkerRowDown--;
                }
            }

            return elements;
        }
        private Queue<CheckerElement> GetAllElementsInLeftUpDiagonal(CheckerElement checker)
        {
            int checkerRowUp = checker.Row;
            var elements = new Queue<CheckerElement>();
            for (int col = checker.Column - 1; col >= 0; col--)
            {
                if (checkerRowUp + 1 < 8)
                {
                    var element = _dataProvider.GetElementAtPosition(col, checkerRowUp + 1);
                    elements.Enqueue(element);
                    checkerRowUp++;
                }
            }

            return elements;
        }

        private List<CheckerElement> GetNextElementsInDiagonal(CheckerElement playerChecker,
            CheckerElement otherSideNeighbor)
        {
            Diagonal diagonal;
            if (playerChecker.Column - otherSideNeighbor.Column > 0)
            {
                diagonal = playerChecker.Row - otherSideNeighbor.Row > 0 ? Diagonal.LeftDown : Diagonal.LeftUp;
            }
            else
            {
                diagonal = playerChecker.Row - otherSideNeighbor.Row > 0 ? Diagonal.RightDown : Diagonal.RightUp;
            }

            Queue<CheckerElement> allElementsInDiagonalFromCurrent = GetAllElementsInDiagonalFromCurrent(otherSideNeighbor, diagonal);
            if (allElementsInDiagonalFromCurrent.Count == 0 )
            {
                return new List<CheckerElement>();
            }
            var header = allElementsInDiagonalFromCurrent.Dequeue();
            var emptyElementsAfterOtherChecker = new List<CheckerElement>();
            while (header.Side == Side.Empty && allElementsInDiagonalFromCurrent.Count > 0)
            {
                emptyElementsAfterOtherChecker.Add(header);
                header = allElementsInDiagonalFromCurrent.Dequeue();
            }
            return emptyElementsAfterOtherChecker;
        }

        public void RemoveCheckers(List<CheckerElement> itemsTakeByOtherUser)
        {
            foreach (var checkerElement in itemsTakeByOtherUser)
            {
                PlayerPositions.Remove(checkerElement);
            }
        }
    }

    public enum Diagonal
    {
        LeftDown,
        LeftUp,
        RightUp,
        RightDown
    }
}