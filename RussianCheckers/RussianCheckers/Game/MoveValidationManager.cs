using RussianCheckers.Game.GameInfrastructure;

namespace RussianCheckers.Game
{
    public class MoveValidationManager
    {
        private readonly CheckerElement _oldSelectedElement;
        private readonly CheckerElement _newSelectedElement;
        private readonly Side _nextMoveSide;

        public MoveValidationManager(CheckerElement oldSelectedElement, CheckerElement newSelectedElement,
            Side nextMoveSide)
        {
            _oldSelectedElement = oldSelectedElement;
            _newSelectedElement = newSelectedElement;
            _nextMoveSide = nextMoveSide;
        }
        public MoveValidationResult GetPreValidationResult()
        {
            if (_oldSelectedElement == null && _newSelectedElement.Side == Side.Empty)
            {
                return new MoveValidationResult(MoveValidationStatus.NothingSelected, "Please select checker first");
            }

            if (_newSelectedElement.Side != _nextMoveSide && _newSelectedElement.Side != Side.Empty)
            {
                return new MoveValidationResult(MoveValidationStatus.Error, $"Next move should be done by {_nextMoveSide}");
            }

            return new MoveValidationResult(MoveValidationStatus.Ok);
        }

        public MoveValidationResult GetMoveValidationResult()
        {
            if (_oldSelectedElement == null)
            {
                return new MoveValidationResult(MoveValidationStatus.NewItemSelected);
            }

            if (_oldSelectedElement == _newSelectedElement)
            {
                return new MoveValidationResult(MoveValidationStatus.DeselectChecker);
            }

            if (_oldSelectedElement.Side == _newSelectedElement.Side)
            {
                return new MoveValidationResult(MoveValidationStatus.NewItemSelected);
            }

            if (_oldSelectedElement != null && !_oldSelectedElement.CanMoveToPosition(_newSelectedElement))
            {
                return new MoveValidationResult(MoveValidationStatus.Error, "Can't move checker to this place");
            }

            return new MoveValidationResult(MoveValidationStatus.Ok);
        }
    }
}