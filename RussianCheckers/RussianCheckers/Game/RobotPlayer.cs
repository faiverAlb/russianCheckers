using System.Collections.Generic;
using System.Linq;

namespace RussianCheckers.Game
{
    public class RobotPlayer: PlayerViewModel
    {
        public RobotPlayer(Side side, DataProvider dataProvider) : base(side, dataProvider, false)
        {
        }

        public KeyValuePair<CheckerElement,CheckerElement> GetOptimalMove()
        {
            //Use strategy
            List<LinkedList<CheckerElement>> availableTakes = AvailablePaths;
            if (availableTakes.Any())
            {
                var checkerToMove = availableTakes.First().First.Value;
                var toMove = availableTakes.First().Last.Value;
                return new KeyValuePair<CheckerElement, CheckerElement>(checkerToMove, toMove);
            }

            var playerPosition  = PlayerPositions.FirstOrDefault(x => x.PossibleMovementElements.Any());
            CheckerElement possibleMove = null;
            if (playerPosition != null)
            {
                playerPosition.IsSelected = true;
                possibleMove = playerPosition.PossibleMovementElements.First();
            }

            return new KeyValuePair<CheckerElement, CheckerElement>(playerPosition,possibleMove);
        }
    }
}