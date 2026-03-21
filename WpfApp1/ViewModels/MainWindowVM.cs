using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Serialization;
using WpfApp1.Command;
using WpfApp1.Command.BMS;
using WpfApp1.Command.Comand_GB3024;
using WpfApp1.Command.Command_CYJ;
using WpfApp1.Command.Command_LB6;
using WpfApp1.Command.Command_PDF302;
using WpfApp1.Command.Command_PDF3024;
using WpfApp1.Command.Command_VQ3024;
using WpfApp1.Command.GB_General;
using WpfApp1.Convert;
using WpfApp1.CustomMessageBox;
using WpfApp1.CustomMessageBox.Service;
using WpfApp1.Models;
using WpfApp1.Services;
using WpfApp1.UserControls;
using static System.Runtime.InteropServices.JavaScript.JSType;
using InputType = WpfApp1.CustomMessageBox.InputType;

namespace WpfApp1.ViewModels
{
    public class MainWindowVM : BaseViewModel
    {

        /// <summary>
        /// 重构函数
        /// </summary>
        /// <param name="messageService"></param>
        public MainWindowVM(IMessageDialogService messageService)
        {
            #region 日志界面
            // 初始化命令
            ClearLogCommand = new DelegateCommand(ClearLog);
            SaveLogCommand = new DelegateCommand(SaveLog);
            AddLogCommand = new DelegateCommand(AddMessage);
            // 示例：添加一些初始日志
            AddLog("程序启动");

            #endregion

            #region 后台线程
            StartCommand = new RelayCommand(StartBackgroundThread);
            StopCommand = new RelayCommand(StopBackgroundThread);

            // 创建串口实例
            _serialPortSetting = new SerialPortSettingViewModel();
            //初始化串口信息
            IniCom();
            OpenCom = new RelayCommand(openCom);

            //发送帧，接收帧
            _SerialCountVM = new SerialCountVM();
            //绑定发送接收帧计数委托
            SerialCommunicationService.AddReceiveFrame = SerialCountVM.AddReceiveFrame;
            SerialCommunicationService.AddSendFrame = SerialCountVM.AddSendFrame;

            //BMS
            BMS_Command_Setting = new SendingCommandSettingsViewModel(_pauseEvent, _semaphore, AddLog, UpdateState);

            //初始化  GB   ViewModel
            hopVm = new HOPViewModel(_pauseEvent, _semaphore, AddLog, UpdateState);
            hEEP1_ViewModel = new HEEP1_ViewModel(_pauseEvent, _semaphore, AddLog, UpdateState);
            _HEEP1_HPVIN02 = new HEEP1_HPVINV02_ViewModel(_pauseEvent, _semaphore, AddLog, UpdateState);
            hEEP2_ViewModel = new HEEP2_ViewModel(_pauseEvent, _semaphore, AddLog, UpdateState);
            _HGEN = new HGEN_ViewModel(_pauseEvent, _semaphore, AddLog, UpdateState);
            _HGRID_GB = new HGRID_GB_ViewModel(_pauseEvent, _semaphore, AddLog, UpdateState);
            _HSTS_GB = new HSTS_GB_ViewModel(_pauseEvent, _semaphore, AddLog, UpdateState);
            special_Command = new Special_Command(_pauseEvent, _semaphore, AddLog, UpdateState);
            _HPVB_GB = new HPVB_GB_ViewModel(_pauseEvent, _semaphore, AddLog, UpdateState);
            _HSTS2_HPVINV08 = new HSTS2_HPVINV08_ViewModel(_pauseEvent, _semaphore, AddLog, UpdateState);

            //初始化  VQ   ViewModel
            _HOP_VQ = new HOP_VQ_ViewModel(_pauseEvent, _semaphore, AddLog, UpdateState);
            _HBMS1_VQ = new HBMS1_VQ_ViewModel(_pauseEvent, _semaphore, AddLog, UpdateState);
            _HBAT_VQ = new HBAT_GB_ViewModel(_pauseEvent, _semaphore, AddLog, UpdateState);
            _HEEP1_VQ = new HEEP1_VQ_ViewModel(_pauseEvent, _semaphore, AddLog, UpdateState);
            _HGRID_VQ = new HGRID_VQ_ViewModel(_pauseEvent, _semaphore, AddLog, UpdateState);
            _HBAT_VQ2 = new HBAT_VQ_ViewModel(_pauseEvent, _semaphore, AddLog, UpdateState);

            //初始化  VDF ViewModel
            _HBAT_PDF = new HBAT_PDF_ViewModel(_pauseEvent, _semaphore, AddLog, UpdateState);
            _HCTMSG1_PDF = new HCTMSG1_PDF_ViewModel(_pauseEvent, _semaphore, AddLog, UpdateState);
            _HGRID_PDF = new HGRID_PDF_ViewModel(_pauseEvent, _semaphore, AddLog, UpdateState);
            _HEEP1_PDF = new HEEP1_PDF_ViewModel(_pauseEvent, _semaphore, AddLog, UpdateState);
            _HEEP3_PDF = new HEEP3_PDF_ViewModel(_pauseEvent, _semaphore, AddLog, UpdateState);
            _HIMSG1 = new HIMSG1_PDF_ViewModel(_pauseEvent, _semaphore, AddLog, UpdateState);
            _HOP_PDF = new HOP_PDF_ViewModel(_pauseEvent, _semaphore, AddLog, UpdateState);
            _HPV_PDF = new HPV_PDF_ViewModel(_pauseEvent, _semaphore, AddLog, UpdateState);
            _HTEMP_PDF = new HTEMP_PDF_ViewModel(_pauseEvent, _semaphore, AddLog, UpdateState);
            _HIGSG2_PDF = new HIMSG2_PDF_ViewModel(_pauseEvent, _semaphore, AddLog, UpdateState);

            //初始化 CYJ ViewModel
            _HGRID_CYJ = new HGRID_CYJ_ViewModel(_pauseEvent, _semaphore, AddLog, UpdateState);
            _HEEP1_CYJ = new HEEP1_CYJ_ViewModel(_pauseEvent, _semaphore, AddLog, UpdateState);
            _HSTS_CYJ = new HSTS_CYJ_ViewModel(_pauseEvent, _semaphore, AddLog, UpdateState);
            //初始化实时时间ViewModel
            _Clock = new ClockViewModel();
            //初始化 LB6 ViewModel
            _HEEP1_LB6 = new HEEP1_LB6_ViewModel(_pauseEvent, _semaphore, AddLog, UpdateState);

            #endregion

            //消息框初始化
            _messageService = messageService;
            ShowMessageCommand = new RelayCommand(OnShowMessage);
            BMS02 = new BMS_UserControl();
            BMS01 = new BMS01_UserControl();
            BMS03 = new BMS03_UserControl();
            ModbusRTU.showStatue = UpdateState;
            //// 模拟电量变化
            //var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3)};
            //timer.Tick += (s, e) =>
            //{
            //    BMS_VM.SOC = (BMS_VM.SOC + 10) % 110;
            //};
            //timer.Start();

            //实时监控列表实例化
            RT_Monitor.PollingList = new ObservableCollection<PollingData>();
            //实例化数据记录列表
            DR_Monitor.CommonDataList = new ObservableCollection<Common_Data>();
            //并联监控实例化
            uinonVN = new UnionMonitorVM();

            // 初始化ComboBox的可用状态
            UpdateComboBoxEnabledState();

            App.ChangeLanguageWithSetting = RefleshSettingParamToLanguage;
        }


        private SerialPortSettingViewModel _serialPortSetting;

        public SerialPortSettingViewModel SerialPortSetting
        {
            get
            {
                return _serialPortSetting;
            }
            set
            {
                _serialPortSetting = value;
                OnPropertyChanged(nameof(SerialPortSetting));
            }
        }

        private bool _isEnableComboBox = true;
        public bool IsEnableComboBox
        {
            get
            {
                return _isEnableComboBox;
            }
            set
            {
                _isEnableComboBox = value;
                OnPropertyChanged(nameof(IsEnableComboBox));
            }
        }

        private void UpdateComboBoxEnabledState()
        {
            IsEnableComboBox = !SerialCommunicationService.IsOpen();
        }

        #region 小标题选项
        public enum BatteryMode
        {
            Mode1,
            Mode2,
            Mode3,
            Mode4,
            Mode5,
            Mode6,
            Mode7
        }

        private BatteryMode _selectedMode = BatteryMode.Mode1;

        public BatteryMode SelectedMode
        {
            get => _selectedMode;
            set
            {
                _selectedMode = value;
                OnPropertyChanged(nameof(SelectedMode));
            }
        }
        #endregion

        #region BMS

        //bmsViewModel
        private ModbusAddr _BMS_VM = new ModbusAddr();

        public ModbusAddr BMS_VM
        {
            get { return _BMS_VM; }
            set
            {
                _BMS_VM = value;
                this.RaiseProperChanged(nameof(BMS_VM));
            }
        }

        //130-220数据项的读取、写入与显示
        private SendingCommandSettingsViewModel BMS_Command_Setting;

        public SendingCommandSettingsViewModel BMS_Setting
        {
            get { return BMS_Command_Setting; }
            set
            {
                BMS_Command_Setting = value;
                this.RaiseProperChanged(nameof(BMS_Setting));
            }
        }

        //实时监控VM
        private RealTimeMonitorVM rtMonitor = new RealTimeMonitorVM();

        public RealTimeMonitorVM RT_Monitor
        {
            get { return rtMonitor; }
            set
            {
                rtMonitor = value;
                this.RaiseProperChanged(nameof(RT_Monitor));
            }
        }

        #region 数据记录VM

        private DataRecrodingVM _DR_Monitor = new DataRecrodingVM();

        public DataRecrodingVM DR_Monitor
        {
            get { return _DR_Monitor; }
            set
            {
                _DR_Monitor = value;
                this.RaiseProperChanged(nameof(DR_Monitor));
            }
        }
        #endregion

        private UnionMonitorVM uinonVN;

        public UnionMonitorVM UnionVM
        {
            get { return uinonVN; }
            set
            {
                uinonVN = value;
                this.RaiseProperChanged(nameof(UnionVM));
            }
        }

        //管理员权限
        private bool administration = false;

        public bool Administration
        {
            get
            {
                return administration;
            }
            set
            {
                administration = value;
                this.RaiseProperChanged(nameof(Administration));
            }
        }

        //刷新用户参数
        public RelayCommand FleshSettingCommand { get { return new RelayCommand(RefleshSettingParam); } }

        #endregion

        #region 图标百分比

        //逆变总功率百分比
        private int _PercentValue;

        public int InvTotalPwr
        {
            get { return _PercentValue; }
            set
            {
                _PercentValue = value;
                this.RaiseProperChanged(nameof(InvTotalPwr));
            }
        }

        /// <summary>
        /// MPPT总功率百分比
        /// </summary>
        private int _MPPTTotalPwr;

        public int MPPTTotalPwr
        {
            get { return _MPPTTotalPwr; }
            set
            {
                _MPPTTotalPwr = value;
                RaiseProperChanged(nameof(MPPTTotalPwr));
            }
        }

        /// <summary>
        /// 市电总功率百分比
        /// </summary>
        private int _ACTotalPwr;

        public int ACTotalPwr
        {
            get { return _ACTotalPwr; }
            set
            {
                _ACTotalPwr = value;
                RaiseProperChanged(nameof(ACTotalPwr));
            }
        }

        /// <summary>
        /// 电池百分比
        /// </summary>
        private int _BattPercent;

        public int BattPercent
        {
            get { return _BattPercent; }
            set
            {
                _BattPercent = value;
                this.RaiseProperChanged(nameof(BattPercent));
            }
        }


        /// <summary>
        /// 市电功率
        /// </summary>
        private int _ACPower;
        public int ACPowerVM
        {
            get { return _ACPower; }
            set
            {
                _ACPower = value;
                this.RaiseProperChanged(nameof(ACPowerVM));
            }
        }

        /// <summary>
        /// 把数字字符串转换成整数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private int StringToIntConversion(string str)
        {
            if (str == null)
            {
                return 0;
            }
            //这是专门应对市电功率的情况
            string tag = str.Substring(0, 1);
            if (tag == "-")
            {
                str = str.Substring(1);
            }
            else if (tag == "+")
            {
                str = str.Substring(1);
            }

            if (int.TryParse(str, out int number))
            {
                Console.WriteLine($"转换后的整数: {number}");
                return number;
            }
            else
            {
                Console.WriteLine("输入的字符串不是有效的整数格式。");
                return 0;
            }
        }

        /// <summary>
        /// 两数相除，返回百分比
        /// </summary>
        /// <param name="front"></param>
        /// <param name="after"></param>
        /// <returns></returns>
        private int CountPercent(string front, string after)
        {
            int newFront = StringToIntConversion(front);
            int newAfter = StringToIntConversion(after);
            if (newAfter <= 0 || newAfter <= 0)
            {
                return 0;
            }
            else
            {
                return (newFront * 100) / newAfter;
            }
        }


        #endregion

        #region 抗干扰功能

        /// <summary>
        /// 抗干扰按钮开关切换
        /// </summary>
        public ICommand SwitchOpenReceiveCRC
        {
            get
            {
                return new RelayCommand(SwitchCRCReceive);
            }

        }
        //按钮是否打开
        private bool _isChecked;

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                this.RaiseProperChanged(nameof(IsChecked));
            }
        }

        bool OnceOpenCRC = false;
        /// <summary>
        /// 打开或关闭抗干扰功能
        /// </summary>
        /// <param name="parameter"></param>
        private void SwitchCRCReceive()
        {
            // 根据 ToggleButton 的选中状态执行相应逻辑
            if (IsChecked)
            {
                //处理选中状态
                OnceOpenCRC = true;
                SerialCommunicationService.OpenReceiveCRC(true);
                //发送HOSTCRCEN
                Task.Run(new Action(() => SpecialCommand.AntiJamModeOperation(true)));
            }
            else
            {
                //发送HOSTCRCDN
                Task.Run(new Action(() => SpecialCommand.AntiJamModeOperation(false)));
                SerialCommunicationService.OpenReceiveCRC(false);
            }
        }

        #endregion

        #region 下拉框选择机器类型

        private string? _selectedItem = "HPVINV02";
        public string? SelectedMachineItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        public BMS_UserControl BMS02 { get; set; }
        public BMS01_UserControl BMS01 { get; set; }
        public BMS03_UserControl BMS03 { get; set; }
        public ObservableCollection<string> MachineItems { get; } = new(){
        "HPVINV02",
        "HPVINV04",
        "HPVINV06",
        "HPVINV07",
        "HPVINV08",
        "LPVINV02",
        "UPSCYX01",
        "LB6",
        "BMS01",
        "BMS02",
        "BMS03"
         };

        //切换指令
        public ICommand SelectionChangedCommand
        {
            get
            {
                return new RelayCommand(OnSelectionChanged);
            }
        }

        //选中项改变
        private void OnSelectionChanged()
        {


            // 如果需要，可以在这里添加业务逻辑
            if (SelectedMachineItem != null)
            {
                // 执行你的命令逻辑
                SwitchViewToVQorGB(SelectedMachineItem);
            }
        }

        /// <summary>
        /// 切换界面到VQ
        /// </summary>
        public void SwitchViewToVQorGB(string view)
        {
            switch (view)
            {
                case "HPVINV04":
                    ContentUC = new GB6042();
                    SelectedMachineItem = "HPVINV04";
                    break;
                case "HPVINV02":
                    ContentUC = new GB_MonitorUC();
                    SelectedMachineItem = "HPVINV02";
                    break;
                case "LPVINV02":
                    ContentUC = new VQ_Monitor();
                    SelectedMachineItem = "LPVINV02";
                    break;
                case "HPVINV06":
                    ContentUC = new HPVINV06_MonitorUC();
                    SelectedMachineItem = "HPVINV06";
                    break;
                case "HPVINV07":
                    ContentUC = new PTF_Monitor();
                    SelectedMachineItem = "HPVINV07";
                    break;
                case "HPVINV08":
                    ContentUC = new HPVINV08_MonitorUC();
                    SelectedMachineItem = "HPVINV08";
                    break;
                case "UPSCYX01":
                    ContentUC = new CYJ_MonitorUC();
                    SelectedMachineItem = "UPSCYX01";
                    break;
                case "LB6":
                    ContentUC = new LB6_MonitorUC();
                    SelectedMachineItem = "LB6";
                    break;
                case "BMS01":
                    ContentUC = BMS01;
                    SelectedMachineItem = "BMS01";
                    break;
                case "BMS02":
                    ContentUC = BMS02;
                    SelectedMachineItem = "BMS02";
                    break;
                case "BMS03":
                    ContentUC = BMS03;
                    SelectedMachineItem = "BMS03";
                    break;
            }
        }

        /// <summary>
        /// 自动获取机型
        /// </summary>
        private bool AutoSelectedMachineType(out string machine)
        {
            string receive_MachineType = SerialCommunicationService.SendCommand(SpecialCommand.QueryMachineType, 10);
            if (receive_MachineType.Length >= 2 && receive_MachineType.StartsWith("-1"))
            {
                //判断抗干扰是否打开
                if (IsChecked)
                {
                    IsChecked = false;
                    OnceOpenCRC = false;
                    SerialCommunicationService.OpenReceiveCRC(false);
                    receive_MachineType = SerialCommunicationService.SendCommand(SpecialCommand.QueryMachineType, 10);
                }
            }

            SerialCommunicationService.MachineType = receive_MachineType;
            if (receive_MachineType.Length >= 9)
            {
                if (receive_MachineType.Substring(0, 9) == "(HPVINV07")
                {
                    //切换到PTF界面
                    SwitchViewToVQorGB("HPVINV07");
                    //默认设置抗干扰模式
                    IsChecked = true;
                    OnceOpenCRC = true;
                    SerialCommunicationService.OpenReceiveCRC(true);
                    //返回机器类型
                    machine = receive_MachineType;
                    return true;
                }
                else if (receive_MachineType.Substring(0, 9) == "(HPVINV02")
                {
                    SwitchViewToVQorGB("HPVINV02");
                    //默认设置抗干扰模式
                    IsChecked = true;
                    OnceOpenCRC = true;
                    SerialCommunicationService.OpenReceiveCRC(true);
                    //返回机器类型
                    machine = receive_MachineType;
                    return true;
                }
                else if (receive_MachineType.Substring(0, 9) == "(LPVINV02")
                {
                    SwitchViewToVQorGB("LPVINV02");
                    //默认设置抗干扰模式
                    IsChecked = true;
                    OnceOpenCRC = true;
                    SerialCommunicationService.OpenReceiveCRC(true);
                    //返回机器类型
                    machine = receive_MachineType;
                    return true;
                }
                else if (receive_MachineType.Substring(0, 9) == "(HPVINV04")
                {
                    SwitchViewToVQorGB("HPVINV04");
                    //默认设置抗干扰模式
                    IsChecked = true;
                    OnceOpenCRC = true;
                    SerialCommunicationService.OpenReceiveCRC(true);
                    //返回机器类型
                    machine = receive_MachineType;
                    return true;
                }
                else if (receive_MachineType.Substring(0, 9) == "(HPVINV06")
                {
                    SwitchViewToVQorGB("HPVINV06");
                    //默认设置抗干扰模式
                    IsChecked = true;
                    OnceOpenCRC = true;
                    SerialCommunicationService.OpenReceiveCRC(true);
                    //返回机器类型
                    machine = receive_MachineType;
                    return true;
                }
                else if (receive_MachineType.Substring(0, 9) == "(HPVINV08")
                {
                    SwitchViewToVQorGB("HPVINV08");
                    //默认设置抗干扰模式
                    IsChecked = true;
                    OnceOpenCRC = true;
                    SerialCommunicationService.OpenReceiveCRC(true);
                    //返回机器类型
                    machine = receive_MachineType;
                    return true;
                }
                else if (receive_MachineType.Substring(0, 9) == "(UPSCYX01")
                {
                    SwitchViewToVQorGB("UPSCYX01");
                    //默认设置抗干扰模式
                    IsChecked = true;
                    OnceOpenCRC = true;
                    SerialCommunicationService.OpenReceiveCRC(true);
                    //返回机器类型
                    machine = receive_MachineType;
                    return true;
                }
                else if ((receive_MachineType.Substring(0, 9) == "LB6"))
                {
                    SwitchViewToVQorGB("LB6");
                    //默认设置抗干扰模式
                    IsChecked = true;
                    OnceOpenCRC = true;
                    SerialCommunicationService.OpenReceiveCRC(true);
                    //返回机器类型
                    machine = receive_MachineType;
                    return true;
                }
                else if ((receive_MachineType.Substring(0, 9) == "(BMS00001"))
                {
                    SwitchViewToVQorGB("BMS01");
                    //默认设置抗干扰模式
                    //IsChecked = true;
                    //OnceOpenCRC = true;
                    //SerialCommunicationService.OpenReceiveCRC(true);
                    //返回机器类型
                    machine = receive_MachineType;
                    return true;
                }

                else if ((receive_MachineType.Substring(0, 9) == "(BMS00002"))
                {
                    SwitchViewToVQorGB("BMS02");
                    //默认设置抗干扰模式
                    //IsChecked = true;
                    //OnceOpenCRC = true;
                    //SerialCommunicationService.OpenReceiveCRC(true);
                    //返回机器类型
                    machine = receive_MachineType;
                    return true;
                }
                else if ((receive_MachineType.Substring(0, 9) == "(BMS00003"))
                {
                    SwitchViewToVQorGB("BMS03");
                    //默认设置抗干扰模式
                    //IsChecked = true;
                    //OnceOpenCRC = true;
                    //SerialCommunicationService.OpenReceiveCRC(true);
                    //返回机器类型
                    machine = receive_MachineType;
                    return true;
                }
                else
                {
                    //返回机器类型(无法识别
                    machine = receive_MachineType;
                    return false;
                }
            }
            else
            {
                machine = receive_MachineType;
                return false;
            }
        }
        #endregion

        #region 实时时间
        /// <summary>
        /// 实时时间
        /// </summary>
        private ClockViewModel _Clock;

        public ClockViewModel Clock
        {
            get { return _Clock; }
            set
            {
                _Clock = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region GB指令ViewModel

        private HOPViewModel hopVm;
        public HOPViewModel HOP
        {
            get { return hopVm; }
            set
            {
                hopVm = value;
                this.RaiseProperChanged(nameof(HOP));
            }
        }

        private HEEP1_ViewModel hEEP1_ViewModel;

        public HEEP1_ViewModel HEEP1
        {
            get { return hEEP1_ViewModel; }
            set
            {
                hEEP1_ViewModel = value;
                this.RaiseProperChanged(nameof(HEEP1));
            }
        }

        private HEEP1_HPVINV02_ViewModel _HEEP1_HPVIN02;
        public HEEP1_HPVINV02_ViewModel HEEP1_HPVINV02
        {
            get { return _HEEP1_HPVIN02; }
            set
            {
                _HEEP1_HPVIN02 = value;
                this.RaiseProperChanged(nameof(HEEP1_HPVINV02));
            }
        }

        private HEEP2_ViewModel hEEP2_ViewModel;

        public HEEP2_ViewModel HEEP2
        {
            get { return hEEP2_ViewModel; }
            set
            {
                hEEP2_ViewModel = value;
                this.RaiseProperChanged(nameof(HEEP2));
            }
        }

        private HGEN_ViewModel _HGEN;

        public HGEN_ViewModel HGEN
        {
            get { return _HGEN; }
            set { _HGEN = value; }
        }


        private Special_Command special_Command;

        public Special_Command SpecialCommand
        {
            get { return special_Command; }
            set
            {
                special_Command = value;
                this.RaiseProperChanged(nameof(SpecialCommand));
            }
        }

        private HGRID_GB_ViewModel _HGRID_GB;

        public HGRID_GB_ViewModel HGRID_GB
        {
            get { return _HGRID_GB; }
            set
            {
                _HGRID_GB = value;
                this.RaiseProperChanged(nameof(HGRID_GB));
            }
        }

        private HSTS_GB_ViewModel _HSTS_GB;

        public HSTS_GB_ViewModel HSTS_GB
        {
            get { return _HSTS_GB; }
            set
            {
                _HSTS_GB = value;
                this.RaiseProperChanged(nameof(HSTS_GB));
            }
        }

        private HPVB_GB_ViewModel _HPVB_GB;

        public HPVB_GB_ViewModel HPVB_GB
        {
            get { return _HPVB_GB; }
            set
            {
                _HPVB_GB = value;
                this.RaiseProperChanged(nameof(HPVB_GB));
            }
        }

        private HSTS2_HPVINV08_ViewModel _HSTS2_HPVINV08;

        public HSTS2_HPVINV08_ViewModel HSTS2_HPVINV08
        {
            get { return _HSTS2_HPVINV08; }
            set
            {
                _HSTS2_HPVINV08 = value;
                this.RaiseProperChanged(nameof(HSTS2_HPVINV08));
            }
        }
        #endregion

        #region VQ指令ViewModel
        //HOP
        private HOP_VQ_ViewModel _HOP_VQ;

        public HOP_VQ_ViewModel HOP_VQ
        {
            get { return _HOP_VQ; }
            set
            {
                _HOP_VQ = value;
                this.RaiseProperChanged(nameof(HOP_VQ));
            }
        }

        //HBMS1
        private HBMS1_VQ_ViewModel _HBMS1_VQ;

        public HBMS1_VQ_ViewModel HBMS1_VQ
        {
            get { return _HBMS1_VQ; }
            set
            {
                _HBMS1_VQ = value;
                this.RaiseProperChanged(nameof(HBMS1_VQ));
            }
        }

        //HBAT，适用GB
        private HBAT_GB_ViewModel _HBAT_VQ;

        public HBAT_GB_ViewModel HBAT_VQ
        {
            get { return _HBAT_VQ; }
            set
            {
                _HBAT_VQ = value;
                this.RaiseProperChanged(nameof(HBAT_VQ));
            }
        }

        //HBAT,适用VQ
        private HBAT_VQ_ViewModel _HBAT_VQ2;

        public HBAT_VQ_ViewModel HBAT_VQ2
        {
            get { return _HBAT_VQ2; }
            set
            {
                _HBAT_VQ2 = value;
                this.RaiseProperChanged(nameof(HBAT_VQ2));
            }
        }

        //HEEP1
        private HEEP1_VQ_ViewModel _HEEP1_VQ;

        public HEEP1_VQ_ViewModel HEEP1_VQ
        {
            get { return _HEEP1_VQ; }
            set
            {
                _HEEP1_VQ = value;
                this.RaiseProperChanged(nameof(HEEP1_VQ));
            }
        }

        //HGRID
        private HGRID_VQ_ViewModel _HGRID_VQ;
        public HGRID_VQ_ViewModel HGRID_VQ
        {
            get { return _HGRID_VQ; }
            set
            {
                _HGRID_VQ = value;
                this.RaiseProperChanged(nameof(HGRID_VQ));
            }
        }
        #endregion

        #region VDF指令ViewModel

        //HIMSG2
        private HIMSG2_PDF_ViewModel _HIGSG2_PDF;

        public HIMSG2_PDF_ViewModel HIGSG2_PDF
        {
            get { return _HIGSG2_PDF; }
            set { _HIGSG2_PDF = value; }
        }

        //HGRID
        private HGRID_PDF_ViewModel _HGRID_PDF;
        public HGRID_PDF_ViewModel HGRID_PDF
        {
            get
            {
                return _HGRID_PDF;
            }
            set
            {
                _HGRID_PDF = value;
                OnPropertyChanged();
            }
        }
        //HOP
        private HOP_PDF_ViewModel _HOP_PDF;

        public HOP_PDF_ViewModel HOP_PDF
        {
            get { return _HOP_PDF; }
            set
            {
                _HOP_PDF = value;
                this.RaiseProperChanged(nameof(HOP_PDF));
            }
        }

        //HBAT
        private HBAT_PDF_ViewModel _HBAT_PDF;

        public HBAT_PDF_ViewModel HBAT_PDF
        {
            get { return _HBAT_PDF; }
            set
            {
                _HBAT_PDF = value;
                this.RaiseProperChanged(nameof(HBAT_PDF));
            }
        }

        //HIMSG1
        private HIMSG1_PDF_ViewModel _HIMSG1;

        public HIMSG1_PDF_ViewModel HIMSG1
        {
            get { return _HIMSG1; }
            set
            {
                _HIMSG1 = value;
                this.RaiseProperChanged(nameof(HIMSG1));
            }
        }

        //HPV
        private HPV_PDF_ViewModel _HPV_PDF;

        public HPV_PDF_ViewModel HPV_PDF
        {
            get { return _HPV_PDF; }
            set
            {
                _HPV_PDF = value;
                this.RaiseProperChanged(nameof(HPV_PDF));
            }
        }

        //HTEMP
        private HTEMP_PDF_ViewModel _HTEMP_PDF;
        public HTEMP_PDF_ViewModel HTEMP_PDF
        {
            get { return _HTEMP_PDF; }
            set
            {
                _HTEMP_PDF = value;
                this.RaiseProperChanged(nameof(HTEMP_PDF));
            }
        }

        //HCTMSG1
        private HCTMSG1_PDF_ViewModel _HCTMSG1_PDF;

        public HCTMSG1_PDF_ViewModel HCTMSG1_PDF
        {
            get { return _HCTMSG1_PDF; }
            set
            {
                _HCTMSG1_PDF = value;
                this.RaiseProperChanged(nameof(HCTMSG1_PDF));
            }
        }

        //HEEP3
        private HEEP3_PDF_ViewModel _HEEP3_PDF;

        public HEEP3_PDF_ViewModel HEEP3_PDF
        {
            get { return _HEEP3_PDF; }
            set
            {
                _HEEP3_PDF = value;
                this.RaiseProperChanged(nameof(HEEP3_PDF));
            }
        }

        //HEEP1
        private HEEP1_PDF_ViewModel _HEEP1_PDF;

        public HEEP1_PDF_ViewModel HEEP1_PDF
        {
            get { return _HEEP1_PDF; }
            set
            {
                _HEEP1_PDF = value;
                this.RaiseProperChanged(nameof(HEEP1_PDF));
            }
        }

        #endregion

        #region CYJ指令ViewModel

        //HGRID
        private HGRID_CYJ_ViewModel _HGRID_CYJ;

        public HGRID_CYJ_ViewModel HGRID_CYJ
        {
            get { return _HGRID_CYJ; }
            set
            {
                _HGRID_CYJ = value;
                this.RaiseProperChanged(nameof(HGRID_CYJ));
            }
        }

        //HEEP1
        private HEEP1_CYJ_ViewModel _HEEP1_CYJ;

        public HEEP1_CYJ_ViewModel HEEP1_CYJ
        {
            get { return _HEEP1_CYJ; }
            set
            {
                _HEEP1_CYJ = value;
                this.RaiseProperChanged(nameof(HEEP1_CYJ));
            }
        }

        //HSTS
        private HSTS_CYJ_ViewModel _HSTS_CYJ;

        public HSTS_CYJ_ViewModel HSTS_CYJ
        {
            get { return _HSTS_CYJ; }
            set
            {
                _HSTS_CYJ = value;
                this.RaiseProperChanged(nameof(HSTS_CYJ));
            }
        }

        #endregion

        #region LB6指令ViewModel
        /// <summary>
        /// HEEP1
        /// </summary>
        private HEEP1_LB6_ViewModel _HEEP1_LB6;

        public HEEP1_LB6_ViewModel HEEP1_LB6
        {
            get { return _HEEP1_LB6; }
            set
            {
                _HEEP1_LB6 = value;
                this.RaiseProperChanged(nameof(HEEP1_LB6));
            }
        }

        #endregion

        #region 串口工具

        //串口实体类
        //   private SerialPortSettings serialPortSettings;

        /// <summary>
        /// 初始化串口工具
        /// </summary>
        private void IniCom()
        {
            // 直接从 SerialPortSetting 获取设置
            var settings = SerialPortSetting.GetCurrentSettings();

            // 初始化串口通讯工具
            SerialCommunicationService.InitiateCom(settings);
        }

        /// <summary>
        /// 重新配置串口参数 - 应用最新的设置
        /// 当用户通过下拉框选择了新串口后，调用此方法更新配置
        /// </summary>
        private void ReconfigureSerialPort()
        {
            try
            {
                // 获取到最新值
                var settings = SerialPortSetting.GetCurrentSettings();

                // 重新初始化串口
                SerialCommunicationService.InitiateCom(settings);

                AddLog($"串口参数已更新: {settings.PortName}, {settings.BaudRate}bps");
            }
            catch (Exception ex)
            {
                AddLog($"更新串口参数失败: {ex.Message}");
            }
        }

        #region 串口图标
        //串口打开图标
        private Visibility _ComIconOpen = Visibility.Visible;

        public Visibility ComIconOpen
        {
            get { return _ComIconOpen; }
            set
            {
                _ComIconOpen = value;
                this.RaiseProperChanged(nameof(ComIconOpen));
            }
        }

        //串口关闭图标
        private Visibility _ComIconClose = Visibility.Collapsed;

        public Visibility ComIconClose
        {
            get { return _ComIconClose; }
            set
            {
                _ComIconClose = value;
                this.RaiseProperChanged(nameof(ComIconClose));
            }
        }


        /// <summary>
        /// 改变串口打开图标
        /// </summary>
        /// <param name="flag"></param>
        private void ChangeComIcon(bool flag)
        {
            if (flag)
            {
                //串口打开
                ComIconClose = Visibility.Visible;
                ComIconOpen = Visibility.Collapsed;
            }
            else
            {
                //串口关闭
                ComIconClose = Visibility.Collapsed;
                ComIconOpen = Visibility.Visible;
            }
        }

        #endregion

        /// <summary>
        /// 打开串口(已打开则关闭串口)
        /// </summary>
        public async void openCom()
        {
            if (SerialCommunicationService.IsOpen())//判断串口是否已打开
            {
                try
                {
                    AddLog("准备关闭通信");
                    //停止后台通信(如果有)
                    StopBackgroundThread();
                    // 等待通讯线程结束
                    var WaitFinish = Task.Run(() =>
                    {
                        while (IsRunning)
                        {
                            // 可以添加短暂延迟避免CPU占用过高
                            Task.Delay(30).Wait();
                        }
                    });
                    //等待后台通讯停止（1s）
                    await Task.WhenAny(WaitFinish, Task.Delay(1000));
                    //关闭串口
                    SerialCommunicationService.CloseCom();
                    AddLog("串口已关闭");

                    ChangeComIcon(false);
                    UpdateState(App.GetText("串口已关闭"));
                    comStateColor(false);
                    AddLog($"关闭串口{SerialCommunicationService.getComName()}成功");
                    //更新combobox状态
                    UpdateComboBoxEnabledState();
                }
                catch (Exception)
                {

                }
                finally
                {
                    ChangeComIcon(false);
                    comStateColor(false);
                    IsRunning = false;
                }
            }
            else
            {
                // 先重新配置串口参数（确保使用最新的设置）
                ReconfigureSerialPort();

                // 尝试打开串口
                if (!SerialCommunicationService.OpenCom())
                {
                    MessageBox.Show("串口打开失败！");
                    return;
                }
                ;
                ChangeComIcon(true);
                comStateColor(true);
                AddLog($"打开串口{SerialCommunicationService.getComName()}成功");

                string machine;
                //自动识别机器
                if (!AutoSelectedMachineType(out machine))
                {
                    //未识别
                    MessageBoxResult result = MessageBox.Show($"无法识别机器机器类型:{machine}", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    //if (result != MessageBoxResult.OK)
                    //{
                    //关闭串口
                    openCom();
                    ChangeComIcon(false);
                    comStateColor(false);
                    AddLog($"关闭串口{SerialCommunicationService.getComName()}成功");
                    IsRunning = false;
                    return;
                    //}
                }
                //更新combobox状态
                UpdateComboBoxEnabledState();
                //开始通讯
                StartBackgroundThread();
            }

        }

        #endregion

        #region 信息显示窗口
        /// <summary>
        /// 内容窗口
        /// </summary>
        private UserControl contentUC;
        public UserControl ContentUC
        {
            get
            {
                if (contentUC == null)
                {
                    return contentUC = new GB_MonitorUC();
                }
                return contentUC;
            }
            set { contentUC = value; RaiseProperChanged(nameof(ContentUC)); }
        }

        /// <summary>
        /// 机器类型
        /// </summary>
        private string? _MachineType;

        public string? MachineType
        {
            get { return _MachineType; }
            set
            {
                _MachineType = value;
                this.RaiseProperChanged(nameof(MachineType));
            }
        }


        /// <summary>
        /// 机器型号
        /// </summary>
        private int _MachineMode;

        public int MachineModel
        {
            get { return _MachineMode; }
            set
            {
                _MachineMode = value;
                this.RaiseProperChanged(nameof(MachineModel));
            }
        }

        #endregion

        #region 日志界面

        /// <summary>
        /// 日志信息
        /// </summary>
        private ObservableCollection<string> _logMessages = new ObservableCollection<string>();
        public ObservableCollection<string> LogMessages
        {
            get { return _logMessages; }
            set
            {
                if (_logMessages != value)
                {
                    _logMessages = value;
                    RaiseProperChanged(nameof(LogMessages));
                }
            }
        }

        private bool _isScrollingEnabled = true;
        public bool IsScrollingEnabled
        {
            get { return _isScrollingEnabled; }
            set
            {
                if (_isScrollingEnabled != value)
                {
                    _isScrollingEnabled = value;
                    RaiseProperChanged(nameof(IsScrollingEnabled));
                }
            }
        }

        public ICommand ClearLogCommand { get; }
        public ICommand SaveLogCommand { get; }

        public ICommand AddLogCommand { get; }

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="logMessage"></param>
        public void AddLog(string logMessage)
        {
            string timestamp = DateTime.Now.ToString("yyyy - MM - dd HH:mm:ss");
            string formattedLog = $"[{timestamp}] {logMessage}";

            Application.Current.Dispatcher.Invoke(() =>
            {

                LogMessages.Insert(0, formattedLog);
                if (LogMessages.Count > 100) LogMessages.RemoveAt(99);
            });
            //从上到下
            //LogMessages.Add(formattedLog);

            //从下到上的添加日志


            if (IsScrollingEnabled)
            {
                // 这里可以通过事件或其他方式通知视图滚动到最新日志
            }
            SaveLogToFile(formattedLog);
        }

        private void AddMessage(object p)
        {
            string timestamp = DateTime.Now.ToString("yyyy - MM - dd HH:mm:ss");
            string formattedLog = $"[{timestamp}] 新增数据";

            //从上到下
            //LogMessages.Add(formattedLog);

            //从下到上的添加日志
            LogMessages.Insert(0, formattedLog);
        }

        private void ClearLog(object parameter)
        {
            LogMessages.Clear();
        }

        /// <summary>
        /// 保存所有日志
        /// </summary>
        /// <param name="parameter"></param>
        private void SaveLog(object parameter)
        {
            try
            {
                foreach (var log in LogMessages)
                {
                    SaveLogToFile(log);
                }
                AddLog("日志已保存");
            }
            catch (Exception ex)
            {
                AddLog($"保存日志时出错: {ex.Message}");
            }
        }

        /// <summary>
        /// 保存一条日志到本地
        /// </summary>
        /// <param name="log"></param>
        private void SaveLogToFile(string log)
        {
            DateTime now = DateTime.Now;
            string logName = "日志";
            string yearMonth = now.ToString("yyyy-MM");
            string DayFolder = now.ToString("dd");

            // 构建日志文件夹路径
            string logFolder = Path.Combine(Directory.GetCurrentDirectory(), logName, yearMonth, DayFolder);
            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }

            // 构建日志文件路径
            string logFilePath = Path.Combine(logFolder, $"log_{now:yyyyMMdd}.txt");
            try
            {
                using (StreamWriter writer = File.AppendText(logFilePath))
                {
                    writer.WriteLine(log);
                }
            }
            catch (Exception ex)
            {
                AddLog($"写入日志文件时出错: {ex.Message}");
            }
        }
        #endregion

        #region 后台通讯线程
        private CancellationTokenSource _cts = new CancellationTokenSource();//取消线程专用
        private ManualResetEventSlim _pauseEvent = new ManualResetEventSlim(true);//暂停线程专用
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1); // 异步竞争

        //帧计数
        private SerialCountVM _SerialCountVM;
        public SerialCountVM SerialCountVM
        {
            get { return _SerialCountVM; }
            set
            {
                _SerialCountVM = value;
                this.RaiseProperChanged(nameof(SerialCountVM));
            }
        }

        // 后台线程是否正在运行
        private bool _isRunning = false;
        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                _isRunning = value;
                OnPropertyChanged();
            }
        }

        // 状态信息
        private string _status = "关闭";
        public string Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    RaiseProperChanged(nameof(Status));
                }

            }
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="state"></param>
        public void UpdateState(string state)
        {
            Status = state;
        }

        // 工作状态指示
        private bool _isWorking;
        public bool IsWorking
        {
            get => _isWorking;
            set
            {
                if (value == _isWorking) return;
                _isWorking = value;
                RaiseProperChanged(nameof(IsWorking));
            }
        }

        //状态灯
        private Brush comStatus = Brushes.Red;
        public Brush ComStatus
        {
            get
            {
                return comStatus;
            }
            set
            {
                comStatus = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 设置状态灯颜色
        /// </summary>
        /// <param name="flag"></param>
        public void comStateColor(bool flag)
        {
            if (flag)
            {
                ComStatus = Brushes.Green;
            }
            else
            {
                ComStatus = Brushes.Red;
            }
        }

        // 命令定义
        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        //public ICommand ExecuteSpecialCommand { get; }
        public ICommand OpenCom { get; }

        /// <summary>
        /// 启动后台通信线程
        /// </summary>
        private void StartBackgroundThread()
        {

            if (!SerialCommunicationService.IsOpen())
            {
                MessageBox.Show(App.GetText("请先打开串口!"), "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (IsRunning) { return; }
            else
            {
                IsRunning = true;
                _cts = new CancellationTokenSource();
                _pauseEvent.Set();
                UpdateState(App.GetText("正在通信"));
                comStateColor(true);
                Task.Run(() => (BackgroundWorker(_cts.Token), _cts.Token));
                AddLog("后台通信线程已启动");
            }

        }

        /// <summary>
        /// 后台工作线程主循环
        /// </summary>
        private async Task BackgroundWorker(CancellationToken token)
        {
            flag = 0;//初始设置值

            try
            {
                if (SelectedMachineItem == "BMS01")
                {
                    //补丁，BMS01时单位稍作修改
                    ModbusRTU.FirstSetReceive_Enum(BMS_Setting.SendingCommands, BMS_Setting.LoadSettings2());
                }
                else
                    ModbusRTU.FirstSetReceive_Enum(BMS_Setting.SendingCommands, BMS_Setting.LoadSettings());
                //COM通讯
                while (!token.IsCancellationRequested)//检查是否请求取消
                {
                    if (SelectedMachineItem == "HPVINV02")
                    {
                        //GB3024通讯
                        CommunicationWithGB3024(token);
                    }
                    else if (SelectedMachineItem == "LPVINV02")
                    {
                        //LPVINV02通讯
                        CommunicationWithVQ3024(token);
                    }
                    else if (SelectedMachineItem == "HPVINV07")
                    {
                        //PTF通讯
                        CommunicationWithPTF3024(token);
                    }
                    else if (SelectedMachineItem == "HPVINV04")
                    {
                        //HPVINV04通讯
                        CommunicationWithHPVINV04(token);
                    }
                    else if (SelectedMachineItem == "HPVINV06")
                    {
                        //GB6042通讯
                        CommunicationWithGB_HPVINV06(token);
                    }
                    else if (SelectedMachineItem == "HPVINV08")
                    {
                        //HPVINV08通讯
                        CommunicationWithGB_HPVINV08(token);
                    }
                    else if (SelectedMachineItem == "UPSCYX01")
                    {
                        //CYJ通讯（UPSCYX01）
                        CommunicationWith_CYJ(token);
                    }
                    else if (SelectedMachineItem == "LB6")
                    {
                        CommunicationWith_LB6(token);
                    }
                    else if (SelectedMachineItem == "BMS02")
                    {
                        //唤醒休眠
                        if (BMS_Setting.isSleeping == 0 && BMS_Setting.SettingStatue[4] == 1)
                        {
                            await Task.Delay(1000, token);
                            byte[] rec = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead20Frame(1, 10, 0), 8);
                            if (rec.Length == 8)
                            {
                                BMS_Setting.SettingStatue[4] = 0;
                            }
                            continue;
                        }
                        //BMS02通讯
                        CommunicationWithBMS02(token);
                    }
                    else if (SelectedMachineItem == "BMS01")
                    {
                        //唤醒休眠
                        if (BMS_Setting.isSleeping == 0 && BMS_Setting.SettingStatue[4] == 1)
                        {
                            await Task.Delay(1000, token);
                            byte[] rec = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead20Frame(1, 10, 0), 8);
                            if (rec.Length == 8)
                            {
                                BMS_Setting.SettingStatue[4] = 0;
                            }
                            continue;
                        }
                        //BMS01通讯
                        CommunicationWithBMS01(token);
                    }
                    else if (SelectedMachineItem == "BMS03")
                    {
                        //唤醒休眠
                        if (BMS_Setting.isSleeping == 0 && BMS_Setting.SettingStatue[4] == 1)
                        {
                            await Task.Delay(1000, token);
                            byte[] rec = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead20Frame(1, 10, 0), 8);
                            if (rec.Length == 8)
                            {
                                BMS_Setting.SettingStatue[4] = 0;
                            }
                            continue;
                        }
                        //BMS03通讯
                        CommunicationWithBMS03(token);
                    }
                    // 模拟常规通信
                    await Task.Delay(100, token);
                    //AddLog($"[后台] 常规通信: {DateTime.Now:HH:mm:ss.fff}");
                }
            }
            catch (OperationCanceledException)
            {
                AddLog("后台通信已终止");
                UpdateState(App.GetText("已停止通信!"));
                IsRunning = false;
            }
            catch (Exception ex)
            {
                string mes = ex.ToString();
                AddLog($"后台通信异常{mes}");
                UpdateState(App.GetText("异常!"));
                //关闭串口
                SerialCommunicationService.CloseCom();
                UpdateComboBoxEnabledState();
            }
            finally
            {
                IsRunning = false;
                ChangeComIcon(false);
                comStateColor(false);
            }
        }

        #endregion

        #region 通讯实现方法

        /// <summary>
        /// 异常解析显示
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        private void ShowError(string value, string name)
        {
            if (string.IsNullOrEmpty(value))
            {
                AddLog($"{name}返回超时");
            }
            else if (value.Length >= 2 && value.StartsWith("-1"))
            {
                AddLog($"{name}CRC异常:{value}");
            }
        }

        #region VDF3024通讯
        /// <summary>
        /// VDF3024通讯
        /// </summary>
        /// <param name="token"></param>
        private void CommunicationWithPTF3024(CancellationToken token)
        {

            string receive = "";

            Thread.Sleep(200);
            //判断是否开启CRC接收校验（抗干扰 默认开启
            if (IsChecked)
            {
                //发送HOSTCRCEN指令
                _pauseEvent.Wait(token);
                receive = SerialCommunicationService.SendSettingCommand("HOSTCRC", "EN");
                ShowError(receive, "HOSTCRC");
            }
            else if (OnceOpenCRC)
            {
                //发送HOSTCRDEN指令
                _pauseEvent.Wait(token);
                receive = SerialCommunicationService.SendSettingCommand("HOSTCRC", "DN");
                OnceOpenCRC = false;
                IsChecked = false;
                SerialCommunicationService.OpenReceiveCRC(false);
            }

            Thread.Sleep(200);
            //获取机器型号
            _pauseEvent.Wait(token);
            string receive_MachineType = SerialCommunicationService.SendCommand(SpecialCommand.QueryMachineType, 10);
            if (receive_MachineType == "")
            {
                AddLog("机器类型返回超时");
                MachineType = "返回超时";
            }
            else if (receive_MachineType.Substring(0, 2) == "-1")
            {
                AddLog($"机器类型校验不通过:{receive_MachineType}");
                MachineType = "CRC异常";
            }
            else
            {
                //AddLog("机器类型返回正常");
                MachineType = receive_MachineType.Substring(1, 8);
                SerialCommunicationService.MachineType = receive_MachineType;
            }

            Thread.Sleep(100);
            //发送HIMSG2N指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HIGSG2_PDF.Command, 50);
            HIGSG2_PDF.AnalysisStringToElement(receive);
            ShowError(receive, "HIMSG2");

            Thread.Sleep(100);
            // 等待暂停或取消信号
            _pauseEvent.Wait(token);
            //发送HGRID指令
            receive = SerialCommunicationService.SendCommand(HGRID_PDF.Command, 50);
            //解析返回命令
            HGRID_PDF.AnalyseStringToElement(receive);
            //显示
            ACPowerVM = StringToIntConversion(HGRID_PDF.ACPower);
            //市电百分比
            ACTotalPwr = CountPercent(HGRID_PDF.ACPower, HIGSG2_PDF.ACTotalPwr);
            ShowError(receive, "HGRID");

            Thread.Sleep(100);
            // 等待暂停或取消信号
            _pauseEvent.Wait(token);
            //发送HOP指令
            receive = SerialCommunicationService.SendCommand(HOP_PDF.Command, 50);
            //解析返回命令
            HOP_PDF.AnalysisStringToElement(receive);
            //逆变百分比
            InvTotalPwr = StringToIntConversion(HOP_PDF.LoadPercent);
            ShowError(receive, "HOP");

            Thread.Sleep(100);
            // 等待暂停或取消信号
            _pauseEvent.Wait(token);
            //发送HBAT指令
            receive = SerialCommunicationService.SendCommand(HBAT_PDF.Command, 50);
            //解析返回命令
            HBAT_PDF.AnalysisStringToElement(receive);
            ShowError(receive, "HBAT");

            Thread.Sleep(100);
            _pauseEvent.Wait(token);
            //发送HEEP1指令
            receive = SerialCommunicationService.SendCommand(HEEP1_PDF.Command, 80);
            //解析返回指令
            HEEP1_PDF.AnalyseStringToElement(receive);
            ShowError(receive, "HEEP1");

            Thread.Sleep(100);
            _pauseEvent.Wait(token);
            //发送HEEP2指令
            receive = SerialCommunicationService.SendCommand(HEEP2.Command, 80);
            //解析返回指令
            HEEP2.AnalyseStringToElement(receive);
            ShowError(receive, "HEEP2");

            Thread.Sleep(100);
            //发送HEEP3指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HEEP3_PDF.Command, 80);
            HEEP3_PDF.AnalysisStringToElement(receive);
            ShowError(receive, "HEEP3");

            Thread.Sleep(100);
            //发送HIMSG1指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HIMSG1.Command, 21);
            HIMSG1.AnalysisStringToElement(receive);
            ShowError(receive, "HIMSG1");

            Thread.Sleep(100);
            //发送HPV指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HPV_PDF.Command, 50);
            HPV_PDF.AnalysisStringToElement(receive);
            //MPPT百分比
            MPPTTotalPwr = CountPercent(HPV_PDF.PVPwr, HIGSG2_PDF.MPPTTotalPwr);
            ShowError(receive, "HPV");

            Thread.Sleep(100);
            //发送HTEMP指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HTEMP_PDF.Command, 50);
            HTEMP_PDF.AnalysisStringToElement(receive);
            ShowError(receive, "HTEMP");

            Thread.Sleep(100);
            //发送HCTMSG1指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HCTMSG1_PDF.Command, 80);
            HCTMSG1_PDF.AnalysisStringToElement(receive);
            ShowError(receive, "HCTMSG1");

            Thread.Sleep(100);
            //发送HGEN指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HGEN.Command, 60);
            HGEN.AnalyseStringToElement(receive);
            ShowError(receive, "HGEN");

            //机器型号
            MachineModel = StringToIntConversion(HOP_PDF.RatedPwr) + StringToIntConversion(HBAT_VQ.BattCells) * 12;

            //数据
            var Common_Data = new Common_Data
            {
                DataNow = DateTime.Now,//日期
                GridConnectedFunction = HEEP1_PDF.GridConnectedFunction,//并网功能
                CT_Enable = HEEP1_PDF.CT_Enable,//CT功能开关
                PV_GridConnectionProtocol = HEEP1_PDF.PV_GridConnectionProtocol,//并网协议
                GridCurrent = HEEP1_PDF.GridCurrent,//并网电流
                ZeroAdjPwr = HEEP3_PDF.ZeroAdjPwr,//调零功率
                CTCurr = HCTMSG1_PDF.CTCurr,//CT电流
                CTPwr = HCTMSG1_PDF.CTPwr,//CT功率
                MaxInvPower = HGRID_PDF.MaxInvPower,//当前允许最大逆变功率

                MainsVoltage = HGRID_GB.MainsVoltage,//市电电压
                MainsFrequency = HGRID_GB.MainsFrequency,//市电频率
                ACPower = HGRID_GB.ACPower,//市电功率

                OutVolt = HOP_PDF.OutVolt,//输出电压
                OutFreq = HOP_PDF.OutFreq,//输出频率
                ApparentPwr = HOP_PDF.ApparentPwr,//视在功率
                ActivePwr = HOP_PDF.ActivePwr,//有功功率
                LoadPercent = HOP_PDF.LoadPercent,//负载百分比
                DCOffset = HOP_PDF.DCOffset,//直流分量
                InductorCurr = HOP_PDF.InductorCurr,//电感电流
                InductorPwr = HOP_PDF.InductorPwr,//电感功率

                PVVolt = HPV_PDF.PVVolt,//PV电压
                PVPwr = HPV_PDF.PVPwr,//PV功率
                PVCurr = HPV_PDF.PVCurr,//PV电流
                DailyGen = HGEN.DailyGen,//日发电量
                MonthlyGen = HGEN.MonthlyGen,//月发电量
                AnnualGen = HGEN.AnnualGen,//年发电量
                TotalGen = HGEN.TotalGen,//总发电量

                BusVolt = HBAT_VQ.BusVolt, //母线电压
                InvTemp = HTEMP_PDF.InvTemp,//逆变温度
                MaxTemp = HTEMP_PDF.MaxTemp,//当前最高温度
                PFCStatus = HBAT_PDF.PFCStatus,//PFC工作状态
                FanSpeed = HTEMP_PDF.FanSpeed,//风扇转速
                FanEnable = HTEMP_PDF.FanEnable,//风扇使能
            };

            // 最新在最前
            Application.Current.Dispatcher.Invoke(() =>
            {
                // 添加到界面
                DR_Monitor.CommonDataList.Insert(0, Common_Data);

                // 保证最多 100 条
                if (DR_Monitor.CommonDataList.Count > 100)
                    DR_Monitor.CommonDataList.RemoveAt(DR_Monitor.CommonDataList.Count - 1);

                if (DR_Monitor.IsSaving && DR_Monitor._savePath != null)
                {
                    DR_Monitor.SaveToExcel(Common_Data);
                }

            });
        }
        #endregion

        #region GB3024通讯
        /// <summary>
        /// GB3024通讯
        /// </summary>
        /// <param name="token"></param>
        private void CommunicationWithGB3024(CancellationToken token)
        {
            string receive = string.Empty;

            Thread.Sleep(100);
            //判断是否开启CRC接收校验（抗干扰 默认开启
            if (IsChecked)
            {
                //发送HOSTCRCEN指令
                _pauseEvent.Wait(token);
                receive = SerialCommunicationService.SendSettingCommand("HOSTCRC", "EN");
                ShowError(receive, "HOSTCRC");
            }
            else if (OnceOpenCRC)
            {
                //发送HOSTCRDEN指令
                _pauseEvent.Wait(token);
                receive = SerialCommunicationService.SendSettingCommand("HOSTCRC", "DN");
                OnceOpenCRC = false;
                IsChecked = false;
                SerialCommunicationService.OpenReceiveCRC(false);
            }

            Thread.Sleep(100);
            // 等待暂停或取消信号
            _pauseEvent.Wait(token);
            //发送查询机器版本指令
            string receive_MachineType = SerialCommunicationService.SendCommand(SpecialCommand.QueryMachineType, 10);
            MachineType = receive_MachineType.Substring(1, 8);
            //解析指令
            SerialCommunicationService.MachineType = receive_MachineType;

            //发送HBMS1指令
            Thread.Sleep(100);
            _pauseEvent.Wait(token); // 等待暂停或取消信号
            receive = SerialCommunicationService.SendCommand(HBMS1_VQ.Command, 70);
            HBMS1_VQ.AnalysisStringToElement(receive);

            Thread.Sleep(100);
            _pauseEvent.Wait(token); // 等待暂停或取消信号
            //发送HEEP1指令
            string receiveHEEP1 = SerialCommunicationService.SendCommand(HEEP1_HPVINV02.Command, 80);
            //解析返回指令
            HEEP1_HPVINV02.AnalyseStringToElement(receiveHEEP1);

            Thread.Sleep(100);
            _pauseEvent.Wait(token); // 等待暂停或取消信号
            //发送HEEP2指令
            string receive_HEEP2 = SerialCommunicationService.SendCommand(HEEP2.Command, 80);
            //解析返回指令
            HEEP2.AnalyseStringToElement(receive_HEEP2);

            Thread.Sleep(100);
            // 等待暂停或取消信号
            _pauseEvent.Wait(token);
            //发送HOP指令
            receive = SerialCommunicationService.SendCommand(HOP_PDF.Command, 50);
            //解析返回命令
            HOP_PDF.AnalysisStringToElement(receive);
            //逆变百分比
            InvTotalPwr = StringToIntConversion(HOP_PDF.LoadPercent);

            Thread.Sleep(100);
            //发送HPV指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HPV_PDF.Command, 50);
            HPV_PDF.AnalysisStringToElement(receive);
            //MPPT百分比
            MPPTTotalPwr = CountPercent(HPV_PDF.PVPwr, HIGSG2_PDF.MPPTTotalPwr);

            Thread.Sleep(100);
            //发送HIMSG2N指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HIGSG2_PDF.Command, 50);
            HIGSG2_PDF.AnalysisStringToElement(receive);

            Thread.Sleep(100);
            //发送HGRID指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HGRID_GB.Command, 50);
            //解析返回命令
            HGRID_GB.AnalyseStringToElement(receive);
            //显示
            ACPowerVM = StringToIntConversion(HGRID_GB.ACPower);
            //市电百分比
            ACTotalPwr = CountPercent(HGRID_GB.ACPower, HIGSG2_PDF.ACTotalPwr);

            Thread.Sleep(100);
            //发送HTEMP指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HTEMP_PDF.Command, 50);
            HTEMP_PDF.AnalysisStringToElement(receive);

            Thread.Sleep(100);
            //发送HBAT指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HBAT_VQ.Command, 50);
            HBAT_VQ.AnalysisStringToElement(receive);
            BattPercent = StringToIntConversion(HBAT_VQ.BattCapacity);


            Thread.Sleep(100);
            //发送HIMSG1指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HIMSG1.Command, 21);
            HIMSG1.AnalysisStringToElement(receive);

            Thread.Sleep(100);
            _pauseEvent.Wait(token); // 等待暂停或取消信号
            //发送Hgen指令
            string receive_HGEN = SerialCommunicationService.SendCommand(HGEN.Command, 60);
            //解析返回指令
            HGEN.AnalyseStringToElement(receive_HGEN);

            Thread.Sleep(100);
            //发送HSTS指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HSTS_GB.Command, 40);
            HSTS_GB.AnalyseStringToElement(receive);


            //机器型号
            MachineModel = StringToIntConversion(HOP_PDF.RatedPwr) + StringToIntConversion(HBAT_VQ.BattCells) * 12;

            //数据
            var Common_Data = new Common_Data
            {
                DataNow = DateTime.Now,
                MainsVoltage = HGRID_GB.MainsVoltage,//市电电压
                MainsFrequency = HGRID_GB.MainsFrequency,//市电频率
                ACPower = HGRID_GB.ACPower,//市电功率

                OutVolt = HOP_PDF.OutVolt,//输出电压
                OutFreq = HOP_PDF.OutFreq,//输出频率
                ApparentPwr = HOP_PDF.ApparentPwr,//视在功率
                ActivePwr = HOP_PDF.ActivePwr,//有功功率
                LoadPercent = HOP_PDF.LoadPercent,//负载百分比

                PVVolt = HPV_PDF.PVVolt,//PV电压
                PVPwr = HPV_PDF.PVPwr,//PV功率
                PVCurr = HPV_PDF.PVCurr,//PV电流
                TotalGen = HGEN.TotalGen,//总发电量
                DailyGen = HGEN.DailyGen,//日发电量
                MonthlyGen = HGEN.MonthlyGen,//月发电量
                AnnualGen = HGEN.AnnualGen,//年发电量

                BattVolt = HBAT_VQ.BattVolt,//电池电压
                BatCurr = HBAT_VQ.BatCurr, //电池电流
                BattCapacity = HBAT_VQ.BattCapacity,//电池容量
                BusVolt = HBAT_VQ.BusVolt, //母线电压

                ProtocolType = HBMS1_VQ.ProtocolType,//协议类型
                BMS_ComOK = HBMS1_VQ.BMS_ComOK,//BMS通信正常
                BMS_LowBattAlarm = HBMS1_VQ.BMS_LowBattAlarm,//BMS低电报警
                BMS_LowBattFault = HBMS1_VQ.BMS_LowBattFault,//BMS低电故障
                BMS_ChgEnable = HBMS1_VQ.BMS_ChgEnable,//BMS允许充电
                BMS_DisEnable = HBMS1_VQ.BMS_DisEnable,//BMS允许放电
                BMS_ChgOC = HBMS1_VQ.BMS_ChgOC,//BMS充电过流
                BMS_DisOC = HBMS1_VQ.BMS_DisOC,//BMS放电过流
                BMS_UnderTemp = HBMS1_VQ.BMS_UnderTemp,//BMS温度过低
                BMS_OverTemp = HBMS1_VQ.BMS_OverTemp,//BMS温度过高
                BMS_AvgTemp = HBMS1_VQ.BMS_AvgTemp,//BMS平均温度
                BMS_ChgCurrLimit = HBMS1_VQ.BMS_ChgCurrLimit,//BMS充电电流限制
                BMS_SOC = HBMS1_VQ.BMS_SOC,//BMS当前SOC
                BMS_ChgVoltLimit = HBMS1_VQ.BMS_ChgVoltLimit,//BMS充电电压限制
                BMS_DisVoltLimit = HBMS1_VQ.BMS_DisVoltLimit,//BMS放电电压限制

                FaultCode = HSTS_GB.FaultCode,//故障代码
                PVTemp = HTEMP_PDF.PVTemp,//PV温度
                InvTemp = HTEMP_PDF.InvTemp,//逆变温度
                BoostTemp = HTEMP_PDF.BoostTemp,//升压温度
                XfmrTemp = HTEMP_PDF.XfmrTemp,//变压器温度
                MaxTemp = HTEMP_PDF.MaxTemp,//当前最高温度
                FanSpeed = HTEMP_PDF.FanSpeed,//风扇转速
                FanEnable = HTEMP_PDF.FanEnable,//风扇使能
                Mode = HSTS_GB.Mode,//模式
                PVToLoadAC = HSTS_GB.PVToLoadAC,//AC状态下PV馈能到负载
                OutputStatus = HSTS_GB.OutputStatus,//机器是否有输出
                BattLowAlarm = HSTS_GB.BattLowAlarm,//电池低电报警
                BattDisconnected = HSTS_GB.BattDisconnected,//电池未接
                OutputOverload = HSTS_GB.OutputOverload,//输出过载
                OverTemp = HSTS_GB.OverTemp,//机器过温
                EEPROM_DataErr = HSTS_GB.EEPROM_DataErr,//EEPROM数据异常
                EEPROM_IOErr = HSTS_GB.EEPROM_IOErr,//EEPROM读写异常
                PVLowPwrFault = HSTS_GB.PVLowPwrFault,//PV功率过低异常
                InputOV = HSTS_GB.InputOV,//输入电压过高
                BattOV = HSTS_GB.BattOV,//电池电压过高
                FanSpeedFault = HSTS_GB.FanSpeedFault,//风扇转速异常
                ParallelUnits = HSTS_GB.ParallelUnits,//并机系统里机器的总数
                GridTieFlag = HSTS_GB.GridTieFlag,//并网标志
                ParallelRole = HSTS_GB.ParallelRole,//并机系统中角色
                MainRelayStat = HSTS_GB.MainRelayStat,//主输出继电器状态
                SecOutStat = HSTS_GB.SecOutStat,//第二输出当前状态
                BMS_ComFault = HSTS_GB.BMS_ComFault,//BMS通讯异常
                TempSensorFault = HSTS_GB.TempSensorFault,//温度传感器异常
                ACLED = HSTS_GB.ACLED,//市电灯状态
                InvLED = HSTS_GB.InvLED,//逆变灯状态
                ChgLED = HSTS_GB.ChgLED,//充电灯状态
                AlarmLED = HSTS_GB.AlarmLED//报警灯状态
            };

            // 最新在最前
            Application.Current.Dispatcher.Invoke(() =>
            {
                // 添加到界面
                DR_Monitor.CommonDataList.Insert(0, Common_Data);

                // 保证最多 100 条
                if (DR_Monitor.CommonDataList.Count > 100)
                    DR_Monitor.CommonDataList.RemoveAt(DR_Monitor.CommonDataList.Count - 1);

                if (DR_Monitor.IsSaving && DR_Monitor._savePath != null)
                {
                    DR_Monitor.SaveToExcel(Common_Data);
                }

            });

        }
        #endregion

        #region LPVINV02通讯
        /// <summary>
        /// LPVINV02通讯
        /// </summary>
        /// <param name="token"></param>
        private void CommunicationWithVQ3024(CancellationToken token)
        {
            string receive = "";

            //判断是否开启CRC接收校验（抗干扰 默认开启
            if (IsChecked)
            {
                //发送HOSTCRCEN指令
                _pauseEvent.Wait(token);
                receive = SerialCommunicationService.SendSettingCommand("HOSTCRC", "EN");
                ShowError(receive, "HOSTCRC");
            }
            else if (OnceOpenCRC)
            {
                //发送HOSTCRDEN指令
                _pauseEvent.Wait(token);
                receive = SerialCommunicationService.SendSettingCommand("HOSTCRC", "DN");
                OnceOpenCRC = false;
                IsChecked = false;
                SerialCommunicationService.OpenReceiveCRC(false);
            }

            Thread.Sleep(100);
            //获取机器型号
            _pauseEvent.Wait(token);
            string receive_MachineType = SerialCommunicationService.SendCommand(SpecialCommand.QueryMachineType, 10);
            MachineType = receive_MachineType;
            SerialCommunicationService.MachineType = receive_MachineType;

            Thread.Sleep(100);
            //发送HBMS1指令
            _pauseEvent.Wait(token); // 等待暂停或取消信号
            receive = SerialCommunicationService.SendCommand(HBMS1_VQ.Command, 70);
            HBMS1_VQ.AnalysisStringToElement(receive);

            Thread.Sleep(100);
            //发送HEEP1指令
            _pauseEvent.Wait(token); // 等待暂停或取消信号
            receive = SerialCommunicationService.SendCommand(HEEP1_VQ.Command, 80);
            HEEP1_VQ.AnalyseStringToElement(receive);

            Thread.Sleep(100);
            //发送HEEP2指令
            _pauseEvent.Wait(token); // 等待暂停或取消信号
            receive = SerialCommunicationService.SendCommand(HEEP2.Command, 80);
            HEEP2.AnalyseStringToElement(receive);

            Thread.Sleep(100);
            //发送HOP指令
            _pauseEvent.Wait(token); // 等待暂停或取消信号
            receive = SerialCommunicationService.SendCommand(HOP_VQ.Command, 50);
            HOP_VQ.AnalyseStringToElement(receive);

            Thread.Sleep(100);
            //发送HGRID指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HGRID_VQ.Command, 50);
            //解析返回命令
            HGRID_VQ.AnalyseStringToElement(receive);
            //显示
            ACPowerVM = StringToIntConversion(HGRID_PDF.ACPower);
            //市电百分比
            ACTotalPwr = CountPercent(HGRID_PDF.ACPower, HIGSG2_PDF.ACTotalPwr);

            Thread.Sleep(100);
            //发送HTEMP指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HTEMP_PDF.Command, 50);
            HTEMP_PDF.AnalysisStringToElement(receive);

            Thread.Sleep(100);
            //发送HBAT指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HBAT_VQ2.Command, 50);
            HBAT_VQ2.AnalysisStringToElement(receive);
            BattPercent = StringToIntConversion(HBAT_VQ2.BattCapacity);

            Thread.Sleep(100);
            //发送HBAT指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HIMSG1.Command, 21);
            HIMSG1.AnalysisStringToElement(receive);

            //数据
            var Common_Data = new Common_Data
            {
                DataNow = DateTime.Now,//日期

                MainsVoltage = HGRID_GB.MainsVoltage,//市电电压
                MainsFrequency = HGRID_GB.MainsFrequency,//市电频率

                OutVolt = HOP_PDF.OutVolt,//输出电压
                OutFreq = HOP_PDF.OutFreq,//输出频率
                ApparentPwr = HOP_PDF.ApparentPwr,//视在功率
                ActivePwr = HOP_PDF.ActivePwr,//有功功率
                LoadPercent = HOP_PDF.LoadPercent,//负载百分比

                BattVolt = HBAT_VQ.BattVolt,//电池电压
                BattCapacity = HBAT_VQ.BattCapacity,//电池容量
                BusVolt = HBAT_VQ.BusVolt, //母线电压
                BattCells = HBAT_VQ2.BattCells,//电池节数
                BattChgCurr = HBAT_VQ2.BattChgCurr,//电池充电电流
                BattDisCurr = HBAT_VQ2.BattDisCurr,//电池放电电流
                ChgMasterSW = HBAT_VQ2.ChgMasterSW,//充电总开关
                ACChgSW = HBAT_VQ2.ACChgSW,//AC充电开关
                SolarChgSW = HBAT_VQ2.SolarChgSW,//太阳能充电开关

                ProtocolType = HBMS1_VQ.ProtocolType,//协议类型
                BMS_ComOK = HBMS1_VQ.BMS_ComOK,//BMS通信正常
                BMS_LowBattAlarm = HBMS1_VQ.BMS_LowBattAlarm,//BMS低电报警
                BMS_LowBattFault = HBMS1_VQ.BMS_LowBattFault,//BMS低电故障
                BMS_ChgEnable = HBMS1_VQ.BMS_ChgEnable,//BMS允许充电
                BMS_DisEnable = HBMS1_VQ.BMS_DisEnable,//BMS允许放电
                BMS_ChgOC = HBMS1_VQ.BMS_ChgOC,//BMS充电过流
                BMS_DisOC = HBMS1_VQ.BMS_DisOC,//BMS放电过流
                BMS_UnderTemp = HBMS1_VQ.BMS_UnderTemp,//BMS温度过低
                BMS_OverTemp = HBMS1_VQ.BMS_OverTemp,//BMS温度过高
                BMS_AvgTemp = HBMS1_VQ.BMS_AvgTemp,//BMS平均温度
                BMS_ChgCurrLimit = HBMS1_VQ.BMS_ChgCurrLimit,//BMS充电电流限制
                BMS_SOC = HBMS1_VQ.BMS_SOC,//BMS当前SOC
                BMS_ChgCurr = HBMS1_VQ.BMS_ChgCurr,//BMS充电电流
                BMS_DisCurr = HBMS1_VQ.BMS_DisCurr,//BMS放电电流
                BMS_ChgVoltLimit = HBMS1_VQ.BMS_ChgVoltLimit,//BMS充电电压限制
                BMS_DisVoltLimit = HBMS1_VQ.BMS_DisVoltLimit,//BMS放电电压限制

                PVTemp = HTEMP_PDF.PVTemp,//PV温度
                BoostTemp = HTEMP_PDF.BoostTemp,//升压温度
                InvTemp = HTEMP_PDF.InvTemp,//逆变温度
                XfmrTemp = HTEMP_PDF.XfmrTemp,//变压器温度
                MaxTemp = HTEMP_PDF.MaxTemp,//当前最高温度
                FanSpeed = HTEMP_PDF.FanSpeed,//风扇转速
                FanEnable = HTEMP_PDF.FanEnable//风扇使能
            };

            // 最新在最前
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (DR_Monitor.IsSaving && DR_Monitor._savePath != null)
                {
                    DR_Monitor.SaveToExcel(Common_Data);
                }

            });

        }
        #endregion

        #region HPVINV04通讯
        /// <summary>
        /// HPVINV04通讯
        /// </summary>
        /// <param name="token"></param>
        private void CommunicationWithHPVINV04(CancellationToken token)
        {

            string receive = "";

            Thread.Sleep(100);
            //判断是否开启CRC接收校验（抗干扰 默认开启
            if (IsChecked)
            {
                //发送HOSTCRCEN指令
                _pauseEvent.Wait(token);
                receive = SerialCommunicationService.SendSettingCommand("HOSTCRC", "EN");
                ShowError(receive, "HOSTCRC");
            }
            else if (OnceOpenCRC)
            {
                //发送HOSTCRDEN指令
                _pauseEvent.Wait(token);
                receive = SerialCommunicationService.SendSettingCommand("HOSTCRC", "DN");
                OnceOpenCRC = false;
                IsChecked = false;
                SerialCommunicationService.OpenReceiveCRC(false);
            }

            Thread.Sleep(100);
            //获取机器型号
            _pauseEvent.Wait(token);
            string receive_MachineType = SerialCommunicationService.SendCommand(SpecialCommand.QueryMachineType, 10);
            MachineType = receive_MachineType.Substring(1, 8);
            SerialCommunicationService.MachineType = receive_MachineType;

            //发送HBMS1指令
            Thread.Sleep(100);
            _pauseEvent.Wait(token); // 等待暂停或取消信号
            receive = SerialCommunicationService.SendCommand(HBMS1_VQ.Command, 70);
            HBMS1_VQ.AnalysisStringToElement(receive);


            //发送HEEP1指令
            Thread.Sleep(100);
            _pauseEvent.Wait(token); // 等待暂停或取消信号
            receive = SerialCommunicationService.SendCommand(HEEP1_HPVINV02.Command, 80);
            HEEP1_HPVINV02.AnalyseStringToElement(receive);

            //发送HEEP2指令
            Thread.Sleep(100);
            _pauseEvent.Wait(token); // 等待暂停或取消信号
            receive = SerialCommunicationService.SendCommand(HEEP2.Command, 80);
            HEEP2.AnalyseStringToElement(receive);

            // 等待暂停或取消信号
            Thread.Sleep(100);
            _pauseEvent.Wait(token);
            //发送HOP指令
            receive = SerialCommunicationService.SendCommand(HOP_PDF.Command, 50);
            //解析返回命令
            HOP_PDF.AnalysisStringToElement(receive);
            //逆变百分比
            InvTotalPwr = StringToIntConversion(HOP_PDF.LoadPercent);


            //发送HPV指令
            Thread.Sleep(100);
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HPV_PDF.Command, 50);
            HPV_PDF.AnalysisStringToElement(receive);
            //MPPT百分比
            MPPTTotalPwr = CountPercent(HPV_PDF.PVPwr, HIGSG2_PDF.MPPTTotalPwr);

            //发送HIMSG2N指令
            Thread.Sleep(100);
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HIGSG2_PDF.Command, 50);
            HIGSG2_PDF.AnalysisStringToElement(receive);

            //发送HGRID指令
            Thread.Sleep(100);
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HGRID_GB.Command, 50);
            //解析返回命令
            HGRID_GB.AnalyseStringToElement(receive);
            //显示
            ACPowerVM = StringToIntConversion(HGRID_GB.ACPower);
            //市电百分比
            ACTotalPwr = CountPercent(HGRID_GB.ACPower, HIGSG2_PDF.ACTotalPwr);

            //发送HTEMP指令
            Thread.Sleep(100);
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HTEMP_PDF.Command, 50);
            HTEMP_PDF.AnalysisStringToElement(receive);

            //发送HBAT指令
            Thread.Sleep(100);
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HBAT_VQ.Command, 50);
            HBAT_VQ.AnalysisStringToElement(receive);
            BattPercent = StringToIntConversion(HBAT_VQ.BattCapacity);

            //发送HIMSG1指令
            Thread.Sleep(100);
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HIMSG1.Command, 21);
            HIMSG1.AnalysisStringToElement(receive);

            //发送HGEN指令
            Thread.Sleep(100);
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HGEN.Command, 60);
            HGEN.AnalyseStringToElement(receive);

            //发送HSTS指令
            Thread.Sleep(100);
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HSTS_GB.Command, 40);//接收下位机的回复数据
            HSTS_GB.AnalyseStringToElement(receive);//解析回复

            //发送HPV指令
            Thread.Sleep(100);
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HPV_PDF.Command, 50);
            HPV_PDF.AnalysisStringToElement(receive);

            //发送HPVB指令
            Thread.Sleep(100);
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HPVB_GB.Command, 50);
            HPVB_GB.AnalysisStringToElement(receive);

            //机器型号
            MachineModel = StringToIntConversion(HOP_PDF.RatedPwr) + StringToIntConversion(HBAT_VQ.BattCells) * 12;

            //数据
            var Common_Data = new Common_Data
            {
                DataNow = DateTime.Now,
                MainsVoltage = HGRID_GB.MainsVoltage,//市电电压
                MainsFrequency = HGRID_GB.MainsFrequency,//市电频率
                ACPower = HGRID_GB.ACPower,//市电功率

                OutVolt = HOP_PDF.OutVolt,//输出电压
                OutFreq = HOP_PDF.OutFreq,//输出频率
                ApparentPwr = HOP_PDF.ApparentPwr,//视在功率
                ActivePwr = HOP_PDF.ActivePwr,//有功功率
                LoadPercent = HOP_PDF.LoadPercent,//负载百分比

                PVVolt = HPV_PDF.PVVolt,//PV电压
                PVVolt2 = HPVB_GB.PV2Volt,//PV2电压
                PVPwr = HPV_PDF.PVPwr,//PV功率
                PVPwr2 = HPVB_GB.PV2Pwr,//PV2功率
                PVCurr2 = HPVB_GB.PV2Curr,//PV2电流
                PVCurr = HPV_PDF.PVCurr,//PV电流
                TotalGen = HGEN.TotalGen,//总发电量
                DailyGen = HGEN.DailyGen,//日发电量
                MonthlyGen = HGEN.MonthlyGen,//月发电量
                AnnualGen = HGEN.AnnualGen,//年发电量

                BattVolt = HBAT_VQ.BattVolt,//电池电压
                BatCurr = HBAT_VQ.BatCurr, //电池电流
                BattCapacity = HBAT_VQ.BattCapacity,//电池容量
                BusVolt = HBAT_VQ.BusVolt, //母线电压

                ProtocolType = HBMS1_VQ.ProtocolType,//协议类型
                BMS_ComOK = HBMS1_VQ.BMS_ComOK,//BMS通信正常
                BMS_LowBattAlarm = HBMS1_VQ.BMS_LowBattAlarm,//BMS低电报警
                BMS_LowBattFault = HBMS1_VQ.BMS_LowBattFault,//BMS低电故障
                BMS_ChgEnable = HBMS1_VQ.BMS_ChgEnable,//BMS允许充电
                BMS_DisEnable = HBMS1_VQ.BMS_DisEnable,//BMS允许放电
                BMS_ChgOC = HBMS1_VQ.BMS_ChgOC,//BMS充电过流
                BMS_DisOC = HBMS1_VQ.BMS_DisOC,//BMS放电过流
                BMS_UnderTemp = HBMS1_VQ.BMS_UnderTemp,//BMS温度过低
                BMS_OverTemp = HBMS1_VQ.BMS_OverTemp,//BMS温度过高
                BMS_AvgTemp = HBMS1_VQ.BMS_AvgTemp,//BMS平均温度
                BMS_ChgCurrLimit = HBMS1_VQ.BMS_ChgCurrLimit,//BMS充电电流限制
                BMS_SOC = HBMS1_VQ.BMS_SOC,//BMS当前SOC
                BMS_ChgVoltLimit = HBMS1_VQ.BMS_ChgVoltLimit,//BMS充电电压限制
                BMS_DisVoltLimit = HBMS1_VQ.BMS_DisVoltLimit,//BMS放电电压限制

                FaultCode = HSTS_GB.FaultCode,//故障代码
                PVTemp = HTEMP_PDF.PVTemp,//PV温度
                InvTemp = HTEMP_PDF.InvTemp,//逆变温度
                BoostTemp = HTEMP_PDF.BoostTemp,//升压温度
                XfmrTemp = HTEMP_PDF.XfmrTemp,//变压器温度
                MaxTemp = HTEMP_PDF.MaxTemp,//当前最高温度
                FanSpeed = HTEMP_PDF.FanSpeed,//风扇转速
                FanEnable = HTEMP_PDF.FanEnable,//风扇使能
                Mode = HSTS_GB.Mode,//模式
                PVToLoadAC = HSTS_GB.PVToLoadAC,//AC状态下PV馈能到负载
                OutputStatus = HSTS_GB.OutputStatus,//机器是否有输出
                BattLowAlarm = HSTS_GB.BattLowAlarm,//电池低电报警
                BattDisconnected = HSTS_GB.BattDisconnected,//电池未接
                OutputOverload = HSTS_GB.OutputOverload,//输出过载
                OverTemp = HSTS_GB.OverTemp,//机器过温
                EEPROM_DataErr = HSTS_GB.EEPROM_DataErr,//EEPROM数据异常
                EEPROM_IOErr = HSTS_GB.EEPROM_IOErr,//EEPROM读写异常
                PVLowPwrFault = HSTS_GB.PVLowPwrFault,//PV功率过低异常
                InputOV = HSTS_GB.InputOV,//输入电压过高
                BattOV = HSTS_GB.BattOV,//电池电压过高
                FanSpeedFault = HSTS_GB.FanSpeedFault,//风扇转速异常
                ParallelUnits = HSTS_GB.ParallelUnits,//并机系统里机器的总数
                GridTieFlag = HSTS_GB.GridTieFlag,//并网标志
                ParallelRole = HSTS_GB.ParallelRole,//并机系统中角色
                MainRelayStat = HSTS_GB.MainRelayStat,//主输出继电器状态
                SecOutStat = HSTS_GB.SecOutStat,//第二输出当前状态
                BMS_ComFault = HSTS_GB.BMS_ComFault,//BMS通讯异常
                TempSensorFault = HSTS_GB.TempSensorFault,//温度传感器异常
                ACLED = HSTS_GB.ACLED,//市电灯状态
                InvLED = HSTS_GB.InvLED,//逆变灯状态
                ChgLED = HSTS_GB.ChgLED,//充电灯状态
                AlarmLED = HSTS_GB.AlarmLED//报警灯状态
            };

            // 最新在最前
            Application.Current.Dispatcher.Invoke(() =>
            {
                // 保存
                if (DR_Monitor.IsSaving && DR_Monitor._savePath != null)
                {
                    DR_Monitor.SaveToExcel(Common_Data);
                }

            });
        }
        #endregion

        #region HPVINV08通讯
        /// <summary>
        /// HPVINV08通讯
        /// </summary>
        /// <param name="token"></param>
        private void CommunicationWithGB_HPVINV08(CancellationToken token)
        {

            string receive = "";

            Thread.Sleep(100);
            //判断是否开启CRC接收校验（抗干扰 默认开启
            if (IsChecked)
            {
                //发送HOSTCRCEN指令
                _pauseEvent.Wait(token);
                receive = SerialCommunicationService.SendSettingCommand("HOSTCRC", "EN");
                ShowError(receive, "HOSTCRC");
            }
            else if (OnceOpenCRC)
            {
                //发送HOSTCRDEN指令
                _pauseEvent.Wait(token);
                receive = SerialCommunicationService.SendSettingCommand("HOSTCRC", "DN");
                OnceOpenCRC = false;
                IsChecked = false;
                SerialCommunicationService.OpenReceiveCRC(false);
            }

            //获取机器型号
            Thread.Sleep(100);
            _pauseEvent.Wait(token);
            string receive_MachineType = SerialCommunicationService.SendCommand(SpecialCommand.QueryMachineType, 10);
            MachineType = receive_MachineType.Substring(1, 8);
            SerialCommunicationService.MachineType = receive_MachineType;

            //发送HSTS2指令
            Thread.Sleep(100);
            _pauseEvent.Wait(token); // 等待暂停或取消信号
            receive = SerialCommunicationService.SendCommand(HSTS2_HPVINV08.Command, 40);
            HSTS2_HPVINV08.AnalyseStringToElement(receive);


            //发送HBMS1指令
            Thread.Sleep(100);
            _pauseEvent.Wait(token); // 等待暂停或取消信号
            receive = SerialCommunicationService.SendCommand(HBMS1_VQ.Command, 70);
            HBMS1_VQ.AnalysisStringToElement(receive);


            //发送HEEP1指令
            Thread.Sleep(100);
            _pauseEvent.Wait(token); // 等待暂停或取消信号
            receive = SerialCommunicationService.SendCommand(HEEP1_HPVINV02.Command, 80);
            HEEP1_HPVINV02.AnalyseStringToElement(receive);


            //发送HEEP2指令
            Thread.Sleep(100);
            _pauseEvent.Wait(token); // 等待暂停或取消信号
            receive = SerialCommunicationService.SendCommand(HEEP2.Command, 80);
            HEEP2.AnalyseStringToElement(receive);


            //发送HEEP3_PDF指令
            Thread.Sleep(100);
            _pauseEvent.Wait(token); // 等待暂停或取消信号
            receive = SerialCommunicationService.SendCommand(HEEP3_PDF.Command, 80);
            HEEP3_PDF.AnalysisStringToElement(receive);

            //发送HOP指令
            Thread.Sleep(100);
            // 等待暂停或取消信号
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HOP_PDF.Command, 50);
            //解析返回命令
            HOP_PDF.AnalysisStringToElement(receive);
            //逆变百分比
            InvTotalPwr = StringToIntConversion(HOP_PDF.LoadPercent);

            //发送HPV指令
            Thread.Sleep(100);
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HPV_PDF.Command, 50);
            HPV_PDF.AnalysisStringToElement(receive);
            //MPPT百分比
            MPPTTotalPwr = CountPercent(HPV_PDF.PVPwr, HIGSG2_PDF.MPPTTotalPwr);

            //发送HIMSG2N指令
            Thread.Sleep(100);
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HIGSG2_PDF.Command, 50);
            HIGSG2_PDF.AnalysisStringToElement(receive);


            //发送HGRID指令
            Thread.Sleep(100);
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HGRID_GB.Command, 50);
            //解析返回命令
            HGRID_GB.AnalyseStringToElement(receive);
            //显示
            ACPowerVM = StringToIntConversion(HGRID_GB.ACPower);
            //市电百分比
            ACTotalPwr = CountPercent(HGRID_GB.ACPower, HIGSG2_PDF.ACTotalPwr);


            //发送HTEMP指令
            Thread.Sleep(100);
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HTEMP_PDF.Command, 50);
            HTEMP_PDF.AnalysisStringToElement(receive);


            //发送HBAT指令
            Thread.Sleep(100);
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HBAT_VQ.Command, 50);
            HBAT_VQ.AnalysisStringToElement(receive);
            BattPercent = StringToIntConversion(HBAT_VQ.BattCapacity);


            //发送HIMSG1指令
            Thread.Sleep(100);
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HIMSG1.Command, 21);
            HIMSG1.AnalysisStringToElement(receive);


            //发送HGEN指令
            Thread.Sleep(100);
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HGEN.Command, 60);
            HGEN.AnalyseStringToElement(receive);


            //发送HSTS指令
            Thread.Sleep(100);
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HSTS_GB.Command, 40);
            HSTS_GB.AnalyseStringToElement(receive);


            //发送HPV指令
            Thread.Sleep(100);
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HPV_PDF.Command, 50);
            HPV_PDF.AnalysisStringToElement(receive);


            //发送HPVB指令
            Thread.Sleep(100);
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HPVB_GB.Command, 50);
            HPVB_GB.AnalysisStringToElement(receive);

            //发送HCTMSG1指令
            Thread.Sleep(100);
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HCTMSG1_PDF.Command, 80);
            HCTMSG1_PDF.AnalysisStringToElement(receive);
            ShowError(receive, "HCTMSG1");


            //机器型号
            MachineModel = StringToIntConversion(HOP_PDF.RatedPwr) + StringToIntConversion(HBAT_VQ.BattCells) * 12;

            //数据
            var Common_Data = new Common_Data
            {
                DataNow = DateTime.Now,//日期

                MainsVoltage = HGRID_GB.MainsVoltage,//市电电压
                MainsFrequency = HGRID_GB.MainsFrequency,//市电频率
                ACPower = HGRID_GB.ACPower,//市电功率

                OutVolt = HOP_PDF.OutVolt,//输出电压
                OutFreq = HOP_PDF.OutFreq,//输出频率
                ApparentPwr = HOP_PDF.ApparentPwr,//视在功率
                ActivePwr = HOP_PDF.ActivePwr,//有功功率
                LoadPercent = HOP_PDF.LoadPercent,//负载百分比
                ZeroAdjPwr = HEEP3_PDF.ZeroAdjPwr,//调零功率
                CTCurr = HCTMSG1_PDF.CTCurr,//CT电流
                CTPwr = HCTMSG1_PDF.CTPwr,//CT功率

                PVVolt = HPV_PDF.PVVolt,//PV电压
                PVPwr = HPV_PDF.PVPwr,//PV功率
                PVCurr = HPV_PDF.PVCurr,//PV电流
                TotalGen = HGEN.TotalGen,//总发电量
                DailyGen = HGEN.DailyGen,//日发电量
                MonthlyGen = HGEN.MonthlyGen,//月发电量
                AnnualGen = HGEN.AnnualGen,//年发电量

                BattVolt = HBAT_VQ.BattVolt,//电池电压
                BatCurr = HBAT_VQ.BatCurr, //电池电流
                BattCapacity = HBAT_VQ.BattCapacity,//电池容量
                BusVolt = HBAT_VQ.BusVolt, //母线电压

                ProtocolType = HBMS1_VQ.ProtocolType,//协议类型
                BMS_ComOK = HBMS1_VQ.BMS_ComOK,//BMS通信正常
                BMS_LowBattAlarm = HBMS1_VQ.BMS_LowBattAlarm,//BMS低电报警
                BMS_LowBattFault = HBMS1_VQ.BMS_LowBattFault,//BMS低电故障
                BMS_ChgEnable = HBMS1_VQ.BMS_ChgEnable,//BMS允许充电
                BMS_DisEnable = HBMS1_VQ.BMS_DisEnable,//BMS允许放电
                BMS_ChgOC = HBMS1_VQ.BMS_ChgOC,//BMS充电过流
                BMS_DisOC = HBMS1_VQ.BMS_DisOC,//BMS放电过流
                BMS_UnderTemp = HBMS1_VQ.BMS_UnderTemp,//BMS温度过低
                BMS_OverTemp = HBMS1_VQ.BMS_OverTemp,//BMS温度过高
                BMS_AvgTemp = HBMS1_VQ.BMS_AvgTemp,//BMS平均温度
                BMS_ChgCurrLimit = HBMS1_VQ.BMS_ChgCurrLimit,//BMS充电电流限制
                BMS_SOC = HBMS1_VQ.BMS_SOC,//BMS当前SOC
                BMS_ChgVoltLimit = HBMS1_VQ.BMS_ChgVoltLimit,//BMS充电电压限制
                BMS_DisVoltLimit = HBMS1_VQ.BMS_DisVoltLimit,//BMS放电电压限制

                FaultCode = HSTS_GB.FaultCode,//故障代码
                PVTemp = HTEMP_PDF.PVTemp,//PV温度
                InvTemp = HTEMP_PDF.InvTemp,//逆变温度
                BoostTemp = HTEMP_PDF.BoostTemp,//升压温度
                XfmrTemp = HTEMP_PDF.XfmrTemp,//变压器温度
                MaxTemp = HTEMP_PDF.MaxTemp,//当前最高温度
                FanSpeed = HTEMP_PDF.FanSpeed,//风扇转速
                FanEnable = HTEMP_PDF.FanEnable,//风扇使能
                Mode = HSTS_GB.Mode,//模式
                PVToLoadAC = HSTS_GB.PVToLoadAC,//AC状态下PV馈能到负载
                OutputStatus = HSTS_GB.OutputStatus,//机器是否有输出
                BattLowAlarm = HSTS_GB.BattLowAlarm,//电池低电报警
                BattDisconnected = HSTS_GB.BattDisconnected,//电池未接
                OutputOverload = HSTS_GB.OutputOverload,//输出过载
                OverTemp = HSTS_GB.OverTemp,//机器过温
                EEPROM_DataErr = HSTS_GB.EEPROM_DataErr,//EEPROM数据异常
                EEPROM_IOErr = HSTS_GB.EEPROM_IOErr,//EEPROM读写异常
                PVLowPwrFault = HSTS_GB.PVLowPwrFault,//PV功率过低异常
                InputOV = HSTS_GB.InputOV,//输入电压过高
                BattOV = HSTS_GB.BattOV,//电池电压过高
                FanSpeedFault = HSTS_GB.FanSpeedFault,//风扇转速异常
                ParallelUnits = HSTS_GB.ParallelUnits,//并机系统里机器的总数
                GridTieFlag = HSTS_GB.GridTieFlag,//并网标志
                ParallelRole = HSTS_GB.ParallelRole,//并机系统中角色
                MainRelayStat = HSTS_GB.MainRelayStat,//主输出继电器状态
                SecOutStat = HSTS_GB.SecOutStat,//第二输出当前状态
                BMS_ComFault = HSTS_GB.BMS_ComFault,//BMS通讯异常
                TempSensorFault = HSTS_GB.TempSensorFault,//温度传感器异常
                ACLED = HSTS_GB.ACLED,//市电灯状态
                InvLED = HSTS_GB.InvLED,//逆变灯状态
                ChgLED = HSTS_GB.ChgLED,//充电灯状态
                AlarmLED = HSTS_GB.AlarmLED,//报警灯状态
                InvStatus = HSTS2_HPVINV08.InvStatus,//逆变器工作状态
                PVVoltStatus = HSTS2_HPVINV08.PVVoltStatus,//PV电压状态
                InvBridgeStatus = HSTS2_HPVINV08.InvBridgeStatus,//逆变桥状态
                MPPTStatus = HSTS2_HPVINV08.MPPTStatus,//MPPT状态
                PLLStatus = HSTS2_HPVINV08.PLLStatus//锁相环状态
            };

            // 最新在最前
            Application.Current.Dispatcher.Invoke(() =>
            {

                if (DR_Monitor.IsSaving && DR_Monitor._savePath != null)
                {
                    DR_Monitor.SaveToExcel(Common_Data);
                }

            });
        }
        #endregion

        #region GB6042通讯(HPVINV06)
        /// <summary>
        /// GB6042通讯(HPVINV06)
        /// </summary>
        /// <param name="token"></param>
        private void CommunicationWithGB_HPVINV06(CancellationToken token)
        {
            string receive = string.Empty;


            //判断是否开启CRC接收校验（抗干扰 默认开启
            if (IsChecked)
            {
                //发送HOSTCRCEN指令
                _pauseEvent.Wait(token);
                receive = SerialCommunicationService.SendSettingCommand("HOSTCRC", "EN");
                ShowError(receive, "HOSTCRC");
            }
            else if (OnceOpenCRC)
            {
                //发送HOSTCRDEN指令
                _pauseEvent.Wait(token);
                receive = SerialCommunicationService.SendSettingCommand("HOSTCRC", "DN");
                OnceOpenCRC = false;
                IsChecked = false;
                SerialCommunicationService.OpenReceiveCRC(false);
            }

            Thread.Sleep(100);
            // 等待暂停或取消信号
            _pauseEvent.Wait(token);
            //发送查询机器指令
            string receive_MachineType = SerialCommunicationService.SendCommand(SpecialCommand.QueryMachineType, 10);
            MachineType = receive_MachineType.Substring(1, 8);
            //解析指令
            SerialCommunicationService.MachineType = receive_MachineType;

            //发送HBMS1指令
            _pauseEvent.Wait(token); // 等待暂停或取消信号
            receive = SerialCommunicationService.SendCommand(HBMS1_VQ.Command, 70);
            HBMS1_VQ.AnalysisStringToElement(receive);


            Thread.Sleep(100);
            _pauseEvent.Wait(token); // 等待暂停或取消信号
            //发送HEEP1指令
            string receiveHEEP1 = SerialCommunicationService.SendCommand(HEEP1.Command, 80);
            //解析返回指令
            HEEP1.AnalyseStringToElement(receiveHEEP1);


            Thread.Sleep(100);
            _pauseEvent.Wait(token); // 等待暂停或取消信号
            //发送HEEP2指令
            string receive_HEEP2 = SerialCommunicationService.SendCommand(HEEP2.Command, 80);
            //解析返回指令
            HEEP2.AnalyseStringToElement(receive_HEEP2);


            Thread.Sleep(100);
            // 等待暂停或取消信号
            _pauseEvent.Wait(token);
            //发送HOP指令
            receive = SerialCommunicationService.SendCommand(HOP_PDF.Command, 50);
            //解析返回命令
            HOP_PDF.AnalysisStringToElement(receive);
            //逆变百分比
            InvTotalPwr = StringToIntConversion(HOP_PDF.LoadPercent);

            Thread.Sleep(100);
            //发送HPV指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HPV_PDF.Command, 50);
            HPV_PDF.AnalysisStringToElement(receive);
            //MPPT百分比
            MPPTTotalPwr = CountPercent(HPV_PDF.PVPwr, HIGSG2_PDF.MPPTTotalPwr);

            Thread.Sleep(100);
            //发送HIMSG2N指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HIGSG2_PDF.Command, 50);
            HIGSG2_PDF.AnalysisStringToElement(receive);

            Thread.Sleep(100);
            //发送HGRID指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HGRID_GB.Command, 50);
            //解析返回命令
            HGRID_GB.AnalyseStringToElement(receive);
            //显示
            ACPowerVM = StringToIntConversion(HGRID_GB.ACPower);
            //市电百分比
            ACTotalPwr = CountPercent(HGRID_GB.ACPower, HIGSG2_PDF.ACTotalPwr);

            Thread.Sleep(100);
            //发送HTEMP指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HTEMP_PDF.Command, 50);
            HTEMP_PDF.AnalysisStringToElement(receive);

            Thread.Sleep(100);
            //发送HBAT指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HBAT_VQ.Command, 50);
            HBAT_VQ.AnalysisStringToElement(receive);
            BattPercent = StringToIntConversion(HBAT_VQ.BattCapacity);

            Thread.Sleep(100);
            //发送HIMSG1指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HIMSG1.Command, 21);
            HIMSG1.AnalysisStringToElement(receive);

            Thread.Sleep(100);
            _pauseEvent.Wait(token); // 等待暂停或取消信号
            //发送Hgen指令
            string receive_HGEN = SerialCommunicationService.SendCommand(HGEN.Command, 60);
            //解析返回指令
            HGEN.AnalyseStringToElement(receive_HGEN);

            Thread.Sleep(100);
            //发送HSTS指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HSTS_GB.Command, 40);
            HSTS_GB.AnalyseStringToElement(receive);

            //机器型号
            MachineModel = StringToIntConversion(HOP_PDF.RatedPwr) + StringToIntConversion(HBAT_VQ.BattCells) * 12;

            //数据
            var Common_Data = new Common_Data
            {
                DataNow = DateTime.Now,
                MainsVoltage = HGRID_GB.MainsVoltage,//市电电压
                MainsFrequency = HGRID_GB.MainsFrequency,//市电频率
                ACPower = HGRID_GB.ACPower,//市电功率

                OutVolt = HOP_PDF.OutVolt,//输出电压
                OutFreq = HOP_PDF.OutFreq,//输出频率
                ApparentPwr = HOP_PDF.ApparentPwr,//视在功率
                ActivePwr = HOP_PDF.ActivePwr,//有功功率
                LoadPercent = HOP_PDF.LoadPercent,//负载百分比

                PVVolt = HPV_PDF.PVVolt,//PV电压
                PVPwr = HPV_PDF.PVPwr,//PV功率
                PVCurr = HPV_PDF.PVCurr,//PV电流
                TotalGen = HGEN.TotalGen,//总发电量
                DailyGen = HGEN.DailyGen,//日发电量
                MonthlyGen = HGEN.MonthlyGen,//月发电量
                AnnualGen = HGEN.AnnualGen,//年发电量

                BattVolt = HBAT_VQ.BattVolt,//电池电压
                BatCurr = HBAT_VQ.BatCurr, //电池电流
                BattCapacity = HBAT_VQ.BattCapacity,//电池容量
                BusVolt = HBAT_VQ.BusVolt, //母线电压

                ProtocolType = HBMS1_VQ.ProtocolType,//协议类型
                BMS_ComOK = HBMS1_VQ.BMS_ComOK,//BMS通信正常
                BMS_LowBattAlarm = HBMS1_VQ.BMS_LowBattAlarm,//BMS低电报警
                BMS_LowBattFault = HBMS1_VQ.BMS_LowBattFault,//BMS低电故障
                BMS_ChgEnable = HBMS1_VQ.BMS_ChgEnable,//BMS允许充电
                BMS_DisEnable = HBMS1_VQ.BMS_DisEnable,//BMS允许放电
                BMS_ChgOC = HBMS1_VQ.BMS_ChgOC,//BMS充电过流
                BMS_DisOC = HBMS1_VQ.BMS_DisOC,//BMS放电过流
                BMS_UnderTemp = HBMS1_VQ.BMS_UnderTemp,//BMS温度过低
                BMS_OverTemp = HBMS1_VQ.BMS_OverTemp,//BMS温度过高
                BMS_AvgTemp = HBMS1_VQ.BMS_AvgTemp,//BMS平均温度
                BMS_ChgCurrLimit = HBMS1_VQ.BMS_ChgCurrLimit,//BMS充电电流限制
                BMS_SOC = HBMS1_VQ.BMS_SOC,//BMS当前SOC
                BMS_ChgVoltLimit = HBMS1_VQ.BMS_ChgVoltLimit,//BMS充电电压限制
                BMS_DisVoltLimit = HBMS1_VQ.BMS_DisVoltLimit,//BMS放电电压限制

                FaultCode = HSTS_GB.FaultCode,//故障代码
                PVTemp = HTEMP_PDF.PVTemp,//PV温度
                InvTemp = HTEMP_PDF.InvTemp,//逆变温度
                BoostTemp = HTEMP_PDF.BoostTemp,//升压温度
                XfmrTemp = HTEMP_PDF.XfmrTemp,//变压器温度
                MaxTemp = HTEMP_PDF.MaxTemp,//当前最高温度
                FanSpeed = HTEMP_PDF.FanSpeed,//风扇转速
                FanEnable = HTEMP_PDF.FanEnable,//风扇使能
                Mode = HSTS_GB.Mode,//模式
                PVToLoadAC = HSTS_GB.PVToLoadAC,//AC状态下PV馈能到负载
                OutputStatus = HSTS_GB.OutputStatus,//机器是否有输出
                BattLowAlarm = HSTS_GB.BattLowAlarm,//电池低电报警
                BattDisconnected = HSTS_GB.BattDisconnected,//电池未接
                OutputOverload = HSTS_GB.OutputOverload,//输出过载
                OverTemp = HSTS_GB.OverTemp,//机器过温
                EEPROM_DataErr = HSTS_GB.EEPROM_DataErr,//EEPROM数据异常
                EEPROM_IOErr = HSTS_GB.EEPROM_IOErr,//EEPROM读写异常
                PVLowPwrFault = HSTS_GB.PVLowPwrFault,//PV功率过低异常
                InputOV = HSTS_GB.InputOV,//输入电压过高
                BattOV = HSTS_GB.BattOV,//电池电压过高
                FanSpeedFault = HSTS_GB.FanSpeedFault,//风扇转速异常
                ParallelUnits = HSTS_GB.ParallelUnits,//并机系统里机器的总数
                GridTieFlag = HSTS_GB.GridTieFlag,//并网标志
                ParallelRole = HSTS_GB.ParallelRole,//并机系统中角色
                MainRelayStat = HSTS_GB.MainRelayStat,//主输出继电器状态
                SecOutStat = HSTS_GB.SecOutStat,//第二输出当前状态
                BMS_ComFault = HSTS_GB.BMS_ComFault,//BMS通讯异常
                TempSensorFault = HSTS_GB.TempSensorFault,//温度传感器异常
                ACLED = HSTS_GB.ACLED,//市电灯状态
                InvLED = HSTS_GB.InvLED,//逆变灯状态
                ChgLED = HSTS_GB.ChgLED,//充电灯状态
                AlarmLED = HSTS_GB.AlarmLED//报警灯状态
            };

            // 最新在最前
            Application.Current.Dispatcher.Invoke(() =>
            {

                if (DR_Monitor.IsSaving && DR_Monitor._savePath != null)
                {
                    DR_Monitor.SaveToExcel(Common_Data);
                }

            });
        }
        #endregion

        #region CYJ通讯(UPSCYX01)
        /// <summary>
        /// CYJ通讯(UPSCYX01)
        /// </summary>
        /// <param name="token"></param>
        private void CommunicationWith_CYJ(CancellationToken token)
        {
            string receive = string.Empty;

            //判断是否开启CRC接收校验（抗干扰 默认开启
            if (IsChecked)
            {
                //发送HOSTCRCEN指令
                _pauseEvent.Wait(token);
                receive = SerialCommunicationService.SendSettingCommand("HOSTCRC", "EN");
                ShowError(receive, "HOSTCRC");
            }
            else if (OnceOpenCRC)
            {
                //发送HOSTCRDEN指令
                _pauseEvent.Wait(token);
                receive = SerialCommunicationService.SendSettingCommand("HOSTCRC", "DN");
                OnceOpenCRC = false;
                IsChecked = false;
                SerialCommunicationService.OpenReceiveCRC(false);
            }

            Thread.Sleep(200);
            // 等待暂停或取消信号
            _pauseEvent.Wait(token);
            //发送查询机器指令
            string receive_MachineType = SerialCommunicationService.SendCommand(SpecialCommand.QueryMachineType, 10);
            MachineType = receive_MachineType.Substring(1, 8);
            //解析指令
            SerialCommunicationService.MachineType = receive_MachineType;

            Thread.Sleep(200);
            //发送HGRID指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HGRID_CYJ.Command, 50);
            //解析返回命令
            HGRID_CYJ.AnalyseStringToElement(receive);
            //显示
            ACPowerVM = StringToIntConversion(HGRID_CYJ.ACPower);
            //市电百分比
            ACTotalPwr = CountPercent(HGRID_CYJ.ACPower, HIGSG2_PDF.ACTotalPwr);

            Thread.Sleep(200);
            // 等待暂停或取消信号
            _pauseEvent.Wait(token);
            //发送HOP指令
            receive = SerialCommunicationService.SendCommand(HOP_PDF.Command, 50);
            //解析返回命令
            HOP_PDF.AnalysisStringToElement(receive);
            //逆变百分比
            InvTotalPwr = StringToIntConversion(HOP_PDF.LoadPercent);

            Thread.Sleep(200);
            //发送HBAT指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HBAT_VQ2.Command, 50);
            HBAT_VQ2.AnalysisStringToElement(receive);
            BattPercent = StringToIntConversion(HBAT_VQ2.BattCapacity);

            Thread.Sleep(200);
            //发送HSTS指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HSTS_CYJ.Command, 40);
            HSTS_CYJ.AnalyseStringToElement(receive);

            Thread.Sleep(200);
            //发送HTEMP指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HTEMP_PDF.Command, 50);
            HTEMP_PDF.AnalysisStringToElement(receive);
            ShowError(receive, "HTEMP");

            Thread.Sleep(200);
            //发送HIMSG1指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HIMSG1.Command, 21);
            HIMSG1.AnalysisStringToElement(receive);
            ShowError(receive, "HIMSG1");

            Thread.Sleep(200);
            //发送HEEP1指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HEEP1_CYJ.Command, 80);
            HEEP1_CYJ.AnalyseStringToElement(receive);

            //数据
            var Common_Data = new Common_Data
            {
                DataNow = DateTime.Now,
                MainsVoltage = HGRID_GB.MainsVoltage,//市电电压
                MainsFrequency = HGRID_GB.MainsFrequency,//市电频率

                OutVolt = HOP_PDF.OutVolt,//输出电压
                OutFreq = HOP_PDF.OutFreq,//输出频率
                ApparentPwr = HOP_PDF.ApparentPwr,//视在功率
                ActivePwr = HOP_PDF.ActivePwr,//有功功率
                LoadPercent = HOP_PDF.LoadPercent,//负载百分比
                RatedPwr = HOP_PDF.RatedPwr,//满载有功功率
                InductorCurr = HOP_PDF.InductorCurr,//电感电流

                BoostTime = HEEP1_CYJ.BoostTime,//强充时间
                InvTime = HEEP1_CYJ.InvTime,//逆变时间
                LoadTime = HEEP1_CYJ.LoadTime, //带载时间

                BattVolt = HBAT_VQ.BattVolt,//电池电压
                BattCapacity = HBAT_VQ.BattCapacity,//电池容量
                BattCells = HBAT_VQ2.BattCells, //电池节数

                FaultCode = HSTS_GB.FaultCode,//故障代码
                Mode = HSTS_CYJ.Mode,//模式
                InvTemp = HTEMP_PDF.InvTemp,//逆变温度
                MaxTemp = HTEMP_PDF.MaxTemp,//当前最高温度
                BattDisconnected = HSTS_CYJ.BattDisconnected,//电池未接
                OutputOverload = HSTS_CYJ.OutputOverload,//输出过载
                OverTemp = HSTS_CYJ.OverTemp,//机器过温
                BattLowAlarm = HSTS_CYJ.BattLowAlarm,//电池低电报警
                EEPROM_DataErr = HSTS_CYJ.EEPROM_DataErr,//EEPROM数据异常
                EEPROM_IOErr = HSTS_CYJ.EEPROM_IOErr,//EEPROM读写异常
                InputOV = HSTS_CYJ.InputOV,//输入电压过高
                BattOV = HSTS_CYJ.BattOV,//电池电压过高
                FanSpeedFault = HSTS_CYJ.FanSpeedFault,//风扇转速异常
                OutputStatus = HSTS_CYJ.OutputStatus//机器是否有输出

            };

            // 最新在最前
            Application.Current.Dispatcher.Invoke(() =>
            {

                if (DR_Monitor.IsSaving && DR_Monitor._savePath != null)
                {
                    DR_Monitor.SaveToExcel(Common_Data);
                }

            });

        }
        #endregion

        #region LB6通讯
        /// <summary>
        /// LB6通讯
        /// </summary>
        /// <param name="token"></param>
        private void CommunicationWith_LB6(CancellationToken token)
        {
            string receive = string.Empty;

            //判断是否开启CRC接收校验（抗干扰 默认开启
            if (IsChecked)
            {
                //发送HOSTCRCEN指令
                _pauseEvent.Wait(token);
                receive = SerialCommunicationService.SendSettingCommand("HOSTCRC", "EN");
                ShowError(receive, "HOSTCRC");
            }
            else if (OnceOpenCRC)
            {
                //发送HOSTCRDEN指令
                _pauseEvent.Wait(token);
                receive = SerialCommunicationService.SendSettingCommand("HOSTCRC", "DN");
                OnceOpenCRC = false;
                IsChecked = false;
                SerialCommunicationService.OpenReceiveCRC(false);
            }

            Thread.Sleep(200);
            // 等待暂停或取消信号
            _pauseEvent.Wait(token);
            //发送查询机器指令
            string receive_MachineType = SerialCommunicationService.SendCommand(SpecialCommand.QueryMachineType, 10);
            MachineType = receive_MachineType.Substring(1, 8);
            //解析指令
            SerialCommunicationService.MachineType = receive_MachineType;

            Thread.Sleep(200);
            //发送HGRID指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HGRID_CYJ.Command, 50);
            //解析返回命令
            HGRID_CYJ.AnalyseStringToElement(receive);
            //显示
            ACPowerVM = StringToIntConversion(HGRID_CYJ.ACPower);
            //市电百分比
            ACTotalPwr = CountPercent(HGRID_CYJ.ACPower, HIGSG2_PDF.ACTotalPwr);

            Thread.Sleep(200);
            // 等待暂停或取消信号
            _pauseEvent.Wait(token);
            //发送HOP指令
            receive = SerialCommunicationService.SendCommand(HOP_PDF.Command, 50);
            //解析返回命令
            HOP_PDF.AnalysisStringToElement(receive);
            //逆变百分比
            InvTotalPwr = StringToIntConversion(HOP_PDF.LoadPercent);

            Thread.Sleep(200);
            //发送HBAT指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HBAT_VQ2.Command, 50);
            HBAT_VQ2.AnalysisStringToElement(receive);
            BattPercent = StringToIntConversion(HBAT_VQ2.BattCapacity);

            Thread.Sleep(200);
            //发送HSTS指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HSTS_CYJ.Command, 40);
            HSTS_CYJ.AnalyseStringToElement(receive);

            Thread.Sleep(200);
            //发送HTEMP指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HTEMP_PDF.Command, 50);
            HTEMP_PDF.AnalysisStringToElement(receive);
            ShowError(receive, "HTEMP");

            Thread.Sleep(200);
            //发送HIMSG1指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HIMSG1.Command, 21);
            HIMSG1.AnalysisStringToElement(receive);
            ShowError(receive, "HIMSG1");

            Thread.Sleep(200);

            //发送HEEP1指令
            _pauseEvent.Wait(token);
            receive = SerialCommunicationService.SendCommand(HEEP1_LB6.Command, 80);
            HEEP1_LB6.AnalyseStringToElement(receive);

            //数据
            var Common_Data = new Common_Data
            {
                DataNow = DateTime.Now,//日期

                MainsVoltage = HGRID_GB.MainsVoltage,//市电电压
                MainsFrequency = HGRID_GB.MainsFrequency,//市电频率

                OutVolt = HOP_PDF.OutVolt,//输出电压
                OutFreq = HOP_PDF.OutFreq,//输出频率
                ApparentPwr = HOP_PDF.ApparentPwr,//视在功率
                ActivePwr = HOP_PDF.ActivePwr,//有功功率
                LoadPercent = HOP_PDF.LoadPercent,//负载百分比
                RatedPwr = HOP_PDF.RatedPwr,//满载有功功率
                InductorCurr = HOP_PDF.InductorCurr,//电感电流

                BoostTime = HEEP1_CYJ.BoostTime,//强充时间
                InvTime = HEEP1_CYJ.InvTime,//逆变时间

                BattVolt = HBAT_VQ.BattVolt,//电池电压
                BatCurr = HBAT_VQ.BatCurr, //电池电流
                BattCells = HBAT_VQ2.BattCells,//电池节数
                BattCapacity = HBAT_VQ.BattCapacity,//电池容量
                BusVolt = HBAT_VQ.BusVolt, //母线电压
                BattChgCurr = HBAT_VQ2.BattChgCurr,//电池充电电流
                BatteryType = HEEP1_LB6.BatteryType,//电池类型

                InvTemp = HTEMP_PDF.InvTemp,//逆变温度
                MaxTemp = HTEMP_PDF.MaxTemp,//当前最高温度
                FaultCode = HSTS_GB.FaultCode,//故障代码
                Mode = HSTS_GB.Mode,//模式
                OutputStatus = HSTS_GB.OutputStatus,//机器是否有输出
                BattLowAlarm = HSTS_GB.BattLowAlarm,//电池低电报警
                BattDisconnected = HSTS_GB.BattDisconnected,//电池未接
                OutputOverload = HSTS_GB.OutputOverload,//输出过载
                OverTemp = HSTS_GB.OverTemp,//机器过温
                EEPROM_DataErr = HSTS_GB.EEPROM_DataErr,//EEPROM数据异常
                EEPROM_IOErr = HSTS_GB.EEPROM_IOErr,//EEPROM读写异常
                InputOV = HSTS_GB.InputOV,//输入电压过高
                BattOV = HSTS_GB.BattOV,//电池电压过高
                FanSpeedFault = HSTS_GB.FanSpeedFault,//风扇转速异常
                AutoStartEnable = HEEP1_CYJ.AutoStartEnable,//自动开机使能
                ChgStage = HEEP1_LB6.ChgStage//充电阶段

            };

            // 最新在最前
            Application.Current.Dispatcher.Invoke(() =>
            {

                if (DR_Monitor.IsSaving && DR_Monitor._savePath != null)
                {
                    DR_Monitor.SaveToExcel(Common_Data);
                }

            });

        }
        #endregion

        #region BMS02通讯
        int flag = 0;
        /// <summary>
        /// BMS02通讯
        /// </summary>
        /// <param name="token"></param>
        private void CommunicationWithBMS02(CancellationToken token)
        {

            byte[] receive; // 接收到的原始字节数据
            short[] data; // 解析后的寄存器数据（16位整数数组）

            // 模式1：读取BMS基本信息和状态
            if (SelectedMode == BatteryMode.Mode1)
            {
                Thread.Sleep(200);
                // 等待暂停或取消信号（支持暂停/恢复机制）
                _pauseEvent.Wait(token);
                // 发送查询机器类型指令
                string receive_MachineType = SerialCommunicationService.SendCommand(SpecialCommand.QueryMachineType, 10);
                if (receive_MachineType != null && receive_MachineType.Length == 10)
                {
                    MachineType = receive_MachineType.Substring(1, 8);// 提取机器类型（跳过起始字节）
                                                                      //解析指令
                    SerialCommunicationService.MachineType = receive_MachineType;// 存储机器类型
                }


                //首界面设置状态显示
                // 等待暂停或取消信号
                _pauseEvent.Wait(token);

                //读写入的参数设置值(充电MOS、放电MOS、关机、休眠)
                Thread.Sleep(200);
                // 构建读取指令：从站地址=1, 读取寄存器120-121（强制开关和强制均衡）
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame(1, 120, 2), 9);
                data = ModbusRTU.ParseRead03Response(receive);
                if (data != null && data.Length >= 2)
                {
                    // 将16位寄存器值转换为位数组（每个位代表一个状态标志）
                    BMS_Setting.SettingStatue = ModbusRTU.GetBits(data[0]);// 设置状态
                    BMS_Setting.JunHen = ModbusRTU.GetBits(data[1]); // 均衡状态
                }

                // 读取电芯数量和温度传感器数量
                _pauseEvent.Wait(token);
                Thread.Sleep(200);
                // 读取寄存器250-251（电芯数量和NTC数量）
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame(1, 250, 2), 9);
                //解析返回的报文
                data = ModbusRTU.ParseRead03Response(receive);
                if (data != null && data.Length == 2)
                {
                    BMS_Setting.CellNum = data[0];// 电芯数量
                    BMS_Setting.NtcNum = data[1];// 温度传感器数量
                }

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //读取寄存器2-17 查是16个电芯的电压
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame(1, 2, 16), 37);
                //解析返回的报文
                BMS_VM.MOD_CELL1_VOL_1_16(ModbusRTU.ParseRead03Response(receive));

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //读取状态码(83)
                Thread.Sleep(200);
                // 读取寄存器80-84
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame(1, 80, 5), 15);
                data = ModbusRTU.ParseRead03Response(receive);
                if (data != null)
                {
                    // 第4个寄存器（索引3）为设备状态，转换为位数组
                    BMS_VM.MOD_INST_STATE_Set(ModbusRTU.GetBits(data[3]));

                    //查告警(80)、保护(81)、硬件错误(82)信息
                    BMS_VM.MOD_WARN_STATE_Set(ModbusRTU.GetBits(data[0]));
                    BMS_VM.MOD_PROT_STATE_Set(ModbusRTU.GetBits(data[1]));
                    BMS_VM.MOD_ERROR_STATE_Set(ModbusRTU.GetBits(data[2]));
                }

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //查电压  //查当前电流 //查温度
                Thread.Sleep(200);
                // 读取寄存器18-35
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame(1, 18, 18), 41);
                data = ModbusRTU.ParseRead03Response(receive);
                BMS_VM.OverViewSet(data); // 更新概览数据到ViewModel

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //查系统信息
                Thread.Sleep(200);
                // 读取寄存器283-296
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame(1, 283, 14), 33);
                data = ModbusRTU.ParseRead03Response(receive);
                BMS_VM.SystemInfoSet(data); // 更新系统信息到ViewModel

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //AFE_Protect
                Thread.Sleep(200);
                // 读取寄存器86（AFE保护状态）
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame(1, 86, 1), 7);
                if (receive != null && receive.Length > 1)
                {
                    data = ModbusRTU.ParseRead03Response(receive);
                    if (data != null)
                    {
                        BMS_VM.AFE_Protect = ModbusRTU.GetBits(data[0]);// AFE保护状态位
                    }
                }

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //读写入的参数设置值
                Thread.Sleep(300);
                // 读取系统设置（寄存器297，6个寄存器）
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame(1, 297, 6), 17);
                short[] registers = ModbusRTU.ParseRead03Response(receive);
                if (registers != null && registers.Length >= 3)
                {
                    // 从 registers 还原为 6 字节（小端序组合）
                    byte[] bluetoothBytes = new byte[6];
                    bluetoothBytes[0] = (byte)registers[0];       // 寄存器297
                    bluetoothBytes[1] = (byte)registers[1];       
                    bluetoothBytes[2] = (byte)registers[2];      
                    bluetoothBytes[3] = (byte)registers[3];       
                    bluetoothBytes[4] = (byte)registers[4];       
                    bluetoothBytes[5] = (byte)registers[5];       //寄存器302

                    // 格式化为 "XX:XX:XX:XX:XX:XX"
                    string bluetoothStr = string.Format("{0:X2}:{1:X2}:{2:X2}:{3:X2}:{4:X2}:{5:X2}",
                        bluetoothBytes[0], bluetoothBytes[1], bluetoothBytes[2],
                        bluetoothBytes[3], bluetoothBytes[4], bluetoothBytes[5]);

                    BMS_Setting.setBuletooth(bluetoothStr);
                }
                else
                {
                    // 处理读取失败，例如显示错误信息
                    AddLog("读取蓝牙地址失败");
                }

            }
            // 模式2：读取设置项并进行初始化设置
            else if (SelectedMode == BatteryMode.Mode2)
            {
                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //发送03功能码(查是91个设置项的电压)
                Thread.Sleep(500);
                // 读取112个设置项（寄存器130-241）
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame(1, 130, 112), 229);
                ModbusRTU.AnalyseSetReceive(ModbusRTU.ParseRead03Response(receive), BMS_Setting.SendingCommands);

                // 首次运行时进行初始化设置
                if (flag == 0)
                {
                    //发送03功能码(查是91个设置项的电压)
                    Thread.Sleep(200);
                    // 再次读取设置项以确保数据准确
                    receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame(1, 130, 112), 229);
                    ModbusRTU.AnalyseSetReceive(ModbusRTU.ParseRead03Response(receive), BMS_Setting.SendingCommands);
                    //初始化设置值
                    ModbusRTU.FirstSetReceive(BMS_Setting.SendingCommands);
                    flag = 1;// 标记已初始化
                }
            }
            // 模式6：读取前端芯片监控数据
            else if (SelectedMode == BatteryMode.Mode6)
            {
                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //查前端芯片
                Thread.Sleep(500);
                // 读取前端芯片数据（寄存器320-355，36个寄存器）
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame(1, 320, 36), 77);
                BMS_Setting.SetFrontMonitor(ModbusRTU.ParseRead03Response(receive));
            }
            // 模式3：读取系统设置和概览信息
            else if (SelectedMode == BatteryMode.Mode3)
            {
                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //读写入的参数设置值
                Thread.Sleep(300);
                // 读取系统设置（寄存器252-255，4个寄存器）
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame(1, 252, 4), 13);
                BMS_Setting.setSystem(ModbusRTU.ParseRead03Response(receive));

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //查电压  //查当前电流 //查温度
                Thread.Sleep(300);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame(1, 18, 18), 41);
                data = ModbusRTU.ParseRead03Response(receive);
                BMS_VM.OverViewSet(data);

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //读写入的参数设置值
                Thread.Sleep(300);
                // 读取系统设置（寄存器297，3个寄存器）
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame(1, 297, 6), 17);
                short[] registers = ModbusRTU.ParseRead03Response(receive);
                if (registers != null && registers.Length >= 3)
                {
                    // 从 registers 还原为 6 字节（小端序组合）
                    byte[] bluetoothBytes = new byte[6];
                    bluetoothBytes[0] = (byte)registers[0];       // 寄存器297
                    bluetoothBytes[1] = (byte)registers[1];
                    bluetoothBytes[2] = (byte)registers[2];
                    bluetoothBytes[3] = (byte)registers[3];
                    bluetoothBytes[4] = (byte)registers[4];
                    bluetoothBytes[5] = (byte)registers[5];

                    // 格式化为 "XX:XX:XX:XX:XX:XX"
                    string bluetoothStr = string.Format("{0:X2}:{1:X2}:{2:X2}:{3:X2}:{4:X2}:{5:X2}",
                        bluetoothBytes[0], bluetoothBytes[1], bluetoothBytes[2],
                        bluetoothBytes[3], bluetoothBytes[4], bluetoothBytes[5]);

                    BMS_Setting.setBuletooth(bluetoothStr);
                }
                else
                {
                    // 处理读取失败，例如显示错误信息
                    AddLog("读取蓝牙地址失败");
                }
            }
            // 模式4：仅读取电芯和温度传感器数量
            else if (SelectedMode == BatteryMode.Mode4)
            {
                // 等待暂停或取消信号
                _pauseEvent.Wait(token);

                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame(1, 250, 2), 9);
                //解析返回的报文
                data = ModbusRTU.ParseRead03Response(receive);
                if (data != null && data.Length == 2)
                {
                    BMS_Setting.CellNum = data[0];
                    BMS_Setting.NtcNum = data[1];
                }
                Thread.Sleep(500);
            }

            // 模式5：实时监控模式（读取数据并保存到列表和Excel）
            else if (SelectedMode == BatteryMode.Mode5)     //实时监控
            {
                //首界面设置状态显示
                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //读写入的参数设置值(充电MOS、放电MOS、关机、休眠)
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame(1, 120, 2), 9);
                data = ModbusRTU.ParseRead03Response(receive);
                if (data != null && data.Length >= 2)
                {
                    BMS_Setting.SettingStatue = ModbusRTU.GetBits(data[0]);
                    BMS_Setting.JunHen = ModbusRTU.GetBits(data[1]);
                }

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame(1, 250, 2), 9);
                //解析返回的报文
                data = ModbusRTU.ParseRead03Response(receive);
                if (data != null && data.Length == 2)
                {
                    BMS_Setting.CellNum = data[0];
                    BMS_Setting.NtcNum = data[1];
                }

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //发送03功能码(查是16个电芯的电压)
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame(1, 2, 16), 37);
                //解析返回的报文
                BMS_VM.MOD_CELL1_VOL_1_16(ModbusRTU.ParseRead03Response(receive));

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //查五个状态码(83)
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame(1, 80, 5), 15);
                data = ModbusRTU.ParseRead03Response(receive);
                if (data != null)
                {
                    BMS_VM.MOD_INST_STATE_Set(ModbusRTU.GetBits(data[3]));

                    //查告警(80)、保护(81)、硬件错误(82)信息
                    BMS_VM.MOD_WARN_STATE_Set(ModbusRTU.GetBits(data[0]));
                    BMS_VM.MOD_PROT_STATE_Set(ModbusRTU.GetBits(data[1]));
                    BMS_VM.MOD_ERROR_STATE_Set(ModbusRTU.GetBits(data[2]));
                }

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //查电压  //查当前电流 //查温度
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame(1, 18, 18), 41);
                data = ModbusRTU.ParseRead03Response(receive);
                BMS_VM.OverViewSet(data);

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //查系统信息
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame(1, 283, 14), 33);
                data = ModbusRTU.ParseRead03Response(receive);
                BMS_VM.SystemInfoSet(data);

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //AFE_Protect
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame(1, 86, 1), 7);
                if (receive != null && receive.Length > 1)
                {
                    data = ModbusRTU.ParseRead03Response(receive);
                    if (data != null)
                    {
                        BMS_VM.AFE_Protect = ModbusRTU.GetBits(data[0]);
                    }
                }

                var polling = new PollingData
                {
                    Date = DateTime.Now,                                       //日期
                    TotalVolt = BMS_VM.MOD_AFECOL_PACKVOL,                     //总电压
                    Current = BMS_VM.MOD_AFECOL_CUR,                           //电流
                    SOC = BMS_VM.MOD_SOC.ToString(),                           //SOC
                    SOH = BMS_VM.MOD_SOH.ToString(),                           //SOH
                    FullCap = (BMS_VM.MOD_FULL_CAP / 100.0).ToString("F2"),                             //满充容量
                    FullRemainCap = (BMS_VM.MOD_RES_CAP / 100.0).ToString("F2"),                        //剩余容量
                    CycleCount = BMS_VM.MOD_CYCLECNT,                          //循环次数
                    Cell1 = BMS_VM.MOD_CELL1_VOL,                              //电芯1
                    Cell2 = BMS_VM.MOD_CELL2_VOL,
                    Cell3 = BMS_VM.MOD_CELL3_VOL,
                    Cell4 = BMS_VM.MOD_CELL4_VOL,
                    AvgVolt = BMS_VM.MOD_CELL_VOLDIFF,                         //最大电芯压差
                    MaxVolt = BMS_VM.MOD_MAXCELL_VOL,                          //最高电压
                    MinVolt = BMS_VM.MOD_MINCELL_VOL,                          //最低电压
                    Temp1 = BMS_VM.MOD_GROUD1_TEMP,                            //电芯温度1
                    Chg_MOS = BMS_VM.MOD_INST_STATE[0].ToString(),             //充电MOS
                    Dis_MOS = BMS_VM.MOD_INST_STATE[1].ToString(),             //放电MOS
                    Chg_Statues = BMS_VM.MOD_INST_STATE[4].ToString(),         //充电
                    Dis_Statues = BMS_VM.MOD_INST_STATE[5].ToString(),         //放电
                    AFE_OverChg_Pro = BMS_VM.AFE_Protect[3].ToString(),        //AFE过充保护
                    AFE_OverDis_Pro = BMS_VM.AFE_Protect[4].ToString(),        //AFE过放保护
                    Chg_Current_Pro = BMS_VM.AFE_Protect[5].ToString(),        //充电过流保护
                    Dis_Current_Pro = BMS_VM.AFE_Protect[6].ToString(),        //放电过流保护
                    AFE_Interrupt = BMS_VM.AFE_Protect[2].ToString(),          //前端芯片中断
                    ShortProtect = BMS_VM.AFE_Protect[7].ToString(),           //短路保护
                    AFE_TriggerProt = BMS_VM.AFE_Protect[0].ToString(),        //前端芯片触发保护
                    AFE_AlertPull = BMS_VM.AFE_Protect[1].ToString(),          //前端芯片告警下拉
                    BalanceStatus = BMS_Setting.JunHen[0].ToString()
                    + ";" + BMS_Setting.JunHen[1].ToString() + ";"
                    + BMS_Setting.JunHen[2].ToString() + ";"
                    + BMS_Setting.JunHen[3].ToString(),                        //均衡状态
                    AlarmStatus = BMS_VM.MOD_WARN_STATE,                      //告警信息
                    ProtectStatus = BMS_VM.MOD_PROT_STATE,                     //保护信息
                    ErrorStatus = BMS_VM.MOD_ERROR_STATE,                      //错误信息
                };


                // 最新在最前
                Application.Current.Dispatcher.Invoke(() =>
                {

                    // 添加到界面
                    RT_Monitor.PollingList.Insert(0, polling);

                    // 保证最多 100 条
                    if (RT_Monitor.PollingList.Count > 100)
                        RT_Monitor.PollingList.RemoveAt(RT_Monitor.PollingList.Count - 1);

                    // 保存
                    if (RT_Monitor._isSaving && RT_Monitor._savePath != null)
                        RT_Monitor.SaveToExcel(polling);
                });
            }

        }
        #endregion

        #region BMS01通讯
        /// <summary>
        /// BMS01通讯
        /// </summary>
        /// <param name="token"></param>
        private void CommunicationWithBMS01(CancellationToken token)
        {

            byte[] receive;
            short[] data;
            int i = 1;
            if (SelectedMode == BatteryMode.Mode1)
            {
                Thread.Sleep(200);
                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //发送查询机器指令
                string receive_MachineType = SerialCommunicationService.SendCommand(SpecialCommand.QueryMachineType, 10);
                if (receive_MachineType != null && receive_MachineType.Length == 10)
                    MachineType = receive_MachineType.Substring(1, 8);
                //解析指令
                SerialCommunicationService.MachineType = receive_MachineType;

                //首界面设置状态显示
                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //读写入的参数设置值(充电MOS、放电MOS、关机、休眠)
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 120, 2), 9);
                data = ModbusRTU.ParseRead03Response(receive);
                if (data != null && data.Length >= 2)
                {
                    BMS_Setting.SettingStatue = ModbusRTU.GetBits(data[0]);
                    BMS_Setting.JunHen = ModbusRTU.GetBits(data[1]);
                }

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 250, 2), 9);
                //解析返回的报文
                data = ModbusRTU.ParseRead03Response(receive);
                if (data != null && data.Length == 2)
                {
                    BMS_Setting.CellNum = data[0];
                    BMS_Setting.NtcNum = data[1];
                }

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //发送03功能码(查是16个电芯的电压)
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 2, 16), 37);
                //解析返回的报文
                BMS_VM.MOD_CELL1_VOL_1_16(ModbusRTU.ParseRead03Response(receive));

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //查五个状态码(83)
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 80, 5), 15);
                data = ModbusRTU.ParseRead03Response(receive);
                if (data != null)
                {
                    BMS_VM.MOD_INST_STATE_Set(ModbusRTU.GetBits(data[3]));

                    //查告警(80)、保护(81)、硬件错误(82)信息
                    BMS_VM.MOD_WARN_STATE_Set_BMS01(ModbusRTU.GetBits(data[0]));
                    BMS_VM.MOD_PROT_STATE_Set_BMS01(ModbusRTU.GetBits(data[1]));
                    BMS_VM.MOD_ERROR_STATE_Set_BMS01(ModbusRTU.GetBits(data[2]));
                }

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //查电压  //查当前电流 //查温度
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 18, 18), 41);
                data = ModbusRTU.ParseRead03Response(receive);
                BMS_VM.OverViewSet(data);

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //查系统信息
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 283, 14), 33);
                data = ModbusRTU.ParseRead03Response(receive);
                BMS_VM.SystemInfoSet(data);

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //AFE_Protect
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 86, 1), 7);
                if (receive != null && receive.Length > 1)
                {
                    data = ModbusRTU.ParseRead03Response(receive);
                    if (data != null)
                    {
                        BMS_VM.AFE_Protect = ModbusRTU.GetBits(data[0]);
                    }
                }

            }
            else if (SelectedMode == BatteryMode.Mode2)
            {
                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //发送03功能码(查是91个设置项的电压)
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 130, 112), 229);
                ModbusRTU.AnalyseSetReceive(ModbusRTU.ParseRead03Response(receive), BMS_Setting.SendingCommands);
                if (flag == 0)
                {
                    //发送03功能码(查是91个设置项的电压)
                    Thread.Sleep(200);
                    receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 130, 112), 229);
                    ModbusRTU.AnalyseSetReceive(ModbusRTU.ParseRead03Response(receive), BMS_Setting.SendingCommands);
                    //初始化设置值
                    ModbusRTU.FirstSetReceive(BMS_Setting.SendingCommands);
                    flag = 1;
                }
            }
            else if (SelectedMode == BatteryMode.Mode6)
            {
                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //查前端芯片
                Thread.Sleep(500);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 320, 36), 77);
                BMS_Setting.SetFrontMonitor(ModbusRTU.ParseRead03Response(receive));
            }
            else if (SelectedMode == BatteryMode.Mode3)
            {
                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //读写入的参数设置值
                Thread.Sleep(600);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 252, 4), 13);
                BMS_Setting.setSystem(ModbusRTU.ParseRead03Response(receive));

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //查电压  //查当前电流 //查温度
                Thread.Sleep(300);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 18, 18), 41);
                data = ModbusRTU.ParseRead03Response(receive);
                BMS_VM.OverViewSet(data);

                _pauseEvent.Wait(token);
                //查当前系统时间
                Thread.Sleep(300);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 110, 3), 11);
                data = ModbusRTU.ParseRead03Response(receive);
                BMS_Setting.setSystemTime(data);

            }
            else if (SelectedMode == BatteryMode.Mode4)
            {
                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                Thread.Sleep(500);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 250, 2), 9);
                //解析返回的报文
                data = ModbusRTU.ParseRead03Response(receive);
                if (data != null && data.Length == 2)
                {
                    BMS_Setting.CellNum = data[0];
                    BMS_Setting.NtcNum = data[1];
                }
            }
            else if (SelectedMode == BatteryMode.Mode5)     //实时监控
            {
                //首界面设置状态显示
                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //读写入的参数设置值(充电MOS、放电MOS、关机、休眠)
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 120, 2), 9);
                data = ModbusRTU.ParseRead03Response(receive);
                if (data != null && data.Length >= 2)
                {
                    BMS_Setting.SettingStatue = ModbusRTU.GetBits(data[0]);
                    BMS_Setting.JunHen = ModbusRTU.GetBits(data[1]);
                }

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 250, 2), 9);
                //解析返回的报文
                data = ModbusRTU.ParseRead03Response(receive);
                if (data != null && data.Length == 2)
                {
                    BMS_Setting.CellNum = data[0];
                    BMS_Setting.NtcNum = data[1];
                }

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //发送03功能码(查是16个电芯的电压)
                Thread.Sleep(100);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 2, 16), 37);
                //解析返回的报文
                BMS_VM.MOD_CELL1_VOL_1_16(ModbusRTU.ParseRead03Response(receive));

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //查五个状态码(83)
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 80, 5), 15);
                data = ModbusRTU.ParseRead03Response(receive);
                if (data != null)
                {
                    BMS_VM.MOD_INST_STATE_Set(ModbusRTU.GetBits(data[3]));

                    //查告警(80)、保护(81)、硬件错误(82)信息
                    BMS_VM.MOD_WARN_STATE_Set_BMS01(ModbusRTU.GetBits(data[0]));
                    BMS_VM.MOD_PROT_STATE_Set_BMS01(ModbusRTU.GetBits(data[1]));
                    BMS_VM.MOD_ERROR_STATE_Set_BMS01(ModbusRTU.GetBits(data[2]));
                }

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //查电压  //查当前电流 //查温度
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 18, 18), 41);
                data = ModbusRTU.ParseRead03Response(receive);
                BMS_VM.OverViewSet(data);

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //查系统信息
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 283, 14), 33);
                data = ModbusRTU.ParseRead03Response(receive);
                BMS_VM.SystemInfoSet(data);

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //AFE_Protect
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 86, 1), 7);
                if (receive != null && receive.Length > 1)
                {
                    data = ModbusRTU.ParseRead03Response(receive);
                    if (data != null)
                    {
                        BMS_VM.AFE_Protect = ModbusRTU.GetBits(data[0]);
                    }
                }

                var polling = new PollingData
                {
                    Date = DateTime.Now,                                       //日期
                    TotalVolt = BMS_VM.MOD_AFECOL_PACKVOL / 100.0,                     //总电压
                    Current = BMS_VM.MOD_AFECOL_CUR,                           //电流
                    SOC = (BMS_VM.MOD_SOC / 100.0).ToString("F2"),               //SOC
                    SOH = (BMS_VM.MOD_SOH / 100.0).ToString("F2"),               //SOH
                    FullCap = (BMS_VM.MOD_FULL_CAP / 100.0).ToString("F2"),    //满充容量
                    FullRemainCap = (BMS_VM.MOD_RES_CAP / 100.0).ToString("F2"), //剩余容量
                    CycleCount = BMS_VM.MOD_CYCLECNT,                          //循环次数
                    Cell1 = BMS_VM.MOD_CELL1_VOL / 1000.0,                              //电芯1
                    Cell2 = BMS_VM.MOD_CELL2_VOL / 1000.0,
                    Cell3 = BMS_VM.MOD_CELL3_VOL / 1000.0,
                    Cell4 = BMS_VM.MOD_CELL4_VOL / 1000.0,
                    Cell5 = BMS_VM.MOD_CELL5_VOL / 1000.0,
                    Cell6 = BMS_VM.MOD_CELL6_VOL / 1000.0,
                    Cell7 = BMS_VM.MOD_CELL7_VOL / 1000.0,
                    Cell8 = BMS_VM.MOD_CELL8_VOL / 1000.0,
                    Cell9 = BMS_VM.MOD_CELL9_VOL / 1000.0,
                    Cell10 = BMS_VM.MOD_CELL10_VOL / 1000.0,
                    Cell11 = BMS_VM.MOD_CELL11_VOL / 1000.0,
                    Cell12 = BMS_VM.MOD_CELL12_VOL / 1000.0,
                    Cell13 = BMS_VM.MOD_CELL13_VOL / 1000.0,
                    Cell14 = BMS_VM.MOD_CELL14_VOL / 1000.0,
                    Cell15 = BMS_VM.MOD_CELL15_VOL / 1000.0,
                    Cell16 = BMS_VM.MOD_CELL16_VOL / 1000.0,
                    AvgVolt = BMS_VM.MOD_CELL_VOLDIFF,                         //最大电芯压差
                    MaxVolt = BMS_VM.MOD_MAXCELL_VOL,                          //最高电压
                    MinVolt = BMS_VM.MOD_MINCELL_VOL,                          //最低电压
                    Temp1 = BMS_VM.MOD_GROUD1_TEMP / 10.0,                            //电芯温度1
                    Temp2 = BMS_VM.MOD_GROUD2_TEMP / 10.0,                            //电芯温度1
                    Temp3 = BMS_VM.MOD_GROUD3_TEMP / 10.0,                            //电芯温度1
                    Temp4 = BMS_VM.MOD_GROUD4_TEMP / 10.0,                            //电芯温度1
                    Chg_MOS = BMS_VM.MOD_INST_STATE[0].ToString(),             //充电MOS
                    Dis_MOS = BMS_VM.MOD_INST_STATE[1].ToString(),             //放电MOS
                    Chg_Statues = BMS_VM.MOD_INST_STATE[4].ToString(),         //充电
                    Dis_Statues = BMS_VM.MOD_INST_STATE[5].ToString(),         //放电
                    AFE_OverChg_Pro = BMS_VM.AFE_Protect[3].ToString(),        //AFE过充保护
                    AFE_OverDis_Pro = BMS_VM.AFE_Protect[4].ToString(),        //AFE过放保护
                    Chg_Current_Pro = BMS_VM.AFE_Protect[5].ToString(),        //充电过流保护
                    Dis_Current_Pro = BMS_VM.AFE_Protect[6].ToString(),        //放电过流保护
                    AFE_Interrupt = BMS_VM.AFE_Protect[2].ToString(),          //前端芯片中断
                    ShortProtect = BMS_VM.AFE_Protect[7].ToString(),           //短路保护
                    AFE_TriggerProt = BMS_VM.AFE_Protect[0].ToString(),        //前端芯片触发保护
                    AFE_AlertPull = BMS_VM.AFE_Protect[1].ToString(),          //前端芯片告警下拉
                    //BalanceStatus = BMS_Setting.JunHen[0].ToString()
                    //+ ";" + BMS_Setting.JunHen[1].ToString() + ";"
                    //+ BMS_Setting.JunHen[2].ToString() + ";"
                    //+ BMS_Setting.JunHen[3].ToString() + ";" + BMS_Setting.JunHen[4].ToString() + ";" + BMS_Setting.JunHen[5].ToString() + ";" + BMS_Setting.JunHen[6].ToString() + ";" + BMS_Setting.JunHen[7].ToString() + ";" + BMS_Setting.JunHen[8].ToString() + ";" + BMS_Setting.JunHen[9].ToString() + ";" + BMS_Setting.JunHen[10].ToString() + ";" + BMS_Setting.JunHen[11].ToString() + ";"+ BMS_Setting.JunHen[12].ToString() + ";"+ BMS_Setting.JunHen[13].ToString() + ";"+ BMS_Setting.JunHen[14].ToString() + ";"+ BMS_Setting.JunHen[15].ToString(),
                    //均衡状态
                    AlarmStatus = BMS_VM.MOD_WARN_STATE,                      //告警信息
                    ProtectStatus = BMS_VM.MOD_PROT_STATE,                     //保护信息
                    ErrorStatus = BMS_VM.MOD_ERROR_STATE,                      //错误信息
                };
                for (int j = 0; j < BMS_Setting.CellNum; j++)
                {
                    polling.BalanceStatus = polling.BalanceStatus + BMS_Setting.JunHen[j].ToString()
                    + ";";
                }


                // 最新在最前
                Application.Current.Dispatcher.Invoke(() =>
                {

                    // 添加到界面
                    RT_Monitor.PollingList.Insert(0, polling);

                    // 保证最多 100 条
                    if (RT_Monitor.PollingList.Count > 100)
                        RT_Monitor.PollingList.RemoveAt(RT_Monitor.PollingList.Count - 1);

                    // 保存
                    if (RT_Monitor._isSaving && RT_Monitor._savePath != null)
                        RT_Monitor.SaveToExcel(polling);
                });
            }
            else if (SelectedMode == BatteryMode.Mode7)
            {
                i = 1;
                while (i <= 16)
                {
                    //并联监控
                    // 等待暂停或取消信号
                    _pauseEvent.Wait(token);
                    //发送03功能码(查1-15的pack号设备)
                    Thread.Sleep(100);
                    receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)i, 2, 33), 71);
                    //解析返回的报文
                    data = ModbusRTU.ParseRead03Response(receive);
                    UnionVM.setElement(data, i);

                    i++;
                }

            }
        }
        #endregion

        #region BMS03通讯
        /// <summary>
        /// BMS01通讯
        /// </summary>
        /// <param name="token"></param>
        private void CommunicationWithBMS03(CancellationToken token)
        {

            byte[] receive;
            short[] data;
            int i = 1;
            if (SelectedMode == BatteryMode.Mode1)
            {
                Thread.Sleep(200);
                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //发送查询机器指令
                string receive_MachineType = SerialCommunicationService.SendCommand(SpecialCommand.QueryMachineType, 10);
                if (receive_MachineType != null && receive_MachineType.Length == 10)
                    MachineType = receive_MachineType.Substring(1, 8);
                //解析指令
                SerialCommunicationService.MachineType = receive_MachineType;

                //首界面设置状态显示
                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //读写入的参数设置值(手动开关和手动均衡)
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 120, 2), 9);
                data = ModbusRTU.ParseRead03Response(receive);
                if (data != null && data.Length >= 2)
                {
                    BMS_Setting.SettingStatue = ModbusRTU.GetBits(data[0]);
                    BMS_Setting.JunHen = ModbusRTU.GetBits(data[1]);
                }

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                Thread.Sleep(200);
                //读寄存器250-251（电池个数和NTC探头个数）
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 250, 2), 9);
                //解析返回的报文
                data = ModbusRTU.ParseRead03Response(receive);
                if (data != null && data.Length == 2)
                {
                    BMS_Setting.CellNum = data[0];
                    BMS_Setting.NtcNum = data[1];
                }

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //发送03功能码(查16个电芯的电压)
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 2, 16), 37);
                //解析返回的报文
                BMS_VM.MOD_CELL1_VOL_1_16(ModbusRTU.ParseRead03Response(receive));

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //查五个状态码(83)
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 80, 5), 15);
                data = ModbusRTU.ParseRead03Response(receive);
                if (data != null)
                {
                    //充放电状态
                    BMS_VM.MOD_INST_STATE_Set(ModbusRTU.GetBits(data[3]));

                    //查告警(80)、保护(81)、硬件错误(82)信息
                    BMS_VM.MOD_WARN_STATE_Set_BMS03(ModbusRTU.GetBits(data[0]));
                    BMS_VM.MOD_PROT_STATE_Set_BMS03(ModbusRTU.GetBits(data[1]));
                    BMS_VM.MOD_ERROR_STATE_Set_BMS03(ModbusRTU.GetBits(data[2]));
                }

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //查电压  //查当前电流 //查温度
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 18, 18), 41);
                data = ModbusRTU.ParseRead03Response(receive);
                BMS_VM.OverViewSet(data);

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //查系统信息
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 283, 14), 33);
                data = ModbusRTU.ParseRead03Response(receive);
                BMS_VM.SystemInfoSet(data);

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //AFE_Protect
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 86, 1), 7);
                if (receive != null && receive.Length > 1)
                {
                    data = ModbusRTU.ParseRead03Response(receive);
                    if (data != null)
                    {
                        BMS_VM.AFE_Protect = ModbusRTU.GetBits(data[0]);
                    }
                }

            }
            else if (SelectedMode == BatteryMode.Mode2)
            {
                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //发送03功能码(91个设置项的电压)
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 130, 91), 187);
                ModbusRTU.AnalyseSetReceive(ModbusRTU.ParseRead03Response(receive), BMS_Setting.SendingCommands);
                if (flag == 0)
                {
                    //发送03功能码(91个设置项的电压)
                    Thread.Sleep(200);
                    receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 130, 91), 187);
                    ModbusRTU.AnalyseSetReceive(ModbusRTU.ParseRead03Response(receive), BMS_Setting.SendingCommands);
                    //初始化设置值
                    ModbusRTU.FirstSetReceive(BMS_Setting.SendingCommands);
                    flag = 1;
                }
            }
            else if (SelectedMode == BatteryMode.Mode6)
            {
                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //查前端芯片
                Thread.Sleep(500);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 320, 36), 77);
                BMS_Setting.SetFrontMonitor(ModbusRTU.ParseRead03Response(receive));
            }
            else if (SelectedMode == BatteryMode.Mode3)
            {
                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //读写入的参数设置值
                Thread.Sleep(600);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 252, 4), 13);
                BMS_Setting.setSystem(ModbusRTU.ParseRead03Response(receive));

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //查电压  //查当前电流 //查温度
                Thread.Sleep(300);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 18, 18), 41);
                data = ModbusRTU.ParseRead03Response(receive);
                BMS_VM.OverViewSet(data);

                _pauseEvent.Wait(token);
                //查当前系统时间
                Thread.Sleep(300);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 110, 3), 11);
                data = ModbusRTU.ParseRead03Response(receive);
                BMS_Setting.setSystemTime(data);

            }
            else if (SelectedMode == BatteryMode.Mode4)
            {
                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                Thread.Sleep(500);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 250, 2), 9);
                //解析返回的报文
                data = ModbusRTU.ParseRead03Response(receive);
                if (data != null && data.Length == 2)
                {
                    BMS_Setting.CellNum = data[0];
                    BMS_Setting.NtcNum = data[1];
                }
            }
            else if (SelectedMode == BatteryMode.Mode5)     //实时监控
            {
                //首界面设置状态显示
                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //读写入的参数设置值(充电MOS、放电MOS、关机、休眠)
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 120, 2), 9);
                data = ModbusRTU.ParseRead03Response(receive);
                if (data != null && data.Length >= 2)
                {
                    BMS_Setting.SettingStatue = ModbusRTU.GetBits(data[0]);
                    BMS_Setting.JunHen = ModbusRTU.GetBits(data[1]);
                }

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 250, 2), 9);
                //解析返回的报文
                data = ModbusRTU.ParseRead03Response(receive);
                if (data != null && data.Length == 2)
                {
                    BMS_Setting.CellNum = data[0];
                    BMS_Setting.NtcNum = data[1];
                }

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //发送03功能码(查是16个电芯的电压)
                Thread.Sleep(100);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 2, 16), 37);
                //解析返回的报文
                BMS_VM.MOD_CELL1_VOL_1_16(ModbusRTU.ParseRead03Response(receive));

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //查五个状态码(83)
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 80, 5), 15);
                data = ModbusRTU.ParseRead03Response(receive);
                if (data != null)
                {
                    //充放电状态（83）
                    BMS_VM.MOD_INST_STATE_Set(ModbusRTU.GetBits(data[3]));

                    //查告警(80)、保护(81)、硬件错误(82)信息
                    BMS_VM.MOD_WARN_STATE_Set_BMS03(ModbusRTU.GetBits(data[0]));
                    BMS_VM.MOD_PROT_STATE_Set_BMS03(ModbusRTU.GetBits(data[1]));
                    BMS_VM.MOD_ERROR_STATE_Set_BMS03(ModbusRTU.GetBits(data[2]));
                }

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //查电压  //查当前电流 //查温度
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 18, 18), 41);
                data = ModbusRTU.ParseRead03Response(receive);
                BMS_VM.OverViewSet(data);

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //查系统信息
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 283, 14), 33);
                data = ModbusRTU.ParseRead03Response(receive);
                BMS_VM.SystemInfoSet(data);

                // 等待暂停或取消信号
                _pauseEvent.Wait(token);
                //AFE_Protect
                Thread.Sleep(200);
                receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 86, 1), 7);
                if (receive != null && receive.Length > 1)
                {
                    data = ModbusRTU.ParseRead03Response(receive);
                    if (data != null)
                    {
                        BMS_VM.AFE_Protect = ModbusRTU.GetBits(data[0]);
                    }
                }

                var polling = new PollingData
                {
                    Date = DateTime.Now,                                       //日期
                    TotalVolt = BMS_VM.MOD_AFECOL_PACKVOL / 100.0,                     //总电压
                    Current = BMS_VM.MOD_AFECOL_CUR,                           //电流
                    SOC = (BMS_VM.MOD_SOC / 100.0).ToString("F2"),               //SOC
                    SOH = (BMS_VM.MOD_SOH / 100.0).ToString("F2"),               //SOH
                    FullCap = (BMS_VM.MOD_FULL_CAP / 100.0).ToString("F2"),    //满充容量
                    FullRemainCap = (BMS_VM.MOD_RES_CAP / 100.0).ToString("F2"), //剩余容量
                    CycleCount = BMS_VM.MOD_CYCLECNT,                          //循环次数
                    Cell1 = BMS_VM.MOD_CELL1_VOL / 1000.0,                              //电芯1
                    Cell2 = BMS_VM.MOD_CELL2_VOL / 1000.0,
                    Cell3 = BMS_VM.MOD_CELL3_VOL / 1000.0,
                    Cell4 = BMS_VM.MOD_CELL4_VOL / 1000.0,
                    Cell5 = BMS_VM.MOD_CELL5_VOL / 1000.0,
                    Cell6 = BMS_VM.MOD_CELL6_VOL / 1000.0,
                    Cell7 = BMS_VM.MOD_CELL7_VOL / 1000.0,
                    Cell8 = BMS_VM.MOD_CELL8_VOL / 1000.0,
                    Cell9 = BMS_VM.MOD_CELL9_VOL / 1000.0,
                    Cell10 = BMS_VM.MOD_CELL10_VOL / 1000.0,
                    Cell11 = BMS_VM.MOD_CELL11_VOL / 1000.0,
                    Cell12 = BMS_VM.MOD_CELL12_VOL / 1000.0,
                    Cell13 = BMS_VM.MOD_CELL13_VOL / 1000.0,
                    Cell14 = BMS_VM.MOD_CELL14_VOL / 1000.0,
                    Cell15 = BMS_VM.MOD_CELL15_VOL / 1000.0,
                    Cell16 = BMS_VM.MOD_CELL16_VOL / 1000.0,
                    AvgVolt = BMS_VM.MOD_CELL_VOLDIFF,                         //最大电芯压差
                    MaxVolt = BMS_VM.MOD_MAXCELL_VOL,                          //最高电压
                    MinVolt = BMS_VM.MOD_MINCELL_VOL,                          //最低电压
                    Temp1 = BMS_VM.MOD_GROUD1_TEMP / 10.0,                            //电芯温度1
                    Temp2 = BMS_VM.MOD_GROUD2_TEMP / 10.0,                            //电芯温度1
                    Temp3 = BMS_VM.MOD_GROUD3_TEMP / 10.0,                            //电芯温度1
                    Temp4 = BMS_VM.MOD_GROUD4_TEMP / 10.0,                            //电芯温度1
                    Chg_MOS = BMS_VM.MOD_INST_STATE[0].ToString(),             //充电MOS
                    Dis_MOS = BMS_VM.MOD_INST_STATE[1].ToString(),             //放电MOS
                    Chg_Statues = BMS_VM.MOD_INST_STATE[4].ToString(),         //充电
                    Dis_Statues = BMS_VM.MOD_INST_STATE[5].ToString(),         //放电
                    AFE_OverChg_Pro = BMS_VM.AFE_Protect[3].ToString(),        //AFE过充保护
                    AFE_OverDis_Pro = BMS_VM.AFE_Protect[4].ToString(),        //AFE过放保护
                    Chg_Current_Pro = BMS_VM.AFE_Protect[5].ToString(),        //充电过流保护
                    Dis_Current_Pro = BMS_VM.AFE_Protect[6].ToString(),        //放电过流保护
                    AFE_Interrupt = BMS_VM.AFE_Protect[2].ToString(),          //前端芯片中断
                    ShortProtect = BMS_VM.AFE_Protect[7].ToString(),           //短路保护
                    AFE_TriggerProt = BMS_VM.AFE_Protect[0].ToString(),        //前端芯片触发保护
                    AFE_AlertPull = BMS_VM.AFE_Protect[1].ToString(),          //前端芯片告警下拉
                    //BalanceStatus = BMS_Setting.JunHen[0].ToString()
                    //+ ";" + BMS_Setting.JunHen[1].ToString() + ";"
                    //+ BMS_Setting.JunHen[2].ToString() + ";"
                    //+ BMS_Setting.JunHen[3].ToString() + ";" + BMS_Setting.JunHen[4].ToString() + ";" + BMS_Setting.JunHen[5].ToString() + ";" + BMS_Setting.JunHen[6].ToString() + ";" + BMS_Setting.JunHen[7].ToString() + ";" + BMS_Setting.JunHen[8].ToString() + ";" + BMS_Setting.JunHen[9].ToString() + ";" + BMS_Setting.JunHen[10].ToString() + ";" + BMS_Setting.JunHen[11].ToString() + ";"+ BMS_Setting.JunHen[12].ToString() + ";"+ BMS_Setting.JunHen[13].ToString() + ";"+ BMS_Setting.JunHen[14].ToString() + ";"+ BMS_Setting.JunHen[15].ToString(),
                    //均衡状态
                    AlarmStatus = BMS_VM.MOD_WARN_STATE,                      //告警信息
                    ProtectStatus = BMS_VM.MOD_PROT_STATE,                     //保护信息
                    ErrorStatus = BMS_VM.MOD_ERROR_STATE,                      //错误信息
                };
                for (int j = 0; j < BMS_Setting.CellNum; j++)
                {
                    polling.BalanceStatus = polling.BalanceStatus + BMS_Setting.JunHen[j].ToString()
                    + ";";
                }

                // 最新在最前
                Application.Current.Dispatcher.Invoke(() =>
                {

                    // 添加到界面
                    RT_Monitor.PollingList.Insert(0, polling);

                    // 保证最多 100 条
                    if (RT_Monitor.PollingList.Count > 100)
                        RT_Monitor.PollingList.RemoveAt(RT_Monitor.PollingList.Count - 1);

                    // 保存
                    if (RT_Monitor._isSaving && RT_Monitor._savePath != null)
                        RT_Monitor.SaveToExcel(polling);
                });
            }
            else if (SelectedMode == BatteryMode.Mode7)
            {
                i = 1;
                while (i <= 16)
                {
                    //并联监控
                    // 等待暂停或取消信号
                    _pauseEvent.Wait(token);
                    //发送03功能码(查2-34的pack号设备)
                    Thread.Sleep(100);
                    receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)i, 2, 33), 71);
                    //解析返回的报文
                    data = ModbusRTU.ParseRead03Response(receive);
                    UnionVM.setElement(data, i);

                    i++;
                }

            }
        }
        #endregion

        /// <summary>
        /// 刷新用户设置参数
        /// </summary>
        public void RefleshSettingParam()
        {
            // 等待暂停或取消信号

            //发送03功能码(查是91个设置项的电压)
            Thread.Sleep(500);
            byte[] receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame((byte)SerialCommunicationService.address, 130, 112), 229);
            ModbusRTU.AnalyseSetReceive(ModbusRTU.ParseRead03Response(receive), BMS_Setting.SendingCommands);
            //初始化设置值
            ModbusRTU.FirstSetReceive(BMS_Setting.SendingCommands);

        }

        /// <summary>
        /// 对用户参数进行对应语言的可视化
        /// </summary>
        public void RefleshSettingParamToLanguage(string Lan)
        {

            if (SelectedMachineItem == "BMS01")
            {
                //补丁，BMS01时单位稍作修改
                ModbusRTU.FirstSetReceive_Enum(BMS_Setting.SendingCommands, BMS_Setting.LoadSettings2());

            }
            else
                ModbusRTU.FirstSetReceive_Enum(BMS_Setting.SendingCommands, BMS_Setting.LoadSettings());

        }

        /// <summary>
        /// 停止后台通信
        /// </summary>
        private void StopBackgroundThread()
        {
            _cts.Cancel();
            AddLog("后台通信停止请求已发送");
        }
        #endregion

        #region 消息框

        private readonly IMessageDialogService _messageService;

        private Visibility showAdmin = Visibility.Hidden;

        public Visibility ShowAdmin
        {
            get { return showAdmin; }
            set
            {
                showAdmin = value;
                this.RaiseProperChanged(nameof(ShowAdmin));
            }
        }


        public ICommand ShowMessageCommand { get; }

        private void OnShowMessage()
        {
            string Password = MShowTestMessage("请输入管理员密码", "前端芯片监控", "密码错误");
            if (Password == "Tqf147258")
            {
                ShowAdmin = Visibility.Visible;
                Administration = true;
            }
            else if (Password == "123456")
            {
                Administration = true;
            }
        }

        /// <summary>
        /// 消息弹框
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="title">标题</param>
        /// <param name="messageIcon">图标</param>
        /// <returns></returns>
        private MessageResult LShowMessage(string message, string title, MessageIcon messageIcon)
        {
            MessageResult result = MessageResult.OK;

            if (Application.Current.Dispatcher.CheckAccess())
            {
                // 当前是UI线程直接调用
                result = _messageService.Show(
                message,
                title,
                messageIcon,
                fontSize: 50
                );
            }
            else
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    result = _messageService.Show(
                    message,
                    title,
                    messageIcon,
                    fontSize: 50
                    );
                }));
            }

            return result;

        }


        /// <summary>
        /// 文本输入消息框
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="title">标题</param>
        /// <param name="error">输入错误提示</param>
        /// <returns></returns>
        private string LShowTestMessage(string message, string title, string error)
        {
            string result = string.Empty;
            if (Application.Current.Dispatcher.CheckAccess())
            {
                // 当前是UI线程直接调用
                result = _messageService.ShowInputDialog(
                message,
                title,
                InputType.Text,
                "软件版本",
                validator: input => input.Length >= 4,
                validationMessage: error,
                fontSize: 50);
            }
            else
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    result = _messageService.ShowInputDialog(
                message,
                title,
                InputType.Text,
                "软件版本",
                validator: input => input.Length >= 4,
                validationMessage: error,
                fontSize: 50);
                }));
            }
            return result;
        }

        /// <summary>
        /// 密码输入消息框
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="title">标题</param>
        /// <param name="error">输入错误提示</param>
        /// <returns></returns>
        private string MShowTestMessage(string message, string title, string error)
        {
            string result = string.Empty; // 初始化返回结果
            // 检查当前线程是否在UI线程上,CheckAccess() 判断当前线程是否为UI线程
            if (Application.Current.Dispatcher.CheckAccess())
            {
                // 当前是UI线程，直接调用对话框服务显示输入框
                result = _messageService.ShowInputDialog(
                message,// 提示信息
                title,// 标题
                InputType.Password, // 输入类型为密码
                "管理员密码",// 输入框的默认占位文本   
                validator: input => (input == "Tqf147258" || input == "123456"),// 验证委托
                validationMessage: error,// 验证失败时显示的错误信息
                fontSize: 50);
            }
            else
            {
                // 当前不是UI线程，需要使用Dispatcher将调用封送到UI线程上执行
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    result = _messageService.ShowInputDialog(
                message,
                title,
                InputType.Password,
                "管理员密码",
                validator: input => (input == "Tqf147258" || input == "123456"),
                validationMessage: error,
                fontSize: 50);
                }));
            }
            // 返回用户输入的密码
            return result;
        }


        //是否打开
        private bool IsExpand1;

        public bool _IsExpand1
        {
            get { return IsExpand1; }
            set
            {
                if (value == _IsExpand1) return;

                // 只有当准备从收起 -> 展开时才需要确认
                if (value && !_IsExpand1)
                {
                    string result = MShowTestMessage("请输入管理员密码", "打开权限", "密码错误，请联系研发人员");
                    if (result != "Tqf147258")
                    {
                        return;
                    }
                }
                IsExpand1 = value;
                this.RaiseProperChanged(nameof(_IsExpand1));
            }
        }

        //是否打开
        private bool IsExpand2;

        public bool _IsExpand2
        {
            get { return IsExpand2; }
            set
            {
                if (value == _IsExpand2) return;

                // 只有当准备从收起 -> 展开时才需要确认
                if (value && !_IsExpand2)
                {
                    string result = MShowTestMessage("请输入管理员密码", "打开权限", "密码错误，请联系研发人员");
                    if (result != "Tqf147258")
                    {
                        return;
                    }
                }
                IsExpand2 = value;
                this.RaiseProperChanged(nameof(_IsExpand2));
            }
        }


        #endregion
    }
}
