using System;

using Qryptio.Pages;
using Qryptio.Wpf;  

namespace Qryptio
{
    public class MainWindowViewModel : ViewModel
    {
        public MainWindowViewModel(Navigator navigator)
            : base(navigator)
        {
            if (String.IsNullOrEmpty(App.InitFile))
                Navigate(new MainPage());
            else
                Navigate(new FilePage(App.InitFile));
        }
    }
}
