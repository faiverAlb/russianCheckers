﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RussianCheckers.Game;

namespace RussianCheckers
{
    public abstract class PlayerViewModel: ObservableObject
    {
        public readonly Side Side;
        public  ObservableCollection<CheckerElement> PlayerPositions { get; protected set; }

        protected PlayerViewModel(Side side)
        {
            Side = side;
        }

        public void MoveCheckerToNewPlace(CheckerElement checker, int column, int row)
        {
            CheckerElement foundChecker = PlayerPositions.Single(x => x == checker);
            foundChecker.SetNewPosition(column, row);
        }

        public void SetPossibleMovementElements(List<CheckerElement> allEmptyElements, List<CheckerElement> otherPlayerCheckers)
        {
            var queueOfElements = new Queue<CheckerElement>(PlayerPositions);
            while (queueOfElements.Any()) 
            {
                var playerChecker = queueOfElements.Dequeue();
                var checkerPossibleMoves = new List<CheckerElement>();

                IEnumerable<CheckerElement> otherPlayerNearestCheckElements = otherPlayerCheckers.Where(x => x.IsCheckerNearChecker(playerChecker));
                foreach(var emptyElement in allEmptyElements)
                {
                    if (playerChecker.IsCheckerNearChecker(emptyElement) && !checkerPossibleMoves.Contains(emptyElement)) 
                    {
                        checkerPossibleMoves.Add(emptyElement);
                    }
                }


                //                foreach (CheckerElement emptyElement in allEmptyElements)
                //                {
                //                    if (playerChecker.Type == PieceType.Checker)
                //                    {
                //                        //TODO: Calculate positions for simple type
                //                    }
                //                    else
                //                    {
                //                        //TODO: Calculate positions for queen type
                //                    }
                //                }
                playerChecker.SetPossibleMovementElements(checkerPossibleMoves);
            }
        }
        public void CalculateNeighbors(CheckerElement[,] currentData)
        {
            foreach (CheckerElement playerPosition in PlayerPositions)
            {
                CheckerElement checkerElement = currentData[playerPosition.Column, playerPosition.Row];
                List<CheckerElement> neighbors = new List<CheckerElement>();
                if (checkerElement.Column -1 >= 0)
                {
                    if (checkerElement.Row - 1 >= 0)
                    {
                        neighbors.Add(currentData[checkerElement.Column - 1, checkerElement.Row - 1]);
                    }
                    if (checkerElement.Row + 1 < 8)
                    {
                        neighbors.Add(currentData[checkerElement.Column - 1, checkerElement.Row + 1]);
                    }
                }

                if (checkerElement.Column + 1 < 8)
                {
                    if (checkerElement.Row - 1 >= 0)
                    {
                        neighbors.Add(currentData[checkerElement.Column + 1, checkerElement.Row - 1]);
                    }
                    if (checkerElement.Row + 1 < 8)
                    {
                        neighbors.Add(currentData[checkerElement.Column + 1, checkerElement.Row + 1]);
                    }
                }

                playerPosition.SetNeighbors(neighbors);
                SetPossibleMovements(playerPosition);
            }
        }

        private void SetPossibleMovements(CheckerElement playerChecker)
        {
            var emptyItems = new List<CheckerElement>();
            foreach (CheckerElement neighbor in playerChecker.Neighbors)
            {
                if (neighbor.Side == Side.Empty)
                {
                    emptyItems.Add(neighbor);
                }
            }

            playerChecker.SetPossibleMovementElements(emptyItems);
        }
    }

    public class MainHumanPlayer : PlayerViewModel
    {
        public MainHumanPlayer(Side side):base(side)
        {
            PlayerPositions = new ObservableCollection<CheckerElement>(GetInitialPositions(side));
        }

        private List<CheckerElement> GetInitialPositions(Side side)
        {
            var positions = new List<CheckerElement>();
            for (int col = 0; col < 8; col++)
            {
                for (int row = 0; row < 3; row++)
                {
                    if (row % 2 == 1 && col % 2 == 1)
                    {
                        positions.Add(new CheckerElement(col, row, PieceType.Checker, side));
                        continue;
                    }
                    if (row % 2 == 0 && col % 2 == 0)
                    {
                        positions.Add(new CheckerElement(col, row, PieceType.Checker, side));
                    }
                }
            }
            return positions;
        }
    }

    public class RobotPlayer: PlayerViewModel
    {
        public RobotPlayer(Side side) : base(side)
        {
            PlayerPositions = new ObservableCollection<CheckerElement>(GetInitialPositions(side));

        }

        private List<CheckerElement> GetInitialPositions(Side side)
        {
            var positions = new List<CheckerElement>();
            for (int col = 0; col < 8; col++)
            {
                for (int row = 5; row < 8; row++)
                {
                    if (row % 2 == 1 && col % 2 == 1)
                    {
                        positions.Add(new CheckerElement(col, row, PieceType.Checker, side));
                        continue;
                    }
                    if (row % 2 == 0 && col % 2 == 0)
                    {
                        positions.Add(new CheckerElement(col, row, PieceType.Checker, side));
                        continue;
                    }
                    //positions.Add(new CheckerElement(col, row, PieceType.Checker, Side.Empty));

                }
            }
            return positions;

        }

    }


}