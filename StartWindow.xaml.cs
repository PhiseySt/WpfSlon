using System;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace SlonResourcesDiagnostika
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow
    {

        #region Proprties

        private DispatcherTimer _timer;

        #endregion

        #region Methods
        public StartWindow()
        {
            InitializeComponent();
            Timer_Start();
        }

        #endregion

        #region Events

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void Timer_Start()
        {
            _timer = new DispatcherTimer();
            _timer.Tick += Timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, 10);
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();
            var wind = new MainWindow();
            Close();
            wind.ShowDialog();
        }

        #endregion
    }
}
