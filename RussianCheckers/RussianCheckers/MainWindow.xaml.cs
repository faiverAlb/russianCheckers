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
