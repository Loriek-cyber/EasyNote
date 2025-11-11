using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace EasyNote
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowState = WindowState.Normal;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Close_MouseEnter(object sender, MouseEventArgs e)
        {
            ((System.Windows.Controls.Button)sender).Background = new SolidColorBrush(Color.FromRgb(180, 0, 0));
        }

        private void Close_MouseLeave(object sender, MouseEventArgs e)
        {
            ((System.Windows.Controls.Button)sender).Background = Brushes.Transparent;
        }
    }
}