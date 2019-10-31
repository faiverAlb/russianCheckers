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
        private readonly Core.Game _game;
        private readonly IDialogService _notificationDialog;
        private readonly bool _isPlayingAutomatically;
        private readonly CompositeCollection _positions = new CompositeCollection();
        public Side WinnerSide { get; set; }
        public ActionCommand UndoCommand { get; private set; }
        public ActionCommand RedoCommand { get; private set; }

        private readonly Stack<HistoryMove> _actionsHistory;

        public string RobotThinkingTime
        {
            get
            {
                return _robotThinkingTime;
            }
            set
            {
                _robotThinkingTime = value;
                RaisePropertyChangedEvent(nameof(RobotThinkingTime));
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

        public GameViewModel(Core.Game game, IDialogService notificationDialog, bool isPlayingAutomatically)
        {
            _game = game;
            _notificationDialog = notificationDialog;
            UndoCommand = new ActionCommand(DoUndo,CanUndo);
            RedoCommand = new ActionCommand(DoRedo,CanRedo);
            _actionsHistory = new Stack<HistoryMove>();
            CurrentHistoryPosition = 0;
            RobotThinkingTime = 4.ToString();
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
            DoRedo();
            DoRedo();

            _playerOne.ReSetPossibleMovements(_emptyCellsPlayerViewModel.PlayerPositions.ToList());
            _playerTwo.ReSetPossibleMovements(_emptyCellsPlayerViewModel.PlayerPositions.ToList());
        }
        private void DoRedo()
        {
            int count = _actionsHistory.Count - 1;
            foreach (HistoryMove historyMove in _actionsHistory)
            {
                if (count == CurrentHistoryPosition)
                {
                    _game.MoveChecker(historyMove.MovedFromTo.Key, historyMove.MovedFromTo.Value);
                    break;
                }
                count--;
            }
            CurrentHistoryPosition++;
        }

        private void DoUndo(object obj)
        {
            DoUndo();
            DoUndo();

            _playerOne.ReSetPossibleMovements(_emptyCellsPlayerViewModel.PlayerPositions.ToList());
            _playerTwo.ReSetPossibleMovements(_emptyCellsPlayerViewModel.PlayerPositions.ToList());
        }

        private void DoUndo()
        {
//            _cancellationTokenSource.Cancel();
            int count = _actionsHistory.Count - 1;
            CurrentHistoryPosition--;
            if (CurrentHistoryPosition < 0)
            {
                return;
            }
            foreach (var previousAction in _actionsHistory)
            {
                if (count == CurrentHistoryPosition)
                {
                    _game.RevertMove(previousAction);
                    break;
                }
                count--;
            }
        }


        public bool CanRedo()
        {
            return CurrentHistoryPosition < _actionsHistory.Count;
        }

        public bool CanUndo()
        {
            return CurrentHistoryPosition > 0;
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

            int timeToThink = 1;

            int.TryParse(RobotThinkingTime, out timeToThink);

            var timeSpan = new TimeSpan(0,0,0,timeToThink);
            _cancellationTokenSource = new CancellationTokenSource(timeSpan);
            var cancellationToken = _cancellationTokenSource.Token;
            if (timeToThink == 0)
            {
                cancellationToken = CancellationToken.None;
            }

            Task.Run(() =>
            {
                Thread.Sleep(timeSpan);
                _cancellationTokenSource.Cancel();
            });
            Task.Run(() => { MakeMoveBySecondUser(cancellationToken); }, cancellationToken);

        }

        private void MakeMoveBySecondUser(CancellationToken token)
        {
            var move = _playerTwo.GetOptimalMove(this, token);
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
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isCalculatingMove;
        private int _currentHistoryPosition;
        private string _robotThinkingTime;

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
            ResetHistoryIfNeeded();
            CurrentHistoryPosition++;

            playerViewModel.MoveCheckerToNewPlace(currentPositionElementViewModel);
            HistoryMove historyMove = _game.MoveChecker(currentPositionElementViewModel.CheckerModel, emptyPosition.CheckerModel);
            _actionsHistory.Push(historyMove);

            _playerOne.ReSetPossibleMovements(_emptyCellsPlayerViewModel.PlayerPositions.ToList());
            _playerTwo.ReSetPossibleMovements(_emptyCellsPlayerViewModel.PlayerPositions.ToList());
        }

        private void ResetHistoryIfNeeded()
        {
            if (CurrentHistoryPosition < _actionsHistory.Count)
            {
                while (CurrentHistoryPosition != _actionsHistory.Count)
                {
                    _actionsHistory.Pop();
                }
            }
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