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
        private readonly Side[,] _data;
        public GameViewModel(PlayerViewModel playerOne
            , PlayerViewModel playerTwo
            , IDialogService notificationDialog)
        {
            _playerOne = playerOne;
            _playerTwo = playerTwo;
            _notificationDialog = notificationDialog;
            NextMoveSide = Side.White;

            _data = GetCurrentGamePositions(_playerOne, playerTwo);
            _emptyCollection = new ObservableCollection<CheckerElement>(GetInitialEmptyPositionsOnBoard());
            playerOne.SetPossibleMovementElements(_emptyCollection.ToList(),new List<CheckerElement>());
            playerTwo.SetPossibleMovementElements(_emptyCollection.ToList(), new List<CheckerElement>());

            var playerOneCollectionContainer = new CollectionContainer { Collection = playerOne.PlayerPositions};
            var playerTwoCollectionContainer = new CollectionContainer{ Collection = playerTwo.PlayerPositions };
            var emptyCollectionContainer = new CollectionContainer{ Collection = _emptyCollection };
            _positions.Add(playerOneCollectionContainer);
            _positions.Add(playerTwoCollectionContainer);
            _positions.Add(emptyCollectionContainer);
        }

        private Side[,] GetCurrentGamePositions(PlayerViewModel playerOne, PlayerViewModel playerTwo)
        {
            var data = new Side[8, 8];
            foreach (CheckerElement position in playerOne.PlayerPositions)
            {
                data[position.Column - 1, position.Row - 1] = playerOne.Side;
            }

            foreach (CheckerElement position in playerTwo.PlayerPositions)
            {
                data[position.Column - 1, position.Row - 1] = playerTwo.Side;
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

        private IEnumerable<CheckerElement> GetInitialEmptyPositionsOnBoard()
        {
            List<CheckerElement> positions = new List<CheckerElement>();  
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (_data[j,i] == Side.Empty)
                    {
                        if (i % 2 == 0 && j % 2 != 0)
                        {
                            _data[j, i] = Side.None;
                            continue;
                        }

                        if (i % 2 != 0 && j % 2 == 0)
                        {
                            _data[j, i] = Side.None;
                            continue;
                        }

                        positions.Add(new CheckerElement(j + 1, i + 1, PieceType.Checker, Side.Empty));
                    }
                }
            }

            return positions;
        }


        public ICommand SelectCheckerCommand { get { return new ActionCommand(OnTryMakeMove); } }

        private CheckerElement _selectedChecker;
        private  Side _nextMoveSide;
        private bool _isGameFinished;
        private readonly ObservableCollection<CheckerElement> _emptyCollection;
        private readonly PlayerViewModel _playerOne;

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

            List<CheckerElement> allEmptyElements = _emptyCollection.ToList();
            if (_playerOne.Side == NextMoveSide)
            {
                _playerTwo.SetPossibleMovementElements(allEmptyElements, _playerOne.PlayerPositions.ToList());
                _playerOne.SetPossibleMovementElements(allEmptyElements, _playerTwo.PlayerPositions.ToList());
            }
            else
            {
                _playerOne.SetPossibleMovementElements(allEmptyElements, _playerTwo.PlayerPositions.ToList());
                _playerTwo.SetPossibleMovementElements(allEmptyElements, _playerOne.PlayerPositions.ToList());
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
                }
                _selectedChecker = newSelectedChecker;
                _selectedChecker.IsSelected = true;
                return false;
            }

            if (validationResult.Status == MoveValidationStatus.DeselectChecker)
            {
                _selectedChecker.IsSelected = false;
                _selectedChecker = null;
                return false;
            }

            if (validationResult.Status == MoveValidationStatus.Error)
            {
                ShowNotificationMessage(validationResult.ErrorMessage);
                return false;
            }

            MoveCheckerToNewPlace(_selectedChecker, newSelectedChecker,player, newSelectedChecker.Column, newSelectedChecker.Row);
            _selectedChecker.IsSelected = false;
            _selectedChecker = null;
            return true;
        }

        private void MoveCheckerToNewPlace(CheckerElement selectedChecker,
            CheckerElement newSelectedChecker,
            PlayerViewModel player, 
            int column, 
            int row)
        {
            int selectedCheckerColumn = selectedChecker.Column;
            int selectedCheckerRow = selectedChecker.Row;
            player.MoveCheckerToNewPlace(selectedChecker, column, row);
            CheckerElement emptyElement = _emptyCollection.Single(x => x == newSelectedChecker);
            emptyElement.SetNewPosition(selectedCheckerColumn, selectedCheckerRow);

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