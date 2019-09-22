using System;

namespace RussianCheckers
{
    public class CheckerElement : ObservableObject
    {
        public CheckerElement(int column, int row, PieceType type, Side side)
        {
            _pos = new Point(row, column);
            _column = column;
            _row = row;
            _type = type;
            _side = side;
        }
        private Point _pos;
        public Point Pos
        {
            get { return this._pos; }
            set { this._pos = value; RaisePropertyChangedEvent(nameof(Pos)); }
        }

        private readonly int _column;
        private readonly int _row;
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

    }


    public class Point
    {
        public Point(int row, int column)
        {
            if (row < 1 || row > 8)
                throw new Exception("Invalid row value");
            if (column < 1 || column > 8)
                throw new Exception("Invalid column value");

            X = CalculateX(column - 1);
            Y = CalculateY(8 - row);
        }

        private int CalculateX(int column)
        {
            return column * 60 + 5;
        }

        private int CalculateY(int row)
        {
            return row * 60 + 5;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }

}