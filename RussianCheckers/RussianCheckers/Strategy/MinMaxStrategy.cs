using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RussianCheckers.Game;

namespace RussianCheckers.Strategy
{
    public class MinMaxStrategy:RobotStrategy
    {
        public MinMaxStrategy()
        {
            
        }

        //TODO: Move to library 
        public override KeyValuePair<CheckerElement, CheckerElement> GetSuggestedMove(GameViewModel initialGameViewModel)
        {
            var resultMove =  new KeyValuePair<CheckerElement, CheckerElement>();
            
            const int minValue = int.MaxValue;
            int maxValue = int.MinValue;
            IEnumerable<KeyValuePair<CheckerElement, CheckerElement>> allAvailableMoves = initialGameViewModel.GetAllAvailableMoves().ToList();
            GameViewModel newGameModel = initialGameViewModel;
            foreach (KeyValuePair<CheckerElement, CheckerElement> availableMove in allAvailableMoves)
            {
                newGameModel = newGameModel.CreateGame();
                newGameModel.MoveChecker(availableMove.Key, availableMove.Value);
            }
            return resultMove;
        }

    }
}