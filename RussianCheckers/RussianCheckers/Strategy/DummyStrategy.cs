using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RussianCheckers.Game;

namespace RussianCheckers.Strategy
{
    public class DummyStrategy:RobotStrategy
    {
        public DummyStrategy()
        {

        }

        //TODO: Move to library 
        public override KeyValuePair<CheckerElement, CheckerElement> GetSuggestedMove(GameViewModel initialGameViewModel)
        {
            //            return new KeyValuePair<CheckerElement, CheckerElement>();
            var availableTakes = initialGameViewModel.NextMovePlayer.AvailablePaths;
            var playerPositions = initialGameViewModel.NextMovePlayer.PlayerPositions;
            if (availableTakes.Any())
            {
                var checkerToMove = availableTakes.First().First.Value;
                var toMove = availableTakes.First().Last.Value;
                return new KeyValuePair<CheckerElement, CheckerElement>(checkerToMove, toMove);
            }

            var playerPosition = playerPositions.FirstOrDefault(x => x.PossibleMovementElements.Any());
            CheckerElement possibleMove = null;
            if (playerPosition != null)
            {
                //                playerPosition.IsSelected = true;
                possibleMove = playerPosition.PossibleMovementElements.First();
            }
//
            return new KeyValuePair<CheckerElement, CheckerElement>(playerPosition, possibleMove);
        }

    }
}