using System;

namespace RussianCheckers
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
        }
        private Point _pos;
        public Point Pos
        {
            get { return this._pos; }
            set { this._pos = value; RaisePropertyChangedEvent(nameof(Pos)); }
        }

        public int Column { get; private set; }
        public int Row { get; private set; }

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
            set { _isSelected = value; RaisePropertyChangedEvent(nameof(IsSelected)); ; }
        }

        public void SetNewPosition(int column, int row)
        {
            Column = column;
            Row = row;
            _pos.ChangePosition(column, row);
        }
    }


    public class Point:ObservableObject
    {
        private int _x;
        private int _y;

        public Point(int row, int column)
        {
            if (row < 1 || row > 8)
                throw new Exception("Invalid row value");
            if (column < 1 || column > 8)
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
            X = CalculateX(column - 1);
            Y = CalculateY(8 - row);
        }
    }

}