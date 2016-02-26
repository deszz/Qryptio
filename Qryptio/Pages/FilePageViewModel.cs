using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

using MahApps.Metro.Controls;

using Qryptio.Core;
using Qryptio.Core.Exchange;
using Qryptio.Properties;
using Qryptio.Utility;
using Qryptio.Wpf;

namespace Qryptio.Pages
{
    public enum OperationType
    {
        Encryption,
        Decryption
    }

    public class OperationEventArgs : EventArgs
    {
        public OperationType Type;
        public Exception Error;
    }

    public class FilePageViewModel : ViewModel
    {
        public event EventHandler InvalidPasswordEntered;

        public event EventHandler FileEncrypted;
        public event EventHandler FileDecrypted;

        public event EventHandler<OperationEventArgs> OperationStarted;
        public event EventHandler<OperationEventArgs> OperationFailed;
        public event EventHandler<OperationEventArgs> OperationFinished;

        public ICommand BackCommand
        { get; private set; }
        public ICommand DecryptCommand
        { get; private set; }
        public ICommand EncryptCommand
        { get; private set; }

        private bool opActive;
        public bool OperationActive
        {
            get
            {
                return opActive;
            }
            private set
            {
                opActive = value;
                NotifyPropertyChanged(nameof(OperationActive));
            }
        }

        private bool opCancelled;
        public bool OperationCancelled
        {
            get
            {
                return opCancelled;
            }
            private set
            {
                opCancelled = value;
                NotifyPropertyChanged(nameof(OperationCancelled));
            }
        }

        private bool qrFileSelected;
        public bool QrFileSelected
        {
            get
            {
                return qrFileSelected;
            }
            private set
            {
                qrFileSelected = value;
                NotifyPropertyChanged(nameof(QrFileSelected));
            }
        }

        private double progress;
        public double Progress
        {
            get
            {
                return progress;
            }
            private set
            {
                progress = value;
                NotifyPropertyChanged(nameof(Progress));
            }
        }

        private ProtectedData password;
        public ProtectedData Password
        {
            set
            {
                password = new ProtectedData(value);
            }
        }

        public string ClearName
        { get; private set; }
        public string FileName
        { get; private set; }
        public string Path
        { get; private set; }
        public string Extenstion
        { get; private set; }
        public FileType FileType
        { get; private set; }
        public ImageSource FileIcon
        { get; private set; }
        public string FileDescription
        { get; private set; }

        private FileInfo fileInfo;
        private CancellationTokenSource tokenSource;
        private bool passwordValid
        {
            get
            {
                return password.Length != 0;
            }
        }

        public FilePageViewModel(Navigator navigator, string fileName)
            : base(navigator)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));

            fileInfo = new FileInfo(fileName);
            if (!fileInfo.Exists)
                throw new FileNotFoundException();

            ClearName = System.IO.Path.GetFileNameWithoutExtension(fileInfo.FullName);
            FileName = fileInfo.Name;
            Path = fileInfo.FullName;
            Extenstion = fileInfo.Extension;
            FileType = fileInfo.GetFileType();

            var fileTypeInfo = FileType.GetFileTypeInfo();
            FileDescription = $"{fileTypeInfo.Description}, {Utils.ConvertBinLengthToReadableString(fileInfo.Length)}";
            FileIcon = fileTypeInfo.Icon.ToImageSource();

            password = ProtectedData.Empty;
            QrFileSelected = QrFile.IsQrFile(fileName);

            BackCommand = new SimpleCommand()
            { SimpleExecuteDelegate = Back };
            EncryptCommand = new SimpleCommand()
            { SimpleExecuteDelegate = Encrypt };
            DecryptCommand = new SimpleCommand()
            { SimpleExecuteDelegate = Decrypt };
        }

        private void _Encrypt(string output, CancellationToken cancelToken)
        {
            var qrEncrypter = new QrFileEncrypter(fileInfo.FullName, output);
            qrEncrypter.ProgressChanged += (sender, e) => Progress = e.Progress.Percent;

            qrEncrypter.Encrypt(password, cancelToken);
        }

        private void _Decrypt(string output, CancellationToken cancelToken)
        {
            var qrDecrypter = new QrFileDecrypter(fileInfo.FullName, output);
            qrDecrypter.ProgressChanged += (sender, e) => Progress = e.Progress.Percent;

            qrDecrypter.Decrypt(password, cancelToken);
        }

        private void OnOperationStarted(OperationType opType)
        {
            OperationActive = true;
            ((MainWindow)App.Current.MainWindow).DisableCloseButton();

            RaiseOperationStarted(opType);
        }

        private void OnOperationFinished(OperationType opType)
        {
            OperationActive = false;
            ((MainWindow)App.Current.MainWindow).EnableCloseButton();

            RaiseOperationFinished(opType);
        }

        #region Events

        private void RaiseInvalidPasswordEntered()
        {
            if (InvalidPasswordEntered != null)
                InvalidPasswordEntered(this, EventArgs.Empty);
        }

        private void RaiseFileDecrypted(string resultFilePath)
        {
            if (FileDecrypted != null)
                FileDecrypted(this, EventArgs.Empty);
        }

        private void RaiseFileEncrypted(string resultFilePath)
        {
            if (FileEncrypted != null)
                FileEncrypted(this, EventArgs.Empty);
        }

        private void RaiseOperationStarted(OperationType type)
        {
            if (OperationStarted != null)
                OperationStarted(this, new OperationEventArgs() { Type = type });
        }

        private void RaiseOperationFailed(OperationType type, Exception error)
        {
            if (OperationFailed != null)
                OperationFailed(this, new OperationEventArgs() { Type = type, Error = error });
        }

        private void RaiseOperationFinished(OperationType type)
        {
            if (OperationFinished != null)
                OperationFinished(this, new OperationEventArgs() { Type = type });
        }

        #endregion

        #region Commands

        private void Back()
        {
            if (!opActive || DialogService.Question(Resources.FilePage_CancellationQuestion) == DialogResult.Yes)
            {
                OperationCancelled = true;
                tokenSource?.Cancel();
                Navigate(new MainPage());
            }
        }

        private async void Encrypt()
        {
            if (!passwordValid)
            {
                RaiseInvalidPasswordEntered();
                return;
            }

            OnOperationStarted(OperationType.Encryption);

            try
            {
                var outputPath = Utils.GetEncryptedFileName(fileInfo.FullName);
                using (tokenSource = new CancellationTokenSource())
                    await Task.Run(() => _Encrypt(outputPath, tokenSource.Token));

                RaiseFileEncrypted(outputPath);
                Navigate(new ResultPage(fileInfo.FullName, outputPath));
            }
            catch (OperationCanceledException e)
            {
                RaiseOperationFailed(OperationType.Encryption, e);
            }
            catch (Exception e)
            {
                DialogService.Error(e.Message, Resources.FilePage_UnknownError);
                RaiseOperationFailed(OperationType.Encryption, e);
            }
            finally
            {
                tokenSource = null;
                Progress = 0;

                OnOperationFinished(OperationType.Encryption);
            }
        }

        private async void Decrypt()
        {
            if (!passwordValid)
            {
                RaiseInvalidPasswordEntered();
                return;
            }

            OnOperationStarted(OperationType.Decryption);

            try
            {
                var outputPath = Utils.GetDecryptedFileName(fileInfo.FullName);
                using (var tokenSource = new CancellationTokenSource())
                    await Task.Run(() => _Decrypt(outputPath, tokenSource.Token));

                RaiseFileDecrypted(outputPath);
                Navigate(new ResultPage(fileInfo.FullName, outputPath));
            }
            catch (OperationCanceledException e)
            {
                RaiseOperationFailed(OperationType.Decryption, e);
            }
            catch (InvalidPasswordException e)
            {
                DialogService.Error(Resources.FilePage_InvalidPassword);
                RaiseOperationFailed(OperationType.Decryption, e);
            }
            catch (FileCorruptedException e)
            {
                DialogService.Error(Resources.FilePage_FileCorrupted);
                RaiseOperationFailed(OperationType.Decryption, e);
            }
            catch (Exception e)
            {
                DialogService.Error(e.Message, Resources.FilePage_UnknownError);
                RaiseOperationFailed(OperationType.Decryption, e);
            }
            finally
            {
                tokenSource = null;
                Progress = 0;

                OnOperationFinished(OperationType.Decryption);
            }
        }

        #endregion
    }
}
