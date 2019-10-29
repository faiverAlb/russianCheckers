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
        public  ObservableCollection<CheckerElementViewModel> PlayerPositions { get; protected set; }



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
        public void MoveCheckerToNewPlace(CheckerElementViewModel checker)
        {
            CheckerElementViewModel existingPlayerChecker = PlayerPositions.Single(x => x == checker);
            existingPlayerChecker.DeSelectPossibleMovement();
        }

        public IEnumerable<LinkedList<CheckerModel>> GetAvailablePaths()
        {
            var playerAvailablePaths = _player.CalculateAvailablePaths();
            return playerAvailablePaths;
        }
    }
}