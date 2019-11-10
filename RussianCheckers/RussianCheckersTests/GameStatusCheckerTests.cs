using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RussianCheckers.Core;

namespace RussianCheckersTests
{
    [TestClass()]
    public class GameStatusCheckerTests
    {
        [TestMethod()]
        public void GetWinnerSide_NoWhiteCheckers_BlackVictory()
        {
            //  Arrange
            var secondPlayerCheckers = new List<CheckerModel>{new CheckerModel(3, 3, PieceType.Checker, Side.Black)};
            DataProvider dataProvider = new DataProvider(new List<CheckerModel>(), secondPlayerCheckers);
            var gameStatusChecker = new GameStatusChecker(new MainPlayer(dataProvider, Side.White),
                new RobotPlayer(dataProvider, Side.Black), new Stack<HistoryMove>());
            //  Act
            Side winnerSide = gameStatusChecker.GetWinnerSide();
            
            //  Assert
            Assert.AreEqual(Side.Black, winnerSide);
        }

        [TestMethod()]
        public void GetWinnerSide_NoBlackCheckers_WhiteVictory()
        {
            //  Arrange
            var playerCheckers = new List<CheckerModel>{new CheckerModel(3, 3, PieceType.Checker, Side.White)};
            DataProvider dataProvider = new DataProvider( playerCheckers, new List<CheckerModel>());
            var gameStatusChecker = new GameStatusChecker(new MainPlayer(dataProvider, Side.White),
                new RobotPlayer(dataProvider, Side.Black), new Stack<HistoryMove>());
            //  Act
            Side winnerSide = gameStatusChecker.GetWinnerSide();
            
            //  Assert
            Assert.AreEqual(Side.White, winnerSide);
        }

    }
}
