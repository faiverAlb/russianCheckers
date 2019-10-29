using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Data;
using System.Windows.Input;
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

        public GameViewModel(Core.Game game,
            IDialogService notificationDialog,
            bool isPlayingAutomatically)
        {
            _game = game;
            _notificationDialog = notificationDialog;
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

        }

        private void WaitMove()
        {
            if (!_isPlayingAutomatically)
            {
                return;
            }
            
            if (_playerOne.Side == NextMoveSide)
            {
                return;
            }
        
            MakeMoveBySecondUser();
        }

        private void MakeMoveBySecondUser()
        {
            var move = _playerTwo.GetOptimalMove(this);
            if (move.Value != null)
            {
                
                CheckerElementViewModel fromChecker = FindChecker(move.Key.Column, move.Key.Row);
                CheckerElementViewModel toPlace = FindChecker(move.Value.Column, move.Value.Row);
                _selectedChecker = fromChecker;
                MoveChecker(toPlace);
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
        public PlayerViewModel NextMovePlayer
        {
            get { return _nextMovePlayer; }
            private set
            {
                _nextMovePlayer = value;
            }
        }


        public ICommand SelectCheckerCommand { get { return new ActionCommand(OnTryMakeMove); } }

        private CheckerElementViewModel _selectedChecker;
        private  Side _nextMoveSide;
        private bool _isGameFinished;
        private readonly PlayerViewModel _playerOne;
        private readonly EmptyCellsPlayerViewModel _emptyCellsPlayerViewModel;
        private readonly DataProvider _dataProvider;
        private readonly RobotPlayerViewModel _playerTwo;
        private PlayerViewModel _nextMovePlayer;

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

            bool isGameFinished = _game.CheckGameStatus();
            if (!isGameFinished)
            {
                _game.ChangeTurn();
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
            playerViewModel.MoveCheckerToNewPlace(currentPositionElementViewModel, emptyPosition.Column, emptyPosition.Row);
            _game.ReCalculateWithRespectToOrder(playerViewModel.IsMainPlayer);
            
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