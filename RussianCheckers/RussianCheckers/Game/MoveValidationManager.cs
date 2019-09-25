using System.Security.Cryptography.X509Certificates;
using RussianCheckers.Game.GameInfrastructure;

namespace RussianCheckers
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
                return new MoveValidationResult(MoveValidationStatus.Error, "Please select checker first");
            }

            if (_newSelectedElement.Side != _nextMoveSide && _newSelectedElement.Side != Side.Empty)
            {
                return new MoveValidationResult(MoveValidationStatus.Error, $"Next move should be done by {_nextMoveSide}");
            }

            return new MoveValidationResult(MoveValidationStatus.Ok);
        }
    }
}