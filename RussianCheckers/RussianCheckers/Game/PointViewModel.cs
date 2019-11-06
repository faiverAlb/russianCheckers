using System;
using RussianCheckers.Infrastructure;

namespace RussianCheckers.Game
{
    public class PointViewModel:ObservableObject
    {
        private int _x;
        private int _y;

        public PointViewModel(int row, int column)
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

        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }
}