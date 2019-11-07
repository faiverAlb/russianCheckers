using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RussianCheckers.Core;

namespace RussianCheckers.Game.Tests
{
    [TestClass()]
    public class NeighborsCalculatorTests
    {
        [TestMethod()]
        public void CalculateNeighbors_ForQueen_NoOtherCheckers_ShouldCountAllEmpty()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerModel>()
            {
                new CheckerModel(4, 6, PieceType.Queen, Side.White)
            };
            DataProvider dataProvider = new DataProvider(mainPlayCheckers, new List<CheckerModel>());
            var neighborsCalculator = new NeighborsCalculator(dataProvider, mainPlayCheckers);

            //  Act
            Dictionary<CheckerModel, List<CheckerModel>> neighbors = neighborsCalculator.CalculateNeighbors();
        
            //  Assert
            Assert.AreEqual(9, neighbors.Single().Value.Count);
        }


        
        [TestMethod()]
        public void CalculateNeighbors_ForQueen_HaveSameOnPath_ShouldCountAllEmpty()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerModel> {new CheckerModel(4, 6, PieceType.Queen, Side.White)};
            var secondPlayerCheckers = new List<CheckerModel> {new CheckerModel(1, 3, PieceType.Checker, Side.Black)};
            MainPlayer playerOne = new MainPlayer(new DataProvider(mainPlayCheckers, secondPlayerCheckers), Side.White);
        
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
            var mainPlayCheckers = new List<CheckerModel> {new CheckerModel(4, 6, PieceType.Queen, Side.White)};
            var secondPlayerCheckers = new List<CheckerModel>() {new CheckerModel(1, 3, PieceType.Checker, Side.Black)};
            var playerOne = new MainPlayer(new DataProvider(mainPlayCheckers, secondPlayerCheckers), Side.White);
        
            //  Act
            playerOne.CalculateNeighbors();
        
            //  Assert
            List<CheckerModel> whiteCheckerNeighbors = playerOne.PlayerPositions.Single().Neighbors;
            CheckerModel blackChecker = secondPlayerCheckers.Single();
            Assert.IsTrue(whiteCheckerNeighbors.Contains(blackChecker) );
        }

        
        [TestMethod()]
        public void CalculateNeighbors_ForQueen_ShouldNotHaveEmptyNeighborAfterBlack()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerModel> {new CheckerModel(4, 6, PieceType.Queen, Side.White)};
            var secondPlayerCheckers = new List<CheckerModel> {new CheckerModel(1, 3, PieceType.Checker, Side.Black)};
            var playerOne = new MainPlayer(new DataProvider(mainPlayCheckers, secondPlayerCheckers), Side.White);
        
            //  Act
            playerOne.CalculateNeighbors();
        
            //  Assert
            List<CheckerModel> whiteCheckerNeighbors = playerOne.PlayerPositions.Single().Neighbors;
            Assert.IsFalse(whiteCheckerNeighbors.Any(x => x.Side == Side.Empty && x.Column == 0 && x.Row == 2) );
        }

        [TestMethod()]
        public void CalculateNeighborsForQueen_HaveTwoBlackNearestNeighbors()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerModel> {new CheckerModel(4, 6, PieceType.Queen, Side.White)};
            var secondPlayerCheckers = new List<CheckerModel>()
            {
                new CheckerModel(1, 3, PieceType.Checker, Side.Black),
                new CheckerModel(5, 5, PieceType.Checker, Side.Black),
            };

            MainPlayer playerOne = new MainPlayer(new DataProvider(mainPlayCheckers, secondPlayerCheckers), Side.White);
        
            //  Act
            playerOne.CalculateNeighbors();
        
            //  Assert
            List<CheckerModel> actualNeighbors = playerOne.PlayerPositions.Single().Neighbors;
            Assert.AreEqual(2, actualNeighbors.Count(x => x.Side == Side.Black));
        }

        [TestMethod()]
        public void CalculateNeighborsForQueen_ShouldHave_6_Neighbors()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerModel> {new CheckerModel(4, 6, PieceType.Queen, Side.White)};
            var secondPlayerCheckers = new List<CheckerModel>()
            {
                new CheckerModel(1, 3, PieceType.Checker, Side.Black),
                new CheckerModel(5, 5, PieceType.Checker, Side.Black),
            };
            var playerOne = new MainPlayer(new DataProvider(mainPlayCheckers, secondPlayerCheckers), Side.White);
        
            //  Act
            playerOne.CalculateNeighbors();
        
            //  Assert
            List<CheckerModel> actualNeighbors = playerOne.PlayerPositions.Single().Neighbors;
            Assert.AreEqual(6, actualNeighbors.Count);
        }


        [TestMethod()]
        public void CalculateNeighborsForQueen_ShouldHave_4_Neighbors()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerModel> {new CheckerModel(4, 6, PieceType.Queen, Side.White)};
            var secondPlayerCheckers = new List<CheckerModel>()
            {
                new CheckerModel(3, 5, PieceType.Checker, Side.Black),
                new CheckerModel(5, 5, PieceType.Checker, Side.Black),
            };            
            var playerOne = new MainPlayer(new DataProvider(mainPlayCheckers, secondPlayerCheckers), Side.White);
        
            //  Act
            playerOne.CalculateNeighbors();
        
            //  Assert
            List<CheckerModel> actualNeighbors = playerOne.PlayerPositions.Single().Neighbors;
            Assert.AreEqual(4, actualNeighbors.Count);
        }

        [TestMethod()]
        public void CalculateNeighborsForQueen_QueenInCorner_ShouldHave_1_Neighbor()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerModel> {new CheckerModel(0, 0, PieceType.Queen, Side.White)};
            var secondPlayerCheckers = new List<CheckerModel>() {new CheckerModel(1, 1, PieceType.Checker, Side.Black)};
            var playerOne = new MainPlayer(new DataProvider(mainPlayCheckers, secondPlayerCheckers), Side.White);
        
            //  Act
            playerOne.CalculateNeighbors();
        
            //  Assert
            List<CheckerModel> actualNeighbors = playerOne.PlayerPositions.Single().Neighbors;
            Assert.AreEqual(1, actualNeighbors.Count);
        }

        [TestMethod()]
        public void CalculateNeighborsForQueen_QueenInCorner_ShouldHave_7_Neighbor()
        {
            //  Arrange
            var mainPlayCheckers = new List<CheckerModel> {new CheckerModel(0, 0, PieceType.Queen, Side.White)};
            var playerOne = new MainPlayer(new DataProvider(mainPlayCheckers, new List<CheckerModel>()), Side.White);
        
            //  Act
            playerOne.CalculateNeighbors();
        
            //  Assert
            List<CheckerModel> actualNeighbors = playerOne.PlayerPositions.Single().Neighbors;
            Assert.AreEqual(7, actualNeighbors.Count);
        }
    }
}