using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Media;

using Qryptio.Properties;
using Qryptio.Utility;
using Qryptio.Wpf;

namespace Qryptio.Pages
{
    public class AboutPageViewModel : ViewModel
    {
        public ICommand BackCommand
        { get; private set; }
        public ICommand GoToSiteCommand
        { get; private set; }

        public ImageSource Logo
        { get; private set; }
        public string Name
        { get; private set; }
        public string Description
        { get; private set; }
        public string Version
        { get; private set; }
        public string Size
        { get; private set; }
        public string Link
        { get; private set; }

        public AboutPageViewModel(Navigator navigator)
            : base(navigator)
        {
            BackCommand = new SimpleCommand()
            { SimpleExecuteDelegate = Back };
            GoToSiteCommand = new SimpleCommand()
            { SimpleExecuteDelegate = GoToSite };

            var assembly = Assembly.GetExecutingAssembly();

            Logo = Resources.app_logo.ToImageSource();
            Name = assembly.GetName().Name;
            Description = assembly
                .GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)
                .OfType<AssemblyDescriptionAttribute>()
                .FirstOrDefault()?.Description ?? "<no description>";
            Version = assembly.GetName().Version.ToString();
            Size = Utils.ConvertBinLengthToReadableString(new FileInfo(assembly.Location).Length);
            Link = @"https://github.com/deszz/qryptio";
        }

        #region Commands

        private void Back()
        {
            Navigate(new MainPage());
        }

        private void GoToSite()
        {
            Process.Start(new ProcessStartInfo(Link));
        }

        #endregion
    }
}
