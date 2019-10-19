using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using RussianCheckers.Game.GameInfrastructure;
using RussianCheckers.Infrastructure;

namespace RussianCheckers.Game
{
    public class GameViewModel : ObservableObject
    {

        private readonly PlayerViewModel _playerTwo;
        private readonly IDialogService _notificationDialog;
        private readonly CompositeCollection _positions = new CompositeCollection();
        public GameViewModel(PlayerViewModel playerOne
            , PlayerViewModel playerTwo
            , EmptyCellsPlayer emptyCellsPlayer
            , DataProvider dataProvider
            , IDialogService notificationDialog)
        {
            _playerOne = playerOne;
            _playerTwo = playerTwo;
            _emptyCellsPlayer = emptyCellsPlayer;
            _dataProvider = dataProvider;

            _notificationDialog = notificationDialog;
            NextMoveSide = Side.White;

            _emptyCellsPlayer.CalculateNeighbors();
            playerOne.CalculateNeighbors();
            playerTwo.CalculateNeighbors(); 
            
            playerOne.CalculateAvailablePaths();
            playerTwo.CalculateAvailablePaths();

            var playerOneCollectionContainer = new CollectionContainer { Collection = playerOne.PlayerPositions};
            var playerTwoCollectionContainer = new CollectionContainer{ Collection = playerTwo.PlayerPositions };
            var emptyCollectionContainer = new CollectionContainer{ Collection = _emptyCellsPlayer.PlayerPositions };
            _positions.Add(playerOneCollectionContainer);
            _positions.Add(playerTwoCollectionContainer);
            _positions.Add(emptyCollectionContainer);
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


        public ICommand SelectCheckerCommand { get { return new ActionCommand(OnTryMakeMove); } }

        private CheckerElement _selectedChecker;
        private  Side _nextMoveSide;
        private bool _isGameFinished;
        private readonly PlayerViewModel _playerOne;
        private readonly EmptyCellsPlayer _emptyCellsPlayer;
        private readonly DataProvider _dataProvider;

        private void OnTryMakeMove(object obj)
        {
            if (IsGameFinished)
            {
                ShowNotificationMessage("Game is over!");
                return;
            }
            PlayerViewModel player = _playerOne.Side == NextMoveSide ? _playerOne : _playerTwo;
            CheckerElement newSelectedChecker = (CheckerElement)obj;
            var validationManager = new MoveValidationManager(_selectedChecker, newSelectedChecker, NextMoveSide, player);

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

            bool makeMoveStatus = IsCheckerMoved(newSelectedChecker, player);
            if (makeMoveStatus == false)
            {
                return;
            }

            NextMoveSide = NextMoveSide == Side.Black ? Side.White : Side.Black;

            var gameStatusChecker = new GameStatusChecker(_dataProvider,_playerOne,_playerTwo);
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

        private bool IsCheckerMoved(CheckerElement newSelectedChecker, PlayerViewModel player)
        {
            var moveValidationManager = new MoveValidationManager(_selectedChecker, newSelectedChecker, NextMoveSide, player);
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

        private void MoveCheckerToNewPlace(CheckerElement currentPositionElement, CheckerElement emptyPosition, PlayerViewModel player)
        {
            int nextCol = emptyPosition.Column;
            int nextRow = emptyPosition.Row;

            List<CheckerElement> itemsTakeByOtherUser = player.MoveCheckerToNewPlace(currentPositionElement, nextCol, nextRow);

            _emptyCellsPlayer.AddNewEmptyElements(itemsTakeByOtherUser);
            if (player == _playerOne)
            {
                _playerTwo.RemoveCheckers(itemsTakeByOtherUser);
            }
            else
            {
                _playerOne.RemoveCheckers(itemsTakeByOtherUser);
            }
            _emptyCellsPlayer.CalculateNeighbors();
            if (player == _playerOne)
            {
                _playerOne.CalculateNeighbors();
                _playerTwo.CalculateNeighbors();
            }
            else
            {
                _playerTwo.CalculateNeighbors();
                _playerOne.CalculateNeighbors();
            }

            _playerOne.CalculateAvailablePaths();
            _playerTwo.CalculateAvailablePaths();

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

        public CompositeCollection Positions
        {
            get
            {
                return _positions;
            }
        }
    }

}