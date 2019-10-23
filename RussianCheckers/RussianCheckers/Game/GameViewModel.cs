using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Data;
using System.Windows.Input;
using RussianCheckers.Game.GameInfrastructure;
using RussianCheckers.Infrastructure;

namespace RussianCheckers.Game
{
    public class GameViewModel : ObservableObject
    {

        private readonly IDialogService _notificationDialog;
        private readonly bool _isPlayingAutomatically;
        private readonly CompositeCollection _positions = new CompositeCollection();
        public Side WinnerSide { get; set; }

        public GameViewModel(PlayerViewModel playerOne
            , RobotPlayer playerTwo
            , EmptyCellsPlayer emptyCellsPlayer
            , DataProvider dataProvider
            , IDialogService notificationDialog = null
            , bool isPlayingAutomatically = true)
        {
            _playerOne = playerOne;
            _playerTwo = playerTwo;
            _emptyCellsPlayer = emptyCellsPlayer;
            _dataProvider = dataProvider;

            _notificationDialog = notificationDialog;
            _isPlayingAutomatically = isPlayingAutomatically;
            NextMoveSide = Side.White;
            NextMovePlayer = _playerOne.Side == NextMoveSide ? _playerOne : _playerTwo;

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

            WaitMove();
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
                CheckerElement fromChecker = move.Key;
                CheckerElement toPlace = move.Value;
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

        private CheckerElement _selectedChecker;
        private  Side _nextMoveSide;
        private bool _isGameFinished;
        private readonly PlayerViewModel _playerOne;
        private readonly EmptyCellsPlayer _emptyCellsPlayer;
        private readonly DataProvider _dataProvider;
        private readonly RobotPlayer _playerTwo;
        private PlayerViewModel _nextMovePlayer;

        private void OnTryMakeMove(object obj)
        {
            MoveChecker((CheckerElement)obj);
        }

        public void MoveChecker(CheckerElement fromPlace, CheckerElement toPlace)
        {
            CheckerElement foundChecker = _playerOne.PlayerPositions.SingleOrDefault(x => x.Column == fromPlace.Column && x.Row == fromPlace.Row);
            NextMoveSide = _playerOne.Side;
            if (foundChecker == null)
            {
                foundChecker = _playerTwo.PlayerPositions.SingleOrDefault(x => x.Column == fromPlace.Column && x.Row == fromPlace.Row);
                NextMoveSide = _playerTwo.Side;
            }
            _selectedChecker = foundChecker;
            CheckerElement emptyChecker = _emptyCellsPlayer.PlayerPositions.SingleOrDefault(x => x.Column == toPlace.Column && x.Row == toPlace.Row);
            MoveChecker(emptyChecker);
        }

        private void MoveChecker(CheckerElement newSelectedChecker)
        {
            if (IsGameFinished)
            {
                ShowNotificationMessage("Game is over!");
                return;
            }
            PlayerViewModel player = _playerOne.Side == NextMoveSide ? _playerOne : _playerTwo;
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


            var gameStatusChecker = new GameStatusChecker(_dataProvider, _playerOne, _playerTwo);
            GameStatus gameStatus = gameStatusChecker.GetGameStatus();
            if (gameStatus == GameStatus.InProgress)
            {
                NextMoveSide = NextMoveSide == Side.Black ? Side.White : Side.Black;
                NextMovePlayer = NextMovePlayer == _playerOne ? _playerTwo : _playerOne;
                WaitMove();
                return;
            }

            IsGameFinished = true;
            WinnerSide = gameStatus == GameStatus.BlackWin ? Side.Black : Side.White;
            string pleaseSelectCheckerFirst = gameStatus == GameStatus.BlackWin ? "Black win!" : "White win!";
            ShowNotificationMessage(pleaseSelectCheckerFirst);
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

        public IEnumerable<KeyValuePair<CheckerElement, CheckerElement>> GetAllAvailableMoves()
        {
            var allAvailableMoves = new List<KeyValuePair<CheckerElement, CheckerElement>>();
            allAvailableMoves.AddRange(_playerOne.GetLegalMovements());
            allAvailableMoves.AddRange(_playerTwo.GetLegalMovements());
            return allAvailableMoves;
        }

        public GameViewModel CreateGame()
        {
            DataProvider newDataProvider = _dataProvider.Clone();
            PlayerViewModel newPlayerOne = _playerOne.Clone(newDataProvider);
            RobotPlayer newPlayerTwo = (RobotPlayer) _playerTwo.Clone(newDataProvider);
            EmptyCellsPlayer  newEmptyCellsPlayer = (EmptyCellsPlayer) _emptyCellsPlayer.Clone(newDataProvider);
            return new GameViewModel(newPlayerOne, newPlayerTwo, newEmptyCellsPlayer, newDataProvider, null, false);
        }

        private PlayerViewModel GetPlayer(bool isMain)
        {
            if (isMain)
            {
                return _playerOne.IsMainPLayer ? _playerOne : _playerTwo;
            }

            return _playerOne.IsMainPLayer ? _playerTwo : _playerOne;

        }

        public int GetSimpleCheckersCount(bool isForMainPlayer)
        {
            PlayerViewModel player = GetPlayer(isForMainPlayer);
            return player.GetSimpleCheckersCount();
        }

        public int GetQueensCount(bool isForMainPlayer)
        {
            PlayerViewModel player = GetPlayer(isForMainPlayer);
            return player.GetQueensCount();
        }
    }

}