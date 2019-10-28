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


//
//
//        [TestMethod()]
//        public void GetNextElementsInDiagonal_Returns_2()
//        {
//            //  Arrange
//            var mainPlayCheckers = new List<CheckerElementViewModel>() { new CheckerElementViewModel(4, 6, PieceType.Checker, Side.White) };
//            var secondPlayerCheckers = new List<CheckerElementViewModel>(){ new CheckerElementViewModel(2, 4, PieceType.Checker, Side.Black) };
//            var playerOne = new MainHumanPlayer(Side.White, new DataProvider(mainPlayCheckers, secondPlayerCheckers));
//
//            //  Act
//            var queueOfDiagonalElements = playerOne.GetNextElementsInDiagonal(mainPlayCheckers.Single(), secondPlayerCheckers.Single());
//            //  Assert
//            Assert.AreEqual(2 , queueOfDiagonalElements.Count);
//        }
//
//
//        [TestMethod()]
//        public void GetNextElementsInDiagonal_Returns_1()
//        {
//            //  Arrange
//            var mainPlayCheckers = new List<CheckerElementViewModel>() { new CheckerElementViewModel(4, 6, PieceType.Checker, Side.White) };
//            var secondPlayerCheckers = new List<CheckerElementViewModel>()
//            {
//                new CheckerElementViewModel(2, 4, PieceType.Checker, Side.Black),
//                new CheckerElementViewModel(0, 2, PieceType.Checker, Side.Black),
//            };
//            var playerOne = new MainHumanPlayer(Side.White, new DataProvider(mainPlayCheckers, secondPlayerCheckers));
//
//            //  Act
//            var queueOfDiagonalElements = playerOne.GetNextElementsInDiagonal(mainPlayCheckers.First(), secondPlayerCheckers.First());
//            //  Assert
//            Assert.AreEqual(1 , queueOfDiagonalElements.Count);
//        }
//        [TestMethod()]
//        public void GetNextElementsInDiagonal_Returns_0()
//        {
//            //  Arrange
//            var mainPlayCheckers = new List<CheckerElementViewModel>() { new CheckerElementViewModel(1, 7, PieceType.Checker, Side.White) };
//            var secondPlayerCheckers = new List<CheckerElementViewModel>()
//            {
//                new CheckerElementViewModel(3, 5, PieceType.Checker, Side.Black),
//                new CheckerElementViewModel(4, 4, PieceType.Checker, Side.Black),
//            };
//            var playerOne = new MainHumanPlayer(Side.White, new DataProvider(mainPlayCheckers, secondPlayerCheckers));
//
//            //  Act
//            var queueOfDiagonalElements = playerOne.GetNextElementsInDiagonal(mainPlayCheckers.First(), secondPlayerCheckers.First());
//            //  Assert
//            Assert.AreEqual(0 , queueOfDiagonalElements.Count);
//        }
//        [TestMethod()]
//        public void GetNextElementsInDiagonal_Returns_3()
//        {
//            //  Arrange
//            var mainPlayCheckers = new List<CheckerElementViewModel>() { new CheckerElementViewModel(1, 7, PieceType.Checker, Side.White) };
//            var secondPlayerCheckers = new List<CheckerElementViewModel>()
//            {
//                new CheckerElementViewModel(3, 5, PieceType.Checker, Side.Black),
//                new CheckerElementViewModel(7, 1, PieceType.Checker, Side.Black),
//            };
//            var playerOne = new MainHumanPlayer(Side.White, new DataProvider(mainPlayCheckers, secondPlayerCheckers));
//
//            //  Act
//            var queueOfDiagonalElements = playerOne.GetNextElementsInDiagonal(mainPlayCheckers.First(), secondPlayerCheckers.First());
//            //  Assert
//            Assert.AreEqual(3 , queueOfDiagonalElements.Count);
//        }
//
//        [TestMethod()]
//        public void CalculateAvailableForQueen_OneBlackChecker_Should_Have_2_Paths()
//        {
//            //  Arrange
//            var mainPlayCheckers = new List<CheckerElementViewModel>()
//            {
//                new CheckerElementViewModel(4, 6, PieceType.Queen, Side.White),
//            };
//            var secondPlayerCheckers = new List<CheckerElementViewModel>()
//            {
//                new CheckerElementViewModel(2, 4, PieceType.Checker, Side.Black),
//            };
//            DataProvider dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
//            MainHumanPlayer playerOne = new MainHumanPlayer(Side.White, dataProvider);
//            var playerTwo = new RobotViewPlayer(Side.Black, dataProvider);
//            var emptyCellsPlayer = new EmptyCellsPlayer(Side.Empty, dataProvider);
//
//            //  Act
//            emptyCellsPlayer.CalculateNeighbors();
//            playerOne.CalculateNeighbors();
//            playerTwo.CalculateNeighbors();
//
//            //  Assert
//            playerOne.CalculateAvailablePaths();
//            Assert.AreEqual(2, playerOne.AvailablePaths.Count);
//        }
//
//        [TestMethod()]
//        public void CalculateAvailableForQueen_TwoBlackCheckers_Should_Have_3_Path()
//        {
//            //  Arrange
//            var mainPlayCheckers = new List<CheckerElementViewModel>()
//            {
//                new CheckerElementViewModel(4, 6, PieceType.Queen, Side.White),
//            };
//            var secondPlayerCheckers = new List<CheckerElementViewModel>()
//            {
//                new CheckerElementViewModel(2, 4, PieceType.Checker, Side.Black),
//                new CheckerElementViewModel(1, 1, PieceType.Checker, Side.Black),
//            };
//            DataProvider dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
//            MainHumanPlayer playerOne = new MainHumanPlayer(Side.White, dataProvider);
//            var playerTwo = new RobotViewPlayer(Side.Black, dataProvider);
//            var emptyCellsPlayer = new EmptyCellsPlayer(Side.Empty, dataProvider);
//
//            //  Act
//            emptyCellsPlayer.CalculateNeighbors();
//            playerOne.CalculateNeighbors();
//            playerTwo.CalculateNeighbors();
//
//            //  Assert
//            playerOne.CalculateAvailablePaths();
//            Assert.AreEqual(3, playerOne.AvailablePaths.Count);
//        }
//
//        [TestMethod()]
//        public void CalculateAvailableForQueen_ThreeBlackCheckers_Should_Have_5_Paths()
//        {
//            //  Arrange
//            var mainPlayCheckers = new List<CheckerElementViewModel>()
//            {
//                new CheckerElementViewModel(4, 6, PieceType.Queen, Side.White),
//            };
//            var secondPlayerCheckers = new List<CheckerElementViewModel>()
//            {
//                new CheckerElementViewModel(2, 4, PieceType.Checker, Side.Black),
//                new CheckerElementViewModel(1, 1, PieceType.Checker, Side.Black),
//                new CheckerElementViewModel(6, 4, PieceType.Checker, Side.Black),
//            };
//            DataProvider dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
//            MainHumanPlayer playerOne = new MainHumanPlayer(Side.White, dataProvider);
//            var playerTwo = new RobotViewPlayer(Side.Black, dataProvider);
//            var emptyCellsPlayer = new EmptyCellsPlayer(Side.Empty, dataProvider);
//
//            //  Act
//            emptyCellsPlayer.CalculateNeighbors();
//            playerOne.CalculateNeighbors();
//            playerTwo.CalculateNeighbors();
//
//            //  Assert
//            playerOne.CalculateAvailablePaths();
//            Assert.AreEqual(5, playerOne.AvailablePaths.Count);
//        }
//
//
//        [TestMethod()]
//        public void CalculateAvailableForQueen_ThreeBlackCheckers_Should_Have_8_Paths()
//        {
//            //  Arrange
//            var mainPlayCheckers = new List<CheckerElementViewModel>()
//            {
//                new CheckerElementViewModel(4, 6, PieceType.Queen, Side.White),
//            };
//            var secondPlayerCheckers = new List<CheckerElementViewModel>()
//            {
//                new CheckerElementViewModel(2, 4, PieceType.Checker, Side.Black),
//                new CheckerElementViewModel(1, 1, PieceType.Checker, Side.Black),
//                new CheckerElementViewModel(3, 1, PieceType.Checker, Side.Black),
//            };
//            DataProvider dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
//            MainHumanPlayer playerOne = new MainHumanPlayer(Side.White, dataProvider);
//            var playerTwo = new RobotViewPlayer(Side.Black, dataProvider);
//            var emptyCellsPlayer = new EmptyCellsPlayer(Side.Empty, dataProvider);
//
//            //  Act
//            emptyCellsPlayer.CalculateNeighbors();
//            playerOne.CalculateNeighbors();
//            playerTwo.CalculateNeighbors();
//
//            //  Assert
//            playerOne.CalculateAvailablePaths();
//            Assert.AreEqual(8, playerOne.AvailablePaths.Count);
//        }
//
//        [TestMethod()]
//        public void CalculateAvailableForQueen_FourBlackCheckers_Should_Have_18_Paths()
//        {
//            //  Arrange
//            var mainPlayCheckers = new List<CheckerElementViewModel>()
//            {
//                new CheckerElementViewModel(4, 6, PieceType.Queen, Side.White),
//            };
//            var secondPlayerCheckers = new List<CheckerElementViewModel>()
//            {
//                new CheckerElementViewModel(2, 4, PieceType.Checker, Side.Black),
//                new CheckerElementViewModel(2, 2, PieceType.Checker, Side.Black),
//                new CheckerElementViewModel(4, 2, PieceType.Checker, Side.Black),
//                new CheckerElementViewModel(5, 5, PieceType.Checker, Side.Black),
//            };
//            DataProvider dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
//            MainHumanPlayer playerOne = new MainHumanPlayer(Side.White, dataProvider);
//            var playerTwo = new RobotViewPlayer(Side.Black, dataProvider);
//            var emptyCellsPlayer = new EmptyCellsPlayer(Side.Empty, dataProvider);
//
//            //  Act
//            emptyCellsPlayer.CalculateNeighbors();
//            playerOne.CalculateNeighbors();
//            playerTwo.CalculateNeighbors();
//
//            //  Assert
//            playerOne.CalculateAvailablePaths();
//            Assert.AreEqual(18, playerOne.AvailablePaths.Count);
//        }
//
//
//        [TestMethod()]
//        public void CalculateAvailableForQueen_SixBlackCheckers_Should_Have_35_Paths()
//        {
//            //  Arrange
//            var mainPlayCheckers = new List<CheckerElementViewModel>()
//            {
//                new CheckerElementViewModel(3, 3, PieceType.Queen, Side.White),
//            };
//            var secondPlayerCheckers = new List<CheckerElementViewModel>()
//            {
//                new CheckerElementViewModel(2, 4, PieceType.Checker, Side.Black),
//                new CheckerElementViewModel(2, 6, PieceType.Checker, Side.Black),
//                new CheckerElementViewModel(4, 4, PieceType.Checker, Side.Black),
//                new CheckerElementViewModel(4, 6, PieceType.Checker, Side.Black),
//
//                new CheckerElementViewModel(2, 2, PieceType.Checker, Side.Black),
//                new CheckerElementViewModel(4, 2, PieceType.Checker, Side.Black),
//            };
//            DataProvider dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
//            MainHumanPlayer playerOne = new MainHumanPlayer(Side.White, dataProvider);
//            var playerTwo = new RobotViewPlayer(Side.Black, dataProvider);
//            var emptyCellsPlayer = new EmptyCellsPlayer(Side.Empty, dataProvider);
//
//            //  Act
//            emptyCellsPlayer.CalculateNeighbors();
//            playerOne.CalculateNeighbors();
//            playerTwo.CalculateNeighbors();
//
//            //  Assert
//            playerOne.CalculateAvailablePaths();
//            Assert.AreEqual(35, playerOne.AvailablePaths.Count);
//        }
//
//
//        [TestMethod()]
//        public void CalculateAvailableForQueen_SameBehaviorAsChecker_ShouldBe_21()
//        {
//            //  Arrange
//            var mainPlayCheckers = new List<CheckerElementViewModel>()
//            {
//                new CheckerElementViewModel(4, 2, PieceType.Queen, Side.White),
//            };
//            var secondPlayerCheckers = new List<CheckerElementViewModel>()
//            {
//                new CheckerElementViewModel(3, 3, PieceType.Checker, Side.Black),
//                new CheckerElementViewModel(5, 3, PieceType.Checker, Side.Black),
//                new CheckerElementViewModel(3, 5, PieceType.Checker, Side.Black),
//                new CheckerElementViewModel(5, 5, PieceType.Checker, Side.Black),
//                new CheckerElementViewModel(1, 5, PieceType.Checker, Side.Black),
//            };
//            DataProvider dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
//            MainHumanPlayer playerOne = new MainHumanPlayer(Side.White, dataProvider);
//            var playerTwo = new RobotViewPlayer(Side.Black, dataProvider);
//            var emptyCellsPlayer = new EmptyCellsPlayer(Side.Empty, dataProvider);
//
//            //  Act
//            emptyCellsPlayer.CalculateNeighbors();
//            playerOne.CalculateNeighbors();
//            playerTwo.CalculateNeighbors();
//
//            //  Assert
//            playerOne.CalculateAvailablePaths();
//            Assert.AreEqual(21, playerOne.AvailablePaths.Count);
//        }
//
//        [TestMethod()]
//        public void CalculateAvailableForQueen_SameBehaviorAsChecker_ShouldBe_23()
//        {
//            //  Arrange
//            var mainPlayCheckers = new List<CheckerElementViewModel>()
//            {
//                new CheckerElementViewModel(4, 2, PieceType.Queen, Side.White),
//            };
//            var secondPlayerCheckers = new List<CheckerElementViewModel>()
//            {
//                new CheckerElementViewModel(3, 3, PieceType.Checker, Side.Black),
//                new CheckerElementViewModel(5, 3, PieceType.Checker, Side.Black),
//                new CheckerElementViewModel(3, 5, PieceType.Checker, Side.Black),
//                new CheckerElementViewModel(5, 5, PieceType.Checker, Side.Black),
//                new CheckerElementViewModel(1, 5, PieceType.Checker, Side.Black),
//                new CheckerElementViewModel(5, 1, PieceType.Checker, Side.Black),
//            };
//            DataProvider dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
//            MainHumanPlayer playerOne = new MainHumanPlayer(Side.White, dataProvider);
//            var playerTwo = new RobotViewPlayer(Side.Black, dataProvider);
//            var emptyCellsPlayer = new EmptyCellsPlayer(Side.Empty, dataProvider);
//
//            //  Act
//            emptyCellsPlayer.CalculateNeighbors();
//            playerOne.CalculateNeighbors();
//            playerTwo.CalculateNeighbors();
//
//            //  Assert
//            playerOne.CalculateAvailablePaths();
//            Assert.AreEqual(23, playerOne.AvailablePaths.Count);
//        }
//
//        [TestMethod()]
//        public void Checker_ConvertsTakeCheckerAndActsAsQueen_ShouldTakeSecondChecker()
//        {
//            //  Arrange
//            var mainPlayCheckers = new List<CheckerElementViewModel>()
//            {
//                new CheckerElementViewModel(5, 5, PieceType.Checker, Side.White),
//            };
//            var secondPlayerCheckers = new List<CheckerElementViewModel>()
//            {
//                new CheckerElementViewModel(4, 6, PieceType.Checker, Side.Black),
//                new CheckerElementViewModel(1, 5, PieceType.Checker, Side.Black),
//            };
//            DataProvider dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
//            MainHumanPlayer playerOne = new MainHumanPlayer(Side.White, dataProvider);
//            var playerTwo = new RobotViewPlayer(Side.Black, dataProvider);
//            var emptyCellsPlayer = new EmptyCellsPlayer(Side.Empty, dataProvider);
//
//            //  Act
//            emptyCellsPlayer.CalculateNeighbors();
//            playerOne.CalculateNeighbors();
//            playerTwo.CalculateNeighbors();
//            playerOne.CalculateAvailablePaths();
//
//            //  Assert
//            Assert.IsTrue(playerOne.AvailablePaths.Any(x => x.Last.Value.Column == 0 && x.Last.Value.Row == 4));
//        }


    }
}