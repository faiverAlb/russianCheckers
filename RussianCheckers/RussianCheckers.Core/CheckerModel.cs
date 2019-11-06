using System;
using System.Collections.Generic;

namespace RussianCheckers.Core
{
    public class CheckerModel
    {

        public CheckerModel(int column, int row, PieceType type, Side side, Action<int,int> onChangePositionFunc = null)
        {
            Column = column;
            Row = row;
            Type = type;
            Side = side;
            PossibleMovementElements = new List<CheckerModel>();
            Neighbors = new List<CheckerModel>();
            IsAtInitialPosition = true;
            PositionChangedAction = onChangePositionFunc;

        }

        public Action<int, int> PositionChangedAction { get; set; }

        public bool IsAtInitialPosition { get; private set; }

        public List<CheckerModel> Neighbors { get; private set; }

        public List<CheckerModel> PossibleMovementElements { get; private set; }

        public int Row { get; private set; }

        public int Column { get; private set; }
        public Side Side { get; }

        public PieceType Type { get; private set; }

        public CheckerModel Clone()
        {
            return new CheckerModel(Column, Row, Type, Side);

        }

        public void SetNeighbors(List<CheckerModel> neighbors)
        {
            Neighbors = neighbors;
        }

        public void SetPossibleMovementElements(List<CheckerModel> possibleMovements)
        {
            PossibleMovementElements = possibleMovements;
        }

        public override string ToString()
        {
            return $"{Side}, {Type}, [{Column},{Row}]";
        }

        public void BecomeAQueen()
        {
            Type = PieceType.Queen;
        }

        public void DowngradeToChecker()
        {
            Type = PieceType.Checker;
        }


        public void SetNewPosition(int column, int row)
        {
            Column = column;
            Row = row;
            IsAtInitialPosition = false;
            PositionChangedAction?.Invoke(column, row);
        }

    }
}