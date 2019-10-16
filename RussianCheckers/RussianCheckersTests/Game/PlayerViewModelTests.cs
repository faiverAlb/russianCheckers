using Microsoft.VisualStudio.TestTools.UnitTesting;
using RussianCheckers.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RussianCheckers.Game.Tests
{
    [TestClass()]
    public class PlayerViewModelTests
    {
        [TestMethod()]
        public void GetPossiblePathsTest()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerElement>()
            {
                new CheckerElement(4, 2, PieceType.Checker, Side.White),
            };
            var secondPlayerCheckers = new List<CheckerElement>()
            {
                new CheckerElement(3, 3, PieceType.Checker, Side.Black),
                new CheckerElement(5, 3, PieceType.Checker, Side.Black),
                new CheckerElement(3, 5, PieceType.Checker, Side.Black),
                new CheckerElement(5, 5, PieceType.Checker, Side.Black),
                new CheckerElement(1, 5, PieceType.Checker, Side.Black),
            };
            DataProvider dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
            MainHumanPlayer playerOne = new MainHumanPlayer(Side.White, dataProvider);
            var playerTwo = new RobotPlayer(Side.Black, dataProvider);
            var emptyCellsPlayer = new EmptyCellsPlayer(Side.Empty, dataProvider);

            //  Act
            emptyCellsPlayer.CalculateNeighbors();
            playerOne.CalculateNeighbors();
            playerTwo.CalculateNeighbors();

            //  Assert
            var availablePathsForWhite = playerOne.GetPossiblePaths(mainPlayCheckers.Single());
            Assert.AreEqual(9, availablePathsForWhite.Max(x => x.Count));
        }


        [TestMethod()]
        public void CalculateAvailablePathsTest()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerElement>()
            {
                new CheckerElement(4, 2, PieceType.Checker, Side.White),
            };
            var secondPlayerCheckers = new List<CheckerElement>()
            {
                new CheckerElement(3, 3, PieceType.Checker, Side.Black),
                new CheckerElement(5, 3, PieceType.Checker, Side.Black),
                new CheckerElement(3, 5, PieceType.Checker, Side.Black),
                new CheckerElement(5, 5, PieceType.Checker, Side.Black),
                new CheckerElement(1, 5, PieceType.Checker, Side.Black),
            };
            DataProvider dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
            MainHumanPlayer playerOne = new MainHumanPlayer(Side.White, dataProvider);
            var playerTwo = new RobotPlayer(Side.Black, dataProvider);
            var emptyCellsPlayer = new EmptyCellsPlayer(Side.Empty, dataProvider);

            //  Act
            emptyCellsPlayer.CalculateNeighbors();
            playerOne.CalculateNeighbors();
            playerTwo.CalculateNeighbors();

            //  Assert
            playerOne.CalculateAvailablePaths();
            Assert.AreEqual(3, playerOne.AvailablePaths.Count);
        }

    }
}