using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RussianCheckers
{
    public class GameViewModel : ObservableObject
    {
        private readonly PlayerViewModel _playerOne;
        private readonly PlayerViewModel _playerTwo;
        private readonly ObservableCollection<CheckerElement> _positions = new ObservableCollection<CheckerElement>();

        public GameViewModel(PlayerViewModel playerOne
            , PlayerViewModel playerTwo)
        {
            _playerOne = playerOne;
            _playerTwo = playerTwo;
            NextMoveSide = Side.White;
            _positions = new ObservableCollection<CheckerElement>(GetInitialPositionsOnBoard(playerOne, playerTwo));
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
            }
            foreach (CheckerElement position in playerTwo.PlayerPositions)
            {
                positions.Add(position);
            }
            foreach (CheckerElement position in GetInitialEmptyPositions())
            {
                positions.Add(position);
            }
            return positions;
        }


        public ICommand SelectCheckerCommand { get { return new RelayCommand<CheckerElement>(OnSelectChecker); } }

        private CheckerElement _selectedChecker;
        private  Side _nextMoveSide;

        private void OnSelectChecker(object obj)
        {
            var newSelectedChecker = (CheckerElement)obj;
            if (_selectedChecker != null)
            {
                _selectedChecker.IsSelected = false;
            }

            _selectedChecker = newSelectedChecker;
            _selectedChecker.IsSelected = true;
        }

        private IEnumerable<CheckerElement> GetInitialEmptyPositions()
        {
            var positions = new List<CheckerElement>();
            for (int col = 1; col <= 8; col++)
            {
                for (int row = 4; row <= 5; row++)
                {
                    positions.Add(new CheckerElement(col, row, PieceType.Checker, Side.Empty));
                }
            }
            return positions;

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