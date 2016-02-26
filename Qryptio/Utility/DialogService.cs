using System;
using System.Windows;

using Microsoft.Win32;
using Ookii.Dialogs.Wpf;

using Qryptio.Properties;

namespace Qryptio.Utility
{
    public static class DialogService
    {
        private static readonly string dialogTitle = "Qryptio";

        public static DialogResult Message(string message, string title = null)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            using (TaskDialog dialog = new TaskDialog())
            {
                dialog.MainInstruction = title ?? Resources.DialogService_Information;
                dialog.MainIcon = TaskDialogIcon.Information;
                dialog.Content = message;
                dialog.WindowTitle = dialogTitle;
                dialog.Buttons.Add(new TaskDialogButton(ButtonType.Ok));

                var result = dialog.ShowDialog();

                switch (result.ButtonType)
                {
                    case ButtonType.Ok:
                        return DialogResult.Ok;
                    default:
                        return DialogResult.None;
                }
            }
        }

        public static DialogResult Question(string message, string title = null)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            using (TaskDialog dialog = new TaskDialog())
            {
                dialog.MainInstruction = title ?? Resources.DialogService_Question;
                dialog.MainIcon = TaskDialogIcon.Information;
                dialog.Content = message;
                dialog.WindowTitle = dialogTitle;
                dialog.Buttons.Add(new TaskDialogButton(ButtonType.Yes));
                dialog.Buttons.Add(new TaskDialogButton(ButtonType.No));
                dialog.Buttons.Add(new TaskDialogButton(ButtonType.Cancel));

                var result = dialog.ShowDialog();

                switch (result.ButtonType)
                {
                    case ButtonType.Yes:
                        return DialogResult.Yes;
                    case ButtonType.No:
                        return DialogResult.No;
                    case ButtonType.Cancel:
                        return DialogResult.Cancel;
                    default:
                        return DialogResult.None;
                }
            }
        }

        public static DialogResult Error(Exception exception, string title = null)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            return Error(exception.Message, title);
        }

        public static DialogResult Error(string message, string title = null)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            using (TaskDialog dialog = new TaskDialog())
            {
                dialog.MainInstruction = title ?? Resources.DialogService_Error;
                dialog.MainIcon = TaskDialogIcon.Error;
                dialog.Content = message;
                dialog.WindowTitle = dialogTitle;
                dialog.Buttons.Add(new TaskDialogButton(ButtonType.Ok));

                var result = dialog.ShowDialog();

                switch (result.ButtonType)
                {
                    case ButtonType.Ok:
                        return DialogResult.Ok;
                    default:
                        return DialogResult.None;
                }
            }
        }

        public static DialogResult FileOpen(out string fileName, string filter)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.ValidateNames = true;
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.Multiselect = false;
            dialog.Filter = filter;

            if (dialog.ShowDialog(Application.Current.MainWindow) ?? false)
            {
                fileName = dialog.FileName;
                return DialogResult.Ok;
            }

            fileName = null;
            return DialogResult.Abort;
        }

        public static DialogResult FileSave(out string fileName, string filter)
        {
            SaveFileDialog dialog = new SaveFileDialog();

            dialog.ValidateNames = true;
            dialog.CheckPathExists = true;
            dialog.Filter = filter;

            if (dialog.ShowDialog(Application.Current.MainWindow) ?? false)
            {
                fileName = dialog.FileName;
                return DialogResult.Ok;
            }

            fileName = null;
            return DialogResult.Abort;
        }

        public static DialogResult SelectFolder(out string folderName)
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();

            if (dialog.ShowDialog(Application.Current.MainWindow) ?? false)
            {
                folderName = dialog.SelectedPath;
                return DialogResult.Ok;
            }

            folderName = null;
            return DialogResult.Abort;
        }
    }
}
