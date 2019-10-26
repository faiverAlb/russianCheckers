﻿using System.Collections.Generic;
using System.Linq;
using RussianCheckers.Core;

namespace RussianCheckers.Game
{
    public class CheckerElementViewModel : ObservableObject
    {
        public CheckerElementViewModel(CheckerModel checkerModel, List<CheckerElementViewModel> emptyCheckerElementViewModels)
        {
            _checkerModel = checkerModel;
            _checkerModel.PositionChangedAction += PositionChangedAction;            

            Side = checkerModel.Side;
            _pos = new PointViewModel(_checkerModel.Row, _checkerModel.Column);
            PossibleMovementElements = emptyCheckerElementViewModels.Where(x => _checkerModel.PossibleMovementElements.Any(y => x.Column == y.Column && x.Row == y.Row)).ToList();
            //            Neighbors = new List<CheckerElementViewModel>(_checkerModel.Neighbors.Select(x => new CheckerElementViewModel(x)));
        }

        private void PositionChangedAction(int column, int row)
        {
            _pos.ChangePosition(column, row);
        }

        public bool IsAtInitialPosition
        {
            get
            {
                return _checkerModel.IsAtInitialPosition;
            }
        }

        private PointViewModel _pos;
        public PointViewModel Pos
        {
            get { return this._pos; }
            set { this._pos = value; RaisePropertyChangedEvent(nameof(Pos)); }
        }

        public List<CheckerElementViewModel> PossibleMovementElements { get; private set; }

//        public void SetPossibleMovementElements(List<CheckerElementViewModel> possibleMovementElements)
//        {
//            PossibleMovementElements = possibleMovementElements;
//        }

        public bool CanMoveToPosition(CheckerElementViewModel elementViewModel)
        {
            return PossibleMovementElements.Contains(elementViewModel);
        }



        public int Column
        {
            get { return _checkerModel.Column; }
        }

        public int Row
        {
            get { return _checkerModel.Row; }

        }

        private PieceType _type;
        public PieceType Type
        {
            get { return _type; }
            set { _type = value; RaisePropertyChangedEvent(nameof(Type)); }
        }

        private Side _side;
        private bool _isSelected;
        private bool _selfAsPossible;
        private CheckerModel _checkerModel;

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

        

//        public List<CheckerElementViewModel> Neighbors { get; private set; }

//        public void SetNeighbors(List<CheckerElementViewModel> neighbors)
//        {
//            Neighbors = neighbors;
//        }

//        public void SetNewPosition(int column, int row)
//        {
//            Column = column;
//            Row = row;
//            _pos.ChangePosition(column, row);
//            IsAtInitialPosition = false;
//        }

        public override string ToString()
        {
            return $"{Side}, {Type}, [{Column},{Row}]";
        }

        public void SelectPossibleMovement()
        {
            foreach (CheckerElementViewModel possibleMovementElement in PossibleMovementElements)
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
            foreach (CheckerElementViewModel possibleMovementElement in PossibleMovementElements)
            {
              possibleMovementElement.IsSelected = false;
              possibleMovementElement.SelfAsPossible = false;
            }
        }

//        public CheckerElementViewModel Clone()
//        {
//            return new CheckerElementViewModel(_checkerModel.Clone());
//        }
        public void ReSetPossibleMovements(List<CheckerElementViewModel> emptyItems)
        {
            PossibleMovementElements = emptyItems.Where(x => _checkerModel.PossibleMovementElements.Any(y => x.Column == y.Column && x.Row == y.Row)).ToList();
        }
    }
}