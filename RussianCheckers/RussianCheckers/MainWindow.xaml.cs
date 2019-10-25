﻿using System.Collections.Generic;
using System.Windows;
using RussianCheckers.Game;
using RussianCheckers.Infrastructure;
using RussianCheckers.Strategy;

namespace RussianCheckers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var notificationDialogService = new DialogService(this);
            notificationDialogService.Register<NotificationDialogViewModel,NotificationDialog>();



//            var mainPlayCheckers = new List<CheckerElement>()
//            {
//                new CheckerElement(0, 0, PieceType.Checker, Side.White),
//                new CheckerElement(2, 0, PieceType.Checker, Side.White),
//                new CheckerElement(4, 0, PieceType.Checker, Side.White),
//                new CheckerElement(6, 0, PieceType.Checker, Side.White),
//                new CheckerElement(5, 1, PieceType.Checker, Side.White),
//                new CheckerElement(7, 1, PieceType.Checker, Side.White),
//                new CheckerElement(2, 2, PieceType.Checker, Side.White),
//                new CheckerElement(4, 2, PieceType.Checker, Side.White),
//                new CheckerElement(6, 2, PieceType.Checker, Side.White),
//                new CheckerElement(3, 3, PieceType.Checker, Side.White),
//            };
//            var secondPlayerCheckers = new List<CheckerElement>()
//            {
//                new CheckerElement(5, 3, PieceType.Checker, Side.Black),
//                new CheckerElement(5, 5, PieceType.Checker, Side.Black),
//                new CheckerElement(4, 6, PieceType.Checker, Side.Black),
//                new CheckerElement(6, 6, PieceType.Checker, Side.Black),
//                new CheckerElement(0, 6, PieceType.Checker, Side.Black),
//                new CheckerElement(1, 7, PieceType.Checker, Side.Black),
//                new CheckerElement(3, 7, PieceType.Checker, Side.Black),
//                new CheckerElement(5, 7, PieceType.Checker, Side.Black),
//                new CheckerElement(7, 7, PieceType.Checker, Side.Black),
//                new CheckerElement(7, 5, PieceType.Checker, Side.Black),
//
//
//            };
//            DataProvider dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);

            var dataProvider = new DataProvider(Side.White);
            var mainPlayer = new MainPlayer(dataProvider, Side.White);
            var robotPlayer = new RobotPlayer(dataProvider, Side.Black);
            var emptyPlayer = new EmptyUserPlayer(dataProvider);
            var game = new Game.Game(mainPlayer, robotPlayer, emptyPlayer, dataProvider);

            var mainHumanPlayer = new HumanPlayerViewModel(mainPlayer);
            var playerViewModel = new RobotPlayerViewModel(robotPlayer);
            var emptyCellsPlayer = new EmptyCellsPlayer(emptyPlayer);
            this.DataContext = new GameViewModel(game, notificationDialogService, false);
        }
    }
}
