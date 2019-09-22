using System;

namespace RussianCheckers
{
    public class Point
    {
        public Point(int row, int column)
        {
            if (row < 1 || row > 8)
                throw new Exception("Invalid row value");
            if (column < 1 || column > 8)
                throw new Exception("Invalid column value");

            X = CalculateX(column - 1);
            Y = CalculateY(row - 1);
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