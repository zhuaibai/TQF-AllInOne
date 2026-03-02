using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;
using WpfApp1.Command;
using WpfApp1.Models;

namespace WpfApp1.ViewModels
{
    /// <summary>
    /// 串口设置视图模型
    /// 负责管理串口配置参数的加载、保存和绑定
    /// 实现INotifyPropertyChanged接口用于属性变更通知
    /// </summary>
    public class SerialPortSettingViewModel : INotifyPropertyChanged
    {
        #region 事件和字段
        /// <summary>
        /// 属性变更事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 串口设置对象，存储当前配置
        /// </summary>
        private SerialPortSettings _settings;
        private bool _isLoading = false; // 标记是否正在加载设置
        #endregion

        #region 属性 - 可选项集合
        /// <summary>
        /// 可用的串口列表（从系统获取）
        /// </summary>
        public ObservableCollection<string> AvailablePorts { get; }
        /// <summary>
        /// 波特率集合
        /// </summary>
        public ObservableCollection<int> BaudRates { get; }
        /// <summary>
        /// 数据位集合
        /// </summary>
        public ObservableCollection<int> DataBits { get; }
        /// <summary>
        /// 停止位集合
        /// </summary>
        public ObservableCollection<StopBits> StopBitsOptions { get; }
        /// <summary>
        /// 校验位集合
        /// </summary>
        public ObservableCollection<Parity> ParityOptions { get; }
        #endregion

        #region 属性 - 当前选择项（双向绑定）
        /// <summary>
        /// 当前选择的串口名称
        /// 绑定到UI的下拉列表框
        /// </summary>
        public string SelectedPort
        {
            get { return _settings.PortName; }
            set
            {
                if (_settings.PortName != value)
                {
                    _settings.PortName = value;
                    OnPropertyChanged(nameof(SelectedPort));
                    // 如果不是正在加载设置，则自动保存
                    if (!_isLoading)
                    {
                        SaveSettingsSilently(); // 静默保存，不显示提示
                    }
                }
            }
        }


        /// <summary>
        /// 当前选择的波特率
        /// </summary>
        public int SelectedBaudRate
        {
            get { return _settings.BaudRate; }
            set
            {
                if (_settings.BaudRate != value)
                {
                    _settings.BaudRate = value;
                    OnPropertyChanged(nameof(SelectedBaudRate));

                }
            }
        }

        // <summary>
        /// 当前选择的数据位
        /// </summary>
        public int SelectedDataBits
        {
            get { return _settings.DataBits; }
            set
            {
                if (_settings.DataBits != value)
                {
                    _settings.DataBits = value;
                    OnPropertyChanged(nameof(SelectedDataBits));

                }
            }
        }

        /// <summary>
        /// 当前选择的停止位
        /// </summary>
        public StopBits SelectedStopBits
        {
            get { return _settings.StopBits; }
            set
            {
                if (_settings.StopBits != value)
                {
                    _settings.StopBits = value;
                    OnPropertyChanged(nameof(SelectedStopBits));

                }
            }
        }

        /// <summary>
        /// 当前选择的校验位
        /// </summary>
        public Parity SelectedParity
        {
            get { return _settings.Parity; }
            set
            {
                if (_settings.Parity != value)
                {
                    _settings.Parity = value;
                    OnPropertyChanged(nameof(SelectedParity));
                }
            }
        }
        #endregion

        /// <summary>
        /// 保存设置命令
        /// 绑定到UI的保存按钮
        /// </summary>
        public ICommand SaveCommand { get; }

        #region 构造函数
        public SerialPortSettingViewModel()
        {
            //  标记为正在加载
            _isLoading = true;

            //  加载已保存的设置（从XML文件）
            _settings = LoadSettings();

            // 初始化可选项集合
            AvailablePorts = new ObservableCollection<string>(SerialPort.GetPortNames());
            BaudRates = new ObservableCollection<int> { 2400, 9600, 115200 };
            DataBits = new ObservableCollection<int> { 7, 8 };
            StopBitsOptions = new ObservableCollection<StopBits> { StopBits.One, StopBits.Two };
            ParityOptions = new ObservableCollection<Parity> { Parity.None, Parity.Even, Parity.Odd };

            //  标记加载完成
            _isLoading = false;

            //  初始化保存命令
            SaveCommand = new DelegateCommand(SaveSettings);
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 刷新可用串口列表
        /// </summary>
        public void RefreshAvailablePorts()
        {
            AvailablePorts.Clear();
            var ports = SerialPort.GetPortNames();

            foreach (var port in ports)
            {
                AvailablePorts.Add(port);
            }
        }
        /// <summary>
        /// 获取当前串口设置
        /// </summary>
        public SerialPortSettings GetCurrentSettings()
        {
            return new SerialPortSettings
            {
                PortName = _settings.PortName,
                BaudRate = _settings.BaudRate,
                DataBits = _settings.DataBits,
                StopBits = _settings.StopBits,
                Parity = _settings.Parity
            };
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 主界面下拉框 静默保存
        /// </summary>
        private void SaveSettingsSilently()
        {
            try
            {
                // 使用XML序列化器序列化设置对象
                var serializer = new XmlSerializer(typeof(SerialPortSettings));
                using (var writer = new StreamWriter("serialSettings.xml"))
                {
                    serializer.Serialize(writer, _settings);
                }
                // 控制台输出和用户提示
                Console.WriteLine("设置已自动保存");

            }
            catch (Exception ex)
            {
                // 保存失败时的错误处理
                Console.WriteLine($"保存设置时出错: {ex.Message}");

            }
        }

        /// <summary>
        /// 加载串口设置
        /// 从XML文件反序列化，如果文件不存在或加载失败则返回默认设置
        /// </summary>
        /// <returns>加载的串口设置或默认设置</returns>
        private SerialPortSettings LoadSettings()
        {
            try
            {
                // 检查设置文件是否存在
                if (File.Exists("serialSettings.xml"))
                {
                    // 使用XML序列化器反序列化
                    var serializer = new XmlSerializer(typeof(SerialPortSettings));
                    using (var reader = new StreamReader("serialSettings.xml"))
                    {
                        return (SerialPortSettings)serializer.Deserialize(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                // 加载失败时记录错误，返回默认设置 
                Console.WriteLine($"加载设置时出错: {ex.Message}");
                return new SerialPortSettings();
            }
            return new SerialPortSettings();
        }

        /// <summary>
        /// 保存设置到XML文件
        /// </summary>
        /// <param name="parameter">命令参数（未使用）</param>
        private void SaveSettings(object parameter)
        {
            try
            {
                // 使用XML序列化器序列化设置对象
                var serializer = new XmlSerializer(typeof(SerialPortSettings));
                using (var writer = new StreamWriter("serialSettings.xml"))
                {
                    serializer.Serialize(writer, _settings);
                }
                // 控制台输出和用户提示
                Console.WriteLine("设置已保存");
                MessageBox.Show("设置已保存,请重新打开串口生效!\r\n The settings have been saved, please re-open the serial port to take effect!");
            }
            catch (Exception ex)
            {
                // 保存失败时的错误处理
                Console.WriteLine($"保存设置时出错: {ex.Message}");
                MessageBox.Show($"保存设置时出错:{ex.Message}");
            }
        }
        #endregion



    }
}
