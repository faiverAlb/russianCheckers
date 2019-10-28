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
            _player.NotificationAction += OnDataChanged;
            IEnumerable<CheckerElementViewModel> checkerElementViewModels =
                player.PlayerPositions.Select(x => new CheckerElementViewModel(x, emptyCheckerViewModelsAsPossible));
            PlayerPositions = new ObservableCollection<CheckerElementViewModel>(checkerElementViewModels);

            Side = player.Side;
            IsMainPlayer = player.IsMainPlayer;
        }

        private void OnDataChanged(List<CheckerModel> added, List<CheckerModel> deleted, List<CheckerModel> modified)
        {
            foreach (CheckerModel checkerModel in added)
            {
                CheckerElementViewModel elementViewModel = new CheckerElementViewModel(checkerModel, new List<CheckerElementViewModel>());
                PlayerPositions.Add(elementViewModel);
            }

            foreach (var checkerModel in deleted)
            {
                CheckerElementViewModel toDelete = PlayerPositions.Single(x => x.Column == checkerModel.Column && x.Row == checkerModel.Row);
                PlayerPositions.Remove(toDelete);
            }

            foreach (CheckerModel modifiedElement in modified)
            {
                CheckerElementViewModel viewModel = PlayerPositions.Single(x => x.Column == modifiedElement.Column && x.Row == modifiedElement.Row);
                if (viewModel.Type != modifiedElement.Type)
                {
                    viewModel.Type = modifiedElement.Type;
                }
            }
        }



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

        public void MoveCheckerToNewPlace(CheckerElementViewModel checker, int nextCol, int nextRow)
        {
            int currentCol = checker.Column;
            int currentRow = checker.Row;
            _player.MoveCheckerToNewPlace(currentCol,currentRow,nextCol,nextRow);
            CheckerElementViewModel existingPlayerChecker = PlayerPositions.Single(x => x == checker);
            existingPlayerChecker.DeSelectPossibleMovement();
//            return resultTuple;
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

        public IEnumerable<LinkedList<CheckerModel>> GetAvailablePaths()
        {
            var playerAvailablePaths = _player.CalculateAvailablePaths();
            return playerAvailablePaths;
        }
    }
}