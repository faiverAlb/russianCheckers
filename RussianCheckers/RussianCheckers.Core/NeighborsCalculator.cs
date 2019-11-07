using System.Collections.Generic;
using System.Linq;

namespace RussianCheckers.Core
{
    public class NeighborsCalculator
    {
        private readonly DataProvider _dataProvider;
        private readonly List<CheckerModel> _playerPositions;

        public NeighborsCalculator(DataProvider dataProvider, List<CheckerModel> playerPositions)
        {
            _dataProvider = dataProvider;
            _playerPositions = playerPositions;
        }
        public Dictionary<CheckerModel, List<CheckerModel>> CalculateNeighbors()
        {
            var dictionary = new Dictionary<CheckerModel, List<CheckerModel>>();
            foreach (CheckerModel playerChecker in _playerPositions)
            {
                CheckerModel checkerModel = _dataProvider.GetElementAtPosition(playerChecker.Column, playerChecker.Row);
                List<CheckerModel> neighbors = checkerModel.Type == PieceType.Checker ? GetNeighborsForChecker(checkerModel) : GetNeighborsForQueen(checkerModel).Select(x => x.Value).ToList();
                dictionary.Add(playerChecker, neighbors);
            }

            return dictionary;
        }


        public List<KeyValuePair<Diagonal, CheckerModel>> GetNeighborsForQueen(CheckerModel checkerAsQueen)
        {
            var neighbors = new List<KeyValuePair<Diagonal, CheckerModel>>();

            int checkerRowUp = checkerAsQueen.Row;
            int checkerRowDown = checkerAsQueen.Row;
            bool skipUpDiagonal = false;
            bool skipDownDiagonal = false;
            for (int col = checkerAsQueen.Column - 1; col >= 0; col--)
            {
                if (checkerRowUp + 1 < 8 && !skipUpDiagonal)
                {
                    var element = _dataProvider.GetElementAtPosition(col, checkerRowUp + 1);
                    if (element.Side != Side.Empty)
                    {
                        skipUpDiagonal = true;
                    }

                    neighbors.Add(new KeyValuePair<Diagonal, CheckerModel>(Diagonal.LeftUp, element));
                    checkerRowUp++;
                }

                if (checkerRowDown - 1 >= 0 && !skipDownDiagonal)
                {
                    var element = _dataProvider.GetElementAtPosition(col, checkerRowDown - 1);
                    if (element.Side != Side.Empty)
                    {
                        skipDownDiagonal = true;
                    }

                    neighbors.Add(new KeyValuePair<Diagonal, CheckerModel>(Diagonal.LeftDown, element));
                    checkerRowDown--;

                }
            }

            checkerRowUp = checkerAsQueen.Row;
            checkerRowDown = checkerAsQueen.Row;
            skipUpDiagonal = false;
            skipDownDiagonal = false;
            for (int col = checkerAsQueen.Column + 1; col < 8; col++)
            {
                if (checkerRowUp + 1 < 8 && !skipUpDiagonal)
                {
                    var element = _dataProvider.GetElementAtPosition(col, checkerRowUp + 1);
                    if (element.Side != Side.Empty)
                    {
                        skipUpDiagonal = true;
                    }

                    neighbors.Add(new KeyValuePair<Diagonal, CheckerModel>(Diagonal.RightUp, element));
                    checkerRowUp++;
                }

                if (checkerRowDown - 1 >= 0 && !skipDownDiagonal)
                {
                    var element = _dataProvider.GetElementAtPosition(col, checkerRowDown - 1);
                    if (element.Side != Side.Empty)
                    {
                        skipDownDiagonal = true;
                    }

                    neighbors.Add(new KeyValuePair<Diagonal, CheckerModel>(Diagonal.RightDown, element));
                    checkerRowDown--;
                }
            }

            return neighbors;
        }

        private List<CheckerModel> GetNeighborsForChecker(CheckerModel сheckerModel)
        {
            var neighbors = new List<CheckerModel>();
            if (сheckerModel.Column - 1 >= 0)
            {
                if (сheckerModel.Row - 1 >= 0)
                {
                    var element = _dataProvider.GetElementAtPosition(сheckerModel.Column - 1, сheckerModel.Row - 1);
                    neighbors.Add(element);
                }

                if (сheckerModel.Row + 1 < 8)
                {
                    var element = _dataProvider.GetElementAtPosition(сheckerModel.Column - 1, сheckerModel.Row + 1);
                    neighbors.Add(element);
                }
            }
            if (сheckerModel.Column + 1 < 8)
            {
                if (сheckerModel.Row - 1 >= 0)
                {
                    var element = _dataProvider.GetElementAtPosition(сheckerModel.Column + 1, сheckerModel.Row - 1);
                    neighbors.Add(element);
                }

                if (сheckerModel.Row + 1 < 8)
                {
                    var element = _dataProvider.GetElementAtPosition(сheckerModel.Column + 1, сheckerModel.Row + 1);
                    neighbors.Add(element);
                }
            }
            return neighbors;
        }

    }
}