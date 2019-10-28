using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RussianCheckers.Core;

namespace RussianCheckers.Game.Tests
{
    [TestClass()]
    public class PathCalculatorTests
    {

        [TestMethod]
        public void CalculateAvailablePaths_Returns_10Paths()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerModel>()
            {
                new CheckerModel(4, 2, PieceType.Checker, Side.White),
            };
            var secondPlayerCheckers = new List<CheckerModel>()
            {
                new CheckerModel(3, 3, PieceType.Checker, Side.Black),
                new CheckerModel(5, 3, PieceType.Checker, Side.Black),
                new CheckerModel(3, 5, PieceType.Checker, Side.Black),
                new CheckerModel(5, 5, PieceType.Checker, Side.Black),
                new CheckerModel(1, 5, PieceType.Checker, Side.Black),
            };

            DataProvider dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);

            var mainPlayer = new MainPlayer(dataProvider, Side.White);
            var robotPlayer = new RobotPlayer(dataProvider, Side.Black);
            var emptyPlayer = new EmptyUserPlayer(dataProvider);

            //  Act
            var game = new Core.Game(mainPlayer, robotPlayer, emptyPlayer, dataProvider);
            game.ReCalculateWithRespectToOrder(true);
            var availablePathsForWhite = mainPlayer.CalculateAvailablePaths();

            //  Assert
            Assert.AreEqual(10, availablePathsForWhite.Count());
        }

        [TestMethod()]
        public void CalculateAvailablePaths_Returns_Paths_MaxIs_9()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerModel>()
            {
                new CheckerModel(4, 2, PieceType.Checker, Side.White),
            };
            var secondPlayerCheckers = new List<CheckerModel>()
            {
                new CheckerModel(3, 3, PieceType.Checker, Side.Black),
                new CheckerModel(5, 3, PieceType.Checker, Side.Black),
                new CheckerModel(3, 5, PieceType.Checker, Side.Black),
                new CheckerModel(5, 5, PieceType.Checker, Side.Black),
                new CheckerModel(1, 5, PieceType.Checker, Side.Black),
            };

            DataProvider dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);

            var mainPlayer = new MainPlayer(dataProvider, Side.White);
            var robotPlayer = new RobotPlayer(dataProvider, Side.Black);
            var emptyPlayer = new EmptyUserPlayer(dataProvider);

            //  Act
            var game = new Core.Game(mainPlayer, robotPlayer, emptyPlayer, dataProvider);
            game.ReCalculateWithRespectToOrder(true);
            var availablePathsForWhite = mainPlayer.CalculateAvailablePaths();

            //  Assert
            Assert.AreEqual(9, availablePathsForWhite.Max(x => x.Count));
        }



    }
}