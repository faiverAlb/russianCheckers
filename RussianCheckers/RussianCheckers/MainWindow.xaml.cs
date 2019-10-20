using System.Collections.Generic;
using System.Windows;
using RussianCheckers.Game;
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
            var notificationDialogService = new DialogService(this);
            notificationDialogService.Register<NotificationDialogViewModel,NotificationDialog>();

            Side mainPlayerSide = Side.White;


//            var mainPlayCheckers = new List<CheckerElement>()
//            {
//                new CheckerElement(6, 0, PieceType.Checker, Side.White),
//                new CheckerElement(5, 1, PieceType.Checker, Side.White),
//                new CheckerElement(4, 0, PieceType.Checker, Side.White),
//                new CheckerElement(7, 3, PieceType.Checker, Side.White),
//            };
//            var secondPlayerCheckers = new List<CheckerElement>()
//            {
//                new CheckerElement(6, 2, PieceType.Checker, Side.Black),
//
//
//            };
//            DataProvider dataProvider = new DataProvider(mainPlayCheckers, secondPlayerCheckers);
            var dataProvider = new DataProvider(mainPlayerSide);

            var mainHumanPlayer = new MainHumanPlayer(mainPlayerSide, dataProvider);
            var playerViewModel = new RobotPlayer(Side.Black, dataProvider);
            var emptyCellsPlayer = new EmptyCellsPlayer(Side.Empty, dataProvider);
            this.DataContext = new GameViewModel(
                mainHumanPlayer
                , playerViewModel,
                emptyCellsPlayer,
                dataProvider,
                notificationDialogService);
        }
    }
}
