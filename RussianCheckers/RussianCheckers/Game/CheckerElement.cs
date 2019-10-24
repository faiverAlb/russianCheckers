using System.Collections.Generic;

namespace RussianCheckers.Game
{
    public class CheckerElement : ObservableObject
    {
        public CheckerElement(int column, int row, PieceType type, Side side)
        {
            _pos = new Point(row, column);
            Column = column;
            Row = row;
            _type = type;
            _side = side;
            PossibleMovementElements = new List<CheckerElement>();
            Neighbors = new List<CheckerElement>();
            IsAtInitialPosition = true;
        }

        public bool IsAtInitialPosition { get; private set; }

        private Point _pos;
        public Point Pos
        {
            get { return this._pos; }
            set { this._pos = value; RaisePropertyChangedEvent(nameof(Pos)); }
        }

        public List<CheckerElement> PossibleMovementElements { get; private set; }

        public void SetPossibleMovementElements(List<CheckerElement> possibleMovementElements)
        {
            PossibleMovementElements = possibleMovementElements;
        }

        public bool CanMoveToPosition(CheckerElement element)
        {
            return PossibleMovementElements.Contains(element);
        }



        public int Column { get; private set; }
        public int Row { get; private set; }

        private PieceType _type;
        public PieceType Type
        {
            get { return _type; }
            set { _type = value; RaisePropertyChangedEvent(nameof(Type)); }
        }

        private Side _side;
        private bool _isSelected;
        private bool _selfAsPossible;

        public Side Side
        {
            get { return _side; }
            set { _side = value; RaisePropertyChangedEvent(nameof(Side)); ; }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set {
                if (_isSelected == value)
                {
                    return;
                }
                _isSelected = value;
                RaisePropertyChangedEvent(nameof(IsSelected));
            }
        }
        public bool SelfAsPossible
        {
            get { return _selfAsPossible; }
            set {
                if (_selfAsPossible == value)
                {
                    return;
                }
                _selfAsPossible = value;
                RaisePropertyChangedEvent(nameof(SelfAsPossible));
            }
        }

        

        public List<CheckerElement> Neighbors { get; private set; }

        public void SetNeighbors(List<CheckerElement> neighbors)
        {
            Neighbors = neighbors;
        }

        public void SetNewPosition(int column, int row)
        {
            Column = column;
            Row = row;
            _pos.ChangePosition(column, row);
            IsAtInitialPosition = false;
        }

        public override string ToString()
        {
            return $"{Side}, {Type}, [{Column},{Row}]";
        }

        public void SelectPossibleMovement()
        {
            foreach (CheckerElement possibleMovementElement in PossibleMovementElements)
            {
                if (possibleMovementElement == this)
                {
                    possibleMovementElement.SelfAsPossible = true;
                }

                possibleMovementElement.IsSelected = true;
            }
        }
        public void DeSelectPossibleMovement()
        {
            foreach (CheckerElement possibleMovementElement in PossibleMovementElements)
            {
              possibleMovementElement.IsSelected = false;
              possibleMovementElement.SelfAsPossible = false;
            }
        }

        public CheckerElement Clone()
        {
            return new CheckerElement(Column, Row, Type, Side);
        }
    }
}