using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

using Qryptio.Wpf;

namespace Qryptio.Pages
{
    public class ResultPageViewModel : ViewModel
    {
        private bool inputFileExists;
        public bool InputFileExists
        {
            get
            {
                return inputFileExists;
            }
            set
            {
                inputFileExists = value;
                NotifyPropertyChanged(nameof(InputFileExists));
            }
        }

        public ICommand StartOverCommand
        { get; private set; }
        public ICommand DeleteInputFileCommand
        { get; private set; }
        public ICommand OpenDirectoryCommand
        { get; private set; }

        public string InputFilePath
        { get; private set; }
        public string OutputFilePath
        { get; private set; }
        public string OutputDirectory
        { get; private set; }

        public ResultPageViewModel(Navigator navigator, string inputFileName, string outputFileName)
            : base(navigator)
        {
            if (outputFileName == null)
                throw new ArgumentNullException(nameof(outputFileName));

            var fileInfo = new FileInfo(outputFileName);
            if (!fileInfo.Exists)
                throw new FileNotFoundException();

            InputFilePath = inputFileName ?? String.Empty;
            OutputFilePath = outputFileName;
            OutputDirectory = fileInfo.DirectoryName;

            InputFileExists = File.Exists(InputFilePath);

            StartOverCommand = new SimpleCommand() { SimpleExecuteDelegate = StartOver };
            DeleteInputFileCommand = new SimpleCommand() { SimpleExecuteDelegate = DeleteInputFile };
            OpenDirectoryCommand = new SimpleCommand() { SimpleExecuteDelegate = OpenDirectory };
        }

        #region Commands

        private void StartOver()
        {
            Navigate(new MainPage());
        }

        private void DeleteInputFile()
        {
            if (File.Exists(InputFilePath))
            {
                File.Delete(InputFilePath);
                InputFileExists = false;
            }
        }

        private void OpenDirectory()
        {
            Process.Start(OutputDirectory);
        }

        #endregion
    }
}
