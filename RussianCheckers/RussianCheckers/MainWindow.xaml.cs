﻿using System;
using System.Collections.Generic;
using System.Windows;
using RussianCheckers.Core;
using RussianCheckers.Core.Strategy;
using RussianCheckers.Game;
using RussianCheckers.Game.DialogsViewModels;
using RussianCheckers.Infrastructure;

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
            var dialogService = new DialogService(this);
            dialogService.Register<NotificationDialogViewModel,NotificationDialog>();
            dialogService.Register<ChooseDialogViewModel,ChooseSideDialog>();


            SourceInitialized += (sender, args) =>
            {
                var chooseDialogViewModel = new ChooseDialogViewModel();
                dialogService.ShowDialog(chooseDialogViewModel);
                Side userSide = chooseDialogViewModel.Side;
////                Side userSide = Side.White;
//                     
//                var mainPlayCheckers = new List<CheckerModel>()
//                {
//                    new CheckerModel(1, 1, PieceType.Checker, Side.Black),
//                    new CheckerModel(1, 3, PieceType.Checker, Side.Black),
//                    new CheckerModel(3, 3, PieceType.Checker, Side.Black),
//                    new CheckerModel(5, 3, PieceType.Checker, Side.Black),
//                    new CheckerModel(7, 1, PieceType.Checker, Side.Black),
//                };
//                var secondPlayerCheckers = new List<CheckerModel>()
//                {
//                    new CheckerModel(4, 4, PieceType.Checker, Side.White),
//                };
//            DataProvider dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
//            var mainPlayer = new MainPlayer(dataProvider, Side.Black);
//            var robotPlayer = new RobotPlayer(dataProvider, Side.White, new MinMaxStrategy());
//            var emptyPlayer = new EmptyUserPlayer(dataProvider);

                var dataProvider = new DataProvider(userSide);
                var mainPlayer = new MainPlayer(dataProvider, userSide);
                var robotPlayer = new RobotPlayer(dataProvider, userSide == Side.White? Side.Black:Side.White, new MinMaxStrategy());
                var emptyPlayer = new EmptyUserPlayer(dataProvider);

                var game = new Core.Game(mainPlayer, robotPlayer, emptyPlayer, dataProvider);
                game.ReCalculateNeighborsAndPaths();



                this.DataContext = new GameViewModel(game, dialogService, true);

            };

            
        }
    }
}
