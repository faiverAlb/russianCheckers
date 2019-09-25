using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using RussianCheckers.Game;
using RussianCheckers.Game.GameInfrastructure;
using RussianCheckers.MVVM;

namespace RussianCheckers
{
    public class GameViewModel : ObservableObject
    {
        private readonly PlayerViewModel _playerOne;
        private readonly PlayerViewModel _playerTwo;
        private readonly IDialogService _notificationDialog;
        private readonly ObservableCollection<CheckerElement> _positions = new ObservableCollection<CheckerElement>();
        private readonly Side[,] _data = new Side[8,8];
        public GameViewModel(PlayerViewModel playerOne
            , PlayerViewModel playerTwo
            , IDialogService notificationDialog)
        {
            _playerOne = playerOne;
            _playerTwo = playerTwo;
            _notificationDialog = notificationDialog;
            NextMoveSide = Side.White;
            IEnumerable<CheckerElement> initialPositionsOnBoard = GetInitialPositionsOnBoard(playerOne, playerTwo);

            _positions = new ObservableCollection<CheckerElement>(initialPositionsOnBoard);
        }

        public Side NextMoveSide
        {
            get { return _nextMoveSide; }
            set
            {
                _nextMoveSide = value;
                RaisePropertyChangedEvent(nameof(NextMoveSide));
            }
        }

        private IEnumerable<CheckerElement> GetInitialPositionsOnBoard(PlayerViewModel playerOne, PlayerViewModel playerTwo)
        {
            List<CheckerElement> positions = new List<CheckerElement>();  
            foreach (CheckerElement position in playerOne.PlayerPositions)
            {
                positions.Add(position);
                _data[position.Column -1 , position.Row - 1] = playerOne.Side;
            }

            foreach (CheckerElement position in playerTwo.PlayerPositions)
            {
                positions.Add(position);
                _data[position.Column - 1, position.Row - 1] = playerTwo.Side;
            }

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (_data[j,i] == Side.Empty)
                    {
                        positions.Add(new CheckerElement(j+1, i + 1, PieceType.Checker, Side.Empty)); 
                    }
                }
            }

            return positions;
        }


        public ICommand SelectCheckerCommand { get { return new ActionCommand(OnTryMakeMove); } }

        private CheckerElement _selectedChecker;
        private  Side _nextMoveSide;
        private bool _isGameFinished;

        private void OnTryMakeMove(object obj)
        {
            if (IsGameFinished)
            {
                ShowNotificationMessage("Game is over!");
                return;
            }
            CheckerElement newSelectedChecker = (CheckerElement)obj;
            var validationManager = new MoveValidationManager(_selectedChecker, newSelectedChecker, NextMoveSide);

            MoveValidationResult preValidationMoveValidationResult = validationManager.GetPreValidationResult();
            if (preValidationMoveValidationResult.Status == MoveValidationStatus.Error)
            {
                ShowNotificationMessage(preValidationMoveValidationResult.ErrorMessage);
                return;
            }

           
            bool makeMoveStatus = IsCheckerMoved(newSelectedChecker);
            if (makeMoveStatus == false)
            {
                return;
            }

            NextMoveSide = NextMoveSide == Side.Black ? Side.White : Side.Black;

            var gameStatusChecker = new GameStatusChecker();
            GameStatus gameStatus = gameStatusChecker.GetGameStatus();
            if (gameStatus != GameStatus.InProgress)
            {
                IsGameFinished = true;
                string pleaseSelectCheckerFirst = gameStatus == GameStatus.BlackWin? "Black win!":"White win!";
                ShowNotificationMessage(pleaseSelectCheckerFirst);
            }
        }

        public bool IsGameFinished
        {
            get { return _isGameFinished; }
            set
            {
                _isGameFinished = value;
                RaisePropertyChangedEvent(nameof(IsGameFinished));
            }
        }

        private bool IsCheckerMoved(CheckerElement newSelectedChecker)
        {

            var moveValidationManager = new MoveValidationManager(_selectedChecker, newSelectedChecker, NextMoveSide);
            MoveValidationResult validationResult =  moveValidationManager.GetMoveValidationResult();
            if (validationResult.Status == MoveValidationStatus.CheckerSelected)
            {
                if (_selectedChecker != null)
                {
                    _selectedChecker.IsSelected = false;
                }
                _selectedChecker = newSelectedChecker;
                _selectedChecker.IsSelected = true;
                return false;
            }

            if (validationResult.Status == MoveValidationStatus.NothingSelected)
            {
                _selectedChecker.IsSelected = false;
                _selectedChecker = null;
                return false;
            }
            _selectedChecker.IsSelected = false;
            _selectedChecker = null;
            return true;
        }


        private void ShowNotificationMessage(string message)
        {
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

        public ObservableCollection<CheckerElement> Positions
        {
            get
            {
                return _positions;
            }
        }
    }

}