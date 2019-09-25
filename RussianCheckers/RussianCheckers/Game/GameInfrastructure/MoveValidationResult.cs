namespace RussianCheckers.Game.GameInfrastructure
{
    public class MoveValidationResult
    {
        public MoveValidationStatus Status { get; }
        public string ErrorMessage { get; }

        public MoveValidationResult(MoveValidationStatus status,string errorMessage = null)
        {
            Status = status;
            ErrorMessage = errorMessage;
        }
    }
}