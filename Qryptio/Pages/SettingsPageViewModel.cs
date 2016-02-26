using System;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Media;

using Microsoft.Win32;

using Qryptio.Properties;
using Qryptio.Utility;
using Qryptio.Wpf;

namespace Qryptio.Pages
{
    public class SettingsPageViewModel : ViewModel
    {
        public ICommand BackCommand
        { get; private set; }
        public ICommand RestartAsAdministratorCommand
        { get; private set; }
        public ICommand EnableContextMenuOptionCommand
        { get; private set; }
        public ICommand DisableContextMenuOptionCommand
        { get; private set; }

        public ImageSource UACIcon
        {
            get
            {
                return Utils.GetShieldIcon();
            }
        }

        public bool HasAdminRights
        {
            get
            {
                return App.HasAdminPrivelegies;
            }
        }

        private bool contextMenuOptionActivated;
        public bool ContextMenuOptionActivated
        {
            get
            {
                return contextMenuOptionActivated;
            }
            set
            {
                contextMenuOptionActivated = value;
                NotifyPropertyChanged(nameof(ContextMenuOptionActivated));
            }
        }

        private static string option = $"*\\shell\\qryptio";
        private static string command = $"{option}\\command";

        public SettingsPageViewModel(Navigator navigator)
            : base(navigator)
        {
            BackCommand = new SimpleCommand()
            { SimpleExecuteDelegate = Back };
            RestartAsAdministratorCommand = new SimpleCommand()
            { SimpleExecuteDelegate = RestartAsAdministrator };
            EnableContextMenuOptionCommand = new SimpleCommand()
            { SimpleExecuteDelegate = EnableContextMenuOption };
            DisableContextMenuOptionCommand = new SimpleCommand()
            { SimpleExecuteDelegate = DisableContextMenuOption };

            ContextMenuOptionActivated = Registry.ClassesRoot.OpenSubKey(command) != null;
        }

        #region Commands

        private void Back()
        {
            Navigate(new MainPage());
        }

        private void RestartAsAdministrator()
        {
            App.RestartWithAdminPrivelegies();
        }

        private void DisableContextMenuOption()
        {
            try
            {
                if (App.HasAdminPrivelegies)
                {
                    Registry.ClassesRoot.DeleteSubKeyTree(option);

                    ContextMenuOptionActivated = false;
                    return;
                }
            }
            catch (Exception e)
            {
                DialogService.Error(e.Message, Resources.Settings_FeatureError);
            }
        }

        private void EnableContextMenuOption()
        {
            try
            {
                if (App.HasAdminPrivelegies)
                {
                    Registry.ClassesRoot.CreateSubKey(option, RegistryKeyPermissionCheck.ReadWriteSubTree).SetValue("", Qryptio.Properties.Resources.App_ContextMenuOpenWith);
                    Registry.ClassesRoot.CreateSubKey(command, RegistryKeyPermissionCheck.ReadWriteSubTree).SetValue("", $"{Assembly.GetExecutingAssembly().Location} \"%1\"");

                    ContextMenuOptionActivated = true;
                    return;
                }
            }
            catch (Exception e)
            {
                DialogService.Error(e.Message, Resources.Settings_FeatureError);
            }
        }

        #endregion
    }
}
