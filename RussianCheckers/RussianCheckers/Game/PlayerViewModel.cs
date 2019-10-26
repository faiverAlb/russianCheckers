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
        public bool IsMainPlayer { get; private set; }
        public readonly Side Side;
//        protected readonly DataProvider _dataProvider;
        public  ObservableCollection<CheckerElementViewModel> PlayerPositions { get; protected set; }
//        public List<LinkedList<CheckerElementViewModel>> AvailablePaths { get;private set; }

        public int GetPossibleMovementsCount()
        {
            return PlayerPositions.Sum(position => position.PossibleMovementElements.Count());
        }


        public void ReSetPossibleMovements(List<CheckerElementViewModel> emptyItems)
        {
            foreach (CheckerElementViewModel checkerElementViewModel in PlayerPositions)
            {
                checkerElementViewModel.ReSetPossibleMovements(emptyItems);
            }
        }

        protected PlayerViewModel(Player player, List<CheckerElementViewModel> emptyCheckerViewModelsAsPossible)
        {
            _player = player;
            _emptyCheckerViewModelsAsPossible = emptyCheckerViewModelsAsPossible;
            IEnumerable<CheckerElementViewModel> checkerElementViewModels = player.PlayerPositions.Select(x => new CheckerElementViewModel(x, emptyCheckerViewModelsAsPossible));
            PlayerPositions = new ObservableCollection<CheckerElementViewModel>(checkerElementViewModels);
            
            Side = player.Side;
            IsMainPlayer = player.IsMainPlayer;
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

        public List<CheckerModel> MoveCheckerToNewPlace(CheckerElementViewModel checker, int nextCol, int nextRow)
        {
            int currentCol = checker.Column;
            int currentRow = checker.Row;
            List<CheckerModel> toTakeCheckers = _player.MoveCheckerToNewPlace(currentCol,currentRow,nextCol,nextRow);
            CheckerElementViewModel existingPlayerChecker = PlayerPositions.Single(x => x == checker);
            existingPlayerChecker.DeSelectPossibleMovement();
            return toTakeCheckers;
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

        public void RemoveCheckers(List<CheckerModel> models)
        {
            var toRemove = PlayerPositions.Where(x => models.SingleOrDefault(y => x.Column == y.Column && x.Row == y.Row) != null).ToList();
            foreach (var checkerElement in toRemove)
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
//            foreach (LinkedList<CheckerModel> playerAvailablePath in playerAvailablePaths)
//            {
//                var checkersLinkedList = new LinkedList<CheckerElementViewModel>();
//                foreach (var checkerModel in playerAvailablePath)
//                {
//                    CheckerElementViewModel emptyCheckerVM = _emptyCheckerViewModelsAsPossible.SingleOrDefault(x => x.Column == checkerModel.Column && x.Row == checkerModel.Row);
//                    checkersLinkedList.AddLast(emptyCheckerVM);
//                }
//                result.Add(checkersLinkedList);
//            }
            return result;
        }

    }
}