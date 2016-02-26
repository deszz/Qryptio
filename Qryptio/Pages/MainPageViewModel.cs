using System;
using System.Windows.Input;

using Qryptio.Core.Exchange;
using Qryptio.Properties;
using Qryptio.Utility;
using Qryptio.Wpf;

namespace Qryptio.Pages
{
    public class MainPageViewModel : ViewModel
    {
        public ICommand OpenSettingsCommand
        { get; private set; }
        public ICommand OpenInfoCommand
        { get; private set; }
        public ICommand FileDroppedCommand
        { get; private set; }
        public ICommand SelectFileCommand
        { get; private set; }

        private readonly string fileFilter;

        public MainPageViewModel(Navigator navigator)
            : base(navigator)
        {
            fileFilter = $"{Resources.MainPage_AllFiles} (*.*)|*.*|" +
                         $"Qryptio (*{QrFile.Extention})|*{QrFile.Extention}";

            OpenSettingsCommand = new SimpleCommand()
            { SimpleExecuteDelegate = OpenSettings };
            OpenInfoCommand = new SimpleCommand()
            { SimpleExecuteDelegate = OpenInfo };
            FileDroppedCommand = new SimpleCommand()
            { ExecuteDelegate = FileDropped };
            SelectFileCommand = new SimpleCommand()
            { SimpleExecuteDelegate = SelectFile };
        }

        #region Commands

        private void OpenSettings()
        {
            Navigate(new SettingsPage());
        }

        private void OpenInfo()
        {
            Navigate(new AboutPage());
        }

        private void FileDropped(object filePath)
        {
            Navigate(new FilePage((string)filePath));
        }

        private void SelectFile()
        {
            string filePath;
            if (DialogService.FileOpen(out filePath, fileFilter) == DialogResult.Ok)
                Navigate(new FilePage(filePath));
        }

        #endregion
    }
}
