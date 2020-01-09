using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Microsoft.VisualBasic.Devices;
using SlonResourcesDiagnostika.Properties;

namespace SlonResourcesDiagnostika
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private NetworkInterface _selectedInterface;
        private string _ip, _status, _type;
        private decimal _dSpeed, _uSpeed, _receivedBytes, _sentBytes, _ramUsage, _cpuUsage, _hddSsdUsage;
        private readonly System.Windows.Threading.DispatcherTimer _dispatcherTimer = new System.Windows.Threading.DispatcherTimer();


        #region Proprties
        private PerformanceCounter _cpuCounter;
        private ComputerInfo _computerInfo;

        private IPv4InterfaceStatistics InterfaceData { get; set; }

        public AnalyzeData AnalyzeValues { get; set; }

        public List<NetworkInterface> Interfaces { get; private set; }

        public NetworkInterface SelectedInterface
        {
            get { return _selectedInterface; }
            set
            {
                //Populating IPv4 address
                if (value?.GetIPProperties().UnicastAddresses != null)
                {
                    UnicastIPAddressInformationCollection ipInfo = value.GetIPProperties().UnicastAddresses;

                    foreach (UnicastIPAddressInformation item in ipInfo)
                    {
                        if (item.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            Ip = item.Address.ToString();
                            break;
                        }
                    }
                }

                NetworkBandwidthCalculator(value);
                if (!_dispatcherTimer.IsEnabled) _dispatcherTimer.Start();
                _selectedInterface = value;
                OnPropertyChanged();
            }
        }

        public string Ip
        {
            get { return _ip; }
            set { _ip = value; OnPropertyChanged(); }
        }
        public string Status
        {
            get { return _status; }
            set { _status = value; OnPropertyChanged(); }
        }

        public string Type
        {
            get { return _type; }
            set { _type = value; OnPropertyChanged(); }
        }
        public decimal ReceivedBytes
        {
            get { return _receivedBytes.GmkByte().Normalize(); }
            set { _receivedBytes = value; OnPropertyChanged(); OnPropertyChanged("ReceivedUnit"); }
        }
        public string ReceivedUnit => _receivedBytes.GmkbUnit();

        public decimal SentBytes
        {
            get { return _sentBytes.GmkByte().Normalize(); }
            set { _sentBytes = value; OnPropertyChanged(); OnPropertyChanged("SentUnit"); }
        }
        public string SentUnit => _sentBytes.GmkbUnit();

        public decimal DSpeed
        {
            get { return _dSpeed.GmkByte().Normalize(); }
            set { _dSpeed = value; OnPropertyChanged(); OnPropertyChanged("DSpeedUnit"); if (TaskbarIcon) OnPropertyChanged("TaskBarTitle"); }
        }
        public string DSpeedUnit => _dSpeed.GmkbUnitPerSecond();

        public decimal USpeed
        {
            get { return _uSpeed.GmkByte().Normalize(); }
            set { _uSpeed = value; OnPropertyChanged(); OnPropertyChanged("USpeedUnit"); if (TaskbarIcon) OnPropertyChanged("TaskBarTitle"); }
        }
        public string USpeedUnit => _uSpeed.GmkbUnitPerSecond();

        public decimal CpuUsage
        {
            get { return _cpuUsage.Normalize(); }
            set { _cpuUsage = value; OnPropertyChanged(); if (TaskbarIcon) OnPropertyChanged("TaskBarTitle"); }
        }
        public string CpuUnit => "%";

        public decimal RamUsage
        {
            get { return _ramUsage.GmkByte().Normalize(); }
            set { _ramUsage = value; OnPropertyChanged(); OnPropertyChanged("RamUnit"); if (TaskbarIcon) OnPropertyChanged("TaskBarTitle"); }
        }
        public string RamUnit => _ramUsage.GmkbUnit();

        public decimal HddSsdUsage
        {
            get { return _hddSsdUsage.GmkByte().Normalize(); }
            set { _hddSsdUsage = value; OnPropertyChanged(); OnPropertyChanged("HddSsdUnit"); }
        }

        public string HddSsdUnit => "%";


        #region Window Properties

        public double WindowOpacity => Settings.Default.WindowOpacity;
        public double HeaderOpacity => WindowOpacity < 0.2 ? 0.2 : WindowOpacity;
        public Color WindowColor => Settings.Default.WindowColor;

        public bool AlwaysOnTop
        {
            get { return Settings.Default.AlwaysOnTop; }
            set
            {
                Settings.Default.AlwaysOnTop = value;
                Settings.Default.Save();
                OnPropertyChanged();
            }
        }
        public bool TaskbarIcon => Settings.Default.TaskbarIcon;

        #endregion

        #region Colors
        public Brush LableColor => new SolidColorBrush(Settings.Default.LableColor);

        public Brush IpColor => new SolidColorBrush(Settings.Default.IpColor);

        public Brush StatusColor => new SolidColorBrush(Settings.Default.StatusColor);

        public Brush ReceivedColor => new SolidColorBrush(Settings.Default.ReceivedColor);
        public Brush DColor => new SolidColorBrush(Settings.Default.DColor);

        public Brush SentColor => new SolidColorBrush(Settings.Default.SentColor);

        public Brush UColor => new SolidColorBrush(Settings.Default.UColor);

        public Brush CpuColor => new SolidColorBrush(Settings.Default.CpuColor);

        public Brush RamColor => new SolidColorBrush(Settings.Default.RamColor);

        public Brush GpuColor => new SolidColorBrush(Settings.Default.GpuColor);

        public Brush HddSsdColor => new SolidColorBrush(Settings.Default.HddSsdColor);

        #endregion

        #region Visibility
        public Visibility InterfaceVisibility => Visibility.Visible;
        public Visibility LableVisiblity => Visibility.Visible;
        public Visibility IpLableVisiblity => Visibility.Visible;
        public Visibility StatusLableVisiblity => Visibility.Visible;
        public Visibility ReceivedLableVisiblity => Visibility.Visible;
        public Visibility DSpeedLableVisiblity => Visibility.Visible;
        public Visibility SentLableVisiblity => Visibility.Visible;
        public Visibility USpeedLableVisiblity => Visibility.Visible;
        public Visibility CpuLabelVisibility => Visibility.Visible;
        public Visibility RamLableVisiblity => Visibility.Visible;
        public Visibility GpuLableVisibility => Visibility.Visible;
        public Visibility HddSsdLableVisiblity => Visibility.Visible;


        public Visibility IpVisibility => Visibility.Visible;
        public Visibility StatusVisibility => Visibility.Visible;
        public Visibility ReceivedVisibility => Visibility.Visible;
        public Visibility DSpeedVisibility => Visibility.Visible;
        public Visibility SentVisibility => Visibility.Visible;
        public Visibility USpeedVisibility => Visibility.Visible;
        public Visibility CpuVisibility => Visibility.Visible;
        public Visibility RamVisibility => Visibility.Visible;
        public Visibility GpuVisibility => Visibility.Visible;
        public Visibility HddSsdVisibility => Visibility.Visible;

        #endregion

        #endregion

        #region Methods
        public MainWindow()
        {
            AnalyzeValues = new AnalyzeData
            {
                ReceivedBytesMax = 0,
                CpuUsageMax = 0,
                DSpeedMax = 0,
                HddSsdUsageMax = 0,
                RamUsageMax = 0,
                SentBytesMax = 0,
                USpeedMax = 0
            };
            InitializeNetworkMeter();
            // Set the timer
            _dispatcherTimer.Tick += DispatcherTimer_Tick;
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);

            InitializeComponent();
            InitializeCpuRam();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Left = SystemParameters.PrimaryScreenWidth - Width;
            Top = 0;
        }

        private void InitializeNetworkMeter()
        {
            Interfaces = new List<NetworkInterface>();
            // Detecting Network Adaptors Using 
            List<NetworkInterface> nics = NetworkInterface.GetAllNetworkInterfaces().Where(network =>
              network.OperationalStatus == OperationalStatus.Up &&
              (network.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
               network.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)).ToList();
            // If none is active get all of them :)
            if (nics.Count == 0)
                nics = NetworkInterface.GetAllNetworkInterfaces().Where(network =>
              (network.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
               network.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)).ToList();

            // Add Items To the combo item source
            nics.ForEach(i => Interfaces.Add(i));
            //Set the selected interface to the first one
            if (Interfaces.Count > 0)
                SelectedInterface = Interfaces.First();
            OnPropertyChanged("Interfaces");
        }

        private void InitializeCpuRam()
        {
            //PrintPerformanceCounterParameters();
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _computerInfo = new ComputerInfo();
        }

        private void HddSsdCalc()
        {
            ulong sumTotalFreeSpace = 0;
            ulong sumTotalSize = 0;

            foreach (var drive in DriveInfo.GetDrives())
            {
                try
                {
                    sumTotalFreeSpace = sumTotalFreeSpace + (ulong)drive.TotalFreeSpace;
                    sumTotalSize = sumTotalSize + (ulong)drive.TotalSize;
                }
                catch
                {
                    // ignored
                }
            }
            HddSsdUsage = Math.Round((decimal)sumTotalFreeSpace / sumTotalSize, 2);
            if (AnalyzeValues.HddSsdUsageMax < HddSsdUsage)
            {
                AnalyzeValues.HddSsdUsageMax = HddSsdUsage;
            }
        }

        private void NetworkBandwidthCalculator(NetworkInterface selecNic)
        {
            if (selecNic == null)
                return;
            //Setting General Information 
            Type = selecNic.NetworkInterfaceType.ToString();
            Status = Helper.GetRussianNicStatusAnalog(selecNic);


            InterfaceData = selecNic.GetIPv4Statistics();

            //todo: this may fail if the BytesReceived is more than 8 GB
            long bytesRecivedSpeedValue = (long)(InterfaceData.BytesReceived - _receivedBytes);
            DSpeed = bytesRecivedSpeedValue;//Download speed
            ReceivedBytes = InterfaceData.BytesReceived;
            if (AnalyzeValues.DSpeedMax < DSpeed)
            {
                AnalyzeValues.DSpeedMax = DSpeed;
            }
            if (AnalyzeValues.ReceivedBytesMax < ReceivedBytes)
            {
                AnalyzeValues.ReceivedBytesMax = ReceivedBytes;
            }
            long bytesSentSpeedValue = (long)(InterfaceData.BytesSent - _sentBytes);
            USpeed = bytesSentSpeedValue;//Upload Speed
            SentBytes = InterfaceData.BytesSent;
            if (AnalyzeValues.USpeedMax < USpeed)
            {
                AnalyzeValues.USpeedMax = USpeed;
            }
            if (AnalyzeValues.SentBytesMax < SentBytes)
            {
                AnalyzeValues.SentBytesMax = SentBytes;
            }
        }
        private void CpuRamCalc()
        {
            CpuUsage = (decimal)_cpuCounter.NextValue();
            if (AnalyzeValues.CpuUsageMax < CpuUsage)
            {
                AnalyzeValues.CpuUsageMax = CpuUsage;
            }
            RamUsage = _computerInfo.TotalPhysicalMemory - _computerInfo.AvailablePhysicalMemory;
            if (AnalyzeValues.RamUsageMax < RamUsage)
            {
                AnalyzeValues.RamUsageMax = RamUsage;
            }
        }

        #endregion

        #region Events
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            NetworkBandwidthCalculator(SelectedInterface);
            CpuRamCalc();
            HddSsdCalc();
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Contact_Click(object sender, RoutedEventArgs e)
        {
            var wind = new ContactWindow();
            Close();
            wind.ShowDialog();
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            var wind = new TestWindow();
            Hide();
            wind.ShowDialog();
        }
        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                DragMove();
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

    public static class Helper
    {
        public static decimal Normalize(this decimal value)
        {
            value = Math.Round(value, Settings.Default.Decimals);
            return value / 1.000000000000000000000000000000000m;
        }
        public static decimal GmkByte(this decimal value)
        {
            return value > 1073741824 ? value / 1073741824 : (value > 1048576 ? value / 1048576 : (value > 1024 ? value / 1024 : value));
        }
        public static string GmkbUnit(this decimal value)
        {
            return value > 1073741824 ? "Гб" : (value > 1048576 ? "Мб" : (value > 1024 ? "Кб" : "Байт"));
        }
        public static string GmkbUnitPerSecond(this decimal value)
        {
            return value > 1073741824 ? "Гб/с" : (value > 1048576 ? "Мб/с" : (value > 1024 ? "Кб/с" : "Б/с"));
        }

        public static string GetRussianNicStatusAnalog(NetworkInterface ni)
        {
            switch (ni.OperationalStatus)
            {
                case OperationalStatus.Up:
                    return "Включен";
                case OperationalStatus.Down:
                    return "Выключен";
                case OperationalStatus.Testing:
                    return "Тестирование";
                case OperationalStatus.Unknown:
                    return "Не известен";
                case OperationalStatus.Dormant:
                    return "Бездействующий";
                case OperationalStatus.NotPresent:
                    return "Отсутствует компонет";
                case OperationalStatus.LowerLayerDown:
                    return "Другой сетевой компонент не доступен";
                default:
                    return "Не определен";
            }
        }
    }
    public class AnalyzeData
    {
        public  decimal DSpeedMax;
        public  decimal USpeedMax;
        public  decimal ReceivedBytesMax;
        public  decimal SentBytesMax;
        public  decimal RamUsageMax;
        public  decimal CpuUsageMax;
        public  decimal HddSsdUsageMax;
    }

}
