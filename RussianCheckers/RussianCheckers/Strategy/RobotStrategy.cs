using System.Collections.Generic;
using System.Collections.ObjectModel;
using RussianCheckers.Game;

namespace RussianCheckers.Strategy
{
    public abstract class RobotStrategy
    {
        public abstract KeyValuePair<CheckerElement, CheckerElement> GetSuggestedMove(List<LinkedList<CheckerElement>> availableTakes, ObservableCollection<CheckerElement> playerPositions);
    }
}