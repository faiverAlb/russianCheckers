using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RussianCheckers
{
    public class Presenter : ObservableObject
    {
        private readonly TextConverter _textConverter = new TextConverter(s => s.ToUpper());
        private string _someText;
        private readonly ObservableCollection<string> _history = new ObservableCollection<string>();
        private CheckerElement _checkerElement;

        public ObservableCollection<CheckerElement> Positions
        {
            get
            {
                List<CheckerElement> positions = GetInitialWhitePositions();
                positions.AddRange(GetInitialBlackPositions());
                var result = new ObservableCollection<CheckerElement>(positions);
                return result;
            }  
        }

        private List<CheckerElement> GetInitialWhitePositions()
        {
            var positions = new List<CheckerElement>();
            for (int col = 1; col <= 8; col++)
            {
                for (int row = 1; row <= 3; row++)
                {
                    if (row %2 == 1 && col % 2 == 1)
                    {
                        positions.Add(new CheckerElement(col, row, PieceType.Checker, Side.White));
                    }
                    if (row %2 == 0 && col % 2 == 0)
                    {
                        positions.Add(new CheckerElement(col, row, PieceType.Checker, Side.White));
                    }
                }
            }
            return  positions;
        }

        private List<CheckerElement> GetInitialBlackPositions()
        {
            var positions = new List<CheckerElement>();
            for (int col = 1; col <= 8; col++)
            {
                for (int row = 6; row <= 8; row++)
                {
                    if (row % 2 == 1 && col % 2 == 1)
                    {
                        positions.Add(new CheckerElement(col, row, PieceType.Checker, Side.Black));
                    }
                    if (row % 2 == 0 && col % 2 == 0)
                    {
                        positions.Add(new CheckerElement(col, row, PieceType.Checker, Side.Black));
                    }
                }
            }
            return positions;

        }



        public ICommand EditCommand { get { return new RelayCommand<CheckerElement>(OnEdit); } }

        private void OnEdit(object obj)
        {
            var test = obj as CheckerElement;
            test.Type = test.Type == PieceType.Checker ? PieceType.Queen : PieceType.Checker;
        }

        private void ConvertText()
        {
//        if (string.IsNullOrWhiteSpace(SomeText)) return;
//        AddToHistory(_textConverter.ConvertText(SomeText));
//        SomeText = string.Empty;
            var test = 123;
        }

        //        private void AddToHistory(string item)
        //        {
        //            if (!_history.Contains(item))
        //                _history.Add(item);
        //        }
    }
}