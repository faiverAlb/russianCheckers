﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace RussianCheckers.Core
{
    public class DataProvider
    {
        public  Action<List<CheckerModel>, List<CheckerModel>, List<CheckerModel>> NotificationAction;
        private readonly CheckerModel[,] _data;
        private List<CheckerModel> _added;
        private List<CheckerModel> _deleted;
        private List<CheckerModel> _modified;

        private DataProvider(CheckerModel[,] data)
        {
            _data = data;
            _added = new List<CheckerModel>();
            _deleted = new List<CheckerModel>();
            _modified = new List<CheckerModel>();

        }
        public DataProvider(List<CheckerModel> mainPlayerCheckers, List<CheckerModel> secondPlayerCheckers)
        {
            List<CheckerModel> emptyCheckers = GetEmptyCheckersPositions(mainPlayerCheckers, secondPlayerCheckers);
            _data = GetCurrentGamePositions(mainPlayerCheckers, secondPlayerCheckers, emptyCheckers);

            _added = new List<CheckerModel>();
            _deleted = new List<CheckerModel>();
            _modified = new List<CheckerModel>();
        }

        public DataProvider(Side mainPlayerSide)
        {
            _added = new List<CheckerModel>();
            _deleted = new List<CheckerModel>();
            _modified = new List<CheckerModel>();

            List<CheckerModel> mainPlayerCheckers = GetMainPlayerCheckers(mainPlayerSide);
            Side secondPlayerSide = mainPlayerSide == Side.White? Side.Black: Side.White;
            
            List<CheckerModel> secondPlayerCheckers = GetSecondPlayerPositions(secondPlayerSide);
            List<CheckerModel> emptyCheckers = GetEmptyCheckersPositions(mainPlayerCheckers, secondPlayerCheckers);

            _data = GetCurrentGamePositions(mainPlayerCheckers, secondPlayerCheckers, emptyCheckers);
            
        }


        public CheckerModel GetElementAtPosition(int column, int row)
        {
            return _data[column, row];
        }

        private List<CheckerModel> GetEmptyCheckersPositions(List<CheckerModel> mainPlayerCheckers, List<CheckerModel> secondPlayerCheckers)
        {
            var positions = new List<CheckerModel>();
            List<CheckerModel> allPositions = mainPlayerCheckers.ToList();
            allPositions.AddRange(secondPlayerCheckers.ToList());

            List<int> colsForPlayer = mainPlayerCheckers.Select(x => x.Column).Distinct().ToList();
            colsForPlayer.AddRange(secondPlayerCheckers.Select(x => x.Column).Distinct());

            for (int col = 0; col < 8; col++)
            {
                for (int row = 0; row < 8; row++)
                {
                    if (allPositions.Any(x => x.Row == row && x.Column == col))
                    {
                        continue;
                    }
                    if (row % 2 == 1 && col % 2 == 1)
                    {
                        positions.Add(new CheckerModel(col, row, PieceType.Checker, Side.Empty));
                        continue;
                    }
                    if (row % 2 == 0 && col % 2 == 0)
                    {
                        positions.Add(new CheckerModel(col, row, PieceType.Checker, Side.Empty));
                    }
                }
            }
            return positions;

        }

        private CheckerModel[,] GetCurrentGamePositions(IEnumerable<CheckerModel> mainPlayerElements, IEnumerable<CheckerModel> secondPlayElements, IEnumerable<CheckerModel> emptyElementsList)
        {
            var data = new CheckerModel[8, 8];
            foreach (CheckerModel position in mainPlayerElements)
            {
                data[position.Column, position.Row] = position;
            }

            foreach (CheckerModel position in secondPlayElements)
            {
                data[position.Column, position.Row] = position;
            }

            foreach (CheckerModel position in emptyElementsList)
            {
                data[position.Column, position.Row] = position;
            }

            return data;
        }
        private List<CheckerModel> GetMainPlayerCheckers(Side side)
        {
            var positions = new List<CheckerModel>();
            for (int col = 0; col < 8; col++)
            {
                for (int row = 0; row < 3; row++)
                {
                    if (row % 2 == 1 && col % 2 == 1)
                    {
                        positions.Add(new CheckerModel(col, row, PieceType.Checker, side));
                        continue;
                    }
                    if (row % 2 == 0 && col % 2 == 0)
                    {
                        positions.Add(new CheckerModel(col, row, PieceType.Checker, side));
                    }
                }
            }
            return positions;
        }


        private List<CheckerModel> GetSecondPlayerPositions(Side side)
        {
            var positions = new List<CheckerModel>();
            for (int col = 0; col < 8; col++)
            {
                for (int row = 5; row < 8; row++)
                {
                    if (row % 2 == 1 && col % 2 == 1)
                    {
                        positions.Add(new CheckerModel(col, row, PieceType.Checker, side));
                        continue;
                    }
                    if (row % 2 == 0 && col % 2 == 0)
                    {
                        positions.Add(new CheckerModel(col, row, PieceType.Checker, side));
                    }

                }
            }
            return positions;

        }

        public List<CheckerModel> GetSideCheckers(Side side)
        {
            var resultList = new List<CheckerModel>();
            foreach (CheckerModel checkerElement in _data)
            {
                if (checkerElement?.Side == side)
                {
                    resultList.Add(checkerElement);
                }
            }

            return resultList;
        }

        public void AddNewChecker(CheckerModel checkerToAdd, int currentEmptyColumn, int currentEmptyRow)
        {
            CheckerModel emptyCheckerToDelete = _data[currentEmptyColumn, currentEmptyRow];
            _data[currentEmptyColumn, currentEmptyRow] = checkerToAdd;
            _added.Add(checkerToAdd);
            _deleted.Add(emptyCheckerToDelete);
        }

        public void MoveCheckerToNewPosition(CheckerModel checkerToMove, int newPositionColumn, int newPositionRow)
        {
            int fromColumn = checkerToMove.Column;
            int fromRow = checkerToMove.Row;
            CheckerModel oldPositionedItem = _data[newPositionColumn, newPositionRow];

            if (checkerToMove == oldPositionedItem)
            {
                return;
            }

            if (fromColumn == newPositionColumn && fromRow == newPositionRow)
            {
                 _data[newPositionColumn, newPositionRow] = checkerToMove;
                _added.Add(checkerToMove);
                _deleted.Add(oldPositionedItem);
                return;
            }

            _data[newPositionColumn, newPositionRow] = checkerToMove;
            checkerToMove.SetNewPosition(newPositionColumn, newPositionRow);
            
            _data[fromColumn, fromRow] = oldPositionedItem;
            oldPositionedItem.SetNewPosition(fromColumn, fromRow);

            _modified.Add(oldPositionedItem);
            _modified.Add(checkerToMove);

        }

        public DataProvider Clone()
        {
            var clonedCheckers = new CheckerModel[8,8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    clonedCheckers[j, i] = _data[j, i] == null? null: _data[j, i].Clone();
                }
            }
            return new DataProvider(clonedCheckers);
        }

        public void StartTrackChanges()
        {
            _added = new List<CheckerModel>();
            _deleted = new List<CheckerModel>();
            _modified = new List<CheckerModel>();
        }

        public void StopTrackChanges()
        {
            NotificationAction?.Invoke(_added, _deleted, _modified);
        }
    }
}