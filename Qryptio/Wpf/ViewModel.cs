using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace Qryptio.Wpf
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected Navigator navigator;

        public ViewModel(Navigator navigator)
        {
            this.navigator = navigator;
        }

        protected void Navigate(UserControl page)
        {
            if (page == null)
                throw new ArgumentNullException(nameof(page));

            (navigator ?? MainWindow.DefaultNavigator).NavigateTo(page);
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
