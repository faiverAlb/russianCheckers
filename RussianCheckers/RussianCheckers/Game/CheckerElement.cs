using System;
using System.Collections.Generic;

namespace RussianCheckers.Game
{
    public class CheckerElement : ObservableObject
    {
        public CheckerElement(int column, int row, PieceType type, Side side)
        {
            _pos = new Point(row, column);
            Column = column;
            Row = row;
            _type = type;
            _side = side;
            PossibleMovementElements = new List<CheckerElement>();
            Neighbors = new List<CheckerElement>();
        }
        private Point _pos;
        public Point Pos
        {
            get { return this._pos; }
            set { this._pos = value; RaisePropertyChangedEvent(nameof(Pos)); }
        }

        public List<CheckerElement> PossibleMovementElements { get; private set; }

        public void SetPossibleMovementElements(List<CheckerElement> possibleMovementElements)
        {
            PossibleMovementElements = possibleMovementElements;
        }

        public bool CanMoveToPosition(CheckerElement element)
        {
            return PossibleMovementElements.Contains(element);
        }



        public int Column { get; private set; }
        public int Row { get; private set; }
        public bool HaveOtherSideNeighbor { get; private set; }

        private PieceType _type;
        public PieceType Type
        {
            get { return _type; }
            set { _type = value; RaisePropertyChangedEvent(nameof(Type)); }
        }

        private Side _side;
        private bool _isSelected;

        public Side Side
        {
            get { return _side; }
            set { _side = value; RaisePropertyChangedEvent(nameof(Side)); ; }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set {
                _isSelected = value;
                RaisePropertyChangedEvent(nameof(IsSelected));
                foreach (CheckerElement possibleMovementElement in PossibleMovementElements)
                {
                    possibleMovementElement.IsSelected = value;
                }
            }
        }

        public List<CheckerElement> Neighbors { get; private set; }

        public void SetNeighbors(List<CheckerElement> neighbors, bool haveOtherSideNeighbor)
        {
            Neighbors = neighbors;
            HaveOtherSideNeighbor = haveOtherSideNeighbor;
        }


        public void SetNewPosition(int column, int row)
        {
            Column = column;
            Row = row;
            _pos.ChangePosition(column, row);
        }

        public bool IsCheckerNearChecker(CheckerElement otherChecker)
        {
            bool isNear = false;
            isNear = Column - 1 == otherChecker.Column && Row - 1 == otherChecker.Row;
            isNear = isNear || Column - 1 == otherChecker.Column && Row + 1 == otherChecker.Row;
            isNear = isNear || Column - 1 == otherChecker.Column && Row + 1 == otherChecker.Row;
            isNear = isNear || Column + 1 == otherChecker.Column && Row - 1 == otherChecker.Row;
            isNear = isNear || Column + 1 == otherChecker.Column && Row + 1 == otherChecker.Row;

            return isNear;
        }

        public override string ToString()
        {
            return $"{Side}, {Type}, [{Column},{Row}]";
        }
    }


    public class Point:ObservableObject
    {
        private int _x;
        private int _y;

        public Point(int row, int column)
        {
            if (row < 0 || row > 7)
                throw new Exception("Invalid row value");
            if (column < 0 || column > 7)
                throw new Exception("Invalid column value");

            ChangePosition(column, row);
            
        }

        private int CalculateX(int column)
        {
            return column * 60 + 5;
        }

        private int CalculateY(int row)
        {
            return row * 60 + 5;
        }

        public int X
        {
            get
            {
                return _x;
            }
            private set
            {
                _x = value;
                RaisePropertyChangedEvent(nameof(X));
            }
        }

        public int Y
        {
            get
            {
                return _y;
            }
            private set
            {
                _y = value;
                RaisePropertyChangedEvent(nameof(Y));
            }
        }

        public void ChangePosition(int column, int row)
        {
            X = CalculateX(column);
            Y = CalculateY(7 - row);
        }
    }

}