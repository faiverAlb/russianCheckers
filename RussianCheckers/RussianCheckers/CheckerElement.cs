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
            get { return this._type; }
            set { this._type = value; RaisePropertyChangedEvent(nameof(Type)); }
        }

        private Side _side;
        public Side Side
        {
            get { return this._side; }
            set { this._side = value; RaisePropertyChangedEvent(nameof(Side)); ; }
        }
    }
}