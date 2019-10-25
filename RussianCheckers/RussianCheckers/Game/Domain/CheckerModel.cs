using System.Collections.Generic;

namespace RussianCheckers.Game
{
    public class CheckerModel
    {
        private readonly PieceType _type;
        private readonly Side _side;

        public CheckerModel(int column, int row, PieceType type, Side side)
        {
            Column = column;
            Row = row;
            _type = type;
            _side = side;
            PossibleMovementElements = new List<CheckerModel>();
            Neighbors = new List<CheckerModel>();
            IsAtInitialPosition = true;

        }

        public bool IsAtInitialPosition { get; set; }

        public List<CheckerModel> Neighbors { get; set; }

        public List<CheckerModel> PossibleMovementElements { get; set; }

        public int Row { get; set; }

        public int Column { get; set; }
        public Side Side
        {
            get { return _side; }
        }

        public PieceType Type
        {
            get { return _type; }
        }

        public CheckerModel Clone()
        {
            return new CheckerModel(Column, Row, _type, _side);

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

    }
}