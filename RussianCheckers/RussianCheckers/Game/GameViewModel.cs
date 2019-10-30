using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using RussianCheckers.Core;
using RussianCheckers.Game.GameInfrastructure;
using RussianCheckers.Infrastructure;

namespace RussianCheckers.Game
{
    public class GameViewModel : ObservableObject
    {
        public readonly Core.Game _game;
        private readonly IDialogService _notificationDialog;
        private readonly bool _isPlayingAutomatically;
        private readonly CompositeCollection _positions = new CompositeCollection();
        public Side WinnerSide { get; set; }
        public ActionCommand UndoCommand { get; private set; }
        public ActionCommand RedoCommand { get; private set; }

        public GameViewModel(Core.Game game,
            IDialogService notificationDialog,
            bool isPlayingAutomatically)
        {
            _game = game;
            _notificationDialog = notificationDialog;
            UndoCommand = new ActionCommand(DoUndo,CanUndo);
            RedoCommand = new ActionCommand(DoRedo,CanRedo);
            _isPlayingAutomatically = isPlayingAutomatically;
            _emptyCellsPlayerViewModel = new EmptyCellsPlayerViewModel(_game.EmptyCellsAsPlayer);
            _playerOne = new HumanPlayerViewModel(_game.MainPlayer, _emptyCellsPlayerViewModel.PlayerPositions.ToList());
            _playerTwo = new RobotPlayerViewModel(_game.RobotPlayer, _emptyCellsPlayerViewModel.PlayerPositions.ToList());

            var playerOneCollectionContainer = new CollectionContainer { Collection = _playerOne.PlayerPositions };
            var playerTwoCollectionContainer = new CollectionContainer { Collection = _playerTwo.PlayerPositions };
            var emptyCollectionContainer = new CollectionContainer { Collection = _emptyCellsPlayerViewModel.PlayerPositions };
            _positions.Add(playerOneCollectionContainer);
            _positions.Add(playerTwoCollectionContainer);
            _positions.Add(emptyCollectionContainer);

            NextMoveSide = _game.NextMoveSide;
            WaitMove();
        }

        private void DoRedo(object obj)
        {
            throw new NotImplementedException();
        }

        private void DoUndo(object obj)
        {
            throw new NotImplementedException();
        }


        private bool CanRedo()
        {
            return true;
        }

        private bool CanUndo()
        {
            return false;
        }


        private void WaitMove()
        {
            if (!_isPlayingAutomatically)
            {
                return;
            }
            
            if (_playerOne.Side == NextMoveSide)
            {
                IsCalculatingMove = false;
                return;
            }
            _cancellationToken = new CancellationTokenSource();
            Task.Run(() => { MakeMoveBySecondUser(); }, _cancellationToken.Token);
        }

        private void MakeMoveBySecondUser()
        {
            var move = _playerTwo.GetOptimalMove(this);
            IsCalculatingMove = true;
            if (move.Value != null)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)(
                    () =>
                    {
                        CheckerElementViewModel fromChecker = FindChecker(move.Key.Column, move.Key.Row);
                        CheckerElementViewModel toPlace = FindChecker(move.Value.Column, move.Value.Row);
                        _selectedChecker = fromChecker;
                        MoveChecker(toPlace);
                    }));
            }
        }

        public bool IsCalculatingMove
        {
            get { return _isCalculatingMove; }
            set
            {
                _isCalculatingMove = value;
                RaisePropertyChangedEvent(nameof(IsCalculatingMove));
            }
        }


        public Side NextMoveSide
        {
            get { return _nextMoveSide; }
            set
            {
                _nextMoveSide = value;
                NextMovePlayer = _playerOne.Side == _nextMoveSide ? _playerOne : _playerTwo;
                RaisePropertyChangedEvent(nameof(NextMoveSide));
            }
        }

        private PlayerViewModel NextMovePlayer { get; set; }

        public ICommand SelectCheckerCommand { get { return new ActionCommand(OnTryMakeMove); } }

        private CheckerElementViewModel _selectedChecker;
        private  Side _nextMoveSide;
        private readonly PlayerViewModel _playerOne;
        private readonly EmptyCellsPlayerViewModel _emptyCellsPlayerViewModel;
        private readonly RobotPlayerViewModel _playerTwo;
        private CancellationTokenSource _cancellationToken;
        private bool _isCalculatingMove;

        private void OnTryMakeMove(object obj)
        {
            MoveChecker((CheckerElementViewModel)obj);
        }


        private void MoveChecker(CheckerElementViewModel newSelectedChecker)
        {
            if (_game.IsGameFinished)
            {
                ShowNotificationMessage("Game is over!");
                return;
            }
            var validationManager = new MoveValidationManager(_selectedChecker
                , newSelectedChecker
                , NextMoveSide
                , NextMovePlayer
                ,this);

            MoveValidationResult preValidationMoveValidationResult = validationManager.GetPreValidationResult();
            if (preValidationMoveValidationResult.Status == MoveValidationStatus.Error)
            {
                ShowNotificationMessage(preValidationMoveValidationResult.ErrorMessage);
                return;
            }

            if (preValidationMoveValidationResult.Status == MoveValidationStatus.NothingSelected)
            {
                return;
            }

            bool makeMoveStatus = IsCheckerMoved(newSelectedChecker, NextMovePlayer);
            if (makeMoveStatus == false)
            {
                return;
            }

            if (!_game.IsGameFinished)
            {
                NextMoveSide = _game.NextMoveSide;
                
                WaitMove();
                return;
            }

            WinnerSide = _game.GetWinnerSide();
            string pleaseSelectCheckerFirst = WinnerSide == Side.Black ? "Black win!" : "White win!";
            ShowNotificationMessage(pleaseSelectCheckerFirst);
        }

        private bool IsCheckerMoved(CheckerElementViewModel newSelectedChecker, PlayerViewModel player)
        {
            var moveValidationManager = new MoveValidationManager(_selectedChecker
                , newSelectedChecker
                , NextMoveSide
                , player
                , this);
            MoveValidationResult validationResult =  moveValidationManager.GetMoveValidationResult();
            if (validationResult.Status == MoveValidationStatus.NewItemSelected)
            {
                if (_selectedChecker != null)
                {
                    _selectedChecker.IsSelected = false;
                    _selectedChecker.DeSelectPossibleMovement();
                }
                _selectedChecker = newSelectedChecker;

                _selectedChecker.IsSelected = true;
                _selectedChecker.SelectPossibleMovement();
                return false;
            }

            if (validationResult.Status == MoveValidationStatus.DeselectChecker)
            {
                _selectedChecker.IsSelected = false;
                _selectedChecker.DeSelectPossibleMovement();
                _selectedChecker = null;
                return false;
            }

            if (validationResult.Status == MoveValidationStatus.Error)
            {
                ShowNotificationMessage(validationResult.ErrorMessage);
                return false;
            }

            MoveCheckerToNewPlace(_selectedChecker, newSelectedChecker, player);
            _selectedChecker.IsSelected = false;
            _selectedChecker = null;
            return true;
        }

        private void MoveCheckerToNewPlace(CheckerElementViewModel currentPositionElementViewModel, CheckerElementViewModel emptyPosition, PlayerViewModel playerViewModel)
        {
            playerViewModel.MoveCheckerToNewPlace(currentPositionElementViewModel);
            _game.MoveChecker(currentPositionElementViewModel.CheckerModel, emptyPosition.CheckerModel);
            _playerOne.ReSetPossibleMovements(_emptyCellsPlayerViewModel.PlayerPositions.ToList());
            _playerTwo.ReSetPossibleMovements(_emptyCellsPlayerViewModel.PlayerPositions.ToList());
        }


        private void ShowNotificationMessage(string message)
        {
            if (_notificationDialog == null)
            {
                return;
            }
            var notificationDialogViewModel = new NotificationDialogViewModel(message);
            bool? result = _notificationDialog.ShowDialog(notificationDialogViewModel);

            //            if (result.HasValue)
            //            {
            //                if (result.Value)
            //                {
            //                    // Accepted
            //                }
            //                else
            //                {
            //                    // Cancelled
            //                }
            //            }
        }

        public CompositeCollection Positions
        {
            get
            {
                return _positions;
            }
        }

        public Core.Game Game
        {
            get { return _game; }
        }

        public CheckerElementViewModel FindChecker(int column, int row)
        {
            var findChecker = _playerOne.PlayerPositions.SingleOrDefault(x => x.Column == column && x.Row == row);
            if (findChecker != null)
                return findChecker;
            findChecker = _playerTwo.PlayerPositions.SingleOrDefault(x => x.Column == column && x.Row == row);
            if (findChecker != null)
                return findChecker;
            findChecker = _emptyCellsPlayerViewModel.PlayerPositions.SingleOrDefault(x => x.Column == column && x.Row == row);
            if (findChecker == null)
            {
                throw new Exception($"Can't find checker at position ({column},{row}) in game view model: ");
            }
            return findChecker;
        }
    }

}