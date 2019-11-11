using System.Collections.Generic;
using System.Linq;

namespace RussianCheckers.Core
{
    public class GameStatusChecker
    {
        private readonly Player _playerOne;
        private readonly Player _playerTwo;
        private readonly Stack<HistoryMove> _actionsHistory;

        public GameStatusChecker(Player playerOne, Player playerTwo, Stack<HistoryMove> actionsHistory)
        {
            _playerOne = playerOne;
            _playerTwo = playerTwo;
            _actionsHistory = actionsHistory;
        }

        public Side GetWinnerSide()
        {
            if (_playerOne.PlayerPositions.Count == 0)
            {
                return _playerOne.Side == Side.White ? Side.Black : Side.White;
            }

            if (_playerTwo.PlayerPositions.Count == 0)
            {
                return _playerTwo.Side == Side.White ? Side.Black : Side.White;
            }

            int playerOnePossibleMoves = _playerOne.GetPossibleMovementsCount();
            int playerTwoPossibleMoves = _playerTwo.GetPossibleMovementsCount();
            if (playerOnePossibleMoves == 0 && playerTwoPossibleMoves > 0)
            {
                return _playerOne.Side == Side.White ? Side.Black : Side.White;
            }
            if (playerTwoPossibleMoves == 0 && playerOnePossibleMoves > 0)
            {
                return _playerTwo.Side == Side.White ? Side.Black : Side.White;
            }
            if (playerTwoPossibleMoves == 0 && playerOnePossibleMoves == 0)
            {
                return Side.Draw;
            }

            bool isDrawCondition = IsMovedOnlyQueensFor15Steps();
            return isDrawCondition ? Side.Draw : Side.None;
        }

        private bool IsMovedOnlyQueensFor15Steps()
        {
            if (_actionsHistory.Count >= 15)
            {
                bool isOnlyQueensMoved = _actionsHistory.All(x => x.MovedFromTo.Key.Type == PieceType.Queen);
                bool isNothingWasTook =  _actionsHistory.All(x => x.DeletedList.Count == 0);
                return isOnlyQueensMoved && isNothingWasTook;
            }

            return false;   

        }
    }
}