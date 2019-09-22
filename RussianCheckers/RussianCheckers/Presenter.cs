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

        public ObservableCollection<ChessPiece> Positions
        {
            get
            {
                return new ObservableCollection<ChessPiece>()
                {
                    new ChessPiece
                    {
                        Pos = new Point(1, 1), Type = PieceType.Queen, Side = Side.Black
                    },
                    new ChessPiece
                    {
                        Pos = new Point(1, 7), Type = PieceType.Checker, Side = Side.White
                    },
                    new ChessPiece
                    {
                        Pos = new Point(8, 8), Type = PieceType.Queen, Side = Side.White
                    },
                    new ChessPiece
                    {
                        Pos = new Point(4, 4), Type = PieceType.Queen, Side = Side.White
                    },
                };
            }  
        }


//        public string SomeText
//        {
//            get { return _someText; }
//            set
//            {
//                _someText = value;
//                RaisePropertyChangedEvent("SomeText");
//            }
//        }

//        public IEnumerable<string> History
//        {
//            get { return _history; }
//        }

//        public ICommand ConvertTextCommand
//        {
//            get { return new DelegateCommand(ConvertText); }
//        }

//        private void ConvertText()
//        {
//            if (string.IsNullOrWhiteSpace(SomeText)) return;
//            AddToHistory(_textConverter.ConvertText(SomeText));
//            SomeText = string.Empty;
//        }

//        private void AddToHistory(string item)
//        {
//            if (!_history.Contains(item))
//                _history.Add(item);
//        }
    }
}