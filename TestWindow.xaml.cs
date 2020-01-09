using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace SlonResourcesDiagnostika
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class TestWindow : INotifyPropertyChanged
    {
        #region Properties

        private const string StartMessageBody
            = "Это окно,где будут результаты исследования. Нажмите кнопку 'Старт'... " +
              "Обычно анализ длится в районе 30 секунд";
        private string _resultAnalyzeData;

        public string ResultAnalyzeData
        {
            get { return _resultAnalyzeData; }
            set { _resultAnalyzeData = value; OnPropertyChanged(); }
        }

        #endregion

        #region Events

        public TestWindow()
        {
            ResultAnalyzeData = StartMessageBody;
            InitializeComponent();
        }


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

        private void StartTest_Click(object sender, RoutedEventArgs e)
        {
            var wind = new LoadingPanelWindow();
            AnalyzeDataAsync();
            wind.ShowDialog();
        }
        #endregion

        #region Data

        private static string CalcData(object dataForAnalyze)
        {
            Thread.Sleep(SetTimeAnalyze());
            var sb = new StringBuilder();
            var data = (AnalyzeData)dataForAnalyze;
            sb.AppendLine("Оценка производительности вашей системы на данный момент : " + PerformanceRating());
            sb.AppendLine("Оценка выставлена по 100 бальной шкале.");
            sb.AppendLine("Подсистема где возможна оптимизация: " + GetOptimizationSystem(data));
            sb.AppendLine("В любом случае по вопросам оптимизации компьютера или ноутбука, вы можете позвонить специалистам фирмы Слон ЛТД.");
            sb.AppendLine("Контактный телефон :  (843) 240-96-66");

            var resultAnalyze = sb.ToString();
            return resultAnalyze;
        }

        private static int SetTimeAnalyze()
        {
            const int etalonTime = 30;
            var rndOffset = new Random();
            var rndOperation = new Random();
            var offset = rndOffset.Next(0, 10);
            var operation = rndOperation.Next(0, 1);
            var result = 0;
            if (operation == 0) result = etalonTime + offset;
            if (operation == 1) result = etalonTime - offset;
            // time in msec
            return result * 1000;
        }

        private static int PerformanceRating()
        {
            var rndPerformance = new Random();
            return rndPerformance.Next(75, 99);
        }

        private static string GetOptimizationSystem(AnalyzeData data)
        {
            var result = "Оперативная память";
            if (data.HddSsdUsageMax > 70) result = "Постоянная память";
            if (data.CpuUsageMax > 50) result = "Процессор";
            if (data.DSpeedMax == 0 && data.USpeedMax == 0) result = "Сетевой интерфейс";
            return result;
        }

        private async void AnalyzeDataAsync()
        {
            object dataForAnalyze = null;
            var mainWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(aw => aw.Name == "MainComponent");
            var dataMainWindow = mainWindow?.DataContext;
            if (dataMainWindow != null)
            {
                var propertyInfo = dataMainWindow.GetType().GetProperty("AnalyzeValues");
                if (propertyInfo != null)
                {
                    dataForAnalyze = propertyInfo.GetValue(dataMainWindow, null);
                }
            }
            ResultAnalyzeData = await Task.Run(() => CalcData(dataForAnalyze));
            var activeWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);
            activeWindow?.Close();
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

