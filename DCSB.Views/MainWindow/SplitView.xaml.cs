using System.Windows;
using System.Windows.Controls;

namespace DCSB.Views.MainWindow
{
    /// <summary>
    /// Interaction logic for SplitView.xaml
    /// </summary>
    public partial class SplitView : UserControl
    {
        public SplitView()
        {
            InitializeComponent();
        }

        private void UserControlSizeChanged(object sender, SizeChangedEventArgs e)
        {
            splitterGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
            splitterGrid.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
        }
    }
}
