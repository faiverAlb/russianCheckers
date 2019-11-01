using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RussianCheckers.Infrastructure;

namespace RussianCheckers
{
    /// <summary>
    /// Interaction logic for ChooseSideDialog.xaml
    /// </summary>
    public partial class ChooseSideDialog : Window, IDialog
    {
        public ChooseSideDialog()
        {
            InitializeComponent();
        }
    }
}
