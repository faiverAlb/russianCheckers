using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RussianCheckers.Strategy
{
//    public class DummyStrategy:RobotStrategy
//    {
//        public DummyStrategy()
//        {
//
//        }
//
//        //TODO: Move to library 
//        public override KeyValuePair<CheckerElementViewModel, CheckerElementViewModel> GetSuggestedMove(GameViewModel initialGameViewModel)
//        {
//            //            return new KeyValuePair<CheckerElement, CheckerElement>();
//            var availableTakes = initialGameViewModel.NextMovePlayer.AvailablePaths;
//            var playerPositions = initialGameViewModel.NextMovePlayer.PlayerPositions;
//            if (availableTakes.Any())
//            {
//                var checkerToMove = availableTakes.First().First.Value;
//                var toMove = availableTakes.First().Last.Value;
//                return new KeyValuePair<CheckerElementViewModel, CheckerElementViewModel>(checkerToMove, toMove);
//            }
//
//            var playerPosition = playerPositions.FirstOrDefault(x => x.PossibleMovementElements.Any());
//            CheckerElementViewModel possibleMove = null;
//            if (playerPosition != null)
//            {
//                //                playerPosition.IsSelected = true;
//                possibleMove = playerPosition.PossibleMovementElements.First();
//            }
////
//            return new KeyValuePair<CheckerElementViewModel, CheckerElementViewModel>(playerPosition, possibleMove);
//        }
//
//    }
}