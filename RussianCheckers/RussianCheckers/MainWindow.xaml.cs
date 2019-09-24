﻿using System.Windows;
using RussianCheckers.MVVM;

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
            var notificationDialogService = new NotificationDialogService(this);
            notificationDialogService.Register<NotificationDialogViewModel,NotificationDialog>();
            this.DataContext = new GameViewModel(
                new MainHumanPlayer(Side.White)
                , new RobotPlayer(Side.Black),
                notificationDialogService);
        }
    }
}
