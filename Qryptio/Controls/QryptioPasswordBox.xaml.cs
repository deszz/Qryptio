using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Qryptio.Core;

namespace Qryptio.Controls
{
    public partial class QryptioPasswordBox : UserControl
    {
        public event EventHandler PasswordChanged;

        public static readonly DependencyProperty HintTextProperty =
            DependencyProperty.Register("HintText", typeof(String),
                                        typeof(QryptioPasswordBox),
                                        new PropertyMetadata(String.Empty));

        public static readonly DependencyProperty HintFontProperty =
            DependencyProperty.Register("HintFont", typeof(FontFamily),
                                        typeof(QryptioPasswordBox));

        public static readonly DependencyProperty PasswordBoxStyleProperty =
            DependencyProperty.Register("PasswordBoxStyle", typeof(Style),
                                        typeof(QryptioPasswordBox));

        public Style PasswordBoxStyle
        {
            get { return (Style)GetValue(PasswordBoxStyleProperty); }
            set { SetValue(PasswordBoxStyleProperty, value); }
        }

        public string HintText
        {
            get { return (string)GetValue(HintTextProperty); }
            set { SetValue(HintTextProperty, value); }
        }

        public FontFamily HintFont
        {
            get { return (FontFamily)GetValue(HintFontProperty); }
            set { SetValue(HintFontProperty, value); }
        }

        public ProtectedData Password
        {
            get
            {
                return new ProtectedData(passwordBox.SecurePassword);
            }
        }

        public QryptioPasswordBox()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void SetHintVisibility(bool visible)
        {
            passwordHint.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
        }

        private Storyboard passwordBoxHightlightStoryboard;
        public void HighlightPasswordBox()
        {
            if (passwordBoxHightlightStoryboard == null)
            {
                passwordBoxHightlightStoryboard = new Storyboard();

                DoubleAnimation fadeIn = new DoubleAnimation(0, 1, new TimeSpan(0, 0, 0, 0, 200));
                DoubleAnimation fadeOut = new DoubleAnimation(1, 0, new TimeSpan(0, 0, 0, 0, 500));

                Storyboard.SetTarget(fadeIn, passwordBoxHighlight);
                Storyboard.SetTargetProperty(fadeIn, new PropertyPath(OpacityProperty));
                passwordBoxHightlightStoryboard.Children.Add(fadeIn);

                Storyboard.SetTarget(fadeOut, passwordBoxHighlight);
                Storyboard.SetTargetProperty(fadeOut, new PropertyPath(OpacityProperty));
                passwordBoxHightlightStoryboard.Children.Add(fadeOut);
            }
            passwordBoxHighlight.BeginStoryboard(passwordBoxHightlightStoryboard);
        }

        #region Events

        // rising

        private void RaisePasswordChanged()
        {
            if (PasswordChanged != null)
                PasswordChanged(this, EventArgs.Empty);
        }

        // handling

        private void PasswordBoxGotFocusHandler(object sender, RoutedEventArgs e)
        {
            SetHintVisibility(false);
        }

        private void PasswordBoxLostFocusHandler(object sender, RoutedEventArgs e)
        {
            if (passwordBox.SecurePassword.Length == 0)
                SetHintVisibility(true);
        }

        private void PasswordBoxPasswordChangedHandler(object sender, RoutedEventArgs e)
        {
            RaisePasswordChanged();
        }

        #endregion
    }
}
