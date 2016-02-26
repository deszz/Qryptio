using System;
using System.Windows.Controls;

using Qryptio.Wpf;

namespace Qryptio
{
    public class Navigator
    {
        private IContentWindow window;

        public Navigator(IContentWindow window)
        {
            if (window == null)
                throw new ArgumentNullException(nameof(window));

            this.window = window;
        }

        public void NavigateTo(Control page)
        {
            if (page == null)
                throw new ArgumentNullException(nameof(page));

            window.ContentControl.Content = page;
        }
    }
}
