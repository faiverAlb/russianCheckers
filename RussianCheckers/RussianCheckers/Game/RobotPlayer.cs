using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RussianCheckers.Game;

namespace RussianCheckers
{
    public class RobotPlayer: PlayerViewModel
    {
        public RobotPlayer(Side side, DataProvider dataProvider) : base(side, dataProvider, false)
        {
        }

        public KeyValuePair<CheckerElement,CheckerElement> GetOptimalMove()
        {
            //Use strategy

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