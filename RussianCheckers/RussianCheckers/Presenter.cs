using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RussianCheckers
{
    public class Presenter : ObservableObject
    {
        private readonly TextConverter _textConverter = new TextConverter(s => s.ToUpper());
        private string _someText = "aaaaa";
        private readonly ObservableCollection<string> _history = new ObservableCollection<string>();


        private Point _Pos;
        public Point Pos
        {
            get { return this._Pos; }
            set { this._Pos = value; RaisePropertyChangedEvent(nameof(this.Pos)); }
        }

        private PieceType _Type;
        public PieceType Type
        {
            get { return this._Type; }
            set { this._Type = value; RaisePropertyChangedEvent(nameof(this.Type)); }
        }

        private Side _Side;
        public Side Side
        {
            get { return this._Side; }
            set { this._Side = value; RaisePropertyChangedEvent(nameof(this.Side)); ; }
        }


        public string SomeText
        {
            get { return _someText; }
            set
            {
                _someText = value;
                RaisePropertyChangedEvent("SomeText");
            }
        }

        public IEnumerable<string> History
        {
            get { return _history; }
        }

        public ICommand ConvertTextCommand
        {
            get { return new DelegateCommand(ConvertText); }
        }

        private void ConvertText()
        {
            if (string.IsNullOrWhiteSpace(SomeText)) return;
            AddToHistory(_textConverter.ConvertText(SomeText));
            SomeText = string.Empty;
        }

        private void AddToHistory(string item)
        {
            if (!_history.Contains(item))
                _history.Add(item);
        }
    }

    public class Point
    {
        public Point(int y, int x)
        {
            Y = y;
            X = x;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }
}