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
        private readonly Core.Game _game;
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
            

            _emptyCellsPlayer = new EmptyCellsPlayer(_game.EmptyCellsAsPlayer);
            _playerOne = new HumanPlayerViewModel(_game.MainPlayer, _emptyCellsPlayer.PlayerPositions.ToList());
            _playerTwo = new RobotPlayerViewModel(_game.RobotPlayer, _emptyCellsPlayer.PlayerPositions.ToList());

            var playerOneCollectionContainer = new CollectionContainer { Collection = _playerOne.PlayerPositions };
            var playerTwoCollectionContainer = new CollectionContainer { Collection = _playerTwo.PlayerPositions };
            var emptyCollectionContainer = new CollectionContainer { Collection = _emptyCellsPlayer.PlayerPositions };
            _positions.Add(playerOneCollectionContainer);
            _positions.Add(playerTwoCollectionContainer);
            _positions.Add(emptyCollectionContainer);

            NextMoveSide = _game.NextMoveSide;
            //            WaitMove();

        }


        //        public GameViewModel(PlayerViewModel playerOne
        //            , RobotViewPlayer playerTwo
        //            , EmptyCellsPlayer emptyCellsPlayer
        //            , DataProvider dataProvider
        //            , IDialogService notificationDialog = null
        //            , bool isPlayingAutomatically = true)
        //        {
        //            _playerOne = playerOne;
        //            _playerTwo = playerTwo;
        //            _emptyCellsPlayer = emptyCellsPlayer;
        //            _dataProvider = dataProvider;
        //
        //            _notificationDialog = notificationDialog;
        //            _isPlayingAutomatically = isPlayingAutomatically;
        //            NextMoveSide = Side.White;
        //            
        //
        //            _emptyCellsPlayer.CalculateNeighbors();
        //            playerOne.CalculateNeighbors();
        //            playerTwo.CalculateNeighbors(); 
        //            
        //            playerOne.CalculateAvailablePaths();
        //            playerTwo.CalculateAvailablePaths();
        //
        //            var playerOneCollectionContainer = new CollectionContainer { Collection = playerOne.PlayerPositions};
        //            var playerTwoCollectionContainer = new CollectionContainer{ Collection = playerTwo.PlayerPositions };
        //            var emptyCollectionContainer = new CollectionContainer{ Collection = _emptyCellsPlayer.PlayerPositions };
        //            _positions.Add(playerOneCollectionContainer);
        //            _positions.Add(playerTwoCollectionContainer);
        //            _positions.Add(emptyCollectionContainer);
        //
        //            WaitMove();
        //        }

        //        private void WaitMove()
        //        {
        //            if (!_isPlayingAutomatically)
        //            {
        //                return;
        //            }
        //            
        //            if (_playerOne.Side == NextMoveSide)
        //            {
        //                return;
        //            }
        //
        //            MakeMoveBySecondUser();
        //        }

        //        private void MakeMoveBySecondUser()
        //        {
        //            var move = _playerTwo.GetOptimalMove(this);
        //            if (move.Value != null)
        //            {
        //                CheckerElementViewModel fromChecker = move.Key;
        //                CheckerElementViewModel toPlace = move.Value;
        //                _selectedChecker = fromChecker;
        //                MoveChecker(toPlace);
        //            }
        //        }


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
        private readonly EmptyCellsPlayer _emptyCellsPlayer;
        private readonly DataProvider _dataProvider;
        private readonly RobotPlayerViewModel _playerTwo;
        private PlayerViewModel _nextMovePlayer;

        private void OnTryMakeMove(object obj)
        {
            MoveChecker((CheckerElementViewModel)obj);
        }

//        public void MoveChecker(CheckerElementViewModel fromPlace, CheckerElementViewModel toPlace)
//        {
//            CheckerElementViewModel foundChecker = NextMovePlayer.PlayerPositions.SingleOrDefault(x => x.Column == fromPlace.Column && x.Row == fromPlace.Row);
//            _selectedChecker = foundChecker;
//            CheckerElementViewModel toPosition = _emptyCellsPlayer.PlayerPositions.SingleOrDefault(x => x.Column == toPlace.Column && x.Row == toPlace.Row);
//            if (toPlace.Side == fromPlace.Side)
//            {
//                toPosition = NextMovePlayer.PlayerPositions.SingleOrDefault(x => x.Column == toPlace.Column && x.Row == toPlace.Row);
//            }
//            MoveChecker(toPosition);
//        }


        private void MoveChecker(CheckerElementViewModel newSelectedChecker)
        {
            if (_game.IsGameFinished)
            {
                ShowNotificationMessage("Game is over!");
                return;
            }
            var validationManager = new MoveValidationManager(_selectedChecker, newSelectedChecker, NextMoveSide, NextMovePlayer);

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


            var gameStatusChecker = new GameStatusChecker(_dataProvider, _playerOne, _playerTwo);
            GameStatus gameStatus = gameStatusChecker.GetGameStatus();
            if (gameStatus == GameStatus.InProgress)
            {
                NextMoveSide = NextMoveSide == Side.Black ? Side.White : Side.Black;
                
//                WaitMove();
                return;
            }

//            IsGameFinished = true;
            WinnerSide = gameStatus == GameStatus.BlackWin ? Side.Black : Side.White;
            string pleaseSelectCheckerFirst = gameStatus == GameStatus.BlackWin ? "Black win!" : "White win!";
            ShowNotificationMessage(pleaseSelectCheckerFirst);
        }


//        public bool IsGameFinished
//        {
//            get { return _isGameFinished; }
//            set
//            {
//                _isGameFinished = value;
//                RaisePropertyChangedEvent(nameof(IsGameFinished));
//            }
//        }

        private bool IsCheckerMoved(CheckerElementViewModel newSelectedChecker, PlayerViewModel player)
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

//            MoveCheckerToNewPlace(_selectedChecker, newSelectedChecker, player);
//            _selectedChecker.IsSelected = false;
//            _selectedChecker = null;
            return true;
        }

//        private void MoveCheckerToNewPlace(CheckerElementViewModel currentPositionElementViewModel, CheckerElementViewModel emptyPosition, PlayerViewModel player)
//        {
//            int nextCol = emptyPosition.Column;
//            int nextRow = emptyPosition.Row;
//
//            List<CheckerElementViewModel> itemsTakeByOtherUser = player.MoveCheckerToNewPlace(currentPositionElementViewModel, nextCol, nextRow);
//
//            _emptyCellsPlayer.AddNewEmptyElements(itemsTakeByOtherUser);
//            if (player == _playerOne)
//            {
//                _playerTwo.RemoveCheckers(itemsTakeByOtherUser);
//            }
//            else
//            {
//                _playerOne.RemoveCheckers(itemsTakeByOtherUser);
//            }
//            _emptyCellsPlayer.CalculateNeighbors();
//            if (player == _playerOne)
//            {
//                _playerOne.CalculateNeighbors();
//                _playerTwo.CalculateNeighbors();
//            }
//            else
//            {
//                _playerTwo.CalculateNeighbors();
//                _playerOne.CalculateNeighbors();
//            }
//
//            _playerOne.CalculateAvailablePaths();
//            _playerTwo.CalculateAvailablePaths();
//
//        }


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

//        public IEnumerable<KeyValuePair<CheckerElementViewModel, CheckerElementViewModel>> GetAllAvailableMoves()
//        {
//            return NextMovePlayer.GetLegalMovements();
//        }

//        public GameViewModel CreateGame()
//        {
//            DataProvider newDataProvider = _dataProvider.Clone();
//            PlayerViewModel newPlayerOne = _playerOne.Clone(newDataProvider);
//            RobotViewPlayer newViewPlayerTwo = (RobotViewPlayer) _playerTwo.Clone(newDataProvider);
//            EmptyCellsPlayer  newEmptyCellsPlayer = (EmptyCellsPlayer) _emptyCellsPlayer.Clone(newDataProvider);
//            var gameViewModel = new GameViewModel(newPlayerOne, newViewPlayerTwo, newEmptyCellsPlayer, newDataProvider, null, false);
//            gameViewModel.NextMoveSide = NextMoveSide;
//            return gameViewModel;
//        }

        public PlayerViewModel GetPlayer(bool isMain)
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