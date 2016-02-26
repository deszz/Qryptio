using System.Windows.Controls;

namespace Qryptio.Pages
{
    public partial class ResultPage : UserControl
    {
        private ResultPageViewModel viewModel;

        public ResultPage(string inputFileName, string outputFileName)
        {
            InitializeComponent();

            viewModel = new ResultPageViewModel(MainWindow.DefaultNavigator, inputFileName, outputFileName);
            DataContext = viewModel;
        }
    }
}
