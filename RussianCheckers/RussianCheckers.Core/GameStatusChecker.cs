﻿using RussianCheckers.Core;

namespace RussianCheckers.Game
{
    internal class GameStatusChecker
    {
        private readonly DataProvider _dataProvider;
        private readonly Player _playerOne;
        private readonly Player _playerTwo;

        public GameStatusChecker(DataProvider dataProvider, Player playerOne, Player playerTwo)
        {
            _dataProvider = dataProvider;
            _playerOne = playerOne;
            _playerTwo = playerTwo;
        }

        public Side GetGameStatus()
        {
            if (_playerOne.PlayerPositions.Count == 0)
            {
                if (_playerOne.Side == Side.White)
                {
                    
                    return Side.Black;
                }

                return Side.White;
            }

            if (_playerTwo.PlayerPositions.Count == 0)
            {
                if (_playerTwo.Side == Side.White)
                {
                    return Side.Black;
                }

                return Side.White;
            }

            if (_playerOne.GetPossibleMovementsCount() == 0)
            {
                if (_playerOne.Side == Side.White)
                {
                    return Side.Black;
                }

                return Side.White;
            }
            if (_playerTwo.GetPossibleMovementsCount() == 0)
            {
                if (_playerTwo.Side == Side.White)
                {
                    return Side.Black;
                }

                return Side.White;
            }

            return Side.None;
        }
    }
}