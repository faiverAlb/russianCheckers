using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace RussianCheckers
{

    public class GameViewModel : ObservableObject
    {
        private readonly PlayerViewModel playerOne;
        private readonly PlayerViewModel playerTwo;

        public GameViewModel(BoardViewModel boardViewModel
            , PlayerViewModel playerOne
            , PlayerViewModel playerTwo)
        {
            BoardViewModel = boardViewModel;
            this.playerOne = playerOne;
            this.playerTwo = playerTwo;
        }

        public BoardViewModel BoardViewModel { get; }
    }


    public class BoardViewModel : ObservableObject
    {
//        private readonly TextConverter _textConverter = new TextConverter(s => s.ToUpper());
//        private string _someText;
//        private readonly ObservableCollection<string> _history = new ObservableCollection<string>();
        private readonly ObservableCollection<CheckerElement> _positions = new ObservableCollection<CheckerElement>();



        public ObservableCollection<CheckerElement> Positions
        {
            get
            {
                List<CheckerElement> positions = GetInitialWhitePositions();
                positions.AddRange(GetInitialBlackPositions());
                positions.AddRange(GetInitialEmptyPositions());
                foreach (CheckerElement position in positions)
                {
                    _positions.Add(position);
                }
                return _positions;
            }  
        }

        private IEnumerable<CheckerElement> GetInitialEmptyPositions()
        {
            var positions = new List<CheckerElement>();
            for (int col = 1; col <= 8; col++)
            {
                for (int row = 4; row <= 5; row++)
                {
                    positions.Add(new CheckerElement(col, row, PieceType.Checker, Side.Empty));
                }
            }
            return positions;

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
                        continue;
                    }
                    if (row %2 == 0 && col % 2 == 0)
                    {
                        positions.Add(new CheckerElement(col, row, PieceType.Checker, Side.White));
                        continue;
                    }
                    positions.Add(new CheckerElement(col, row, PieceType.Checker, Side.Empty));
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
                        continue;
                    }
                    if (row % 2 == 0 && col % 2 == 0)
                    {
                        positions.Add(new CheckerElement(col, row, PieceType.Checker, Side.Black));
                        continue;
                    }
                    positions.Add(new CheckerElement(col, row, PieceType.Checker, Side.Empty));

                }
            }
            return positions;

        }



        public ICommand SelectCheckerCommand { get { return new RelayCommand<CheckerElement>(OnSelectChecker); } }

        private CheckerElement _selectedChecker;
        private void OnSelectChecker(object obj)
        {
            var newSelectedChecker = (CheckerElement)obj;
            if (_selectedChecker != null)
            {
                _selectedChecker.IsSelected = false;
            }

            _selectedChecker = newSelectedChecker;
            _selectedChecker.IsSelected = true;
        }

//        private void ConvertText()
//        {
////        if (string.IsNullOrWhiteSpace(SomeText)) return;
////        AddToHistory(_textConverter.ConvertText(SomeText));
////        SomeText = string.Empty;
//            var test = 123;
//        }

        //        private void AddToHistory(string item)
        //        {
        //            if (!_history.Contains(item))
        //                _history.Add(item);
        //        }
    }
}