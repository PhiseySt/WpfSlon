using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace SlonResourcesDiagnostika
{
    /// <summary>
    /// Interaction logic for LoadingPanelWindow.xaml
    /// </summary>
    public partial class LoadingPanelWindow : INotifyPropertyChanged
    {

        #region Proprties

        private const string MessageFooter = "Время тестирования (сек) : ";
        private DispatcherTimer _timerLoadingPanel = new DispatcherTimer();
        private long _currentValTimer;
        private string _currentValTimerStr;

        public  string CurrentValTimerStr
        {
            get { return _currentValTimerStr; }
            set { _currentValTimerStr = value; OnPropertyChanged(); }
        }

        #endregion
        #region Events
        public LoadingPanelWindow()
        {
            InitializeComponent();
            Timer_Start();
        }

        private void Timer_Start()
        {
            _timerLoadingPanel = new DispatcherTimer();
            _timerLoadingPanel.Tick += Timer_Tick;
            _timerLoadingPanel.Interval = new TimeSpan(0, 0, 1);
            _timerLoadingPanel.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _currentValTimer++;
            CurrentValTimerStr = MessageFooter + _currentValTimer;
        }
        #endregion

        #region NotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
