using System.Collections.Generic;
using System.Linq;
using RussianCheckers.Core;

namespace RussianCheckers.Game
{
    public class MoveValidationManager
    {
        private readonly CheckerElementViewModel _oldSelectedElementViewModel;
        private readonly CheckerElementViewModel _newSelectedElementViewModel;
        private readonly Side _nextMoveSide;
        private readonly PlayerViewModel _player;
        private readonly GameViewModel _gameViewModel;

        public MoveValidationManager(CheckerElementViewModel oldSelectedElementViewModel
            , CheckerElementViewModel newSelectedElementViewModel
            , Side nextMoveSide
            , PlayerViewModel player
            , GameViewModel gameViewModel)
        {
            _oldSelectedElementViewModel = oldSelectedElementViewModel;
            _newSelectedElementViewModel = newSelectedElementViewModel;
            _nextMoveSide = nextMoveSide;
            _player = player;
            _gameViewModel = gameViewModel;
        }

        public MoveValidationResult GetPreValidationResult()
        {
            if (_oldSelectedElementViewModel == null && _newSelectedElementViewModel.Side == Side.Empty)
            {
                return new MoveValidationResult(MoveValidationStatus.NothingSelected, "Please select checker first");
            }

            if (_newSelectedElementViewModel.Side != _nextMoveSide && _newSelectedElementViewModel.Side != Side.Empty)
            {
                return new MoveValidationResult(MoveValidationStatus.NothingSelected, $"Next move should be done by {_nextMoveSide}");
            }

            return new MoveValidationResult(MoveValidationStatus.Ok);
        }

        public MoveValidationResult GetMoveValidationResult()
        {
            IEnumerable<LinkedList<CheckerModel>> availablePathsModels = _player.GetAvailablePaths();
            var availablePaths = new List<LinkedList<CheckerElementViewModel>>();
            foreach (LinkedList<CheckerModel> playerAvailablePath in availablePathsModels)
            {
                var checkersLinkedList = new LinkedList<CheckerElementViewModel>();
                foreach (var checkerModel in playerAvailablePath)
                {
                    CheckerElementViewModel foundCheckerViewModel = _gameViewModel.FindChecker(checkerModel.Column,checkerModel.Row);
                    checkersLinkedList.AddLast(foundCheckerViewModel);
                }
                availablePaths.Add(checkersLinkedList);
            }

            if (availablePaths.Any() && (_oldSelectedElementViewModel == null || _oldSelectedElementViewModel.Side == _newSelectedElementViewModel.Side) &&availablePaths.All(x => x.First.Value != _newSelectedElementViewModel))
            {

                return new MoveValidationResult(MoveValidationStatus.Error, "You have required move by other checker");
            }

            if (_oldSelectedElementViewModel == null)
            {
                return new MoveValidationResult(MoveValidationStatus.NewItemSelected);
            }

            if (_oldSelectedElementViewModel == _newSelectedElementViewModel && availablePaths.Any(x => x.Last.Value == _newSelectedElementViewModel))
            {
                return new MoveValidationResult(MoveValidationStatus.Ok);
            }

            if (_oldSelectedElementViewModel == _newSelectedElementViewModel)
            {
                return new MoveValidationResult(MoveValidationStatus.DeselectChecker);
            }

            if (_oldSelectedElementViewModel.Side == _newSelectedElementViewModel.Side)
            {
                return new MoveValidationResult(MoveValidationStatus.NewItemSelected);
            }

            if (_oldSelectedElementViewModel != null && !_oldSelectedElementViewModel.CanMoveToPosition(_newSelectedElementViewModel))
            {
                return new MoveValidationResult(MoveValidationStatus.Error, "Can't move checker to this place");
            }

            return new MoveValidationResult(MoveValidationStatus.Ok);
        }
    }
}