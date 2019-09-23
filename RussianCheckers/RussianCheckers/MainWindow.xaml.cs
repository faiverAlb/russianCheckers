using System.Windows;

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
            this.DataContext = new GameViewModel(
                  new BoardViewModel()
                , new HumanPlayer()
                , new RobotPlayer());
        }
    }
}
