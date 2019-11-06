using System.Collections.Generic;

namespace RussianCheckers.Core
{
    public class HistoryMove
    {
        public readonly bool IsConvertedToQueen;

        public HistoryMove(bool isConvertedToQueen)
        {
            IsConvertedToQueen = isConvertedToQueen;
            DeletedList = new List<KeyValuePair<CheckerModel, CheckerModel>>();
            MovedFromTo = new KeyValuePair<CheckerModel, CheckerModel>();
        }

        public List<KeyValuePair<CheckerModel,CheckerModel>> DeletedList { get; }
        public KeyValuePair<CheckerModel, CheckerModel> MovedFromTo { get; set; }
    }
}