using MahApps.Metro;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MaximovInkToDoApp
{
    /// <summary>
    /// Логика взаимодействия для ThemesWindow.xaml
    /// </summary>
    public partial class ThemesWindow
    {
        public static ThemesWindow window;

        public ThemesWindow()
        {
            if (window != null)
            {
                Close();
                return;
            }
            InitializeComponent();
            window = this;
            Closing += OnWindowClosing;
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (this == window)
                window = null;
        }

        private void ChangeAppThemeButtonClick(object sender, RoutedEventArgs e)
        {
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            ThemeManager.ChangeAppStyle(this, theme.Item2, ThemeManager.GetAppTheme("Base" + ((Button)sender).Content));
            ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, ThemeManager.GetAppTheme("Base" + ((Button)sender).Content));
        }

        private void ChangeAppAccentButtonClick(object sender, RoutedEventArgs e)
        {
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            ThemeManager.ChangeAppStyle(this, ThemeManager.GetAccent(((Button)sender).Content.ToString()), theme.Item1);
            ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(((Button)sender).Content.ToString()), theme.Item1);
        }
    }
}
