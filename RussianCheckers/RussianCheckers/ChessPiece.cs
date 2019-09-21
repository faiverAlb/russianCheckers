namespace RussianCheckers
{
    public class ChessPiece : ObservableObject
    {

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
    }
}