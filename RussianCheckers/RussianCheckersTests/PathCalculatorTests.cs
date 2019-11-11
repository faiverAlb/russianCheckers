using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RussianCheckers.Core;

namespace RussianCheckersTests
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
            var game = new RussianCheckers.Core.Game(mainPlayer, robotPlayer, emptyPlayer, dataProvider);

            //  Act
            game.ReCalculateNeighborsAndPaths();
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
            var game = new RussianCheckers.Core.Game(mainPlayer, robotPlayer, emptyPlayer, dataProvider);
            game.ReCalculateNeighborsAndPaths();
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

        [TestMethod()]
        public void GetNextElementsInDiagonal_Returns_2()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerModel>() { new CheckerModel(4, 6, PieceType.Checker, Side.White) };
            var secondPlayerCheckers = new List<CheckerModel>(){ new CheckerModel(2, 4, PieceType.Checker, Side.Black) };
            var dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
            var playerOne = new MainPlayer(dataProvider, Side.White);
        
            //  Act
            var pathCalculator = new PathCalculator(dataProvider, playerOne.PlayerPositions, true);
            var queueOfDiagonalElements = pathCalculator.GetNextElementsInDiagonal(mainPlayCheckers.Single(), secondPlayerCheckers.Single());
            //  Assert
            Assert.AreEqual(2 , queueOfDiagonalElements.Count);
        }

        [TestMethod()]
        public void GetNextElementsInDiagonal_Returns_1()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerModel>() { new CheckerModel(4, 6, PieceType.Checker, Side.White) };
            var secondPlayerCheckers = new List<CheckerModel>()
            {
                new CheckerModel(2, 4, PieceType.Checker, Side.Black),
                new CheckerModel(0, 2, PieceType.Checker, Side.Black),
            };
            var dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
            var playerOne = new MainPlayer(dataProvider, Side.White);
        
            //  Act
            var pathCalculator = new PathCalculator(dataProvider, playerOne.PlayerPositions, true);
            var queueOfDiagonalElements = pathCalculator.GetNextElementsInDiagonal(mainPlayCheckers.First(), secondPlayerCheckers.First());
            //  Assert
            Assert.AreEqual(1 , queueOfDiagonalElements.Count);
        }


        [TestMethod()]
        public void GetNextElementsInDiagonal_Returns_0()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerModel>() { new CheckerModel(1, 7, PieceType.Checker, Side.White) };
            var secondPlayerCheckers = new List<CheckerModel>()
            {
                new CheckerModel(3, 5, PieceType.Checker, Side.Black),
                new CheckerModel(4, 4, PieceType.Checker, Side.Black),
            };
            var dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
            var playerOne = new MainPlayer(dataProvider, Side.White);
        
            //  Act
            var pathCalculator = new PathCalculator(dataProvider, playerOne.PlayerPositions, true);
            var queueOfDiagonalElements = pathCalculator.GetNextElementsInDiagonal(mainPlayCheckers.First(), secondPlayerCheckers.First());
            //  Assert
            Assert.AreEqual(0 , queueOfDiagonalElements.Count);
        }

        [TestMethod()]
        public void GetNextElementsInDiagonal_Returns_3()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerModel>() { new CheckerModel(1, 7, PieceType.Checker, Side.White) };
            var secondPlayerCheckers = new List<CheckerModel>()
            {
                new CheckerModel(3, 5, PieceType.Checker, Side.Black),
                new CheckerModel(7, 1, PieceType.Checker, Side.Black),
            };
            var dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
            var playerOne = new MainPlayer(dataProvider, Side.White);
        
            //  Act
            var pathCalculator = new PathCalculator(dataProvider, playerOne.PlayerPositions, true);
            var queueOfDiagonalElements = pathCalculator.GetNextElementsInDiagonal(mainPlayCheckers.First(), secondPlayerCheckers.First());
            //  Assert
            Assert.AreEqual(3 , queueOfDiagonalElements.Count);
        }


        [TestMethod()]
        public void CalculateAvailableForQueen_OneBlackChecker_Should_Have_2_Paths()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerModel> {new CheckerModel(4, 6, PieceType.Queen, Side.White)};
            var secondPlayerCheckers = new List<CheckerModel> {new CheckerModel(2, 4, PieceType.Checker, Side.Black)};
            DataProvider dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
            var mainPlayer = new MainPlayer(dataProvider, Side.White);
            var robotPlayer = new RobotPlayer(dataProvider, Side.Black);
            var emptyPlayer = new EmptyUserPlayer(dataProvider);
            var game = new RussianCheckers.Core.Game(mainPlayer, robotPlayer, emptyPlayer, dataProvider);

            //  Act
            game.ReCalculateNeighborsAndPaths();

            //  Assert

            var availablePaths = mainPlayer.CalculateAvailablePaths();
            Assert.AreEqual(2, availablePaths.Count());
        }

        [TestMethod()]
        public void CalculateAvailableForQueen_TwoBlackCheckers_Should_Have_3_Path()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerModel> {new CheckerModel(4, 6, PieceType.Queen, Side.White)};
            var secondPlayerCheckers = new List<CheckerModel>{
                new CheckerModel(2, 4, PieceType.Checker, Side.Black),
                new CheckerModel(1, 1, PieceType.Checker, Side.Black),
            };
            DataProvider dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
            var mainPlayer = new MainPlayer(dataProvider, Side.White);
            var robotPlayer = new RobotPlayer(dataProvider, Side.Black);
            var emptyPlayer = new EmptyUserPlayer(dataProvider);
            var game = new RussianCheckers.Core.Game(mainPlayer, robotPlayer, emptyPlayer, dataProvider);

            //  Act
            game.ReCalculateNeighborsAndPaths();

            //  Assert
            var availablePaths = mainPlayer.CalculateAvailablePaths();
            Assert.AreEqual(3, availablePaths.Count());
        }

        [TestMethod()]
        public void CalculateAvailableForQueen_ThreeBlackCheckers_Should_Have_5_Paths()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerModel> {new CheckerModel(4, 6, PieceType.Queen, Side.White)};
            var secondPlayerCheckers = new List<CheckerModel>()
            {
                new CheckerModel(2, 4, PieceType.Checker, Side.Black),
                new CheckerModel(1, 1, PieceType.Checker, Side.Black),
                new CheckerModel(6, 4, PieceType.Checker, Side.Black),
            };
            DataProvider dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
            var mainPlayer = new MainPlayer(dataProvider, Side.White);
            var robotPlayer = new RobotPlayer(dataProvider, Side.Black);
            var emptyPlayer = new EmptyUserPlayer(dataProvider);
            var game = new RussianCheckers.Core.Game(mainPlayer, robotPlayer, emptyPlayer, dataProvider);

            //  Act
            game.ReCalculateNeighborsAndPaths();

            //  Assert
            var availablePaths = mainPlayer.CalculateAvailablePaths();
            Assert.AreEqual(5, availablePaths.Count());
        }
        
        
        [TestMethod()]
        public void CalculateAvailableForQueen_ThreeBlackCheckers_Should_Have_8_Paths()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerModel>()
            {
                new CheckerModel(4, 6, PieceType.Queen, Side.White),
            };
            var secondPlayerCheckers = new List<CheckerModel>()
            {
                new CheckerModel(2, 4, PieceType.Checker, Side.Black),
                new CheckerModel(1, 1, PieceType.Checker, Side.Black),
                new CheckerModel(3, 1, PieceType.Checker, Side.Black),
            };
            DataProvider dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
            var mainPlayer = new MainPlayer(dataProvider, Side.White);
            var robotPlayer = new RobotPlayer(dataProvider, Side.Black);
            var emptyPlayer = new EmptyUserPlayer(dataProvider);
            var game = new RussianCheckers.Core.Game(mainPlayer, robotPlayer, emptyPlayer, dataProvider);

            //  Act
            game.ReCalculateNeighborsAndPaths();

            //  Assert
            var availablePaths = mainPlayer.CalculateAvailablePaths();
            Assert.AreEqual(8, availablePaths.Count());
        }
        
        [TestMethod()]
        public void CalculateAvailableForQueen_FourBlackCheckers_Should_Have_18_Paths()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerModel>()
            {
                new CheckerModel(4, 6, PieceType.Queen, Side.White),
            };
            var secondPlayerCheckers = new List<CheckerModel>()
            {
                new CheckerModel(2, 4, PieceType.Checker, Side.Black),
                new CheckerModel(2, 2, PieceType.Checker, Side.Black),
                new CheckerModel(4, 2, PieceType.Checker, Side.Black),
                new CheckerModel(5, 5, PieceType.Checker, Side.Black),
            };
            DataProvider dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
            var mainPlayer = new MainPlayer(dataProvider, Side.White);
            var robotPlayer = new RobotPlayer(dataProvider, Side.Black);
            var emptyPlayer = new EmptyUserPlayer(dataProvider);
            var game = new RussianCheckers.Core.Game(mainPlayer, robotPlayer, emptyPlayer, dataProvider);

            //  Act
            game.ReCalculateNeighborsAndPaths();

            //  Assert
            var availablePaths = mainPlayer.CalculateAvailablePaths();
            Assert.AreEqual(18, availablePaths.Count());
        }
        
        
        [TestMethod()]
        public void CalculateAvailableForQueen_SixBlackCheckers_Should_Have_35_Paths()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerModel>()
            {
                new CheckerModel(3, 3, PieceType.Queen, Side.White),
            };
            var secondPlayerCheckers = new List<CheckerModel>()
            {
                new CheckerModel(2, 4, PieceType.Checker, Side.Black),
                new CheckerModel(2, 6, PieceType.Checker, Side.Black),
                new CheckerModel(4, 4, PieceType.Checker, Side.Black),
                new CheckerModel(4, 6, PieceType.Checker, Side.Black),
        
                new CheckerModel(2, 2, PieceType.Checker, Side.Black),
                new CheckerModel(4, 2, PieceType.Checker, Side.Black),
            };
            DataProvider dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
            var mainPlayer = new MainPlayer(dataProvider, Side.White);
            var robotPlayer = new RobotPlayer(dataProvider, Side.Black);
            var emptyPlayer = new EmptyUserPlayer(dataProvider);
            var game = new RussianCheckers.Core.Game(mainPlayer, robotPlayer, emptyPlayer, dataProvider);

            //  Act
            game.ReCalculateNeighborsAndPaths();

            //  Assert
            var availablePaths = mainPlayer.CalculateAvailablePaths();
            Assert.AreEqual(35, availablePaths.Count());
        }
        
        
        [TestMethod()]
        public void CalculateAvailableForQueen_SameBehaviorAsChecker_ShouldBe_21()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerModel>()
            {
                new CheckerModel(4, 2, PieceType.Queen, Side.White),
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
            var game = new RussianCheckers.Core.Game(mainPlayer, robotPlayer, emptyPlayer, dataProvider);

            //  Act
            game.ReCalculateNeighborsAndPaths();

            //  Assert
            var availablePaths = mainPlayer.CalculateAvailablePaths();
            Assert.AreEqual(21, availablePaths.Count());
        }
        
        [TestMethod()]
        public void CalculateAvailableForQueen_SameBehaviorAsChecker_ShouldBe_23()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerModel>()
            {
                new CheckerModel(4, 2, PieceType.Queen, Side.White),
            };
            var secondPlayerCheckers = new List<CheckerModel>()
            {
                new CheckerModel(3, 3, PieceType.Checker, Side.Black),
                new CheckerModel(5, 3, PieceType.Checker, Side.Black),
                new CheckerModel(3, 5, PieceType.Checker, Side.Black),
                new CheckerModel(5, 5, PieceType.Checker, Side.Black),
                new CheckerModel(1, 5, PieceType.Checker, Side.Black),
                new CheckerModel(5, 1, PieceType.Checker, Side.Black),
            };
            DataProvider dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
            var mainPlayer = new MainPlayer(dataProvider, Side.White);
            var robotPlayer = new RobotPlayer(dataProvider, Side.Black);
            var emptyPlayer = new EmptyUserPlayer(dataProvider);
            var game = new RussianCheckers.Core.Game(mainPlayer, robotPlayer, emptyPlayer, dataProvider);

            //  Act
            game.ReCalculateNeighborsAndPaths();

            //  Assert
            var availablePaths = mainPlayer.CalculateAvailablePaths();
            Assert.AreEqual(23, availablePaths.Count());
        }

        [TestMethod()]
        public void Checker_ConvertsTakeCheckerAndActsAsQueen_ShouldTakeSecondChecker()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerModel>()
            {
                new CheckerModel(5, 5, PieceType.Checker, Side.White),
            };
            var secondPlayerCheckers = new List<CheckerModel>()
            {
                new CheckerModel(4, 6, PieceType.Checker, Side.Black),
                new CheckerModel(1, 5, PieceType.Checker, Side.Black),
            };
            DataProvider dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
            var mainPlayer = new MainPlayer(dataProvider, Side.White);
            var robotPlayer = new RobotPlayer(dataProvider, Side.Black);
            var emptyPlayer = new EmptyUserPlayer(dataProvider);
            var game = new RussianCheckers.Core.Game(mainPlayer, robotPlayer, emptyPlayer, dataProvider);

            //  Act
            game.ReCalculateNeighborsAndPaths();

            //  Assert
            var availablePaths = mainPlayer.CalculateAvailablePaths();
            Assert.IsTrue(availablePaths.Any(x => x.Last.Value.Column == 0 && x.Last.Value.Row == 4));
        }
    }
}