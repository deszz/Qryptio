using Qryptio.Utility;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Qryptio.Pages
{
    public partial class MainPage : UserControl
    {
        private readonly Brush defaultBrush;
        private readonly Brush hightlightBrush;

        private readonly MainPageViewModel viewModel;

        public MainPage()
        {
            InitializeComponent();

            defaultBrush = dropField.Stroke;
            hightlightBrush = (Brush)Application.Current.Resources["MainBrush"];

            viewModel = new MainPageViewModel(MainWindow.DefaultNavigator);
            DataContext = viewModel;
        }

        #region Events

        private void DropHandler(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                try
                {
                    string filePath = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
                    viewModel.FileDroppedCommand.Execute(filePath);
                }
                catch (FileNotFoundException)
                {
                    // folder dropped
                }
            }

            DragLeaveHandler(sender, e);
        }

        private void DragEnterHandler(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                dropField.Stroke = hightlightBrush;
            else
                dropHint.Visibility = Visibility.Visible;
        }

        private void DragLeaveHandler(object sender, DragEventArgs e)
        {
            dropField.Stroke = defaultBrush;
            dropHint.Visibility = Visibility.Collapsed;
        }


        #endregion
    }
}
