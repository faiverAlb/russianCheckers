namespace RussianCheckers.Game
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
    public enum MoveValidationStatus
    {
        Ok,
        Error,
        NewItemSelected,
        DeselectChecker,
        NothingSelected
    }

}