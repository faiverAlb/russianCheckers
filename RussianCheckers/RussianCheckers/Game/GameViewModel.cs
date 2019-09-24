﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace RussianCheckers
{
    public class GameViewModel : ObservableObject
    {
        private readonly PlayerViewModel _playerOne;
        private readonly PlayerViewModel _playerTwo;
        private readonly ObservableCollection<CheckerElement> _positions = new ObservableCollection<CheckerElement>();
        private readonly Side[,] _data = new Side[8,8];
        public GameViewModel(PlayerViewModel playerOne
            , PlayerViewModel playerTwo)
        {
            _playerOne = playerOne;
            _playerTwo = playerTwo;
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

        public ObservableCollection<CheckerElement> Positions
        {
            get
            {
                return _positions;
            }
        }
    }
}