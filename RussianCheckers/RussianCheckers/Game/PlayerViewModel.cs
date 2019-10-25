using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RussianCheckers.Core;

namespace RussianCheckers.Game
{
    public abstract class PlayerViewModel: ObservableObject
    {
        private readonly Player _player;
        private readonly List<CheckerElementViewModel> _emptyCheckerViewModelsAsPossible;
        public bool IsMainPLayer { get; private set; }
        public readonly Side Side;
//        protected readonly DataProvider _dataProvider;
        public  ObservableCollection<CheckerElementViewModel> PlayerPositions { get; protected set; }
//        public List<LinkedList<CheckerElementViewModel>> AvailablePaths { get;private set; }

        public int GetPossibleMovementsCount()
        {
            return PlayerPositions.Sum(position => position.PossibleMovementElements.Count());
        }

        protected PlayerViewModel(Player player, List<CheckerElementViewModel> emptyCheckerViewModelsAsPossible)
        {
            _player = player;
            _emptyCheckerViewModelsAsPossible = emptyCheckerViewModelsAsPossible;
            IEnumerable<CheckerElementViewModel> checkerElementViewModels = player.PlayerPositions.Select(x => new CheckerElementViewModel(x, emptyCheckerViewModelsAsPossible));
            PlayerPositions = new ObservableCollection<CheckerElementViewModel>(checkerElementViewModels);
            
            Side = player.Side;
            IsMainPLayer = player.IsMainPlayer;
//            AvailablePaths = new List<LinkedList<CheckerElementViewModel>>();
        }
//        protected PlayerViewModel(Player player)
//        {
//            _player = player;
//            IEnumerable<CheckerElementViewModel> checkerElementViewModels = player.PlayerPositions.Select(x => new CheckerElementViewModel(x));
//            PlayerPositions = new ObservableCollection<CheckerElementViewModel>(checkerElementViewModels);
//            
//            Side = player.Side;
//            IsMainPLayer = player.IsMainPlayer;
////            AvailablePaths = new List<LinkedList<CheckerElementViewModel>>();
//        }

//        public IEnumerable<KeyValuePair<CheckerElementViewModel, CheckerElementViewModel>> GetLegalMovements()
//        {
//            if (AvailablePaths.Any())
//            {
//                var keyValuePairs = AvailablePaths.Select(x => new KeyValuePair<CheckerElementViewModel, CheckerElementViewModel>(x.First.Value, x.Last.Value));
//                return keyValuePairs;
//            }
//
//            var resultList = new List<KeyValuePair<CheckerElementViewModel, CheckerElementViewModel>>();
//            foreach (var playerPosition in PlayerPositions)
//            {
//                resultList.AddRange(playerPosition.PossibleMovementElements.Select(playerPositionPossibleMovementElement => new KeyValuePair<CheckerElementViewModel, CheckerElementViewModel>(playerPosition, playerPositionPossibleMovementElement)));
//            }
//            return resultList;
//        }

//        public List<CheckerElementViewModel> MoveCheckerToNewPlace(CheckerElementViewModel checker, int nextCol, int nextRow)
//        {
//            int currentCol = checker.Column;
//            int currentRow = checker.Row;
//
//            var path = AvailablePaths.Where(x =>x.Last.Value.Column == nextCol && x.Last.Value.Row == nextRow).OrderByDescending(x => x.Count).FirstOrDefault();
//            if (ShouldConvertToQueenByPathDuringTaking(path))
//            {
//                checker.Type = PieceType.Queen;
//            }
//
//            CheckerElementViewModel newPosition = _dataProvider.GetElementAtPosition(nextCol, nextRow);
//            CheckerElementViewModel oldPositionedChecker = _dataProvider.GetElementAtPosition(currentCol, currentRow);
//            if (IsTouchedBorder(newPosition))
//            {
//                oldPositionedChecker.Type = PieceType.Queen;
//            }
//
//
//            _dataProvider.MoveCheckerToNewPosition(oldPositionedChecker, nextCol, nextRow);
//
//            CheckerElementViewModel existingPlayerChecker = PlayerPositions.Single(x => x == checker);
//            List<CheckerElementViewModel> itemsToTake = TakeCheckers(AvailablePaths, nextCol, nextRow, checker);
//            existingPlayerChecker.SetNewPosition(nextCol, nextRow);
//
//            foreach (CheckerElementViewModel checkerElement in itemsToTake)
//            {
//                var element = new CheckerElementViewModel(checkerElement.Column, checkerElement.Row, PieceType.Checker, Side.Empty);
//                _dataProvider.MoveCheckerToNewPosition(element, checkerElement.Column, checkerElement.Row);
//                
//            }
//            newPosition.SetNewPosition(currentCol, currentRow);
//            _dataProvider.MoveCheckerToNewPosition(newPosition, currentCol, currentRow);
//            
//            existingPlayerChecker.DeSelectPossibleMovement();
//            return itemsToTake;
//        }

        private bool ShouldConvertToQueenByPathDuringTaking(LinkedList<CheckerElementViewModel> path)
        {
            if (path == null)
            {
                return false;
            }
            foreach (CheckerElementViewModel checkerElement in path)
            {
                if (IsTouchedBorder(checkerElement))
                {
                    return true;
                }
            }

            return false;
        }

        private List<CheckerElementViewModel> TakeCheckers(List<LinkedList<CheckerElementViewModel>> availablePaths, int column, int row, CheckerElementViewModel checker)
        {
            if (!availablePaths.Any())
            {
                return new List<CheckerElementViewModel>();
            }

            LinkedList<CheckerElementViewModel> neededPath = availablePaths.Where(x => x.Last.Value.Column == column && x.Last.Value.Row == row).OrderByDescending(x => x.Count).FirstOrDefault();
            if (neededPath == null)
            {
                return new List<CheckerElementViewModel>();
            }

            var itemsToRemove = new List<CheckerElementViewModel>(neededPath.Where(x => x.Side != Side.Empty && x.Side != checker.Side));
            return itemsToRemove;
        }

//        public void CalculateNeighbors()
//        {
//            foreach (CheckerElementViewModel playerPosition in PlayerPositions)
//            {
//                CheckerElementViewModel checkerElementViewModel = _dataProvider.GetElementAtPosition(playerPosition.Column, playerPosition.Row);
//                List<CheckerElementViewModel> neighbors = new List<CheckerElementViewModel>();
//                if (checkerElementViewModel.Type == PieceType.Checker)
//                {
//                    neighbors = GetNeighborsForChecker(checkerElementViewModel);
//                }
//                else
//                {
//                    neighbors = GetNeighborsForQueen(checkerElementViewModel).Select(x => x.Value).ToList();
//                }
//
//
//
//                playerPosition.SetNeighbors(neighbors);
//               
//            }
//        }



//        public void CalculateAvailablePaths()
//        {
//            AvailablePaths.Clear();
//            foreach (CheckerElementViewModel playerPosition in PlayerPositions)
//            {
//                if (playerPosition.Side == Side.Empty)
//                {
//                    continue;
//                }
//
//                List<LinkedList<CheckerElementViewModel>> paths = GetPossiblePaths(playerPosition);
//                var possibleMovements = new List<CheckerElementViewModel>();
//                
//                foreach (LinkedList<CheckerElementViewModel> max in paths)
//                {
//                    if (max.Count == 1)
//                    {
//                        continue;
//                    }
//
//                    possibleMovements.Add(max.Last.Value);
//                    AvailablePaths.Add(max);
//
//                }
//
//                if (possibleMovements.Count == 0)
//                {
//                    if (playerPosition.Type == PieceType.Checker)
//                    {
//                        foreach (LinkedList<CheckerElementViewModel> path in paths)
//                        {
//                            possibleMovements.AddRange(playerPosition.Type == PieceType.Checker
//                                ? GetSimpleEmptyMoves(path.Last.Value)
//                                : GetSimpleEmptyMovesForQueen(playerPosition));
//                        }
//                    }
//                    else
//                    {
//                        possibleMovements.AddRange(GetSimpleEmptyMovesForQueen(playerPosition));
//                    }
//                }
//                playerPosition.SetPossibleMovementElements(possibleMovements);
//            }
//        }

//        private IEnumerable<CheckerElementViewModel> GetSimpleEmptyMovesForQueen(CheckerElementViewModel queen)
//        {
//            return queen.Neighbors.Where(x => x.Side == Side.Empty);
//        }
//
//        private List<CheckerElementViewModel> GetSimpleEmptyMoves(CheckerElementViewModel initialChecker)
//        {
//            List<CheckerElementViewModel> moves = initialChecker.Neighbors.Where(x => x.Side == Side.Empty).ToList();
//            if (IsMainPLayer)
//            {
//                return moves.Where(x => x.Row > initialChecker.Row).ToList();
//            }
//            return moves.Where(x => x.Row < initialChecker.Row).ToList();
//
//        }


//        public List<LinkedList<CheckerElementViewModel>> GetPossiblePaths(CheckerElementViewModel playerPosition)
//        {
//            var paths = new List<LinkedList<CheckerElementViewModel>>();
//            if (playerPosition.Type == PieceType.Checker)
//            {
//                SetPossibleMovementsRecursive(playerPosition, new LinkedList<CheckerElementViewModel>(), new List<CheckerElementViewModel>(),playerPosition.Side, paths);
//            }
//            else
//            {
//                if (playerPosition.Neighbors.All(x => x.Side == Side.Empty))
//                {
//                    foreach (var neighbor in playerPosition.Neighbors)
//                    {
//                        paths.Add(new LinkedList<CheckerElementViewModel>(new List<CheckerElementViewModel>(){ neighbor }));
//                    }
//                }
//                else
//                {
//                    SetPossibleMovementsForQueenRecursive(playerPosition, new LinkedList<CheckerElementViewModel>(), new List<CheckerElementViewModel>(),playerPosition.Side, paths);
//                }
//
//            }
//            return paths;
//        }

//        private void SetPossibleMovementsForQueenRecursive(CheckerElementViewModel currentChecker
//            , LinkedList<CheckerElementViewModel> path
//            , List<CheckerElementViewModel> visited
//            , Side checkerSide
//            , List<LinkedList<CheckerElementViewModel>> paths, Diagonal fromDiagonal = Diagonal.Initial)
//        {
//            path.AddLast(currentChecker);
//            paths.Add(new LinkedList<CheckerElementViewModel>(path));
//            visited.Add(currentChecker);
//
//            List<KeyValuePair<Diagonal, CheckerElementViewModel>> neighborsForQueen = GetNeighborsForQueen(currentChecker);
//            neighborsForQueen = FilterNeighborsOnOppositeDirection(neighborsForQueen,fromDiagonal);
//            var otherSideNeighbors = neighborsForQueen.Where(x => x.Value.Side != Side.Empty && x.Value.Side != checkerSide);
//            foreach (KeyValuePair<Diagonal, CheckerElementViewModel> otherSideNeighborPair in otherSideNeighbors)
//            {
//                if (path.Contains(otherSideNeighborPair.Value))
//                {
//                    continue;
//                }
//                Diagonal diagonal = otherSideNeighborPair.Key;
//                CheckerElementViewModel otherSideNeighbor = otherSideNeighborPair.Value;
//                List<CheckerElementViewModel> elementsAfterOpponent = GetNextElementsInDiagonal(currentChecker, otherSideNeighbor, path.First.Value);
//                foreach (CheckerElementViewModel positionAfterNextChecker in elementsAfterOpponent)
//                {
//                    if (positionAfterNextChecker == null || (positionAfterNextChecker.Side != Side.Empty && !path.Contains(positionAfterNextChecker)))
//                    {
//                        continue;
//                    }
//
//                    var cycle = new LinkedList<CheckerElementViewModel>();
//                    if (path.Contains(positionAfterNextChecker)) // Cycle here
//                    {
//                        int indexOfChecker = 0;
//                        int index = 0;
//                        foreach (var checkerElement in path)
//                        {
//                            cycle.AddLast(checkerElement);
//                            if (checkerElement == positionAfterNextChecker)
//                            {
//                                indexOfChecker = index;
//                            }
//
//                            index++;
//                        }
//
//                        int len = index - indexOfChecker;
//                        if (len > 3)
//                        {
//
//                            List<KeyValuePair<Diagonal, CheckerElementViewModel>> neighborsForCycleRoot = GetNeighborsForQueen(positionAfterNextChecker);
//                            foreach (var checkerElement in neighborsForCycleRoot.Where(x => x.Value.Side != Side.Empty))
//                            {
//                                List<CheckerElementViewModel> toVisitAgain = GetNextElementsInDiagonal(positionAfterNextChecker, checkerElement.Value);
//                                foreach (CheckerElementViewModel previouslyVisitedToWalkAgain in toVisitAgain)
//                                {
//                                    CheckerElementViewModel firstToNotDelete = path.Last.Value;
//                                    CheckerElementViewModel secondToNotDelete = path.Find(positionAfterNextChecker).Next.Value;
//                                    if (previouslyVisitedToWalkAgain != null
//                                        && (previouslyVisitedToWalkAgain.Side == Side.Empty)
//                                        && previouslyVisitedToWalkAgain != firstToNotDelete
//                                        && previouslyVisitedToWalkAgain != secondToNotDelete)
//                                    {
//                                        visited.Remove(previouslyVisitedToWalkAgain);
//                                    }
//                                }
//
//                            }
//
//                            path.AddLast(otherSideNeighbor);
//                            SetPossibleMovementsForQueenRecursive(positionAfterNextChecker, path, visited, checkerSide, paths, diagonal);
//                            path.RemoveLast();
//                        }
//                    }
//
//                    bool notContainsInCycle = !cycle.Contains(positionAfterNextChecker);
//                    if (notContainsInCycle)
//                    {
//                        path.AddLast(otherSideNeighbor);
//                        SetPossibleMovementsForQueenRecursive(positionAfterNextChecker, path, visited, checkerSide, paths, diagonal);
//                        path.RemoveLast();
//                    }
//                }
//            }
//
//            path.RemoveLast();
//        }

        private List<KeyValuePair<Diagonal, CheckerElementViewModel>> FilterNeighborsOnOppositeDirection(List<KeyValuePair<Diagonal, CheckerElementViewModel>> neighborsForQueen, Diagonal fromDiagonal)
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

        private bool IsVisitedAsPartOfSomePath(LinkedList<CheckerElementViewModel> currentPath, CheckerElementViewModel positionAfterNextChecker, List<LinkedList<CheckerElementViewModel>> paths)
        {
            foreach (LinkedList<CheckerElementViewModel> historyPath in paths)
            {
                var tempPath =new LinkedList<CheckerElementViewModel>(currentPath);
                tempPath.AddLast(positionAfterNextChecker);
                bool isPathsEquals = CompareLists(tempPath, historyPath);
                if (isPathsEquals)
                {
                    return true;
                }

            }

            return false;
        }

        private bool CompareLists(LinkedList<CheckerElementViewModel> tempPath, LinkedList<CheckerElementViewModel> historyPath)
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


//        public CheckerElementViewModel GetNextElementInDiagonal(CheckerElementViewModel playerChecker, CheckerElementViewModel otherSideNeighbor)
//        {
//            if (playerChecker.Column - otherSideNeighbor.Column > 0)
//            {
//                if (playerChecker.Row -  otherSideNeighbor.Row > 0)
//                {
//                    return otherSideNeighbor.Neighbors.SingleOrDefault(x => x.Column == playerChecker.Column - 2 && x.Row == playerChecker.Row - 2);
//                }
//                else
//                {
//                    return otherSideNeighbor.Neighbors.SingleOrDefault(x => x.Column == playerChecker.Column - 2 && x.Row == playerChecker.Row + 2);
//                }
//            }
//
//            if (playerChecker.Row - otherSideNeighbor.Row > 0)
//            {
//                return otherSideNeighbor.Neighbors.SingleOrDefault(x => x.Column == playerChecker.Column + 2 && x.Row == playerChecker.Row - 2);
//            }
//            else
//            {
//                return otherSideNeighbor.Neighbors.SingleOrDefault(x => x.Column == playerChecker.Column + 2 && x.Row == playerChecker.Row + 2);
//            }
//        }


//        public Queue<CheckerElementViewModel> GetAllElementsInDiagonalFromCurrent(CheckerElementViewModel checker, Diagonal diagonal)
//        {
//            switch (diagonal)
//            {
//                case Diagonal.LeftDown:
//                    return GetAllElementsInLeftDownDiagonal(checker);
//                case Diagonal.LeftUp:
//                    return GetAllElementsInLeftUpDiagonal(checker);
//                case Diagonal.RightUp:
//                    return GetAllElementsInRightUpDiagonal(checker);
//                case Diagonal.RightDown:
//                    return GetAllElementsInRightDownDiagonal(checker);
//                default:
//                    throw new ArgumentOutOfRangeException(nameof(diagonal), diagonal, null);
//            }
//        }

//        private Queue<CheckerElementViewModel> GetAllElementsInLeftDownDiagonal(CheckerElementViewModel checker)
//        {
//            int checkerRowDown = checker.Row;
//            var elements = new Queue<CheckerElementViewModel>();
//            for (int col = checker.Column - 1; col >= 0; col--)
//            {
//                if (checkerRowDown - 1 >= 0)
//                {
//                    var element = _dataProvider.GetElementAtPosition(col, checkerRowDown - 1);
//                    elements.Enqueue(element);
//                    checkerRowDown--;
//                }
//            }
//            return elements;
//        }

//        private Queue<CheckerElementViewModel> GetAllElementsInRightUpDiagonal(CheckerElementViewModel checker)
//        {
//            var checkerRowUp = checker.Row;
//            var elements = new Queue<CheckerElementViewModel>();
//            for (int col = checker.Column + 1; col < 8; col++)
//            {
//                if (checkerRowUp + 1 < 8)
//                {
//                    var element = _dataProvider.GetElementAtPosition(col, checkerRowUp + 1);
//
//                    elements.Enqueue(element);
//                    checkerRowUp++;
//                }
//            }
//
//            return elements;
//        }

//        private void SetPossibleMovementsRecursive(CheckerElementViewModel currentChecker
//            , LinkedList<CheckerElementViewModel> path
//            , List<CheckerElementViewModel> visited
//            , Side checkerSide
//            , List<LinkedList<CheckerElementViewModel>> paths
//            , LinkedList<CheckerElementViewModel> outerCycle = null)
//        {
//            path.AddLast(currentChecker);
//            paths.Add(new LinkedList<CheckerElementViewModel>(path));
//            visited.Add(currentChecker);
//            var otherSideNeighbors = currentChecker.Neighbors.Where(x => x.Side != Side.Empty && x.Side != checkerSide);
//
//            foreach (CheckerElementViewModel otherSideNeighbor in otherSideNeighbors)
//            {
//                CheckerElementViewModel positionAfterNextChecker = GetNextElementInDiagonal(currentChecker, otherSideNeighbor);
//                if (positionAfterNextChecker != null 
//                    && (   positionAfterNextChecker.Side == Side.Empty 
//                           || path.Contains(positionAfterNextChecker)))
//                {
//                    if (outerCycle != null && outerCycle.Contains(positionAfterNextChecker))
//                    {
//                        continue;
//                    }
//
//                    var cycle = new LinkedList<CheckerElementViewModel>();
//                    if (path.Contains(positionAfterNextChecker)) // Cycle here
//                    {
//                        int indexOfChecker = 0;
//                        int index = 0;
//                        foreach (var checkerElement in path)
//                        {
//                            cycle.AddLast(checkerElement);
//                            if (checkerElement == positionAfterNextChecker)
//                            {
//                                indexOfChecker = index;
//                            }
//
//                            index++;
//                        }
//
//                        int len = index - indexOfChecker;
//                        if (len > 3)
//                        {
//
//                            foreach (CheckerElementViewModel checkerElement in positionAfterNextChecker.Neighbors.Where(x => x.Side != Side.Empty))
//                            {
//                                CheckerElementViewModel tempToDelete = GetNextElementInDiagonal(positionAfterNextChecker, checkerElement);
//                                CheckerElementViewModel firstToNotDelete = path.Last.Value;
//                                CheckerElementViewModel secondToNotDelete = path.Find(positionAfterNextChecker).Next.Value;
//                                if (tempToDelete != null 
//                                    && (tempToDelete.Side == Side.Empty) 
//                                    && tempToDelete != firstToNotDelete 
//                                    && tempToDelete != secondToNotDelete)
//                                {
//                                    visited.Remove(tempToDelete);
//                                }
//
//                            }
//
//                            path.AddLast(otherSideNeighbor);
//                            if (IsTouchedBorder(positionAfterNextChecker))
//                            {
//                                
//                              SetPossibleMovementsForQueenRecursive(positionAfterNextChecker, path, visited, checkerSide, paths);
//                            }
//                            else
//                            {
//                             SetPossibleMovementsRecursive(positionAfterNextChecker, path, visited, checkerSide, paths, cycle);
//                            }
//                            path.RemoveLast();
//                        }
//                    }
//
//                    bool notContainsInCycle = !cycle.Contains(positionAfterNextChecker);
//
//                    if (notContainsInCycle)
//                    {
//                        path.AddLast(otherSideNeighbor);
//
//                        if (IsTouchedBorder(positionAfterNextChecker))
//                        {
//
//                            SetPossibleMovementsForQueenRecursive(positionAfterNextChecker, path, visited, checkerSide,paths);
//                        }
//                        else
//                        {
//                            SetPossibleMovementsRecursive(positionAfterNextChecker, path, visited, checkerSide, paths, cycle);
//                        }
//
//                        path.RemoveLast();
//                    }
//
//                }
//            }
//
//            path.RemoveLast();
//        }

        private bool IsTouchedBorder(CheckerElementViewModel positionAfterNextChecker)
        {
            return (IsMainPLayer && positionAfterNextChecker.Row == 7) || (!IsMainPLayer && positionAfterNextChecker.Row == 0);
        }

//        private Queue<CheckerElementViewModel> GetAllElementsInRightDownDiagonal(CheckerElementViewModel checker)
//        {
//            var checkerRowDown = checker.Row;
//            var elements = new Queue<CheckerElementViewModel>();
//            for (int col = checker.Column + 1; col < 8; col++)
//            {
//                if (checkerRowDown - 1 >= 0 )
//                {
//                    var element = _dataProvider.GetElementAtPosition(col, checkerRowDown - 1);
//                    elements.Enqueue(element);
//                    checkerRowDown--;
//                }
//            }
//
//            return elements;
//        }
//        private Queue<CheckerElementViewModel> GetAllElementsInLeftUpDiagonal(CheckerElementViewModel checker)
//        {
//            int checkerRowUp = checker.Row;
//            var elements = new Queue<CheckerElementViewModel>();
//            for (int col = checker.Column - 1; col >= 0; col--)
//            {
//                if (checkerRowUp + 1 < 8)
//                {
//                    var element = _dataProvider.GetElementAtPosition(col, checkerRowUp + 1);
//                    elements.Enqueue(element);
//                    checkerRowUp++;
//                }
//            }
//
//            return elements;
//        }

//        public List<CheckerElementViewModel> GetNextElementsInDiagonal(CheckerElementViewModel playerChecker, CheckerElementViewModel otherSideNeighbor,CheckerElementViewModel rootElementViewModel = null)
//        {
//            Diagonal diagonal;
//            if (playerChecker.Column - otherSideNeighbor.Column > 0)
//            {
//                diagonal = playerChecker.Row - otherSideNeighbor.Row > 0 ? Diagonal.LeftDown : Diagonal.LeftUp;
//            }
//            else
//            {
//                diagonal = playerChecker.Row - otherSideNeighbor.Row > 0 ? Diagonal.RightDown : Diagonal.RightUp;
//            }
//
//            Queue<CheckerElementViewModel> allElementsInDiagonalFromCurrent = GetAllElementsInDiagonalFromCurrent(otherSideNeighbor, diagonal);
//            if (allElementsInDiagonalFromCurrent.Count == 0 )
//            {
//                return new List<CheckerElementViewModel>();
//            }
//            var emptyElementsAfterOtherChecker = new List<CheckerElementViewModel>();
//            while (allElementsInDiagonalFromCurrent.Count > 0)
//            {
//                var value = allElementsInDiagonalFromCurrent.Dequeue();
//                if (value.Side != Side.Empty && value != rootElementViewModel)
//                {
//                    break;
//                }
//                emptyElementsAfterOtherChecker.Add(value);
//            }
//            return emptyElementsAfterOtherChecker;
//        }

        public void RemoveCheckers(List<CheckerElementViewModel> itemsTakeByOtherUser)
        {
            foreach (var checkerElement in itemsTakeByOtherUser)
            {
                PlayerPositions.Remove(checkerElement);
            }
        }

        public abstract PlayerViewModel Clone(DataProvider dataProvider);

        public int GetSimpleCheckersCount()
        {
            int counter = PlayerPositions.Count(playerPosition => playerPosition.Type == PieceType.Checker);
            return counter;
        }

        public int GetQueensCount()
        {
            int counter = PlayerPositions.Count(playerPosition => playerPosition.Type == PieceType.Queen);
            return counter;

        }

        public IEnumerable<LinkedList<CheckerElementViewModel>> GetAvailablePaths()
        {
            var result = new List<LinkedList<CheckerElementViewModel>>();
            var playerAvailablePaths = _player.AvailablePaths;
            foreach (LinkedList<CheckerModel> playerAvailablePath in playerAvailablePaths)
            {
                var checkersLinkedList = new LinkedList<CheckerElementViewModel>();
                foreach (var checkerModel in playerAvailablePath)
                {
                    CheckerElementViewModel emptyCheckerVM = _emptyCheckerViewModelsAsPossible.SingleOrDefault(x => x.Column == checkerModel.Column && x.Row == checkerModel.Row);
                    checkersLinkedList.AddLast(emptyCheckerVM);
                }
                result.Add(checkersLinkedList);
            }
            return result;
        }
    }
}