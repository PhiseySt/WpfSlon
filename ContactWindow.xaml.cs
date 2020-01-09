
using System.Windows;
using System.Windows.Navigation;

namespace SlonResourcesDiagnostika
{
    /// <summary>
    /// Interaction logic for ContactWindow.xaml
    /// </summary>
    public partial class ContactWindow
    {
        public ContactWindow()
        {
            InitializeComponent();
        }

        #region Events

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            var wind = new MainWindow();
            Close();
            wind.ShowDialog();
        }
        #endregion
    }
}
