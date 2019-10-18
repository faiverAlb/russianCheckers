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


        [TestMethod()]
        public void CalculateNeighbors_ForQueen_NoOtherCheckers_ShouldCountAllEmpty()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerElement>()
            {
                new CheckerElement(4, 6, PieceType.Queen, Side.White),
            };
            var secondPlayerCheckers = new List<CheckerElement>()
            {
                
            };
            DataProvider dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);

            MainHumanPlayer playerOne = new MainHumanPlayer(Side.White, dataProvider);

            //  Act
            playerOne.CalculateNeighbors();

            //  Assert
            var neighbors = playerOne.PlayerPositions.Single().Neighbors;
            Assert.AreEqual(9,neighbors.Count );
        }

        [TestMethod()]
        public void CalculateNeighbors_ForQueen_HaveSameOnPath_ShouldCountAllEmpty()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerElement>()
            {
                new CheckerElement(4, 6, PieceType.Queen, Side.White),
            };
            var secondPlayerCheckers = new List<CheckerElement>()
            {
                new CheckerElement(1, 3, PieceType.Checker, Side.Black),
            };
            MainHumanPlayer playerOne = new MainHumanPlayer(Side.White, new DataProvider(mainPlayCheckers, secondPlayerCheckers));

            //  Act
            playerOne.CalculateNeighbors();

            //  Assert
            var neighbors = playerOne.PlayerPositions.Single().Neighbors;
            Assert.AreEqual(8,neighbors.Count );
        }

        [TestMethod()]
        public void CalculateNeighbors_ForQueen_ShouldHaveBlackNeighbor()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerElement>()
            {
                new CheckerElement(4, 6, PieceType.Queen, Side.White),
            };
            var secondPlayerCheckers = new List<CheckerElement>()
            {
                new CheckerElement(1, 3, PieceType.Checker, Side.Black),
            };
            var playerOne = new MainHumanPlayer(Side.White, new DataProvider(mainPlayCheckers, secondPlayerCheckers));

            //  Act
            playerOne.CalculateNeighbors();

            //  Assert
            List<CheckerElement> whiteCheckerNeighbors = playerOne.PlayerPositions.Single().Neighbors;
            CheckerElement blackChecker = secondPlayerCheckers.Single();
            Assert.IsTrue(whiteCheckerNeighbors.Contains(blackChecker) );
        }

        [TestMethod()]
        public void CalculateNeighbors_ForQueen_ShouldNotHaveEmptyNeighborAfterBlack()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerElement>()
            {
                new CheckerElement(4, 6, PieceType.Queen, Side.White),
            };
            var secondPlayerCheckers = new List<CheckerElement>()
            {
                new CheckerElement(1, 3, PieceType.Checker, Side.Black),
            };
            var playerOne = new MainHumanPlayer(Side.White, new DataProvider(mainPlayCheckers, secondPlayerCheckers));

            //  Act
            playerOne.CalculateNeighbors();

            //  Assert
            List<CheckerElement> whiteCheckerNeighbors = playerOne.PlayerPositions.Single().Neighbors;
            Assert.IsFalse(whiteCheckerNeighbors.Any(x => x.Side == Side.Empty && x.Column == 0 && x.Row == 2) );
        }

        [TestMethod()]
        public void CalculateNeighborsForQueen_HaveTwoBlackNearestNeighbors()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerElement>()
            {
                new CheckerElement(4, 6, PieceType.Queen, Side.White),
            };
            var secondPlayerCheckers = new List<CheckerElement>()
            {
                new CheckerElement(1, 3, PieceType.Checker, Side.Black),
                new CheckerElement(5, 5, PieceType.Checker, Side.Black),
            };

            MainHumanPlayer playerOne = new MainHumanPlayer(Side.White, new DataProvider(mainPlayCheckers, secondPlayerCheckers));

            //  Act
            playerOne.CalculateNeighbors();

            //  Assert
            List<CheckerElement> actualNeighbors = playerOne.PlayerPositions.Single().Neighbors;
            Assert.AreEqual(2, actualNeighbors.Count(x => x.Side == Side.Black));
        }


        [TestMethod()]
        public void CalculateNeighborsForQueen_ShouldHave_6_Neighbors()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerElement>()
            {
                new CheckerElement(4, 6, PieceType.Queen, Side.White),
            };
            var secondPlayerCheckers = new List<CheckerElement>()
            {
                new CheckerElement(1, 3, PieceType.Checker, Side.Black),
                new CheckerElement(5, 5, PieceType.Checker, Side.Black),
            };
            var playerOne = new MainHumanPlayer(Side.White, new DataProvider(mainPlayCheckers, secondPlayerCheckers));

            //  Act
            playerOne.CalculateNeighbors();

            //  Assert
            List<CheckerElement> actualNeighbors = playerOne.PlayerPositions.Single().Neighbors;
            Assert.AreEqual(6, actualNeighbors.Count);
        }


        [TestMethod()]
        public void CalculateNeighborsForQueen_ShouldHave_4_Neighbors()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerElement>()
            {
                new CheckerElement(4, 6, PieceType.Queen, Side.White),
            };
            var secondPlayerCheckers = new List<CheckerElement>()
            {
                new CheckerElement(3, 5, PieceType.Checker, Side.Black),
                new CheckerElement(5, 5, PieceType.Checker, Side.Black),
            };            
            var playerOne = new MainHumanPlayer(Side.White, new DataProvider(mainPlayCheckers, secondPlayerCheckers));

            //  Act
            playerOne.CalculateNeighbors();

            //  Assert
            List<CheckerElement> actualNeighbors = playerOne.PlayerPositions.Single().Neighbors;
            Assert.AreEqual(4, actualNeighbors.Count);
        }


        [TestMethod()]
        public void CalculateNeighborsForQueen_QueenInCorner_ShouldHave_1_Neighbor()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerElement>()
            {
                new CheckerElement(0, 0, PieceType.Queen, Side.White),
            };
            var secondPlayerCheckers = new List<CheckerElement>()
            {
                new CheckerElement(1, 1, PieceType.Checker, Side.Black),
            };

            var playerOne = new MainHumanPlayer(Side.White, new DataProvider(mainPlayCheckers, secondPlayerCheckers));

            //  Act
            playerOne.CalculateNeighbors();

            //  Assert
            List<CheckerElement> actualNeighbors = playerOne.PlayerPositions.Single().Neighbors;
            Assert.AreEqual(1, actualNeighbors.Count);
        }

        [TestMethod()]
        public void CalculateNeighborsForQueen_QueenInCorner_ShouldHave_7_Neighbor()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerElement>()
            {
                new CheckerElement(0, 0, PieceType.Queen, Side.White),
            };

            MainHumanPlayer playerOne = new MainHumanPlayer(Side.White, new DataProvider(mainPlayCheckers, new List<CheckerElement>()));

            //  Act
            playerOne.CalculateNeighbors();

            //  Assert
            List<CheckerElement> actualNeighbors = playerOne.PlayerPositions.Single().Neighbors;
            Assert.AreEqual(7, actualNeighbors.Count);
        }


        [TestMethod()]
        public void GetAllElementsInDiagonalFromCurrent_LeftDownInCorner_Returns_0()
        {
            //  Arrange
            var checker = new CheckerElement(0, 0, PieceType.Checker, Side.White);
            var mainPlayCheckers = new List<CheckerElement>() {checker};
            var secondPlayerCheckers = new List<CheckerElement>(){};
            var playerOne = new MainHumanPlayer(Side.White, new DataProvider(mainPlayCheckers, secondPlayerCheckers));

            //  Act
            var queueOfDiagonalElements = playerOne.GetAllElementsInDiagonalFromCurrent(checker,Diagonal.LeftDown);
            //  Assert
            Assert.AreEqual(0, queueOfDiagonalElements.Count);
        }
        [TestMethod()]
        public void GetAllElementsInDiagonalFromCurrent_LeftUpInCorner_Returns_0()
        {
            //  Arrange
            var checker = new CheckerElement(0, 0, PieceType.Checker, Side.White);
            var mainPlayCheckers = new List<CheckerElement>() {checker};
            var secondPlayerCheckers = new List<CheckerElement>(){};
            var playerOne = new MainHumanPlayer(Side.White, new DataProvider(mainPlayCheckers, secondPlayerCheckers));

            //  Act
            var queueOfDiagonalElements = playerOne.GetAllElementsInDiagonalFromCurrent(checker,Diagonal.LeftUp);
            //  Assert
            Assert.AreEqual(0, queueOfDiagonalElements.Count);
        }


        [TestMethod()]
        public void GetAllElementsInDiagonalFromCurrent_RightUpInCorner_Returns_7()
        {
            //  Arrange
            var checker = new CheckerElement(0, 0, PieceType.Checker, Side.White);
            var mainPlayCheckers = new List<CheckerElement>() {checker};
            var secondPlayerCheckers = new List<CheckerElement>(){};
            var playerOne = new MainHumanPlayer(Side.White, new DataProvider(mainPlayCheckers, secondPlayerCheckers));

            //  Act
            var queueOfDiagonalElements = playerOne.GetAllElementsInDiagonalFromCurrent(checker,Diagonal.RightUp);
            //  Assert
            Assert.AreEqual(7, queueOfDiagonalElements.Count);
        }


        [TestMethod()]
        public void GetAllElementsInDiagonalFromCurrent_RightDownInCorner_Returns_6()
        {
            //  Arrange
            var checker = new CheckerElement(0, 6, PieceType.Checker, Side.White);
            var mainPlayCheckers = new List<CheckerElement>() {checker};
            var secondPlayerCheckers = new List<CheckerElement>(){};
            var playerOne = new MainHumanPlayer(Side.White, new DataProvider(mainPlayCheckers, secondPlayerCheckers));

            //  Act
            var queueOfDiagonalElements = playerOne.GetAllElementsInDiagonalFromCurrent(checker,Diagonal.RightDown);
            //  Assert
            Assert.AreEqual(6 , queueOfDiagonalElements.Count);
        }


        [TestMethod()]
        public void GetAllElementsInDiagonalFromCurrent_InTheBoard_LeftUp_Returns()
        {
            //  Arrange
            var checker = new CheckerElement(3, 3, PieceType.Checker, Side.White);
            var mainPlayCheckers = new List<CheckerElement>() {checker};
            var secondPlayerCheckers = new List<CheckerElement>(){};
            var playerOne = new MainHumanPlayer(Side.White, new DataProvider(mainPlayCheckers, secondPlayerCheckers));

            //  Act
            var queueOfDiagonalElements = playerOne.GetAllElementsInDiagonalFromCurrent(checker,Diagonal.LeftUp);
            //  Assert
            Assert.AreEqual(3 , queueOfDiagonalElements.Count);
        }


        [TestMethod()]
        public void GetAllElementsInDiagonalFromCurrent_InTheBoard_LeftDown_Returns_2()
        {
            //  Arrange
            var checker = new CheckerElement(2, 4, PieceType.Checker, Side.White);
            var mainPlayCheckers = new List<CheckerElement>() {checker};
            var secondPlayerCheckers = new List<CheckerElement>(){};
            var playerOne = new MainHumanPlayer(Side.White, new DataProvider(mainPlayCheckers, secondPlayerCheckers));

            //  Act
            var queueOfDiagonalElements = playerOne.GetAllElementsInDiagonalFromCurrent(checker,Diagonal.LeftDown);
            //  Assert
            Assert.AreEqual(2 , queueOfDiagonalElements.Count);
        }


        [TestMethod()]
        public void GetAllElementsInDiagonalFromCurrent_InTheBoard_RightUp_Returns_3()
        {
            //  Arrange
            var checker = new CheckerElement(2, 4, PieceType.Checker, Side.White);
            var mainPlayCheckers = new List<CheckerElement>() {checker};
            var secondPlayerCheckers = new List<CheckerElement>(){};
            var playerOne = new MainHumanPlayer(Side.White, new DataProvider(mainPlayCheckers, secondPlayerCheckers));

            //  Act
            var queueOfDiagonalElements = playerOne.GetAllElementsInDiagonalFromCurrent(checker,Diagonal.RightUp);
            //  Assert
            Assert.AreEqual(3 , queueOfDiagonalElements.Count);
        }


        [TestMethod()]
        public void GetAllElementsInDiagonalFromCurrent_InTheBoard_RightDown_Returns_4()
        {
            //  Arrange
            var checker = new CheckerElement(2, 4, PieceType.Checker, Side.White);
            var mainPlayCheckers = new List<CheckerElement>() {checker};
            var secondPlayerCheckers = new List<CheckerElement>(){};
            var playerOne = new MainHumanPlayer(Side.White, new DataProvider(mainPlayCheckers, secondPlayerCheckers));

            //  Act
            var queueOfDiagonalElements = playerOne.GetAllElementsInDiagonalFromCurrent(checker,Diagonal.RightDown);
            //  Assert
            Assert.AreEqual(4 , queueOfDiagonalElements.Count);
        }


        [TestMethod()]
        public void GetNextElementsInDiagonal_Returns_2()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerElement>() { new CheckerElement(4, 6, PieceType.Checker, Side.White) };
            var secondPlayerCheckers = new List<CheckerElement>(){ new CheckerElement(2, 4, PieceType.Checker, Side.Black) };
            var playerOne = new MainHumanPlayer(Side.White, new DataProvider(mainPlayCheckers, secondPlayerCheckers));

            //  Act
            var queueOfDiagonalElements = playerOne.GetNextElementsInDiagonal(mainPlayCheckers.Single(), secondPlayerCheckers.Single());
            //  Assert
            Assert.AreEqual(2 , queueOfDiagonalElements.Count);
        }


        [TestMethod()]
        public void GetNextElementsInDiagonal_Returns_1()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerElement>() { new CheckerElement(4, 6, PieceType.Checker, Side.White) };
            var secondPlayerCheckers = new List<CheckerElement>()
            {
                new CheckerElement(2, 4, PieceType.Checker, Side.Black),
                new CheckerElement(0, 2, PieceType.Checker, Side.Black),
            };
            var playerOne = new MainHumanPlayer(Side.White, new DataProvider(mainPlayCheckers, secondPlayerCheckers));

            //  Act
            var queueOfDiagonalElements = playerOne.GetNextElementsInDiagonal(mainPlayCheckers.First(), secondPlayerCheckers.First());
            //  Assert
            Assert.AreEqual(1 , queueOfDiagonalElements.Count);
        }
        [TestMethod()]
        public void GetNextElementsInDiagonal_Returns_0()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerElement>() { new CheckerElement(1, 7, PieceType.Checker, Side.White) };
            var secondPlayerCheckers = new List<CheckerElement>()
            {
                new CheckerElement(3, 5, PieceType.Checker, Side.Black),
                new CheckerElement(4, 4, PieceType.Checker, Side.Black),
            };
            var playerOne = new MainHumanPlayer(Side.White, new DataProvider(mainPlayCheckers, secondPlayerCheckers));

            //  Act
            var queueOfDiagonalElements = playerOne.GetNextElementsInDiagonal(mainPlayCheckers.First(), secondPlayerCheckers.First());
            //  Assert
            Assert.AreEqual(0 , queueOfDiagonalElements.Count);
        }
        [TestMethod()]
        public void GetNextElementsInDiagonal_Returns_3()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerElement>() { new CheckerElement(1, 7, PieceType.Checker, Side.White) };
            var secondPlayerCheckers = new List<CheckerElement>()
            {
                new CheckerElement(3, 5, PieceType.Checker, Side.Black),
                new CheckerElement(7, 1, PieceType.Checker, Side.Black),
            };
            var playerOne = new MainHumanPlayer(Side.White, new DataProvider(mainPlayCheckers, secondPlayerCheckers));

            //  Act
            var queueOfDiagonalElements = playerOne.GetNextElementsInDiagonal(mainPlayCheckers.First(), secondPlayerCheckers.First());
            //  Assert
            Assert.AreEqual(3 , queueOfDiagonalElements.Count);
        }

        [TestMethod()]
        public void CalculateAvailableForQueen_OneBlackChecker_Should_Have_2_Paths()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerElement>()
            {
                new CheckerElement(4, 6, PieceType.Queen, Side.White),
            };
            var secondPlayerCheckers = new List<CheckerElement>()
            {
                new CheckerElement(2, 4, PieceType.Checker, Side.Black),
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
            Assert.AreEqual(2, playerOne.AvailablePaths.Count);
        }

        [TestMethod()]
        public void CalculateAvailableForQueen_TwoBlackCheckers_Should_Have_1_Path()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerElement>()
            {
                new CheckerElement(4, 6, PieceType.Queen, Side.White),
            };
            var secondPlayerCheckers = new List<CheckerElement>()
            {
                new CheckerElement(2, 4, PieceType.Checker, Side.Black),
                new CheckerElement(1, 1, PieceType.Checker, Side.Black),
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
            Assert.AreEqual(1, playerOne.AvailablePaths.Count);
        }

        [TestMethod()]
        public void CalculateAvailableForQueen_ThreeBlackCheckers_Should_Have_1_Paths()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerElement>()
            {
                new CheckerElement(4, 6, PieceType.Queen, Side.White),
            };
            var secondPlayerCheckers = new List<CheckerElement>()
            {
                new CheckerElement(2, 4, PieceType.Checker, Side.Black),
                new CheckerElement(1, 1, PieceType.Checker, Side.Black),
                new CheckerElement(6, 4, PieceType.Checker, Side.Black),
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
            Assert.AreEqual(1, playerOne.AvailablePaths.Count);
        }


        [TestMethod()]
        public void CalculateAvailableForQueen_ThreeBlackCheckers_Should_Have_4_Paths()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerElement>()
            {
                new CheckerElement(4, 6, PieceType.Queen, Side.White),
            };
            var secondPlayerCheckers = new List<CheckerElement>()
            {
                new CheckerElement(2, 4, PieceType.Checker, Side.Black),
                new CheckerElement(1, 1, PieceType.Checker, Side.Black),
                new CheckerElement(3, 1, PieceType.Checker, Side.Black),
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
            Assert.AreEqual(4, playerOne.AvailablePaths.Count);
        }

        [TestMethod()]
        public void CalculateAvailableForQueen_FourBlackCheckers_Should_Have_5_Paths()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerElement>()
            {
                new CheckerElement(4, 6, PieceType.Queen, Side.White),
            };
            var secondPlayerCheckers = new List<CheckerElement>()
            {
                new CheckerElement(2, 4, PieceType.Checker, Side.Black),
                new CheckerElement(2, 2, PieceType.Checker, Side.Black),
                new CheckerElement(4, 2, PieceType.Checker, Side.Black),
                new CheckerElement(5, 5, PieceType.Checker, Side.Black),
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
            Assert.AreEqual(5, playerOne.AvailablePaths.Count);
        }


    }
}