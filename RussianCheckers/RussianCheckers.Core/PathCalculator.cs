using System;
using System.Collections.Generic;
using System.Linq;

namespace RussianCheckers.Core
{
    public class PathCalculator
    {
        private readonly DataProvider _dataProvider;
        private readonly List<CheckerModel> _playerPositions;
        private readonly NeighborsCalculator _neighborsCalculator;
        private readonly bool _isMainPlayer;

        public PathCalculator(DataProvider dataProvider,List<CheckerModel> playerPositions, bool isMainPlayer)
        {
            _dataProvider = dataProvider;
            _playerPositions = playerPositions;
            _isMainPlayer = isMainPlayer;
            _neighborsCalculator = new NeighborsCalculator(_dataProvider, playerPositions);
        }

        public IEnumerable<LinkedList<CheckerModel>> CalculateAvailablePaths()
        {
            var availablePaths = new List<LinkedList<CheckerModel>>();
            foreach (CheckerModel playerPosition in _playerPositions)
            {
                if (playerPosition.Side == Side.Empty)
                {
                    continue;
                }

                List<LinkedList<CheckerModel>> paths = GetPossiblePaths(playerPosition);
                var possibleMovements = new List<CheckerModel>();

                foreach (LinkedList<CheckerModel> max in paths)
                {
                    if (max.Count == 1)
                    {
                        continue;
                    }

                    possibleMovements.Add(max.Last.Value);
                    availablePaths.Add(max);

                }

                if (possibleMovements.Count == 0)
                {
                    if (playerPosition.Type == PieceType.Checker)
                    {
                        foreach (LinkedList<CheckerModel> path in paths)
                        {
                            possibleMovements.AddRange(playerPosition.Type == PieceType.Checker
                                ? GetSimpleEmptyMoves(path.Last.Value)
                                : GetSimpleEmptyMovesForQueen(playerPosition));
                        }
                    }
                    else
                    {
                        possibleMovements.AddRange(GetSimpleEmptyMovesForQueen(playerPosition));
                    }
                }
                playerPosition.SetPossibleMovementElements(possibleMovements);
            }

            return availablePaths;
        }

        private List<CheckerModel> GetSimpleEmptyMoves(CheckerModel initialChecker)
        {
            List<CheckerModel> moves = initialChecker.Neighbors.Where(x => x.Side == Side.Empty).ToList();
            if (_isMainPlayer)
            {
                return moves.Where(x => x.Row > initialChecker.Row).ToList();
            }
            return moves.Where(x => x.Row < initialChecker.Row).ToList();

        }


        private IEnumerable<CheckerModel> GetSimpleEmptyMovesForQueen(CheckerModel queen)
        {
            return queen.Neighbors.Where(x => x.Side == Side.Empty);
        }
        public List<LinkedList<CheckerModel>> GetPossiblePaths(CheckerModel playerPosition)
        {
            var paths = new List<LinkedList<CheckerModel>>();
            if (playerPosition.Type == PieceType.Checker)
            {
                SetPossibleMovementsRecursive(playerPosition, new LinkedList<CheckerModel>(), new List<CheckerModel>(), playerPosition.Side, paths);
            }
            else
            {
                if (playerPosition.Neighbors.All(x => x.Side == Side.Empty))
                {
                    foreach (var neighbor in playerPosition.Neighbors)
                    {
                        paths.Add(new LinkedList<CheckerModel>(new List<CheckerModel>() { neighbor }));
                    }
                }
                else
                {
                    SetPossibleMovementsForQueenRecursive(playerPosition, new LinkedList<CheckerModel>(), new List<CheckerModel>(), playerPosition.Side, paths);
                }

            }
            return paths;
        }

        private CheckerModel GetNextElementInDiagonal(CheckerModel playerChecker, CheckerModel otherSideNeighbor)
        {
            if (playerChecker.Column - otherSideNeighbor.Column > 0)
            {
                if (playerChecker.Row - otherSideNeighbor.Row > 0)
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


        private void SetPossibleMovementsRecursive(CheckerModel currentChecker
            , LinkedList<CheckerModel> path
            , List<CheckerModel> visited
            , Side checkerSide
            , List<LinkedList<CheckerModel>> paths
            , LinkedList<CheckerModel> outerCycle = null)
        {
            path.AddLast(currentChecker);
            paths.Add(new LinkedList<CheckerModel>(path));
            visited.Add(currentChecker);
            var otherSideNeighbors = currentChecker.Neighbors.Where(x => x.Side != Side.Empty && x.Side != checkerSide);

            foreach (CheckerModel otherSideNeighbor in otherSideNeighbors)
            {
                CheckerModel positionAfterNextChecker = GetNextElementInDiagonal(currentChecker, otherSideNeighbor);
                if (positionAfterNextChecker != null
                    && (positionAfterNextChecker.Side == Side.Empty
                        || path.Contains(positionAfterNextChecker)))
                {
                    if (outerCycle != null && outerCycle.Contains(positionAfterNextChecker))
                    {
                        continue;
                    }

                    var cycle = new LinkedList<CheckerModel>();
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

                            foreach (CheckerModel checkerElement in positionAfterNextChecker.Neighbors.Where(x =>
                                x.Side != Side.Empty))
                            {
                                CheckerModel tempToDelete =
                                    GetNextElementInDiagonal(positionAfterNextChecker, checkerElement);
                                CheckerModel firstToNotDelete = path.Last.Value;
                                CheckerModel secondToNotDelete = path.Find(positionAfterNextChecker).Next.Value;
                                if (tempToDelete != null
                                    && (tempToDelete.Side == Side.Empty)
                                    && tempToDelete != firstToNotDelete
                                    && tempToDelete != secondToNotDelete)
                                {
                                    visited.Remove(tempToDelete);
                                }

                            }

                            path.AddLast(otherSideNeighbor);
                            if (IsMoveToucheBoard(positionAfterNextChecker))
                            {

                                SetPossibleMovementsForQueenRecursive(positionAfterNextChecker, path, visited,
                                    checkerSide, paths);
                            }
                            else
                            {
                                SetPossibleMovementsRecursive(positionAfterNextChecker, path, visited, checkerSide,
                                    paths, cycle);
                            }

                            path.RemoveLast();
                        }
                    }

                    bool notContainsInCycle = !cycle.Contains(positionAfterNextChecker);

                    if (notContainsInCycle)
                    {
                        path.AddLast(otherSideNeighbor);

                        if (IsMoveToucheBoard(positionAfterNextChecker))
                        {

                            SetPossibleMovementsForQueenRecursive(positionAfterNextChecker, path, visited, checkerSide,
                                paths);
                        }
                        else
                        {
                            SetPossibleMovementsRecursive(positionAfterNextChecker, path, visited, checkerSide, paths,
                                cycle);
                        }

                        path.RemoveLast();
                    }

                }
            }

            path.RemoveLast();
        }

        public bool IsMoveToucheBoard(CheckerModel positionAfterNextChecker)
        {
            return (_isMainPlayer && positionAfterNextChecker.Row == 7) || (!_isMainPlayer && positionAfterNextChecker.Row == 0);
        }

        private Queue<CheckerModel> GetAllElementsInLeftDownDiagonal(CheckerModel checker)
        {
            int checkerRowDown = checker.Row;
            var elements = new Queue<CheckerModel>();
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

        private Queue<CheckerModel> GetAllElementsInRightUpDiagonal(CheckerModel checker)
        {
            var checkerRowUp = checker.Row;
            var elements = new Queue<CheckerModel>();
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

        public Queue<CheckerModel> GetAllElementsInDiagonalFromCurrent(CheckerModel checker, Diagonal diagonal)
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
        private Queue<CheckerModel> GetAllElementsInLeftUpDiagonal(CheckerModel checker)
        {
            int checkerRowUp = checker.Row;
            var elements = new Queue<CheckerModel>();
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

        private Queue<CheckerModel> GetAllElementsInRightDownDiagonal(CheckerModel checker)
        {
            var checkerRowDown = checker.Row;
            var elements = new Queue<CheckerModel>();
            for (int col = checker.Column + 1; col < 8; col++)
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


        public List<CheckerModel> GetNextElementsInDiagonal(CheckerModel playerChecker, CheckerModel otherSideNeighbor, CheckerModel rootElementViewModel = null)
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

            Queue<CheckerModel> allElementsInDiagonalFromCurrent = GetAllElementsInDiagonalFromCurrent(otherSideNeighbor, diagonal);
            if (allElementsInDiagonalFromCurrent.Count == 0)
            {
                return new List<CheckerModel>();
            }
            var emptyElementsAfterOtherChecker = new List<CheckerModel>();
            while (allElementsInDiagonalFromCurrent.Count > 0)
            {
                var value = allElementsInDiagonalFromCurrent.Dequeue();
                if (value.Side != Side.Empty && value != rootElementViewModel)
                {
                    break;
                }
                emptyElementsAfterOtherChecker.Add(value);
            }
            return emptyElementsAfterOtherChecker;
        }

        private List<KeyValuePair<Diagonal, CheckerModel>> FilterNeighborsOnOppositeDirection(List<KeyValuePair<Diagonal, CheckerModel>> neighborsForQueen, Diagonal fromDiagonal)
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

        private void SetPossibleMovementsForQueenRecursive(CheckerModel currentChecker
            , LinkedList<CheckerModel> path
            , List<CheckerModel> visited
            , Side checkerSide
            , List<LinkedList<CheckerModel>> paths, Diagonal fromDiagonal = Diagonal.Initial)
        {
            path.AddLast(currentChecker);
            paths.Add(new LinkedList<CheckerModel>(path));
            visited.Add(currentChecker);

            List<KeyValuePair<Diagonal, CheckerModel>> neighborsForQueen = _neighborsCalculator.GetNeighborsForQueen(currentChecker);
            neighborsForQueen = FilterNeighborsOnOppositeDirection(neighborsForQueen, fromDiagonal);
            var otherSideNeighbors =
                neighborsForQueen.Where(x => x.Value.Side != Side.Empty && x.Value.Side != checkerSide);
            foreach (KeyValuePair<Diagonal, CheckerModel> otherSideNeighborPair in otherSideNeighbors)
            {
                if (path.Contains(otherSideNeighborPair.Value))
                {
                    continue;
                }

                Diagonal diagonal = otherSideNeighborPair.Key;
                CheckerModel otherSideNeighbor = otherSideNeighborPair.Value;
                List<CheckerModel> elementsAfterOpponent = GetNextElementsInDiagonal(currentChecker, otherSideNeighbor, path.First.Value);
                foreach (CheckerModel positionAfterNextChecker in elementsAfterOpponent)
                {
                    if (positionAfterNextChecker == null ||
                        (positionAfterNextChecker.Side != Side.Empty && !path.Contains(positionAfterNextChecker)))
                    {
                        continue;
                    }

                    var cycle = new LinkedList<CheckerModel>();
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

                            List<KeyValuePair<Diagonal, CheckerModel>> neighborsForCycleRoot = _neighborsCalculator.GetNeighborsForQueen(positionAfterNextChecker);
                            foreach (var checkerElement in neighborsForCycleRoot.Where(x => x.Value.Side != Side.Empty))
                            {
                                List<CheckerModel> toVisitAgain = GetNextElementsInDiagonal(positionAfterNextChecker, checkerElement.Value);
                                foreach (CheckerModel previouslyVisitedToWalkAgain in toVisitAgain)
                                {
                                    CheckerModel firstToNotDelete = path.Last.Value;
                                    CheckerModel secondToNotDelete = path.Find(positionAfterNextChecker).Next.Value;
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
                            SetPossibleMovementsForQueenRecursive(positionAfterNextChecker, path, visited, checkerSide,
                                paths, diagonal);
                            path.RemoveLast();
                        }
                    }

                    bool notContainsInCycle = !cycle.Contains(positionAfterNextChecker);
                    if (notContainsInCycle)
                    {
                        path.AddLast(otherSideNeighbor);
                        SetPossibleMovementsForQueenRecursive(positionAfterNextChecker, path, visited, checkerSide,
                            paths, diagonal);
                        path.RemoveLast();
                    }
                }
            }

            path.RemoveLast();
        }



    }
}