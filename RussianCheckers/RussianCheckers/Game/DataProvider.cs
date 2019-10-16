using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace RussianCheckers.Game
{
    public class DataProvider
    {
        private readonly CheckerElement[,] _data;

        public DataProvider(List<CheckerElement> mainPlayerCheckers, List<CheckerElement> secondPlayerCheckers)
        {
            List<CheckerElement> emptyCheckers = GetEmptyCheckersPositions(mainPlayerCheckers, secondPlayerCheckers);
            _data = GetCurrentGamePositions(mainPlayerCheckers, secondPlayerCheckers, emptyCheckers);
        }

        public DataProvider(Side mainPlayerSide)
        {
            List<CheckerElement> mainPlayerCheckers = GetMainPlayerCheckers(mainPlayerSide);
            Side secondPlayerSide = mainPlayerSide == Side.White? Side.Black: Side.White;
            
            List<CheckerElement> secondPlayerCheckers = GetSecondPlayerPositions(secondPlayerSide);
            List<CheckerElement> emptyCheckers = GetEmptyCheckersPositions(mainPlayerCheckers, secondPlayerCheckers);

            _data = GetCurrentGamePositions(mainPlayerCheckers, secondPlayerCheckers, emptyCheckers);

        }


        public CheckerElement GetElementAtPosition(int column, int row)
        {
            return _data[column, row];
        }

        private List<CheckerElement> GetEmptyCheckersPositions(List<CheckerElement> mainPlayerCheckers, List<CheckerElement> secondPlayerCheckers)
        {
            var positions = new List<CheckerElement>();
            List<CheckerElement> allPositions = mainPlayerCheckers.ToList();
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
                        positions.Add(new CheckerElement(col, row, PieceType.Checker, Side.Empty));
                        continue;
                    }
                    if (row % 2 == 0 && col % 2 == 0)
                    {
                        positions.Add(new CheckerElement(col, row, PieceType.Checker, Side.Empty));
                        continue;
                    }
                }
            }
            return positions;

        }

        private CheckerElement[,] GetCurrentGamePositions(IEnumerable<CheckerElement> mainPlayerElements, IEnumerable<CheckerElement> secondPlayElements, IEnumerable<CheckerElement> emptyElementsList)
        {
            var data = new CheckerElement[8, 8];
            foreach (CheckerElement position in mainPlayerElements)
            {
                data[position.Column, position.Row] = position;
            }

            foreach (CheckerElement position in secondPlayElements)
            {
                data[position.Column, position.Row] = position;
            }

            foreach (CheckerElement position in emptyElementsList)
            {
                data[position.Column, position.Row] = position;
            }

            return data;
        }
        private List<CheckerElement> GetMainPlayerCheckers(Side side)
        {
            var positions = new List<CheckerElement>();
            for (int col = 0; col < 8; col++)
            {
                for (int row = 0; row < 3; row++)
                {
                    if (row % 2 == 1 && col % 2 == 1)
                    {
                        positions.Add(new CheckerElement(col, row, PieceType.Checker, side));
                        continue;
                    }
                    if (row % 2 == 0 && col % 2 == 0)
                    {
                        positions.Add(new CheckerElement(col, row, PieceType.Checker, side));
                    }
                }
            }
            return positions;
        }


        private List<CheckerElement> GetSecondPlayerPositions(Side side)
        {
            var positions = new List<CheckerElement>();
            for (int col = 0; col < 8; col++)
            {
                for (int row = 5; row < 8; row++)
                {
                    if (row % 2 == 1 && col % 2 == 1)
                    {
                        positions.Add(new CheckerElement(col, row, PieceType.Checker, side));
                        continue;
                    }
                    if (row % 2 == 0 && col % 2 == 0)
                    {
                        positions.Add(new CheckerElement(col, row, PieceType.Checker, side));
                        continue;
                    }

                }
            }
            return positions;

        }


        public List<CheckerElement> GetMySideCheckers(Side side)
        {
            var resultList = new List<CheckerElement>();
            foreach (CheckerElement checkerElement in _data)
            {
                if (checkerElement?.Side == side)
                {
                    resultList.Add(checkerElement);
                }
            }

            return resultList;
        }


        public void SetNewCheckerAtElement(int checkerElementColumn, int checkerElementRow, CheckerElement element)
        {
            _data[checkerElementColumn, checkerElementRow] = element;
        }
    }
}