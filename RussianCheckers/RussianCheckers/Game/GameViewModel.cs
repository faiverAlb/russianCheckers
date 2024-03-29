﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using RussianCheckers.Core;
using RussianCheckers.Game.DialogsViewModels;
using RussianCheckers.Infrastructure;

namespace RussianCheckers.Game
{
    public class GameViewModel : ObservableObject
    {
        private readonly Core.Game _game;
        private readonly IDialogService _dialogService;
        private readonly bool _isPlayingAutomatically;
        private readonly CompositeCollection _positions = new CompositeCollection();
        public Side WinnerSide { get; set; }
        public ActionCommand UndoCommand { get; }
        public ActionCommand RedoCommand { get; }

        public int RobotThinkingTime
        {
            get
            {
                return _robotThinkingTime;
            }
            set
            {
                if (value < 0)
                {
                    return;
                }
                _robotThinkingTime = value;
                RaisePropertyChangedEvent(nameof(RobotThinkingTime));
            }
        }


        public string CurrentGameStatus
        {
            get
            {
                if (_game.IsGameFinished)
                {
                    Side winnerSide = _game.GetWinnerSide();
                    return winnerSide != Side.Draw? winnerSide + " won" : "No one won. Draw.";
                }
                if (NextMovePlayer.IsMainPlayer)
                {
                    return "Your turn to move checker";
                }

                return $"{NextMovePlayer.Side} is moving. Please wait for his decision";
            }
        }

        public bool IsCheckersMovable
        {
            get
            {
                return NextMovePlayer.IsMainPlayer && !_game.IsGameFinished;
            }
            
        }

        private int CurrentHistoryPosition
        {
            get
            {
                return _currentHistoryPosition;
            }
            set
            {
                _currentHistoryPosition = value;
                UndoCommand.RaiseCanExecuteChanged();
                RedoCommand.RaiseCanExecuteChanged();
            }
        }

        public GameViewModel(Core.Game game, IDialogService dialogService, bool isPlayingAutomatically)
        {
            _game = game;
            _dialogService = dialogService;
            UndoCommand = new ActionCommand(DoUndo,CanUndo);
            RedoCommand = new ActionCommand(DoRedo,CanRedo);
            CurrentHistoryPosition = 0;
            RobotThinkingTime = 5;
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

            WaitMove();
        }

        private void DoRedo(object obj)
        {
            DoRedo();
            DoRedo();

            _playerOne.ReSetPossibleMovements(_emptyCellsPlayerViewModel.PlayerPositions.ToList());
            _playerTwo.ReSetPossibleMovements(_emptyCellsPlayerViewModel.PlayerPositions.ToList());
            WaitMove();
        }
        private void DoRedo()
        {
            _game.MoveCheckerToHistoryPosition(CurrentHistoryPosition);
            CurrentHistoryPosition++;
        }

        private void DoUndo(object obj)
        {
            DoUndo();
            DoUndo();

            _playerOne.ReSetPossibleMovements(_emptyCellsPlayerViewModel.PlayerPositions.ToList());
            _playerTwo.ReSetPossibleMovements(_emptyCellsPlayerViewModel.PlayerPositions.ToList());
            WaitMove();
        }

        private void DoUndo()
        {
            if (CurrentHistoryPosition - 1 < 0)
            {
                return;
            }

            CurrentHistoryPosition--;

            _game.RevertCheckerToHistoryPosition(CurrentHistoryPosition);
        }


        public bool CanRedo()
        {
            return CurrentHistoryPosition < _game.GetHistoryCount();
        }

        public bool CanUndo()
        {
            return CurrentHistoryPosition > 0;
        }


        private void WaitMove()
        {
            NextMoveSide = _game.NextMoveSide;
            if (!_isPlayingAutomatically)
            {
                return;
            }
            
            if (_playerOne.Side == NextMoveSide)
            {
                IsCalculatingMove = false;
                return;
            }

            var timeSpan = new TimeSpan(0,0,0, RobotThinkingTime);
            _cancellationTokenSource = new CancellationTokenSource(timeSpan);
            if (RobotThinkingTime == 0)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                _cancellationTokenSource.Cancel();
                timeSpan = new TimeSpan(0, 0, 0, 0, 0);
            }
            var cancellationToken = _cancellationTokenSource.Token;


            Task.Run(() =>
            {
                Thread.Sleep(timeSpan);
                _cancellationTokenSource.Cancel();
            });
            Task.Run(() => { MakeMoveBySecondUser(cancellationToken); });

        }

        private void MakeMoveBySecondUser(CancellationToken token)
        {
            KeyValuePair<CheckerModel, CheckerModel> move = _playerTwo.GetOptimalMove(this, token);
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
                RaisePropertyChangedEvent(nameof(IsCheckersMovable));
                RaisePropertyChangedEvent(nameof(CurrentGameStatus));
            }
        }

        private PlayerViewModel NextMovePlayer { get; set; }

        public ICommand SelectCheckerCommand { get { return new ActionCommand(OnTryMakeMove); } }

        private CheckerElementViewModel _selectedChecker;
        private  Side _nextMoveSide;
        private readonly PlayerViewModel _playerOne;
        private readonly EmptyCellsPlayerViewModel _emptyCellsPlayerViewModel;
        private readonly RobotPlayerViewModel _playerTwo;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isCalculatingMove;
        private int _currentHistoryPosition;
        private int _robotThinkingTime;

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
                
                WaitMove();
                return;
            }

            WinnerSide = _game.GetWinnerSide();
            RaisePropertyChangedEvent(nameof(CurrentGameStatus));
            RaisePropertyChangedEvent(nameof(IsCheckersMovable));
            ShowNotificationMessage(CurrentGameStatus);
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
            _game.ResetHistoryIfNeeded(CurrentHistoryPosition);
            CurrentHistoryPosition++;

            playerViewModel.MoveCheckerToNewPlace(currentPositionElementViewModel);
            _game.MoveChecker(currentPositionElementViewModel.CheckerModel, emptyPosition.CheckerModel);

            _playerOne.ReSetPossibleMovements(_emptyCellsPlayerViewModel.PlayerPositions.ToList());
            _playerTwo.ReSetPossibleMovements(_emptyCellsPlayerViewModel.PlayerPositions.ToList());
        }

        private void ShowNotificationMessage(string message)
        {
            if (_dialogService == null)
            {
                return;
            }
            var notificationDialogViewModel = new NotificationDialogViewModel(message);
            _dialogService.ShowDialog(notificationDialogViewModel);
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