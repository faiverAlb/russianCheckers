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
        private readonly CheckerElement[,] _data;
        public GameViewModel(PlayerViewModel playerOne
            , PlayerViewModel playerTwo
            , IDialogService notificationDialog)
        {
            _playerOne = playerOne;
            _playerTwo = playerTwo;
            _emptyCellsPlayer = new EmptyCellsPlayer(Side.Empty);
            _data = GetCurrentGamePositions(_playerOne, playerTwo, _emptyCellsPlayer);
            _notificationDialog = notificationDialog;
            NextMoveSide = Side.White;

            _emptyCellsPlayer.CalculateNeighbors(_data);
            playerOne.CalculateNeighbors(_data);
            playerTwo.CalculateNeighbors(_data);

            var playerOneCollectionContainer = new CollectionContainer { Collection = playerOne.PlayerPositions};
            var playerTwoCollectionContainer = new CollectionContainer{ Collection = playerTwo.PlayerPositions };
            var emptyCollectionContainer = new CollectionContainer{ Collection = _emptyCellsPlayer.PlayerPositions };
            _positions.Add(playerOneCollectionContainer);
            _positions.Add(playerTwoCollectionContainer);
            _positions.Add(emptyCollectionContainer);
        }


        private CheckerElement[,] GetCurrentGamePositions(PlayerViewModel playerOne, PlayerViewModel playerTwo,
            EmptyCellsPlayer emptyCellsPlayer)
        {
            var data = new CheckerElement[8, 8];
            foreach (CheckerElement position in playerOne.PlayerPositions)
            {
                data[position.Column , position.Row] = position;
            }

            foreach (CheckerElement position in playerTwo.PlayerPositions)
            {
                data[position.Column, position.Row] = position;
            }

            foreach (CheckerElement position in emptyCellsPlayer.PlayerPositions)
            {
                data[position.Column, position.Row] = position;
            }

            return data;
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

            if (preValidationMoveValidationResult.Status == MoveValidationStatus.NothingSelected)
            {
                return;
            }

            PlayerViewModel player = _playerOne.Side == NextMoveSide ? _playerOne : _playerTwo;
            bool makeMoveStatus = IsCheckerMoved(newSelectedChecker, player);
            if (makeMoveStatus == false)
            {
                return;
            }

            NextMoveSide = NextMoveSide == Side.Black ? Side.White : Side.Black;

            var gameStatusChecker = new GameStatusChecker(_data);
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
            var moveValidationManager = new MoveValidationManager(_selectedChecker, newSelectedChecker, NextMoveSide);
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

            MoveCheckerToNewPlace(_selectedChecker, newSelectedChecker,player);
            _selectedChecker.IsSelected = false;
            _selectedChecker = null;
            return true;
        }

        private void MoveCheckerToNewPlace(CheckerElement currentPositionElement, CheckerElement nextPositionElement, PlayerViewModel player)
        {
            int currentCol = currentPositionElement.Column;
            int currentRow = currentPositionElement.Row;

            int nextCol = nextPositionElement.Column;
            int nextRow = nextPositionElement.Row;

            CheckerElement newPosition = _data[nextCol, nextRow];
            _data[nextCol, nextRow] = _data[currentCol, currentRow];

            player.MoveCheckerToNewPlace(currentPositionElement, nextCol, nextRow);

            newPosition.SetNewPosition(currentCol, currentRow);
            _data[currentCol, currentRow] = newPosition;

            _emptyCellsPlayer.CalculateNeighbors(_data);
            _playerOne.CalculateNeighbors(_data);
            _playerTwo.CalculateNeighbors(_data);
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