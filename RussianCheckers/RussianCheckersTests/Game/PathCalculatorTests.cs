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
        [TestMethod()]
        public void GetAllElementsInDiagonalFromCurrent_LeftDownInCorner_Returns_0()
        {
            //  Arrange
            var checker = new CheckerModel(0, 0, PieceType.Checker, Side.White);
            var mainPlayCheckers = new List<CheckerModel>() {checker};
            var secondPlayerCheckers = new List<CheckerModel>(){};
            var dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
            var playerOne = new MainPlayer(dataProvider, Side.White);
        
            //  Act
            var pathCalculator = new PathCalculator(dataProvider, playerOne.PlayerPositions, true);
            var queueOfDiagonalElements = pathCalculator.GetAllElementsInDiagonalFromCurrent(checker,Diagonal.LeftDown);
            //  Assert
            Assert.AreEqual(0, queueOfDiagonalElements.Count);
        }

        [TestMethod()]
        public void GetAllElementsInDiagonalFromCurrent_LeftUpInCorner_Returns_0()
        {
            //  Arrange
            var checker = new CheckerModel(0, 0, PieceType.Checker, Side.White);
            var mainPlayCheckers = new List<CheckerModel>() { checker };
            var secondPlayerCheckers = new List<CheckerModel>() { };
            var dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
            var playerOne = new MainPlayer(dataProvider, Side.White);


            //  Act
            var pathCalculator = new PathCalculator(dataProvider, playerOne.PlayerPositions, true);
            var queueOfDiagonalElements = pathCalculator.GetAllElementsInDiagonalFromCurrent(checker,Diagonal.LeftUp);
            //  Assert
            Assert.AreEqual(0, queueOfDiagonalElements.Count);
        }

        [TestMethod()]
        public void GetAllElementsInDiagonalFromCurrent_RightUpInCorner_Returns_7()
        {
            //  Arrange
            var checker = new CheckerModel(0, 0, PieceType.Checker, Side.White);
            var mainPlayCheckers = new List<CheckerModel>() { checker };
            var secondPlayerCheckers = new List<CheckerModel>() { };
            var dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
            var playerOne = new MainPlayer(dataProvider, Side.White);

            //  Act
            var pathCalculator = new PathCalculator(dataProvider, playerOne.PlayerPositions, true);
            var queueOfDiagonalElements = pathCalculator.GetAllElementsInDiagonalFromCurrent(checker,Diagonal.RightUp);
            //  Assert
            Assert.AreEqual(7, queueOfDiagonalElements.Count);
        }

        [TestMethod()]
        public void GetAllElementsInDiagonalFromCurrent_RightDownInCorner_Returns_6()
        {
            //  Arrange
            var checker = new CheckerModel(0, 6, PieceType.Checker, Side.White);
            var mainPlayCheckers = new List<CheckerModel>() { checker };
            var secondPlayerCheckers = new List<CheckerModel>() { };
            var dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
            var playerOne = new MainPlayer(dataProvider, Side.White);

            //  Act
            var pathCalculator = new PathCalculator(dataProvider, playerOne.PlayerPositions, true);
            var queueOfDiagonalElements = pathCalculator.GetAllElementsInDiagonalFromCurrent(checker,Diagonal.RightDown);
            //  Assert
            Assert.AreEqual(6 , queueOfDiagonalElements.Count);
        }


        [TestMethod()]
        public void GetAllElementsInDiagonalFromCurrent_InTheBoard_LeftUp_Returns()
        {
            //  Arrange
            var checker = new CheckerModel(3, 3, PieceType.Checker, Side.White);
            var mainPlayCheckers = new List<CheckerModel>() { checker };
            var secondPlayerCheckers = new List<CheckerModel>() { };
            var dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
            var playerOne = new MainPlayer(dataProvider, Side.White);

            //  Act
            var pathCalculator = new PathCalculator(dataProvider, playerOne.PlayerPositions, true);
            var queueOfDiagonalElements = pathCalculator.GetAllElementsInDiagonalFromCurrent(checker,Diagonal.LeftUp);
            //  Assert
            Assert.AreEqual(3 , queueOfDiagonalElements.Count);
        }

        [TestMethod()]
        public void GetAllElementsInDiagonalFromCurrent_InTheBoard_LeftDown_Returns_2()
        {
            //  Arrange
            var checker = new CheckerModel(2, 4, PieceType.Checker, Side.White);
            var mainPlayCheckers = new List<CheckerModel>() { checker };
            var secondPlayerCheckers = new List<CheckerModel>() { };
            var dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
            var playerOne = new MainPlayer(dataProvider, Side.White);

            //  Act
            var pathCalculator = new PathCalculator(dataProvider, playerOne.PlayerPositions, true);
            var queueOfDiagonalElements = pathCalculator.GetAllElementsInDiagonalFromCurrent(checker,Diagonal.LeftDown);
            //  Assert
            Assert.AreEqual(2 , queueOfDiagonalElements.Count);
        }

        [TestMethod()]
        public void GetAllElementsInDiagonalFromCurrent_InTheBoard_RightUp_Returns_3()
        {
            //  Arrange
            var checker = new CheckerModel(2, 4, PieceType.Checker, Side.White);
            var mainPlayCheckers = new List<CheckerModel>() { checker };
            var secondPlayerCheckers = new List<CheckerModel>() { };
            var dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
            var playerOne = new MainPlayer(dataProvider, Side.White);

            //  Act
            var pathCalculator = new PathCalculator(dataProvider, playerOne.PlayerPositions, true);
            var queueOfDiagonalElements = pathCalculator.GetAllElementsInDiagonalFromCurrent(checker,Diagonal.RightUp);
            //  Assert
            Assert.AreEqual(3 , queueOfDiagonalElements.Count);
        }

        [TestMethod()]
        public void GetAllElementsInDiagonalFromCurrent_InTheBoard_RightDown_Returns_4()
        {
            //  Arrange
            var checker = new CheckerModel(2, 4, PieceType.Checker, Side.White);
            var mainPlayCheckers = new List<CheckerModel>() { checker };
            var secondPlayerCheckers = new List<CheckerModel>() { };
            var dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
            var playerOne = new MainPlayer(dataProvider, Side.White);

            //  Act
            var pathCalculator = new PathCalculator(dataProvider, playerOne.PlayerPositions, true);
            var queueOfDiagonalElements = pathCalculator.GetAllElementsInDiagonalFromCurrent(checker,Diagonal.RightDown);
            //  Assert
            Assert.AreEqual(4 , queueOfDiagonalElements.Count);
        }
    }
}