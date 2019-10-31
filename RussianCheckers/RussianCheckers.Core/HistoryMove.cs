using System.Collections.Generic;
using RussianCheckers.Core;

namespace RussianCheckers.Game
{
    public class HistoryMove
    {
        public HistoryMove()
        {
            DeletedList = new List<KeyValuePair<CheckerModel, CheckerModel>>();
            MovedFromTo = new KeyValuePair<CheckerModel, CheckerModel>();
        }

        public List<KeyValuePair<CheckerModel,CheckerModel>> DeletedList { get; private set; }
        public KeyValuePair<CheckerModel, CheckerModel> MovedFromTo { get; set; }
    }
}