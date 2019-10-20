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

        public int GetPossibleMovementsCount()
        {
            return PlayerPositions.Sum(position => position.PossibleMovementElements.Count());
        }

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

            var path = AvailablePaths.FirstOrDefault(x =>x.Last.Value.Column == nextCol && x.Last.Value.Row == nextRow);
            if (ShouldConvertToQueenByPathDuringTaking(path))
            {
                checker.Type = PieceType.Queen;
            }

            CheckerElement newPosition = _dataProvider.GetElementAtPosition(nextCol, nextRow);
            CheckerElement oldPositionedChecker = _dataProvider.GetElementAtPosition(currentCol, currentRow);
            if (IsTouchedBorder(newPosition))
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

        private bool ShouldConvertToQueenByPathDuringTaking(LinkedList<CheckerElement> path)
        {
            if (path == null)
            {
                return false;
            }
            foreach (CheckerElement checkerElement in path)
            {
                if (IsTouchedBorder(checkerElement))
                {
                    return true;
                }
            }

            return false;
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
                    neighbors = GetNeighborsForQueen(checkerElement).Select(x => x.Value).ToList();
                }



                playerPosition.SetNeighbors(neighbors);
               
            }
        }

        private List<KeyValuePair<Diagonal, CheckerElement>> GetNeighborsForQueen(CheckerElement checkerElement)
        {
            var neighbors = new List<KeyValuePair<Diagonal, CheckerElement>>();

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

                    neighbors.Add(new KeyValuePair<Diagonal, CheckerElement>(Diagonal.LeftUp, element));
                    checkerRowUp++;
                }

                if (checkerRowDown - 1 >= 0 && !skipDownDiagonal)
                {
                    var element = _dataProvider.GetElementAtPosition(col, checkerRowDown - 1);
                    if (element.Side != Side.Empty)
                    {
                        skipDownDiagonal = true;
                    }

                    neighbors.Add(new KeyValuePair<Diagonal, CheckerElement>(Diagonal.LeftDown, element));
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

                    neighbors.Add(new KeyValuePair<Diagonal, CheckerElement>(Diagonal.RightUp, element));
                    checkerRowUp++;
                }

                if (checkerRowDown - 1 >= 0 && !skipDownDiagonal)
                {
                    var element = _dataProvider.GetElementAtPosition(col, checkerRowDown - 1);
                    if (element.Side != Side.Empty)
                    {
                        skipDownDiagonal = true;
                    }

                    neighbors.Add(new KeyValuePair<Diagonal, CheckerElement>(Diagonal.RightDown, element));
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
            , List<LinkedList<CheckerElement>> paths, Diagonal fromDiagonal = Diagonal.Initial)
        {
            path.AddLast(currentChecker);
            paths.Add(new LinkedList<CheckerElement>(path));
            visited.Add(currentChecker);

            List<KeyValuePair<Diagonal, CheckerElement>> neighborsForQueen = GetNeighborsForQueen(currentChecker);
            neighborsForQueen = FilterNeighborsOnOppositeDirection(neighborsForQueen,fromDiagonal);
            var otherSideNeighbors = neighborsForQueen.Where(x => x.Value.Side != Side.Empty && x.Value.Side != checkerSide);
            foreach (KeyValuePair<Diagonal, CheckerElement> otherSideNeighborPair in otherSideNeighbors)
            {
                if (path.Contains(otherSideNeighborPair.Value))
                {
                    continue;
                }
                Diagonal diagonal = otherSideNeighborPair.Key;
                CheckerElement otherSideNeighbor = otherSideNeighborPair.Value;
                List<CheckerElement> elementsAfterOpponent = GetNextElementsInDiagonal(currentChecker, otherSideNeighbor, path.First.Value);
                foreach (CheckerElement positionAfterNextChecker in elementsAfterOpponent)
                {
                    if (positionAfterNextChecker == null || (positionAfterNextChecker.Side != Side.Empty && !path.Contains(positionAfterNextChecker)))
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

                            List<KeyValuePair<Diagonal, CheckerElement>> neighborsForCycleRoot = GetNeighborsForQueen(positionAfterNextChecker);
                            foreach (var checkerElement in neighborsForCycleRoot.Where(x => x.Value.Side != Side.Empty))
                            {
                                List<CheckerElement> toVisitAgain = GetNextElementsInDiagonal(positionAfterNextChecker, checkerElement.Value);
                                foreach (CheckerElement previouslyVisitedToWalkAgain in toVisitAgain)
                                {
                                    CheckerElement firstToNotDelete = path.Last.Value;
                                    CheckerElement secondToNotDelete = path.Find(positionAfterNextChecker).Next.Value;
                                    if (previouslyVisitedToWalkAgain != null
                                        && (previouslyVisitedToWalkAgain.Side == Side.Empty)
                                        && previouslyVisitedToWalkAgain != firstToNotDelete
                                        && previouslyVisitedToWalkAgain != secondToNotDelete)
                                    {
                                        visited.Remove(previouslyVisitedToWalkAgain);
                                    }
                                }

                            }

                            path.AddLast(otherSideNeighbor);
                            SetPossibleMovementsForQueenRecursive(positionAfterNextChecker, path, visited, checkerSide, paths, diagonal);
                            path.RemoveLast();
                        }
                    }

                    bool notContainsInCycle = !cycle.Contains(positionAfterNextChecker);
                    if (notContainsInCycle)
                    {
                        path.AddLast(otherSideNeighbor);
                        SetPossibleMovementsForQueenRecursive(positionAfterNextChecker, path, visited, checkerSide, paths, diagonal);
                        path.RemoveLast();
                    }
                }
            }

            path.RemoveLast();
        }

        private List<KeyValuePair<Diagonal, CheckerElement>> FilterNeighborsOnOppositeDirection(List<KeyValuePair<Diagonal, CheckerElement>> neighborsForQueen, Diagonal fromDiagonal)
        {
            switch (fromDiagonal)
            {
                case Diagonal.LeftDown:
                    return neighborsForQueen.Where(x => x.Key != Diagonal.RightUp).ToList();
                case Diagonal.LeftUp:
                    return neighborsForQueen.Where(x => x.Key != Diagonal.RightDown).ToList();
                case Diagonal.RightUp:
                    return neighborsForQueen.Where(x => x.Key != Diagonal.LeftDown).ToList();
                case Diagonal.RightDown:
                    return neighborsForQueen.Where(x => x.Key != Diagonal.LeftUp).ToList();
                case Diagonal.Initial:
                    return neighborsForQueen;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fromDiagonal), fromDiagonal, null);
            }
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
                            if (IsTouchedBorder(positionAfterNextChecker))
                            {
                                
                              SetPossibleMovementsForQueenRecursive(positionAfterNextChecker, path, visited, checkerSide, paths);
                            }
                            else
                            {
                             SetPossibleMovementsRecursive(positionAfterNextChecker, path, visited, checkerSide, paths, cycle);
                            }
                            path.RemoveLast();
                        }
                    }

                    bool notContainsInCycle = !cycle.Contains(positionAfterNextChecker);

                    if (notContainsInCycle)
                    {
                        path.AddLast(otherSideNeighbor);

                        if (IsTouchedBorder(positionAfterNextChecker))
                        {

                            SetPossibleMovementsForQueenRecursive(positionAfterNextChecker, path, visited, checkerSide,paths);
                        }
                        else
                        {
                            SetPossibleMovementsRecursive(positionAfterNextChecker, path, visited, checkerSide, paths, cycle);
                        }

                        path.RemoveLast();
                    }

                }
            }

            path.RemoveLast();
        }

        private bool IsTouchedBorder(CheckerElement positionAfterNextChecker)
        {
            return (IsMainPLayer && positionAfterNextChecker.Row == 7) || (!IsMainPLayer && positionAfterNextChecker.Row == 0);
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

        public List<CheckerElement> GetNextElementsInDiagonal(CheckerElement playerChecker, CheckerElement otherSideNeighbor,CheckerElement rootElement = null)
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
            var emptyElementsAfterOtherChecker = new List<CheckerElement>();
            while (allElementsInDiagonalFromCurrent.Count > 0)
            {
                var value = allElementsInDiagonalFromCurrent.Dequeue();
                if (value.Side != Side.Empty && value != rootElement)
                {
                    break;
                }
                emptyElementsAfterOtherChecker.Add(value);
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
        RightDown,
        Initial
    }
}