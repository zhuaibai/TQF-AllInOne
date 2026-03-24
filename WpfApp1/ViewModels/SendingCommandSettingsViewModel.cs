using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Xml.Serialization;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.Win32;
using WpfApp1.Command;
using WpfApp1.Command.BMS;
using WpfApp1.Convert;
using WpfApp1.Models;
using WpfApp1.Services;
using System.Text.RegularExpressions;

namespace WpfApp1.ViewModels
{
    public class SendingCommandSettingsViewModel : BaseViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        ManualResetEventSlim _pauseEvent;//线程的开启、暂停
        SemaphoreSlim _semaphore;        //异步竞争，资源锁
        Action<string> AddLog;           //添加日志委托
        Action<string> UpdateState;      //更新状态日志



        public SendingCommandSettingsViewModel(ManualResetEventSlim pauseEvent, SemaphoreSlim semaphore, Action<string> addLog, Action<string> _updateState)
        {
            _pauseEvent = pauseEvent;
            _semaphore = semaphore;
            AddLog = addLog;
            UpdateState = _updateState;

            SendingCommands = LoadSettings();
            SaveCommand = new DelegateCommand(SaveSettings);
            SaveCommandToFile = new DelegateCommand(SaveSettingsToAtherFile);
            LoadFromFile = new DelegateCommand((object ds) => { SendingCommands = LoadSettingsFromFile(); });
            // 初始化命令，传入异步操作和按钮状态判断
            WriteCommand = new RelayCommand(ExecuteWriteAsync, () => IsButtonEnabled);
            WriteCommandByBMS01 = new RelayCommand(ExecuteWriteAsyncByBMS01, () => IsButtonEnabled);
            StopReadComamnd = new RelayCommand(StopReadOperation, () => !stopReadFlag_isWorking);
            Command_SetSystemTime = new RelayCommand(SystemTimeOperation, () => !SystemTime_IsWorking);

            #region 初始化参数分类

            //单体过充
            SendingCommandsView_1 = new ListCollectionView(SendingCommands);
            SendingCommandsView_1.Filter = (object item) =>
            {
                var index = SendingCommands.IndexOf((SendingCommand)item);
                return index >= 0 && index <= 4;
            };
            //总体过充
            SendingCommandsView_2 = new ListCollectionView(SendingCommands);
            SendingCommandsView_2.Filter = (object item) =>
            {
                var index = SendingCommands.IndexOf((SendingCommand)item);
                return index >= 5 && index <= 9;
            };
            //单体过放
            SendingCommandsView_3 = new ListCollectionView(SendingCommands);
            SendingCommandsView_3.Filter = (object item) =>
            {
                var index = SendingCommands.IndexOf((SendingCommand)item);
                return index >= 10 && index <= 13;
            };
            //总体过放
            SendingCommandsView_4 = new ListCollectionView(SendingCommands);
            SendingCommandsView_4.Filter = (object item) =>
            {
                var index = SendingCommands.IndexOf((SendingCommand)item);
                return index >= 14 && index <= 17;
            };
            //充电过流
            SendingCommandsView_5 = new ListCollectionView(SendingCommands);
            SendingCommandsView_5.Filter = (object item) =>
            {
                var index = SendingCommands.IndexOf((SendingCommand)item);
                return index >= 18 && index <= 21;
            };
            //充电过流2
            SendingCommandsView_20 = new ListCollectionView(SendingCommands);
            SendingCommandsView_20.Filter = (object item) =>
            {
                var index = SendingCommands.IndexOf((SendingCommand)item);
                return index >= 91 && index <= 94;
            };
            //放电过流
            SendingCommandsView_6 = new ListCollectionView(SendingCommands);
            SendingCommandsView_6.Filter = (object item) =>
            {
                var index = SendingCommands.IndexOf((SendingCommand)item);
                return index >= 22 && index <= 27;
            };
            //短路保护
            SendingCommandsView_30 = new ListCollectionView(SendingCommands);
            SendingCommandsView_30.Filter = (object item) =>
            {
                var index = SendingCommands.IndexOf((SendingCommand)item);
                return index >= 28 && index <= 28;
            };
            //均衡
            SendingCommandsView_7 = new ListCollectionView(SendingCommands);
            SendingCommandsView_7.Filter = (object item) =>
            {
                var index = SendingCommands.IndexOf((SendingCommand)item);
                return index >= 29 && index <= 32;
            };
            //单体休眠和电池包
            SendingCommandsView_8 = new ListCollectionView(SendingCommands);
            SendingCommandsView_8.Filter = (object item) =>
            {
                var index = SendingCommands.IndexOf((SendingCommand)item);
                return index >= 33 && index <= 37;
            };
            //充电高温1
            SendingCommandsView_9 = new ListCollectionView(SendingCommands);
            SendingCommandsView_9.Filter = (object item) =>
            {
                var index = SendingCommands.IndexOf((SendingCommand)item);
                return index >= 38 && index <= 41;
            };
            //充点低温
            SendingCommandsView_10 = new ListCollectionView(SendingCommands);
            SendingCommandsView_10.Filter = (object item) =>
            {
                var index = SendingCommands.IndexOf((SendingCommand)item);
                return index >= 42 && index <= 45;
            };
            //充电高温2
            SendingCommandsView_11 = new ListCollectionView(SendingCommands);
            SendingCommandsView_11.Filter = (object item) =>
            {
                var index = SendingCommands.IndexOf((SendingCommand)item);
                return index >= 46 && index <= 49;
            };
            //放电低温
            SendingCommandsView_12 = new ListCollectionView(SendingCommands);
            SendingCommandsView_12.Filter = (object item) =>
            {
                var index = SendingCommands.IndexOf((SendingCommand)item);
                return index >= 50 && index <= 53;
            };
            //MOS
            SendingCommandsView_13 = new ListCollectionView(SendingCommands);
            SendingCommandsView_13.Filter = (object item) =>
            {
                var index = SendingCommands.IndexOf((SendingCommand)item);
                return index >= 54 && index <= 57;
            };
            //放电高温
            SendingCommandsView_14 = new ListCollectionView(SendingCommands);
            SendingCommandsView_14.Filter = (object item) =>
            {
                var index = SendingCommands.IndexOf((SendingCommand)item);
                return index >= 58 && index <= 61;
            };
            //环境高温
            SendingCommandsView_15 = new ListCollectionView(SendingCommands);
            SendingCommandsView_15.Filter = (object item) =>
            {
                var index = SendingCommands.IndexOf((SendingCommand)item);
                return index >= 62 && index <= 65;
            };
            //电芯、预充与限流
            SendingCommandsView_16 = new ListCollectionView(SendingCommands);
            SendingCommandsView_16.Filter = (object item) =>
            {
                var index = SendingCommands.IndexOf((SendingCommand)item);
                return index >= 66 && index <= 72;
            };
            //散热与加热
            SendingCommandsView_17 = new ListCollectionView(SendingCommands);
            SendingCommandsView_17.Filter = (object item) =>
            {
                var index = SendingCommands.IndexOf((SendingCommand)item);
                return index >= 73 && index <= 78;
            };
            //其他
            SendingCommandsView_18 = new ListCollectionView(SendingCommands);
            SendingCommandsView_18.Filter = (object item) =>
            {
                var index = SendingCommands.IndexOf((SendingCommand)item);
                return index >= 79 && index <= 84;
            };
            //其他
            SendingCommandsView_19 = new ListCollectionView(SendingCommands);
            SendingCommandsView_19.Filter = (object item) =>
            {
                var index = SendingCommands.IndexOf((SendingCommand)item);
                return index >= 85 && index <= 90;
            };
            //AFE 短路保护参数
            SendingCommandsView_21 = new ListCollectionView(SendingCommands);
            SendingCommandsView_21.Filter = (object item) =>
            {
                var index = SendingCommands.IndexOf((SendingCommand)item);
                return index >= 95 && index <= 96;
            };
            //AFE 过充保护参数
            SendingCommandsView_22 = new ListCollectionView(SendingCommands);
            SendingCommandsView_22.Filter = (object item) =>
            {
                var index = SendingCommands.IndexOf((SendingCommand)item);
                return index >= 97 && index <= 101;
            };
            //AFE 过放保护参数
            SendingCommandsView_23 = new ListCollectionView(SendingCommands);
            SendingCommandsView_23.Filter = (object item) =>
            {
                var index = SendingCommands.IndexOf((SendingCommand)item);
                return index >= 102 && index <= 105;
            };
            //AFE 充电过流保护参数
            SendingCommandsView_24 = new ListCollectionView(SendingCommands);
            SendingCommandsView_24.Filter = (object item) =>
            {
                var index = SendingCommands.IndexOf((SendingCommand)item);
                return index >= 106 && index <= 108;
            };
            //AFE 充电过流保护参数
            SendingCommandsView_25 = new ListCollectionView(SendingCommands);
            SendingCommandsView_25.Filter = (object item) =>
            {
                var index = SendingCommands.IndexOf((SendingCommand)item);
                return index >= 109 && index <= 111;
            };

            #endregion

            //点击开启配置窗口
            //DoubleClickCommand = new RelayCommand(OnDoubleClick);
            //初始化串口
            SerialPort1 = new SerialPortSettingViewModel();

            #region 初始化命令
            //导出到Excel
            ExportExcel = new RelayCommand(() =>
            {
                ExcelExportHelper.ExportHistoryToExcel(HistoryLods);
            });
            //零点电流
            Command_SetZeroCurrent = new RelayCommand(
                execute: () => ZeroCurrentOperation(),
                canExecute: () => !string.IsNullOrEmpty(ZeroCurrent_Inputs) && !ZeroCurrent_IsWorking // 增加处理状态检查
            );
            //充电电流和放电电流校准
            Command_SetChgCurrent = new RelayCommand(
                execute: () => ChgCurrentOperation(),
                canExecute: () => !string.IsNullOrEmpty(ChgCurrent_Inputs) && !ChgCurrent_IsWorking // 增加处理状态检查
            );
            //设计容量
            Command_SetDesignCap = new RelayCommand(
                execute: () => DesignCapOperation(),
                canExecute: () => !string.IsNullOrEmpty(DesignCap_Inputs) && !DesignCap_IsWorking // 增加处理状态检查
            );
            //满充容量
            Command_SetFullCap = new RelayCommand(
                execute: () => FullCapOperation(),
                canExecute: () => !string.IsNullOrEmpty(FullCap_Inputs) && !FullCap_IsWorking // 增加处理状态检查
            );
            //剩余容量
            Command_SetRemainCap = new RelayCommand(
                execute: () => RemainCapOperation(),
                canExecute: () => !string.IsNullOrEmpty(RemainCap_Inputs) && !RemainCap_IsWorking // 增加处理状态检查
            );
            //循环次数
            Command_SetCycleCount = new RelayCommand(
                execute: () => CycleCountOperation(),
                canExecute: () => !string.IsNullOrEmpty(CycleCount_Inputs) && !CycleCount_IsWorking // 增加处理状态检查
            );
            //蓝牙地址
            Command_SetBuleTooth = new RelayCommand(
                execute: () => BuleToothOperation(),
                canExecute: () => !string.IsNullOrEmpty(BuleTooth_Inputs) && !BuleTooth_IsWorking // 增加处理状态检查
            );
            //零点校准系数增加
            Command_SetZeroCalibFactorInc = new RelayCommand(
                execute: () => ZeroCalibFactorIncOperation(),
                canExecute: () => !ZeroCalibFactorInc_IsWorking // 增加处理状态检查
            );
            //零点校准系数减少
            Command_SetZeroCalibFactorDec = new RelayCommand(
                execute: () => ZeroCalibFactorDecOperation(),
                canExecute: () => !ZeroCalibFactorDec_IsWorking // 增加处理状态检查
            );
            //充电校准系数增加
            Command_SetChgCalibFactorInc = new RelayCommand(
                execute: () => ChgCalibFactorIncOperation(),
                canExecute: () => !ChgCalibFactorInc_IsWorking // 增加处理状态检查
            );
            //充电校准系数减少
            Command_SetChgCalibFactorDec = new RelayCommand(
                execute: () => ChgCalibFactorDecOperation(),
                canExecute: () => !ChgCalibFactorDec_IsWorking // 增加处理状态检查
            );
            //放电校准系数增加
            Command_SetDisCalibFactorInc = new RelayCommand(
                execute: () => DisCalibFactorIncOperation(),
                canExecute: () => !DisCalibFactorInc_IsWorking // 增加处理状态检查
            );
            //放电校准系数较少
            Command_SetDisCalibFactorDec = new RelayCommand(
                execute: () => DisCalibFactorDecOperation(),
                canExecute: () => !DisCalibFactorDec_IsWorking // 增加处理状态检查
            );
            //放电校准系数较少
            Command_SetDisCalibFactorDec = new RelayCommand(
                execute: () => DisCalibFactorDecOperation(),
                canExecute: () => !DisCalibFactorDec_IsWorking // 增加处理状态检查
            );
            //清除历史记录
            Command_SetClearHistory = new RelayCommand(
                execute: () => ClearHistoryOperation(),
                canExecute: () => !ClearHistory_IsWorking // 增加处理状态检查
            );
            //重置用户参数
            Command_SetResetUserParams = new RelayCommand(
                execute: () => ResetUserParamsOperation(),
                canExecute: () => !ResetUserParams_IsWorking // 增加处理状态检查
            );
            //重置系统参数
            Command_SetResetSysParams = new RelayCommand(
                execute: () => ResetSysParamsOperation(),
                canExecute: () => !ResetSysParams_IsWorking // 增加处理状态检查
            );
            //读取历史记录BMS02
            HistoryReadCommandBMS02 = new RelayCommand(
                execute: () => HistoryReadOperationBMS02(),
                canExecute: () => !HistoryRead_IsWorking // 增加处理状态检查
            );
            //读取历史记录BMS01
            HistoryReadCommandBMS01 = new RelayCommand(
                execute: () => HistoryReadOperationBMS01(),
                canExecute: () => !HistoryRead_IsWorking // 增加处理状态检查
            );
            //读取历史记录BMS03
            HistoryReadCommandBMS03 = new RelayCommand(
               execute: () => HistoryReadOperationBMS03(),
               canExecute: () => !HistoryRead_IsWorking // 增加处理状态检查
           );
            //读取充电电流校准系数
            Command_SetChgCalibFactorRead = new RelayCommand(
                execute: () => ChgCalibFactorReadOperation(),
                canExecute: () => !ChgCalibFactorRead_IsWorking // 增加处理状态检查
            );
            //写入充电电流校准系数
            Command_SetChgCalibFactorWrite = new RelayCommand(
                execute: () => ChgCalibFactorWriteOperation(),
                canExecute: () => !ChgCalibFactorWrite_IsWorking // 增加处理状态检查
            );
            //读取放电电流校准
            Command_SetDisCalibFactorRead = new RelayCommand(
                execute: () => DisCalibFactorReadOperation(),
                canExecute: () => !DisCalibFactorRead_IsWorking // 增加处理状态检查
            );
            //写入放电电流校准
            Command_SetDisCalibFactorWrite = new RelayCommand(
                execute: () => DisCalibFactorWriteOperation(),
                canExecute: () => !DisCalibFactorWrite_IsWorking // 增加处理状态检查
            );
            //关闭充电MOS
            CloseCharMOSCommand = new RelayCommand(
                execute: () => CloseCharMOSOperation(),
                canExecute: () => !CloseCharMOS_IsWorking // 增加处理状态检查
            );
            //关闭放电MOS
            CloseDisCharMOSCommand = new RelayCommand(
              execute: () => CloseDisCharMOSOperation(),
              canExecute: () => !TurnOff_IsWorking // 增加处理状态检查
            );
            //关机
            TurnOffCommand = new RelayCommand(
              execute: () => TurnOffOperation(),
              canExecute: () => !TurnOff_IsWorking // 增加处理状态检查
            );
            //限流板
            LimitCommand = new RelayCommand(
                 execute: () => LimitOperation(),
              canExecute: () => !Limit_IsWorking // 增加处理状态检查
            );
            //获取超出范围数据索引
            GetOutIndexCommand = new RelayCommand(
                execute: () => GetOutIndexOperation(),
                canExecute: () => !GetOutIndex_IsWorking // 增加处理状态检查
            );
            //休眠
            SleepCommand = new RelayCommand(
                execute: () => SleepOperation(),
                canExecute: () => !Sleep_IsWorking // 增加处理状态检查
            );
            //取消休眠
            CancelSleepCommand = new RelayCommand(
                execute: () => CancelSleepOperation(),
                canExecute: () => !CancelSleep_IsWorking // 增加处理状态检查
            );
            //电池组温度1+
            Command_SetCellTemp1Inc = new RelayCommand(
                execute: () => CellTemp1IncOperation(),
                canExecute: () => !CellTemp1Inc_IsWorking // 增加处理状态检查
            );
            //电池组温度1-
            Command_SetCellTemp1Dec = new RelayCommand(
                execute: () => CellTemp1DecOperation(),
                canExecute: () => !CellTemp1Dec_IsWorking // 增加处理状态检查
            );
            //电池组温度1 读取
            Command_SetCellTemp1Read = new RelayCommand(
                execute: () => CellTemp1ReadOperation(),
                canExecute: () => !CellTemp1Read_IsWorking // 增加处理状态检查
            );
            //电池组温度1 写入
            Command_SetCellTemp1Write = new RelayCommand(
                execute: () => CellTemp1WriteOperation(),
                canExecute: () => !CellTemp1Write_IsWorking // 增加处理状态检查
            );

            //电池组温度2+
            Command_SetCellTemp2Inc = new RelayCommand(
                execute: () => CellTemp2IncOperation(),
                canExecute: () => !CellTemp2Inc_IsWorking // 增加处理状态检查
            );
            //电池组温度2-
            Command_SetCellTemp2Dec = new RelayCommand(
                execute: () => CellTemp2DecOperation(),
                canExecute: () => !CellTemp2Dec_IsWorking // 增加处理状态检查
            );
            //电池组温度2 读取
            Command_SetCellTemp2Read = new RelayCommand(
                execute: () => CellTemp2ReadOperation(),
                canExecute: () => !CellTemp2Read_IsWorking // 增加处理状态检查
            );
            //电池组温度2 写入
            Command_SetCellTemp2Write = new RelayCommand(
                execute: () => CellTemp2WriteOperation(),
                canExecute: () => !CellTemp2Write_IsWorking // 增加处理状态检查
            );

            //电池组温度3+
            Command_SetCellTemp3Inc = new RelayCommand(
                execute: () => CellTemp3IncOperation(),
                canExecute: () => !CellTemp3Inc_IsWorking // 增加处理状态检查
            );
            //电池组温度3-
            Command_SetCellTemp3Dec = new RelayCommand(
                execute: () => CellTemp3DecOperation(),
                canExecute: () => !CellTemp3Dec_IsWorking // 增加处理状态检查
            );
            //电池组温度3 读取
            Command_SetCellTemp3Read = new RelayCommand(
                execute: () => CellTemp3ReadOperation(),
                canExecute: () => !CellTemp3Read_IsWorking // 增加处理状态检查
            );
            //电池组温度3 写入
            Command_SetCellTemp3Write = new RelayCommand(
                execute: () => CellTemp3WriteOperation(),
                canExecute: () => !CellTemp3Write_IsWorking // 增加处理状态检查
            );

            //电池组温度4+
            Command_SetCellTemp4Inc = new RelayCommand(
                execute: () => CellTemp4IncOperation(),
                canExecute: () => !CellTemp4Inc_IsWorking // 增加处理状态检查
            );
            //电池组温度4-
            Command_SetCellTemp4Dec = new RelayCommand(
                execute: () => CellTemp4DecOperation(),
                canExecute: () => !CellTemp4Dec_IsWorking // 增加处理状态检查
            );
            //电池组温度4 读取
            Command_SetCellTemp4Read = new RelayCommand(
                execute: () => CellTemp4ReadOperation(),
                canExecute: () => !CellTemp4Read_IsWorking // 增加处理状态检查
            );
            //电池组温度4 写入
            Command_SetCellTemp4Write = new RelayCommand(
                execute: () => CellTemp4WriteOperation(),
                canExecute: () => !CellTemp4Write_IsWorking // 增加处理状态检查
            );

            #endregion

            #region 监控参数初始化
            frontMonitorInitial();
            FrontView1 = new ListCollectionView(FrontMonitor);
            FrontView1.Filter = (object item) =>
            {
                var index = FrontMonitor.IndexOf((SendingCommand)item);
                return index >= 0 && index <= 5;
            };
            FrontView2 = new ListCollectionView(FrontMonitor);
            FrontView2.Filter = (object item) =>
            {
                var index = FrontMonitor.IndexOf((SendingCommand)item);
                return index >= 6 && index <= 11;
            };
            FrontView3 = new ListCollectionView(FrontMonitor);
            FrontView3.Filter = (object item) =>
            {
                var index = FrontMonitor.IndexOf((SendingCommand)item);
                return index >= 12 && index <= 17;
            };
            FrontView4 = new ListCollectionView(FrontMonitor);
            FrontView4.Filter = (object item) =>
            {
                var index = FrontMonitor.IndexOf((SendingCommand)item);
                return index >= 18 && index <= 23;
            };
            FrontView5 = new ListCollectionView(FrontMonitor);
            FrontView5.Filter = (object item) =>
            {
                var index = FrontMonitor.IndexOf((SendingCommand)item);
                return index >= 24 && index <= 29;
            };
            FrontView6 = new ListCollectionView(FrontMonitor);
            FrontView6.Filter = (object item) =>
            {
                var index = FrontMonitor.IndexOf((SendingCommand)item);
                return index >= 30 && index <= 35;
            };

            #endregion

        }

        public void setSystem(short[] data)
        {
            if (data == null || data.Length < 4)
            {
                return;
            }
            DesignCap = data[0] / 100;
            FullCap = data[1] / 100;
            RemainCap = data[2] / 100;
            CycleCount = data[3];

        }


        //电芯数量
        private int cellNum = 16;

        public int CellNum
        {
            get { return cellNum; }
            set
            {
                cellNum = value;
                this.RaiseProperChanged(nameof(CellNum));
            }
        }

        //温度探头数量
        private int ntcNum;

        public int NtcNum
        {
            get { return ntcNum; }
            set
            {
                ntcNum = value;
                this.RaiseProperChanged(nameof(NtcNum));
            }
        }

        #region 系统时间

        //系统时间
        private string _SystemTime;

        public string SystemTime
        {
            get { return _SystemTime; }
            set
            {
                _SystemTime = value;
                RaiseProperChanged(nameof(SystemTime));


            }
        }

        public void setSystemTime(short[] time)
        {
            if (time != null && time.Length == 3)
            {
                SystemTime = ParseDateTimeRegistersLE(time[0], time[1], time[2]);
            }
        }
        /// <summary>
        /// 解析寄存器中的时间
        /// </summary>
        /// <param name="reg1"></param>
        /// <param name="reg2"></param>
        /// <param name="reg3"></param>
        /// <returns></returns>
        public string ParseDateTimeRegistersLE(short reg1, short reg2, short reg3)
        {
            byte year = (byte)(reg1 & 0xFF);       // 低字节
            byte month = (byte)(reg1 >> 8);         // 高字节

            byte day = (byte)(reg2 & 0xFF);
            byte hour = (byte)(reg2 >> 8);

            byte minute = (byte)(reg3 & 0xFF);
            byte second = (byte)(reg3 >> 8);


            return "20" + year + "-" + month + "-" + day + " " + hour + ":" + minute + ":" + second;
        }


        /// <summary>
        /// 创建时间字节数组
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public int[] BuildDateTimeRegistersLE(
        byte year, byte month, byte day, byte hour, byte minute, byte second)
        {
            ushort reg1 = (ushort)((month << 8) | year);    // 月(低) 年(高)
            ushort reg2 = (ushort)((hour << 8) | day);      // 时(低) 日(高)
            ushort reg3 = (ushort)((second << 8) | minute); // 秒(低) 分(高)

            return new int[] { reg1, reg2, reg3 };
        }

        /// <summary>
        /// 获取现在的时间
        /// </summary>
        /// <returns></returns>
        public int[] GetNowDateTimeRegistersLE()
        {
            DateTime now = DateTime.Now;

            byte year = (byte)(now.Year % 100);
            byte month = (byte)now.Month;
            byte day = (byte)now.Day;
            byte hour = (byte)now.Hour;
            byte minute = (byte)now.Minute;
            byte second = (byte)now.Second;

            return BuildDateTimeRegistersLE(year, month, day, hour, minute, second);
        }


        private bool SystemTime_IsWorking;


        //设置值
        private string _SystemTime_Inputs;

        public string SystemTime_Inputs
        {
            get { return _SystemTime_Inputs; }
            set
            {
                _SystemTime_Inputs = value;
                RaiseProperChanged(nameof(SystemTime_Inputs));
                Command_SetSystemTime.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand Command_SetSystemTime { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void SystemTimeOperation()
        {
            try
            {
                SystemTime_IsWorking = true;
                // 禁用按钮
                Command_SetSystemTime.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(1000);//没有这个延时会报错
                    byte[] receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildWriteMultiRegisterFrame(1, 110, GetNowDateTimeRegistersLE()), 8);

                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                SystemTime_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetSystemTime.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        /// <summary>
        /// 获取当前时间，并以特定格式返回
        /// </summary>
        /// <returns></returns>
        private string GetTimeNow()
        {
            string timeNow = DateTime.Now.ToString("yy/MM/dd/HH/mm/ss");
            if (string.IsNullOrEmpty(timeNow))
            {
                return "010719121212";
            }
            else
            {
                string result = timeNow.Replace("/", "");

                return result;
            }
        }

        #endregion

        #region 选择Pack

        //下拉选项
        private List<string> _Pack = new List<string> { "Pack1", "Pack2", "Pack3", "Pack4", "Pack5", "Pack6", "Pack7", "Pack8", "Pack9", "Pack10", "Pack11", "Pack12", "Pack13", "Pack14", "Pack15" };

        public List<string> Pack
        {
            get { return _Pack; }
            set
            {
                _Pack = value;
                RaiseProperChanged(nameof(Pack));
            }
        }

        //已选项
        private string selectedPack = "Pack1";

        public string SelectedPack
        {
            get { return selectedPack; }
            set
            {
                selectedPack = value;
                SerialCommunicationService.address = getPackAddress(value);
                this.RaiseProperChanged(nameof(SelectedPack));
            }
        }


        /// <summary>
        /// 获取地址
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        int getPackAddress(string pack)
        {
            return int.Parse(pack.Substring(4));
        }


        #endregion

        #region 历史记录

        //历史记录
        private ObservableCollection<HistoryLodModel> _historyLodModels = new ObservableCollection<HistoryLodModel>();

        public ObservableCollection<HistoryLodModel> HistoryLods
        {
            get => _historyLodModels;
            set
            {
                if (_historyLodModels != value)
                {
                    _historyLodModels = value;
                    OnPropertyChanged(nameof(HistoryLods));
                }
                ;
            }

        }

        //导出到excel表中
        public RelayCommand ExportExcel { get; }

        //停止读取历史记录
        public RelayCommand StopReadComamnd { get; }

        //读取的条数
        private int _readCounts;

        public int ReadCounts
        {
            get { return _readCounts; }
            set
            {
                _readCounts = value;
                this.RaiseProperChanged(nameof(ReadCounts));
            }
        }


        private bool HistoryRead_IsWorking;

        public RelayCommand HistoryReadCommandBMS02 { get; }

        public RelayCommand HistoryReadCommandBMS01 { get; }

        public RelayCommand HistoryReadCommandBMS03 { get; }
        private bool stopReadFlag;
        private bool stopReadFlag_isWorking;

        /// <summary>
        /// BMS02点击设置
        /// </summary>
        private async void HistoryReadOperationBMS02()
        {
            try
            {
                HistoryRead_IsWorking = true;
                stopReadFlag = false;
                // 禁用按钮
                HistoryReadCommandBMS02.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在读取历史记录");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");
                HistoryLods.Clear();

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    for (int i = ReadCounts; ; i++)
                    {
                        //读取指令
                        Thread.Sleep(100);//没有这个延时会报错
                        short[] data = ModbusRTU.ParseRead20Response(SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead20Frame(1, 4, (ushort)i), 133));
                        if (data.Length == 64)
                        {
                            var model = new HistoryLodModel(data, cellNum);
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                HistoryLods.Add(model);
                            });
                        }
                        else
                        {
                            break;
                        }
                        if (stopReadFlag)
                        {
                            break;
                        }
                        //HistoryLods.Add(new HistoryLodModel(data));
                    }

                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                HistoryRead_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                HistoryReadCommandBMS02.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                stopReadFlag = true;
                UpdateState("历史记录读取完成");
            }
        }

        /// <summary>
        /// BMS01点击设置
        /// </summary>
        private async void HistoryReadOperationBMS01()
        {
            try
            {
                HistoryRead_IsWorking = true;
                stopReadFlag = false;
                // 禁用按钮
                HistoryReadCommandBMS01.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在读取历史记录");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");
                HistoryLods.Clear();

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    for (int i = ReadCounts; ; i++)
                    {
                        //读取指令
                        Thread.Sleep(100);//没有这个延时会报错
                        short[] data = ModbusRTU.ParseRead20Response(SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead20Frame(1, 4, (ushort)i), 133));
                        if (data.Length == 64)
                        {
                            var model = new HistoryLodModel(data, cellNum, 1);
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                HistoryLods.Add(model);
                            });
                        }
                        else
                        {
                            break;
                        }
                        if (stopReadFlag)
                        {
                            break;
                        }
                        //HistoryLods.Add(new HistoryLodModel(data));
                    }

                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                HistoryRead_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                HistoryReadCommandBMS01.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                stopReadFlag = true;
                UpdateState("历史记录读取完成");
            }
        }

        /// <summary>
        /// BMS03点击设置
        /// </summary>
        private async void HistoryReadOperationBMS03()
        {
            try
            {
                HistoryRead_IsWorking = true;
                stopReadFlag = false;
                // 禁用按钮
                HistoryReadCommandBMS03.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在读取历史记录");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");
                HistoryLods.Clear();

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    for (int i = ReadCounts; ; i++)
                    {
                        //读取指令
                        Thread.Sleep(100);//没有这个延时会报错
                        short[] data = ModbusRTU.ParseRead20Response(SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead20Frame(1, 4, (ushort)i), 133));
                        if (data.Length == 64)
                        {
                            var model = new HistoryLodModel(data, cellNum, 1, 0);
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                HistoryLods.Add(model);
                            });
                        }
                        else
                        {
                            break;
                        }
                        if (stopReadFlag)
                        {
                            break;
                        }
                        //HistoryLods.Add(new HistoryLodModel(data));
                    }

                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                HistoryRead_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                HistoryReadCommandBMS03.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                stopReadFlag = true;
                UpdateState("历史记录读取完成");
            }
        }


        /// <summary>
        /// 停止读取历史记录
        /// </summary>
        private async void StopReadOperation()
        {
            try
            {
                stopReadFlag_isWorking = true;
                // 禁用按钮
                StopReadComamnd.RaiseCanExecuteChanged();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    stopReadFlag = true;

                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                stopReadFlag_isWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                StopReadComamnd.RaiseCanExecuteChanged();


            }
        }

        #endregion

        #region 零点电流

        //零点电流
        private int _ZeroCurrent;

        public int ZeroCurrent
        {
            get { return _ZeroCurrent; }
            set
            {
                _ZeroCurrent = value;
                this.RaiseProperChanged(nameof(ZeroCurrent));
            }
        }


        private bool ZeroCurrent_IsWorking;


        //设置值
        private string _ZeroCurrent_Inputs;

        public string ZeroCurrent_Inputs
        {
            get { return _ZeroCurrent_Inputs; }
            set
            {
                _ZeroCurrent_Inputs = value;
                this.RaiseProperChanged(nameof(ZeroCurrent_Inputs));
                Command_SetZeroCurrent.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand Command_SetZeroCurrent { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void ZeroCurrentOperation()
        {
            try
            {
                ZeroCurrent_IsWorking = true;
                // 禁用按钮
                Command_SetZeroCurrent.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(2000);//没有这个延时会报错
                    string receive = SerialCommunicationService.SendSettingCommand("设置指令", ZeroCurrent_Inputs);

                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                ZeroCurrent_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetZeroCurrent.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        #endregion

        #region 充电电流

        private int _ChgCurrent;

        public int ChgCurrent
        {
            get { return _ChgCurrent; }
            set
            {
                _ChgCurrent = value;
                this.RaiseProperChanged(nameof(ChgCurrent));
            }
        }


        private bool ChgCurrent_IsWorking;


        //设置值
        private string _ChgCurrent_Inputs;

        public string ChgCurrent_Inputs
        {
            get { return _ChgCurrent_Inputs; }
            set
            {
                _ChgCurrent_Inputs = value;
                this.RaiseProperChanged(nameof(ChgCurrent_Inputs));
                Command_SetChgCurrent.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand Command_SetChgCurrent { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void ChgCurrentOperation()
        {
            try
            {
                ChgCurrent_IsWorking = true;
                // 禁用按钮
                Command_SetChgCurrent.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    byte[] receive;
                    int setData;
                    //执行设置指令
                    Thread.Sleep(2000);//没有这个延时会报错
                    if (ChgCurrent > 0)
                    {
                        //设置充电电流
                        setData = (int)(getDoubleValue(ChgCurrent_Inputs) * ChgCalibFactor / (ChgCurrent * 1000)) * 1000;
                        receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildWriteSingleRegisterFrame(1, 273, setData), 8);
                    }
                    else
                    {
                        //设置放电电流
                        setData = -(int)(getDoubleValue(ChgCurrent_Inputs) * DisCalibFactor / (ChgCurrent * 1000)) * 1000;
                        receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildWriteSingleRegisterFrame(1, 274, setData), 8);
                    }


                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                ChgCurrent_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetChgCurrent.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        #endregion

        #region 放电电流

        private int _DisCurrent;

        public int DisCurrent
        {
            get { return _DisCurrent; }
            set
            {
                _DisCurrent = value;
                this.RaiseProperChanged(nameof(DisCurrent));
            }
        }


        private bool DisCurrent_IsWorking;


        //设置值
        private string _DisCurrent_Inputs;

        public string DisCurrent_Inputs
        {
            get { return _DisCurrent_Inputs; }
            set
            {
                _DisCurrent_Inputs = value;
                this.RaiseProperChanged(nameof(DisCurrent_Inputs));
                Command_SetDisCurrent.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand Command_SetDisCurrent { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void DisCurrentOperation()
        {
            try
            {
                DisCurrent_IsWorking = true;
                // 禁用按钮
                Command_SetDisCurrent.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(2000);//没有这个延时会报错
                    string receive = SerialCommunicationService.SendSettingCommand("设置指令", DisCurrent_Inputs);

                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                DisCurrent_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetDisCurrent.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }


        #endregion

        #region 设计容量
        private int _DesignCap;

        public int DesignCap
        {
            get { return _DesignCap; }
            set
            {
                _DesignCap = value;
                this.RaiseProperChanged(nameof(DesignCap));
            }
        }


        private bool DesignCap_IsWorking;


        //设置值
        private string _DesignCap_Inputs;

        public string DesignCap_Inputs
        {
            get { return _DesignCap_Inputs; }
            set
            {
                _DesignCap_Inputs = value;
                this.RaiseProperChanged(nameof(DesignCap_Inputs));
                Command_SetDesignCap.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand Command_SetDesignCap { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void DesignCapOperation()
        {
            try
            {
                DesignCap_IsWorking = true;
                // 禁用按钮
                Command_SetDesignCap.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(2000);//没有这个延时会报错
                    byte[] receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildWriteSingleRegisterFrame(1, 252, (int)getDoubleValue(DesignCap_Inputs) * 100), 8);
                    if (receive.Length != 8)
                    {

                        receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead20Frame(1, 10, 1), 8);
                        if (receive.Length == 8)
                        {
                            OutIndex = receive[5];
                            MessageBox.Show($"写入失败，超出数据范围，索引：{OutIndex}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                DesignCap_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetDesignCap.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        #endregion

        #region 满充容量

        private int _FullCap;

        public int FullCap
        {
            get { return _FullCap; }
            set
            {
                _FullCap = value;
                this.RaiseProperChanged(nameof(FullCap));
            }
        }


        private bool FullCap_IsWorking;


        //设置值
        private string _FullCap_Inputs;

        public string FullCap_Inputs
        {
            get { return _FullCap_Inputs; }
            set
            {
                _FullCap_Inputs = value;
                this.RaiseProperChanged(nameof(FullCap_Inputs));
                Command_SetFullCap.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand Command_SetFullCap { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void FullCapOperation()
        {
            try
            {
                FullCap_IsWorking = true;
                // 禁用按钮
                Command_SetFullCap.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(2000);//没有这个延时会报错
                    //string receive = SerialCommunicationService.SendSettingCommand("设置指令", FullCap_Inputs);
                    byte[] receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildWriteSingleRegisterFrame(1, 253, (int)getDoubleValue(FullCap_Inputs) * 100), 8);
                    if (receive.Length != 8)
                    {

                        receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead20Frame(1, 10, 1), 8);
                        if (receive.Length == 8)
                        {
                            OutIndex = receive[5];
                            MessageBox.Show($"写入失败，超出数据范围，索引：{OutIndex}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                FullCap_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetFullCap.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        #endregion

        #region 剩余容量

        private int _RemainCap;

        public int RemainCap
        {
            get { return _RemainCap; }
            set
            {
                _RemainCap = value;
                this.RaiseProperChanged(nameof(RemainCap));
            }
        }


        private bool RemainCap_IsWorking;


        //设置值
        private string _RemainCap_Inputs;

        public string RemainCap_Inputs
        {
            get { return _RemainCap_Inputs; }
            set
            {
                _RemainCap_Inputs = value;
                this.RaiseProperChanged(nameof(RemainCap_Inputs));
                Command_SetRemainCap.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand Command_SetRemainCap { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void RemainCapOperation()
        {
            try
            {
                RemainCap_IsWorking = true;
                // 禁用按钮
                Command_SetRemainCap.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(2000);//没有这个延时会报错
                    //string receive = SerialCommunicationService.SendSettingCommand("设置指令", RemainCap_Inputs);
                    byte[] receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildWriteSingleRegisterFrame(1, 254, (int)getDoubleValue(RemainCap_Inputs) * 100), 8);
                    if (receive.Length != 8)
                    {

                        receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead20Frame(1, 10, 1), 8);
                        if (receive.Length == 8)
                        {
                            OutIndex = receive[5];
                            MessageBox.Show($"写入失败，超出数据范围，索引：{OutIndex}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                RemainCap_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetRemainCap.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }


        #endregion

        #region 循环次数

        private int _CycleCount;

        public int CycleCount
        {
            get { return _CycleCount; }
            set
            {
                _CycleCount = value;
                this.RaiseProperChanged(nameof(CycleCount));
            }
        }


        private bool CycleCount_IsWorking;


        //设置值
        private string _CycleCount_Inputs;

        public string CycleCount_Inputs
        {
            get { return _CycleCount_Inputs; }
            set
            {
                _CycleCount_Inputs = value;
                this.RaiseProperChanged(nameof(CycleCount_Inputs));
                Command_SetCycleCount.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand Command_SetCycleCount { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void CycleCountOperation()
        {
            try
            {
                CycleCount_IsWorking = true;
                // 禁用按钮
                Command_SetCycleCount.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(2000);//没有这个延时会报错
                    //string receive = SerialCommunicationService.SendSettingCommand("设置指令", CycleCount_Inputs);
                    byte[] receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildWriteSingleRegisterFrame(1, 255, (int)getDoubleValue(CycleCount_Inputs)), 8);
                    if (receive.Length != 8)
                    {

                        receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead20Frame(1, 10, 1), 8);
                        if (receive.Length == 8)
                        {
                            OutIndex = receive[5];
                            MessageBox.Show($"写入失败，超出数据范围，索引：{OutIndex}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                CycleCount_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetCycleCount.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        #endregion

        #region 蓝牙地址
        private string _BuleTooth;

        public string BuleTooth
        {
            get { return _BuleTooth; }
            set
            {
                _BuleTooth = value;
                this.RaiseProperChanged(nameof(BuleTooth));
            }
        }
        public void setBuletooth(string data)
        {
            if (data == null || data.Length < 4)
            {
                return;
            }
            BuleTooth = data;
        }
        public void SetBuleTooth(short[] data)
        {
            if (data != null && data.Length >= 6)
            {

                byte[] bluetoothBytes = new byte[6];
                bluetoothBytes[0] = (byte)(data[5] >> 8);       // 寄存器297的低字节
                bluetoothBytes[1] = (byte)(data[4] >> 8);
                bluetoothBytes[2] = (byte)(data[3] >> 8);
                bluetoothBytes[3] = (byte)(data[2] >> 8);
                bluetoothBytes[4] = (byte)(data[1] >> 8);
                bluetoothBytes[5] = (byte)(data[0] >> 8);

                // 格式化为 "XX:XX:XX:XX:XX:XX"
                string bluetoothStr = string.Format("{0:X2}:{1:X2}:{2:X2}:{3:X2}:{4:X2}:{5:X2}",
                    bluetoothBytes[0], bluetoothBytes[1], bluetoothBytes[2],
                    bluetoothBytes[3], bluetoothBytes[4], bluetoothBytes[5]);

                setBuletooth(bluetoothStr);
            }
            else
            {
                // 处理读取失败，例如显示错误信息
                AddLog("读取蓝牙地址失败");
            }
        }


        private bool BuleTooth_IsWorking;
        //蓝牙设置值
        private string _BuleTooth_Inputs;

        public string BuleTooth_Inputs
        {
            get { return _BuleTooth_Inputs; }
            set
            {
                _BuleTooth_Inputs = value;
                this.RaiseProperChanged(nameof(_BuleTooth_Inputs));
                Command_SetBuleTooth.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand Command_SetBuleTooth { get; }

        public int[] GetBullTooth()
        {
            // 先解析蓝牙地址
            if (!TryParseBluetoothAddress(BuleTooth_Inputs, out byte[] bluetoothBytes))
            {
                // 解析失败，弹出提示框
                MessageBox.Show(
                    "蓝牙地址格式不正确，应为12位十六进制数（可包含冒号或短横分隔）\n例如：00:1A:7D:DA:71:13",
                    "输入错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return null;
            }
            ushort[] bluetoothShort = new ushort[6];
            bluetoothShort[0] = bluetoothBytes[5];
            bluetoothShort[1] = bluetoothBytes[4];
            bluetoothShort[2] = bluetoothBytes[3];
            bluetoothShort[3] = bluetoothBytes[2];
            bluetoothShort[4] = bluetoothBytes[1];
            bluetoothShort[5] = bluetoothBytes[0];

            int[] BuleToothres = new int[] { bluetoothShort[0] << 8, bluetoothShort[1] << 8, bluetoothShort[2] << 8, bluetoothShort[3] << 8, bluetoothShort[4] << 8, bluetoothShort[5] << 8 };
            // 将6字节蓝牙地址拆分为6个寄存器值
            return BuleToothres;
        }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void BuleToothOperation()
        {
            try
            {
                BuleTooth_IsWorking = true;
                // 禁用按钮
                Command_SetBuleTooth.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                UpdateState("正在执行设置命令");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(() =>
                {
                    // 执行设置指令（硬件可能需要短暂延时）
                    Thread.Sleep(2000); // 保留原延时


                    byte[] receive = SerialCommunicationService.SendCommandToBMS(
                        ModbusRTU.BuildWriteMultiRegisterFrame(1, 297, GetBullTooth()), 8);
                    if (receive.Length != 8)
                    {
                        receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead20Frame(1, 14, 3), 8);
                        if (receive.Length == 8)
                        {
                            OutIndex = receive[5];
                            // 注意：MessageBox 不能在后台线程直接调用，需要调度到 UI 线程
                            Application.Current.Dispatcher.Invoke(() =>
                                MessageBox.Show($"写入失败，超出数据范围，索引：{OutIndex}", "错误", MessageBoxButton.OK, MessageBoxImage.Error));
                        }
                    }
                }, timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");

                BuleTooth_IsWorking = false;
                // 重新启用按钮
                Command_SetBuleTooth.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        public bool TryParseBluetoothAddress(string input, out byte[] bytes)
        {
            bytes = null;
            if (string.IsNullOrWhiteSpace(input))
                return false;

            string cleaned = Regex.Replace(input, @"[^0-9A-Fa-f]", "");
            if (cleaned.Length != 12)
                return false;

            try
            {
                bytes = new byte[6];
                for (int i = 0; i < 6; i++)
                {
                    string byteStr = cleaned.Substring(i * 2, 2);

                    bytes[i] = byte.Parse(byteStr, System.Globalization.NumberStyles.HexNumber, null);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 零点校准系数、+、-
        //系数
        private int _ZeroCalibFactor;

        public int ZeroCalibFactor
        {
            get { return _ZeroCalibFactor; }
            set
            {
                _ZeroCalibFactor = value;
                this.RaiseProperChanged(nameof(ZeroCalibFactor));
            }
        }


        private bool ZeroCalibFactorInc_IsWorking;

        //增加
        public RelayCommand Command_SetZeroCalibFactorInc { get; }

        /// <summary>
        /// 增加
        /// </summary>
        private async void ZeroCalibFactorIncOperation()
        {
            try
            {
                ZeroCalibFactorInc_IsWorking = true;
                // 禁用按钮
                Command_SetZeroCalibFactorInc.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(2000);//没有这个延时会报错
                    string receive = SerialCommunicationService.SendSettingCommand("设置指令", "ZeroCalibFactorInc_Inputs");

                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                ZeroCalibFactorInc_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetZeroCalibFactorInc.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        private bool ZeroCalibFactorDec_IsWorking;
        //减少
        public RelayCommand Command_SetZeroCalibFactorDec { get; }

        /// <summary>
        /// 减少
        /// </summary>
        private async void ZeroCalibFactorDecOperation()
        {
            try
            {
                ZeroCalibFactorDec_IsWorking = true;
                // 禁用按钮
                Command_SetZeroCalibFactorDec.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(2000);//没有这个延时会报错
                    string receive = SerialCommunicationService.SendSettingCommand("设置指令", "ZeroCalibFactorDec_Inputs");

                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                ZeroCalibFactorDec_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetZeroCalibFactorDec.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }


        #endregion

        #region 充电校准系数、+、-
        //校准系数
        private int _ChgCalibFactor;

        public int ChgCalibFactor
        {
            get { return _ChgCalibFactor; }
            set
            {
                _ChgCalibFactor = value;
                this.RaiseProperChanged(nameof(ChgCalibFactor));
            }
        }

        private bool ChgCalibFactorInc_IsWorking;
        //增加
        public RelayCommand Command_SetChgCalibFactorInc { get; }

        /// <summary>
        /// 点击设置 +
        /// </summary>
        private async void ChgCalibFactorIncOperation()
        {
            try
            {
                ChgCalibFactorInc_IsWorking = true;
                // 禁用按钮
                Command_SetChgCalibFactorInc.RaiseCanExecuteChanged();
                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    ChgCalibFactor = ChgCalibFactor + 1;
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                ChgCalibFactorInc_IsWorking = false;
                // 重新启用按钮
                Command_SetChgCalibFactorInc.RaiseCanExecuteChanged();
            }
        }


        private bool ChgCalibFactorDec_IsWorking;
        // 减少
        public RelayCommand Command_SetChgCalibFactorDec { get; }

        /// <summary>
        /// 点击设置 -
        /// </summary>
        private async void ChgCalibFactorDecOperation()
        {
            try
            {
                ChgCalibFactorDec_IsWorking = true;
                // 禁用按钮
                Command_SetChgCalibFactorDec.RaiseCanExecuteChanged();
                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    ChgCalibFactor = ChgCalibFactor - 1;
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                ChgCalibFactorDec_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetChgCalibFactorDec.RaiseCanExecuteChanged();
            }
        }


        private bool ChgCalibFactorRead_IsWorking;
        // 读取
        public RelayCommand Command_SetChgCalibFactorRead { get; }

        private async void ChgCalibFactorReadOperation()
        {
            try
            {
                ChgCalibFactorRead_IsWorking = true;
                // 禁用按钮
                Command_SetChgCalibFactorRead.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(1000);//没有这个延时会报错
                    //string receive = SerialCommunicationService.SendSettingCommand("设置指令", "ChgCalibFactorInc_Inputs");
                    byte[] receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame(1, 273, 1), 7);
                    ChgCalibFactor = ModbusRTU.ParseRead03Response(receive)[0];
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                ChgCalibFactorRead_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetChgCalibFactorRead.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        private bool ChgCalibFactorWrite_IsWorking;
        // 写入
        public RelayCommand Command_SetChgCalibFactorWrite { get; }

        private async void ChgCalibFactorWriteOperation()
        {
            try
            {
                ChgCalibFactorWrite_IsWorking = true;
                // 禁用按钮
                Command_SetChgCalibFactorWrite.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(1000);//没有这个延时会报错
                    //string receive = SerialCommunicationService.SendSettingCommand("设置指令", "ChgCalibFactorInc_Inputs");
                    byte[] receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildWriteSingleRegisterFrame(1, 273, ChgCalibFactor), 8);

                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                ChgCalibFactorWrite_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetChgCalibFactorWrite.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }


        #endregion

        #region 放电校准系数、+、-

        //放电校准系数
        private int _DisCalibFactor;

        public int DisCalibFactor
        {
            get { return _DisCalibFactor; }
            set
            {
                _DisCalibFactor = value;
                this.RaiseProperChanged(nameof(DisCalibFactor));
            }
        }

        //增加
        private bool DisCalibFactorInc_IsWorking;

        public RelayCommand Command_SetDisCalibFactorInc { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void DisCalibFactorIncOperation()
        {
            try
            {
                DisCalibFactorInc_IsWorking = true;
                // 禁用按钮
                Command_SetDisCalibFactorInc.RaiseCanExecuteChanged();

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    DisCalibFactor = DisCalibFactor + 1;

                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程

                DisCalibFactorInc_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetDisCalibFactorInc.RaiseCanExecuteChanged();

            }
        }

        //减少
        private bool DisCalibFactorDec_IsWorking;

        public RelayCommand Command_SetDisCalibFactorDec { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void DisCalibFactorDecOperation()
        {
            try
            {
                DisCalibFactorDec_IsWorking = true;
                // 禁用按钮
                Command_SetDisCalibFactorDec.RaiseCanExecuteChanged();

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    DisCalibFactor = DisCalibFactor - 1;

                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {

                DisCalibFactorDec_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetDisCalibFactorDec.RaiseCanExecuteChanged();

            }
        }


        //读取
        private bool DisCalibFactorRead_IsWorking;

        public RelayCommand Command_SetDisCalibFactorRead { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void DisCalibFactorReadOperation()
        {
            try
            {
                DisCalibFactorRead_IsWorking = true;
                // 禁用按钮
                Command_SetDisCalibFactorRead.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(1000);//没有这个延时会报错
                    //string receive = SerialCommunicationService.SendSettingCommand("设置指令", "DisCalibFactorDec_Inputs");
                    byte[] receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame(1, 274, 1), 7);
                    DisCalibFactor = ModbusRTU.ParseRead03Response(receive)[0];

                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                DisCalibFactorRead_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetDisCalibFactorRead.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        //写入
        private bool DisCalibFactorWrite_IsWorking;

        public RelayCommand Command_SetDisCalibFactorWrite { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void DisCalibFactorWriteOperation()
        {
            try
            {
                DisCalibFactorWrite_IsWorking = true;
                // 禁用按钮
                Command_SetDisCalibFactorWrite.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(1000);//没有这个延时会报错
                    //string receive = SerialCommunicationService.SendSettingCommand("设置指令", "DisCalibFactorDec_Inputs");
                    byte[] receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildWriteSingleRegisterFrame(1, 274, DisCalibFactor), 8);
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                DisCalibFactorWrite_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetDisCalibFactorWrite.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        #endregion

        #region 电池组温度系数1、+、-
        //cellTemp1
        //校准系数
        private int _cellTemp1;

        public int CellTemp1
        {
            get { return _cellTemp1; }
            set
            {
                _cellTemp1 = value;
                this.RaiseProperChanged(nameof(CellTemp1));
            }
        }

        private bool CellTemp1Inc_IsWorking;
        //增加
        public RelayCommand Command_SetCellTemp1Inc { get; }

        /// <summary>
        /// 点击设置 +
        /// </summary>
        private async void CellTemp1IncOperation()
        {
            try
            {
                CellTemp1Inc_IsWorking = true;
                // 禁用按钮
                Command_SetCellTemp1Inc.RaiseCanExecuteChanged();
                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    CellTemp1 = CellTemp1 + 1;
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                CellTemp1Inc_IsWorking = false;
                // 重新启用按钮
                Command_SetCellTemp1Inc.RaiseCanExecuteChanged();
            }
        }


        private bool CellTemp1Dec_IsWorking;
        // 减少
        public RelayCommand Command_SetCellTemp1Dec { get; }

        /// <summary>
        /// 点击设置 -
        /// </summary>
        private async void CellTemp1DecOperation()
        {
            try
            {
                CellTemp1Dec_IsWorking = true;
                // 禁用按钮
                Command_SetCellTemp1Dec.RaiseCanExecuteChanged();
                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    CellTemp1 = CellTemp1 - 1;
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                CellTemp1Dec_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetCellTemp1Dec.RaiseCanExecuteChanged();
            }
        }


        private bool CellTemp1Read_IsWorking;
        // 读取
        public RelayCommand Command_SetCellTemp1Read { get; }

        private async void CellTemp1ReadOperation()
        {
            try
            {
                CellTemp1Read_IsWorking = true;
                // 禁用按钮
                Command_SetCellTemp1Read.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(1000);//没有这个延时会报错
                    //string receive = SerialCommunicationService.SendSettingCommand("设置指令", "ChgCalibFactorInc_Inputs");
                    byte[] receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame(1, 277, 1), 7);
                    CellTemp1 = ModbusRTU.ParseRead03Response(receive)[0];
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                CellTemp1Read_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetCellTemp1Read.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        private bool CellTemp1Write_IsWorking;
        // 写入
        public RelayCommand Command_SetCellTemp1Write { get; }

        private async void CellTemp1WriteOperation()
        {
            try
            {
                CellTemp1Write_IsWorking = true;
                // 禁用按钮
                Command_SetCellTemp1Write.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(1000);//没有这个延时会报错
                    //string receive = SerialCommunicationService.SendSettingCommand("设置指令", "ChgCalibFactorInc_Inputs");
                    byte[] receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildWriteSingleRegisterFrame(1, 277, CellTemp1), 8);
                    if (receive.Length != 8)
                    {

                        receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead20Frame(1, 10, 1), 8);
                        if (receive.Length == 8)
                        {
                            OutIndex = receive[5];
                            MessageBox.Show($"写入失败，超出数据范围，索引：{OutIndex}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                CellTemp1Write_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetCellTemp1Write.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }


        #endregion

        #region 电池组温度系数2、+、-
        //cellTemp1
        //校准系数
        private int _cellTemp2;

        public int CellTemp2
        {
            get { return _cellTemp2; }
            set
            {
                _cellTemp2 = value;
                this.RaiseProperChanged(nameof(CellTemp2));
            }
        }

        private bool CellTemp2Inc_IsWorking;
        //增加
        public RelayCommand Command_SetCellTemp2Inc { get; }

        /// <summary>
        /// 点击设置 +
        /// </summary>
        private async void CellTemp2IncOperation()
        {
            try
            {
                CellTemp2Inc_IsWorking = true;
                // 禁用按钮
                Command_SetCellTemp2Inc.RaiseCanExecuteChanged();
                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    CellTemp2 = CellTemp2 + 1;
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                CellTemp2Inc_IsWorking = false;
                // 重新启用按钮
                Command_SetCellTemp2Inc.RaiseCanExecuteChanged();
            }
        }


        private bool CellTemp2Dec_IsWorking;
        // 减少
        public RelayCommand Command_SetCellTemp2Dec { get; }

        /// <summary>
        /// 点击设置 -
        /// </summary>
        private async void CellTemp2DecOperation()
        {
            try
            {
                CellTemp2Dec_IsWorking = true;
                // 禁用按钮
                Command_SetCellTemp2Dec.RaiseCanExecuteChanged();
                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    CellTemp2 = CellTemp2 - 1;
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                CellTemp2Dec_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetCellTemp2Dec.RaiseCanExecuteChanged();
            }
        }


        private bool CellTemp2Read_IsWorking;
        // 读取
        public RelayCommand Command_SetCellTemp2Read { get; }

        private async void CellTemp2ReadOperation()
        {
            try
            {
                CellTemp2Read_IsWorking = true;
                // 禁用按钮
                Command_SetCellTemp2Read.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(1000);//没有这个延时会报错
                    //string receive = SerialCommunicationService.SendSettingCommand("设置指令", "ChgCalibFactorInc_Inputs");
                    byte[] receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame(1, 278, 1), 7);
                    CellTemp2 = ModbusRTU.ParseRead03Response(receive)[0];
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                CellTemp2Read_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetCellTemp2Read.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        private bool CellTemp2Write_IsWorking;
        // 写入
        public RelayCommand Command_SetCellTemp2Write { get; }

        private async void CellTemp2WriteOperation()
        {
            try
            {
                CellTemp2Write_IsWorking = true;
                // 禁用按钮
                Command_SetCellTemp2Write.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(1000);//没有这个延时会报错
                    //string receive = SerialCommunicationService.SendSettingCommand("设置指令", "ChgCalibFactorInc_Inputs");
                    byte[] receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildWriteSingleRegisterFrame(1, 278, CellTemp2), 8);
                    if (receive.Length != 8)
                    {

                        receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead20Frame(1, 10, 1), 8);
                        if (receive.Length == 8)
                        {
                            OutIndex = receive[5];
                            MessageBox.Show($"写入失败，超出数据范围，索引：{OutIndex}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                CellTemp2Write_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetCellTemp2Write.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }


        #endregion

        #region 电池组温度系数3、+、-
        //cellTemp1
        //校准系数
        private int _cellTemp3;

        public int CellTemp3
        {
            get { return _cellTemp3; }
            set
            {
                _cellTemp3 = value;
                this.RaiseProperChanged(nameof(CellTemp3));
            }
        }

        private bool CellTemp3Inc_IsWorking;
        //增加
        public RelayCommand Command_SetCellTemp3Inc { get; }

        /// <summary>
        /// 点击设置 +
        /// </summary>
        private async void CellTemp3IncOperation()
        {
            try
            {
                CellTemp3Inc_IsWorking = true;
                // 禁用按钮
                Command_SetCellTemp3Inc.RaiseCanExecuteChanged();
                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    CellTemp3 = CellTemp3 + 1;
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                CellTemp3Inc_IsWorking = false;
                // 重新启用按钮
                Command_SetCellTemp3Inc.RaiseCanExecuteChanged();
            }
        }


        private bool CellTemp3Dec_IsWorking;
        // 减少
        public RelayCommand Command_SetCellTemp3Dec { get; }

        /// <summary>
        /// 点击设置 -
        /// </summary>
        private async void CellTemp3DecOperation()
        {
            try
            {
                CellTemp3Dec_IsWorking = true;
                // 禁用按钮
                Command_SetCellTemp3Dec.RaiseCanExecuteChanged();
                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    CellTemp3 = CellTemp3 - 1;
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                CellTemp3Dec_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetCellTemp3Dec.RaiseCanExecuteChanged();
            }
        }


        private bool CellTemp3Read_IsWorking;
        // 读取
        public RelayCommand Command_SetCellTemp3Read { get; }

        private async void CellTemp3ReadOperation()
        {
            try
            {
                CellTemp3Read_IsWorking = true;
                // 禁用按钮
                Command_SetCellTemp3Read.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(1000);//没有这个延时会报错
                    //string receive = SerialCommunicationService.SendSettingCommand("设置指令", "ChgCalibFactorInc_Inputs");
                    byte[] receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame(1, 279, 1), 7);
                    CellTemp3 = ModbusRTU.ParseRead03Response(receive)[0];
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                CellTemp3Read_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetCellTemp3Read.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        private bool CellTemp3Write_IsWorking;
        // 写入
        public RelayCommand Command_SetCellTemp3Write { get; }

        private async void CellTemp3WriteOperation()
        {
            try
            {
                CellTemp3Write_IsWorking = true;
                // 禁用按钮
                Command_SetCellTemp3Write.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(1000);//没有这个延时会报错
                    //string receive = SerialCommunicationService.SendSettingCommand("设置指令", "ChgCalibFactorInc_Inputs");
                    byte[] receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildWriteSingleRegisterFrame(1, 279, CellTemp3), 8);
                    if (receive.Length != 8)
                    {

                        receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead20Frame(1, 10, 1), 8);
                        if (receive.Length == 8)
                        {
                            OutIndex = receive[5];
                            MessageBox.Show($"写入失败，超出数据范围，索引：{OutIndex}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                CellTemp3Write_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetCellTemp3Write.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }


        #endregion

        #region 电池组温度系数4、+、-
        //cellTemp1
        //校准系数
        private int _cellTemp4;

        public int CellTemp4
        {
            get { return _cellTemp4; }
            set
            {
                _cellTemp4 = value;
                this.RaiseProperChanged(nameof(CellTemp4));
            }
        }

        private bool CellTemp4Inc_IsWorking;
        //增加
        public RelayCommand Command_SetCellTemp4Inc { get; }

        /// <summary>
        /// 点击设置 +
        /// </summary>
        private async void CellTemp4IncOperation()
        {
            try
            {
                CellTemp4Inc_IsWorking = true;
                // 禁用按钮
                Command_SetCellTemp4Inc.RaiseCanExecuteChanged();
                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    CellTemp4 = CellTemp4 + 1;
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                CellTemp4Inc_IsWorking = false;
                // 重新启用按钮
                Command_SetCellTemp4Inc.RaiseCanExecuteChanged();
            }
        }


        private bool CellTemp4Dec_IsWorking;
        // 减少
        public RelayCommand Command_SetCellTemp4Dec { get; }

        /// <summary>
        /// 点击设置 -
        /// </summary>
        private async void CellTemp4DecOperation()
        {
            try
            {
                CellTemp4Dec_IsWorking = true;
                // 禁用按钮
                Command_SetCellTemp4Dec.RaiseCanExecuteChanged();
                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    CellTemp4 = CellTemp4 - 1;
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                CellTemp4Dec_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetCellTemp4Dec.RaiseCanExecuteChanged();
            }
        }


        private bool CellTemp4Read_IsWorking;
        // 读取
        public RelayCommand Command_SetCellTemp4Read { get; }

        private async void CellTemp4ReadOperation()
        {
            try
            {
                CellTemp4Read_IsWorking = true;
                // 禁用按钮
                Command_SetCellTemp4Read.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(1000);//没有这个延时会报错
                    //string receive = SerialCommunicationService.SendSettingCommand("设置指令", "ChgCalibFactorInc_Inputs");
                    byte[] receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead03Frame(1, 280, 1), 7);
                    CellTemp4 = ModbusRTU.ParseRead03Response(receive)[0];
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                CellTemp4Read_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetCellTemp4Read.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        private bool CellTemp4Write_IsWorking;
        // 写入
        public RelayCommand Command_SetCellTemp4Write { get; }

        private async void CellTemp4WriteOperation()
        {
            try
            {
                CellTemp4Write_IsWorking = true;
                // 禁用按钮
                Command_SetCellTemp4Write.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(1000);//没有这个延时会报错
                    //string receive = SerialCommunicationService.SendSettingCommand("设置指令", "ChgCalibFactorInc_Inputs");
                    byte[] receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildWriteSingleRegisterFrame(1, 280, CellTemp4), 8);
                    if (receive.Length != 8)
                    {

                        receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead20Frame(1, 10, 1), 8);
                        if (receive.Length == 8)
                        {
                            OutIndex = receive[5];
                            MessageBox.Show($"写入失败，超出数据范围，索引：{OutIndex}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                CellTemp4Write_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetCellTemp4Write.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }


        #endregion

        #region 清除历史记录


        private bool ClearHistory_IsWorking;


        public RelayCommand Command_SetClearHistory { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void ClearHistoryOperation()
        {
            try
            {
                ClearHistory_IsWorking = true;
                // 禁用按钮
                Command_SetClearHistory.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(2000);//没有这个延时会报错
                    //string receive = SerialCommunicationService.SendSettingCommand("设置指令", "ClearHistory_Inputs");
                    byte[] receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead20Frame(1, 3, 1), 8);

                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                ClearHistory_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetClearHistory.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }


        #endregion

        #region 重置用户参数

        private bool ResetUserParams_IsWorking;

        public RelayCommand Command_SetResetUserParams { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void ResetUserParamsOperation()
        {
            try
            {
                ResetUserParams_IsWorking = true;
                // 禁用按钮
                Command_SetResetUserParams.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(2000);//没有这个延时会报错
                    byte[] receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead20Frame(1, 1, 1), 8);
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                ResetUserParams_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetResetUserParams.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        #endregion

        #region 重置系统参数

        private bool ResetSysParams_IsWorking;

        public RelayCommand Command_SetResetSysParams { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void ResetSysParamsOperation()
        {
            try
            {
                ResetSysParams_IsWorking = true;
                // 禁用按钮
                Command_SetResetSysParams.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(2000);//没有这个延时会报错
                    byte[] receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead20Frame(1, 2, 1), 8);

                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                ResetSysParams_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetResetSysParams.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        #endregion

        #region 关闭充电MOS


        public RelayCommand CloseCharMOSCommand
        {
            get;
            set
            ;
        }

        private bool CloseCharMOS_IsWorking;

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void CloseCharMOSOperation()
        {
            try
            {
                CloseCharMOS_IsWorking = true;
                // 禁用按钮
                CloseCharMOSCommand.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(2000);//没有这个延时会报错
                    byte[] receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead20Frame(1, 5, (ushort)(settingStatue[0] == 1 ? 0 : 1)), 8);
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                CloseCharMOS_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                CloseCharMOSCommand.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }


        #endregion

        #region 关闭放电MOS


        public RelayCommand CloseDisCharMOSCommand
        {
            get;
            set
            ;
        }

        private bool CloseDisCharMOS_IsWorking;

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void CloseDisCharMOSOperation()
        {
            try
            {
                CloseDisCharMOS_IsWorking = true;
                // 禁用按钮
                CloseDisCharMOSCommand.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(2000);//没有这个延时会报错
                    byte[] receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead20Frame(1, 6, (ushort)(settingStatue[1] == 1 ? 0 : 1)), 8);
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                CloseDisCharMOS_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                CloseDisCharMOSCommand.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }


        #endregion

        #region 休眠

        public int isSleeping = 0;

        public RelayCommand SleepCommand
        {
            get;
            set
            ;
        }

        private bool Sleep_IsWorking;

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void SleepOperation()
        {
            try
            {
                Sleep_IsWorking = true;
                // 禁用按钮
                SleepCommand.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(1000);//没有这个延时会报错
                    ushort open = (ushort)(settingStatue[4] == 1 ? 0 : 1);

                    isSleeping = open;

                    byte[] receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead20Frame(1, 10, open), 8);
                    if (receive.Length == 8)
                    {
                        SettingStatue[4] = open;
                    }
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                Sleep_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                SleepCommand.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        #endregion

        #region 深度睡眠


        public RelayCommand CancelSleepCommand
        {
            get;
            set
            ;
        }

        private bool CancelSleep_IsWorking;

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void CancelSleepOperation()
        {
            try
            {
                CancelSleep_IsWorking = true;
                // 禁用按钮
                CancelSleepCommand.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(2000);//没有这个延时会报错
                    byte[] receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead20Frame(1, 11, 1), 8);
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                CancelSleep_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                CancelSleepCommand.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }
        #endregion

        #region 关机

        public RelayCommand TurnOffCommand { get; set; }


        private bool TurnOff_IsWorking;

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void TurnOffOperation()
        {
            try
            {
                TurnOff_IsWorking = true;
                // 禁用按钮
                TurnOffCommand.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(2000);//没有这个延时会报错
                    ushort shutDown = (ushort)(settingStatue[2] == 1 ? 0 : 1);
                    byte[] receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead20Frame(1, 7, shutDown), 8);
                    if (receive.Length == 8)
                    {
                        settingStatue[2] = (ushort)(settingStatue[2] == 1 ? 0 : 1);
                    }
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                TurnOff_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                TurnOffCommand.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        #endregion

        #region 限流板

        public RelayCommand LimitCommand { get; set; }


        private bool Limit_IsWorking;

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void LimitOperation()
        {
            try
            {
                Limit_IsWorking = true;
                // 禁用按钮
                LimitCommand.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(2000);//没有这个延时会报错
                    ushort shutDown = (ushort)(settingStatue[3] == 1 ? 0 : 1);
                    byte[] receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead20Frame(1, 9, shutDown), 8);
                    if (receive.Length == 8)
                    {
                        settingStatue[2] = (ushort)(settingStatue[2] == 1 ? 0 : 1);
                    }
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                Limit_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                LimitCommand.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }


        #endregion

        #region 设置状态颜色显示 和 均衡状态显示

        /// <summary>
        /// 设置状态颜色
        /// </summary>
        private int[] settingStatue = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public int[] SettingStatue
        {
            get { return settingStatue; }
            set
            {
                settingStatue = value;
                this.RaiseProperChanged(nameof(SettingStatue));
            }
        }

        /// <summary>
        /// 均衡状态
        /// </summary>
        private int[] junHen = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public int[] JunHen
        {
            get { return junHen; }
            set
            {
                junHen = value;
                this.RaiseProperChanged(nameof(JunHen));
            }
        }




        #endregion

        #region 获取超出范围数据索引

        private int outIndex;

        public int OutIndex
        {
            get { return outIndex; }
            set
            {
                outIndex = value;
                this.RaiseProperChanged(nameof(OutIndex));
            }
        }


        public RelayCommand GetOutIndexCommand { get; set; }


        private bool GetOutIndex_IsWorking;

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void GetOutIndexOperation()
        {
            try
            {
                GetOutIndex_IsWorking = true;
                // 禁用按钮
                GetOutIndexCommand.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(2000);//没有这个延时会报错
                    byte[] receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead20Frame(1, 8, 1), 8);
                    if (receive.Length == 8)
                    {
                        OutIndex = receive[5];
                    }
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                GetOutIndex_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                GetOutIndexCommand.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        #endregion

        #region 获取超出范围数据索引2

        private int outIndex2;

        public int OutIndex2
        {
            get { return outIndex2; }
            set
            {
                outIndex2 = value;
                this.RaiseProperChanged(nameof(OutIndex2));
            }
        }


        public RelayCommand GetOutIndexCommand2 { get; set; }


        private bool GetOutIndex_IsWorking2;

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void GetOutIndexOperation2()
        {
            try
            {
                GetOutIndex_IsWorking2 = true;
                // 禁用按钮
                GetOutIndexCommand2.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState("正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(2000);//没有这个延时会报错
                    byte[] receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead20Frame(1, 8, 1), 8);
                    if (receive.Length == 8)
                    {
                        OutIndex2 = receive[5];
                    }
                })
                , timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                AddLog("特殊操作执行超时");
            }
            finally
            {
                // 恢复后台线程
                _pauseEvent.Set();
                AddLog("恢复后台通信");
                GetOutIndex_IsWorking2 = false;
                //Status = "就绪";
                // 重新启用按钮
                GetOutIndexCommand2.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        #endregion

        #region 功能方法
        //配置文件信息
        // 集合属性
        private ObservableCollection<SendingCommand> _sendingCommands;
        public ObservableCollection<SendingCommand> SendingCommands
        {
            get => _sendingCommands;
            set
            {
                if (_sendingCommands != value)
                {
                    _sendingCommands = value;
                    OnPropertyChanged(nameof(SendingCommands)); // 触发属性变更通知
                }
            }
        }

        //前端芯片监控
        private ObservableCollection<SendingCommand> frontMonitor;

        public ObservableCollection<SendingCommand> FrontMonitor
        {
            get { return frontMonitor; }
            set
            {
                frontMonitor = value;
                this.RaiseProperChanged(nameof(FrontMonitor));
            }
        }

        /// <summary>
        /// 初始化前端芯片监控数据
        /// </summary>
        private void frontMonitorInitial()
        {

            FrontMonitor = new ObservableCollection<SendingCommand>();
            for (int i = 0; i < 36; i++)
            {
                FrontMonitor.Add(new SendingCommand() { ReturnCount = (i + 320).ToString(), Command = FrontMonitorNames[i] });
            }
        }

        //前端芯片监控集合
        public ListCollectionView FrontView1 { get; set; }
        public ListCollectionView FrontView2 { get; set; }
        public ListCollectionView FrontView3 { get; set; }
        public ListCollectionView FrontView4 { get; set; }
        public ListCollectionView FrontView5 { get; set; }
        public ListCollectionView FrontView6 { get; set; }

        /// <summary>
        /// 前端芯片监控赋值
        /// </summary>
        /// <param name="data"></param>
        public void SetFrontMonitor(short[] data)
        {
            if (data == null || data.Length < 36)
                return;
            for (int i = 0; i < 36; i++)
            {
                FrontMonitor[i].AnalysisMethod = "0x" + data[i].ToString("X4");
            }
        }

        string[] FrontMonitorNames = new string[] { "AFE STRG0 0", "AFE STRG0 1" , "AFE STRG0 2" , "AFE STRG0 3" , "AFE STRG0 4" , "AFE STRG0 5" ,
        "AFE STRG1 0","AFE STRG1 1","AFE STRG1 2","AFE STRG1 3","AFE STRG1 4","AFE STRG1 5","AFE STRG2 0","AFE STRG2 1","AFE STRG2 2" ,
        "AFE STRG2 3","AFE STRG2 4","AFE STRG2 5","AFE STRG3 0","AFE STRG3 1","AFE STRG3 2","AFE STRG3 3","AFE STRG3 4","AFE STRG3 5",
        "AFE CFRG0 0","AFE CFRG0 1","AFE CFRG0 2","AFE CFRG0 3","AFE CFRG0 4","AFE CFRG0 5","AFE CFRG1 0","AFE CFRG1 1","AFE CFRG1 2",
        "AFE CFRG1 3","AFE CFRG1 4","AFE CFRG1 5" };




        /// <summary>
        /// 把字符串转成double
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private double getDoubleValue(string data)
        {
            // 尝试解析字符串为整数
            if (!double.TryParse(data, out double value))
            {
                throw new ArgumentException($"无法将 '{data}' 解析为整数。", "字符串");
            }
            return value;
        }


        //单体过充
        public ListCollectionView SendingCommandsView_1 { get; set; }
        //总体过充
        public ListCollectionView SendingCommandsView_2 { get; set; }
        //单体过放
        public ListCollectionView SendingCommandsView_3 { get; set; }
        //总体过放
        public ListCollectionView SendingCommandsView_4 { get; set; }
        //充电过流
        public ListCollectionView SendingCommandsView_5 { get; set; }
        //放电过流
        public ListCollectionView SendingCommandsView_6 { get; set; }
        //均衡
        public ListCollectionView SendingCommandsView_7 { get; set; }
        //单体休眠和电池包
        public ListCollectionView SendingCommandsView_8 { get; set; }
        //充电高温1
        public ListCollectionView SendingCommandsView_9 { get; set; }
        //充电低温
        public ListCollectionView SendingCommandsView_10 { get; set; }
        //充电高温2
        public ListCollectionView SendingCommandsView_11 { get; set; }
        //放电低温
        public ListCollectionView SendingCommandsView_12 { get; set; }
        //MOS
        public ListCollectionView SendingCommandsView_13 { get; set; }
        //放电高温
        public ListCollectionView SendingCommandsView_14 { get; set; }
        //环境高温
        public ListCollectionView SendingCommandsView_15 { get; set; }
        //预充与限流
        public ListCollectionView SendingCommandsView_16 { get; set; }
        //散热与加热
        public ListCollectionView SendingCommandsView_17 { get; set; }
        //其他
        public ListCollectionView SendingCommandsView_18 { get; set; }
        //其他
        public ListCollectionView SendingCommandsView_19 { get; set; }
        //充电过流2
        public ListCollectionView SendingCommandsView_20 { get; set; }
        //AFE 短路保护参数
        public ListCollectionView SendingCommandsView_21 { get; set; }
        //AFE 过充保护参数
        public ListCollectionView SendingCommandsView_22 { get; set; }
        //AFE 过放保护参数
        public ListCollectionView SendingCommandsView_23 { get; set; }
        //AFE 充电过流保护电流
        public ListCollectionView SendingCommandsView_24 { get; set; }
        //AFE 充电过流保护延时
        public ListCollectionView SendingCommandsView_25 { get; set; }
        //AFE 充电过流保护恢复时间
        public ListCollectionView SendingCommandsView_26 { get; set; }
        //AFE 放电过流保护电流
        public ListCollectionView SendingCommandsView_27 { get; set; }
        //AFE 放电过流保护延时
        public ListCollectionView SendingCommandsView_28 { get; set; }
        //AFE 放电过流保护恢复时间
        public ListCollectionView SendingCommandsView_29 { get; set; }

        //短路保护延时
        public ListCollectionView SendingCommandsView_30 { get; set; }

        //保存
        public ICommand SaveCommand { get; }
        //导出
        public ICommand SaveCommandToFile { get; set; }
        //导入
        public ICommand LoadFromFile { get; set; }
        //保存路径
        private string _filePathToSave;

        public string filePathToSave
        {
            get { return _filePathToSave; }
            set
            {
                _filePathToSave = value;
                this.OnPropertyChanged(nameof(filePathToSave));
            }
        }

        //导入的文件名
        private string fileName;

        public string FileName
        {
            get { return fileName; }
            set
            {
                fileName = value;
                this.OnPropertyChanged(nameof(FileName));
            }
        }

        /// <summary>
        /// 加载默认配置文件
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<SendingCommand> LoadSettings()
        {
            try
            {
                string path = "default.xml";
                filePathToSave = path;
                if (File.Exists(path))
                {
                    // 1. 读取文件内容
                    using var fileStream = new FileStream(path, FileMode.Open);
                    var wrapperSerializer = new XmlSerializer(typeof(ConfigWrapper));
                    var wrapper = (ConfigWrapper)wrapperSerializer.Deserialize(fileStream);

                    // 2. 计算读取数据的哈希（直接对字节数组计算）
                    using var sha256 = SHA256.Create();
                    string computedHashString = System.Convert.ToBase64String(sha256.ComputeHash(wrapper.DataBytes));

                    // 3. 验证哈希
                    if (computedHashString != wrapper.Hash)
                    {
                        MessageBox.Show("配置文件已损坏或被篡改！", "错误",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return new ObservableCollection<SendingCommand>();
                    }

                    // 4. 反序列化原始数据
                    using var dataStream = new MemoryStream(wrapper.DataBytes);
                    var serializer = new XmlSerializer(typeof(ObservableCollection<SendingCommand>));
                    return (ObservableCollection<SendingCommand>)serializer.Deserialize(dataStream);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("配置文件已损坏或被篡改！", "错误",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"加载设置时出错: {ex.Message}");
            }
            return new ObservableCollection<SendingCommand>();
        }

        public ObservableCollection<SendingCommand> LoadSettings2()
        {
            try
            {
                string path = "default2.xml";
                filePathToSave = path;
                if (File.Exists(path))
                {
                    // 1. 读取文件内容
                    using var fileStream = new FileStream(path, FileMode.Open);
                    var wrapperSerializer = new XmlSerializer(typeof(ConfigWrapper));
                    var wrapper = (ConfigWrapper)wrapperSerializer.Deserialize(fileStream);

                    // 2. 计算读取数据的哈希（直接对字节数组计算）
                    using var sha256 = SHA256.Create();
                    string computedHashString = System.Convert.ToBase64String(sha256.ComputeHash(wrapper.DataBytes));

                    // 3. 验证哈希
                    if (computedHashString != wrapper.Hash)
                    {
                        MessageBox.Show("配置文件已损坏或被篡改！", "错误",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return new ObservableCollection<SendingCommand>();
                    }

                    // 4. 反序列化原始数据
                    using var dataStream = new MemoryStream(wrapper.DataBytes);
                    var serializer = new XmlSerializer(typeof(ObservableCollection<SendingCommand>));
                    return (ObservableCollection<SendingCommand>)serializer.Deserialize(dataStream);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("配置文件已损坏或被篡改！", "错误",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"加载设置时出错: {ex.Message}");
            }
            return new ObservableCollection<SendingCommand>();
        }

        /// <summary>
        /// 导入配置文件
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<SendingCommand> LoadSettingsFromFile()
        {
            try
            {
                // 创建文件打开对话框
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "XML文件 (*.xml)|*.xml",
                    Title = "选择配置文件",
                    CheckFileExists = true,
                    Multiselect = false
                };

                // 显示对话框并等待用户选择
                var dialogResult = openFileDialog.ShowDialog();
                if (dialogResult == true)
                {
                    // 获取用户选择的文件路径
                    string filePath = openFileDialog.FileName;
                    //存一下保存路径
                    filePathToSave = filePath;
                    //显示导入文件名
                    FileName = Path.GetFileName(filePath);

                    // 1. 读取文件内容
                    using var fileStream = new FileStream(filePath, FileMode.Open);
                    var wrapperSerializer = new XmlSerializer(typeof(ConfigWrapper));
                    var wrapper = (ConfigWrapper)wrapperSerializer.Deserialize(fileStream);

                    // 2. 计算读取数据的哈希（直接对字节数组计算）
                    using var sha256 = SHA256.Create();
                    string computedHashString = System.Convert.ToBase64String(sha256.ComputeHash(wrapper.DataBytes));

                    // 3. 验证哈希
                    if (computedHashString != wrapper.Hash)
                    {
                        MessageBox.Show("哈希校验不通过，配置文件已损坏或被篡改！", "错误",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return new ObservableCollection<SendingCommand>();
                    }

                    // 4. 反序列化原始数据
                    using var dataStream = new MemoryStream(wrapper.DataBytes);
                    var serializer = new XmlSerializer(typeof(ObservableCollection<SendingCommand>));

                    // 5. 修改一下保存路径和显示
                    //filePathToSave = wrapper.machineName + ".xml";
                    SelectedMachineItem = wrapper.machineName;

                    // 6. 直接保存
                    //SaveSettings(this);

                    return (ObservableCollection<SendingCommand>)serializer.Deserialize(dataStream);
                }
                //取消选择则显示原来的配置文件
                return SendingCommands;
            }
            catch (Exception ex)
            {
                MessageBox.Show("文件加载失败，配置文件已损坏或被篡改！", "错误",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"加载设置时出错: {ex.Message}");
                return new ObservableCollection<SendingCommand>();
            }

        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="parameter"></param>
        private void SaveSettings(object parameter)
        {
            try
            {
                string path = filePathToSave ?? "default.xml";
                //var serializer = new XmlSerializer(typeof(ObservableCollection<SendingCommand>));
                //using (var writer = new StreamWriter(path))
                //{
                //    serializer.Serialize(writer, SendingCommands);
                //}

                // 1. 序列化原始数据到内存流
                var serializer = new XmlSerializer(typeof(ObservableCollection<SendingCommand>));
                byte[] dataBytes;

                using (var ms = new MemoryStream())
                {
                    serializer.Serialize(ms, SendingCommands);
                    dataBytes = ms.ToArray(); // 直接获取字节数组
                }

                // 2. 计算哈希值（直接对字节数组计算）
                using var sha256 = SHA256.Create();
                string hashString = System.Convert.ToBase64String(sha256.ComputeHash(dataBytes));

                // 3. 创建包含哈希的包装对象
                var wrapper = new ConfigWrapper
                {
                    machineName = SelectedMachineItem,
                    Hash = hashString,
                    DataBytes = dataBytes
                };

                // 4. 保存到文件
                using var fileStream = new FileStream(path, FileMode.Create);
                var wrapperSerializer = new XmlSerializer(typeof(ConfigWrapper));
                wrapperSerializer.Serialize(fileStream, wrapper);
                MessageBox.Show($"已保存");
                Console.WriteLine("设置已保存");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存设置时出错: {ex.Message}");
                MessageBox.Show($"保存设置时出错:{ex.Message}");
            }
        }

        /// <summary>
        /// 导出配置文件
        /// </summary>
        /// <param name="parameter"></param>
        private void SaveSettingsToAtherFile(object parameter)
        {
            try
            {
                // 创建文件保存对话框
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "XML文件 (*.xml)|*.xml",
                    Title = "保存配置文件",
                    DefaultExt = "xml",
                    AddExtension = true
                };

                // 显示对话框并等待用户选择
                var dialogResult = saveFileDialog.ShowDialog();
                if (dialogResult == true)
                {
                    // 获取用户选择的文件路径
                    string filePath = saveFileDialog.FileName;

                    //var serializer = new XmlSerializer(typeof(ObservableCollection<SendingCommand>));
                    //using (var writer = new StreamWriter(filePath))
                    //{
                    //    serializer.Serialize(writer, SendingCommands);
                    //}
                    //MessageBox.Show($"保存设置成功");
                    //Console.WriteLine("设置已保存");

                    // 1. 序列化原始数据到内存流
                    var serializer = new XmlSerializer(typeof(ObservableCollection<SendingCommand>));
                    byte[] dataBytes;

                    using (var ms = new MemoryStream())
                    {
                        serializer.Serialize(ms, SendingCommands);
                        dataBytes = ms.ToArray(); // 直接获取字节数组
                    }

                    // 2. 计算哈希值（直接对字节数组计算）
                    using var sha256 = SHA256.Create();
                    string hashString = System.Convert.ToBase64String(sha256.ComputeHash(dataBytes));

                    // 3. 创建包含哈希的包装对象
                    var wrapper = new ConfigWrapper
                    {
                        machineName = SelectedMachineItem,
                        Hash = hashString,
                        DataBytes = dataBytes
                    };

                    // 4. 保存到文件
                    using var fileStream = new FileStream(filePath, FileMode.Create);
                    var wrapperSerializer = new XmlSerializer(typeof(ConfigWrapper));
                    wrapperSerializer.Serialize(fileStream, wrapper);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存配置时出错: {ex.Message}", "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region 写入

        // 按钮文本属性
        private string _buttonText = "写入";
        public string ButtonText
        {
            get => _buttonText;
            set
            {
                _buttonText = value;
                OnPropertyChanged(nameof(ButtonText));
            }
        }

        // 按钮可用状态
        private bool _isButtonEnabled = true;
        public bool IsButtonEnabled
        {
            get => _isButtonEnabled;
            set
            {
                _isButtonEnabled = value;
                OnPropertyChanged(nameof(IsButtonEnabled));
            }

        }

        public Action<string, int> ShowBoubleWithTime;
        //命令
        public RelayCommand WriteCommand { get; }
        public RelayCommand WriteCommandByBMS01 { get; }

        //异步执行写入操作
        private async void ExecuteWriteAsync()
        {
            try
            {
                // 更新按钮状态为"写入中"且不可点击
                ButtonText = "ing...";
                IsButtonEnabled = false;
                //Debug.WriteLine($"ButtonText 设置为: {ButtonText}");

                // 通知命令状态已更改
                WriteCommand.RaiseCanExecuteChanged();

                //组装报文
                byte[] sendBuffer = ModbusRTU.GetSendBytes(SelectedMachineItem, SendingCommands);
                byte[] receive = new byte[] { 0 };
                // 模拟耗时操作（实际应用中替换为真实的写入逻辑）
                await Task.Run(() =>
                {
                    // 模拟耗时操作，例如写入文件或网络请求
                    System.Threading.Thread.Sleep(1000);
                    receive = SerialCommunicationService.SendCommandToBMS(sendBuffer, 8);
                    if (receive.Length != 8)
                    {
                        Thread.Sleep(500);
                        receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead20Frame(1, 8, 1), 8);
                        if (receive.Length == 8)
                        {
                            OutIndex = receive[5];
                            MessageBox.Show($"写入失败，超出数据范围，索引：{OutIndex}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                });

                ButtonText = receive.Length == 8 ? "成功" : "失败";
                //ShowBoubleWithTime($"{ButtonText}", 1500);

                await Task.Delay(500); // 短暂显示"完成"状态
                ButtonText = "写入";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
                ButtonText = "写入";
            }
            finally
            {
                // 恢复按钮可点击状态
                IsButtonEnabled = true;
                WriteCommand.RaiseCanExecuteChanged();
                //关闭串口
                //SerialCommunicationService.CloseCom();
            }
        }

        //异步执行写入操作
        private async void ExecuteWriteAsyncByBMS01()
        {
            try
            {
                // 更新按钮状态为"写入中"且不可点击
                ButtonText = "ing...";
                IsButtonEnabled = false;
                //Debug.WriteLine($"ButtonText 设置为: {ButtonText}");

                // 通知命令状态已更改
                WriteCommandByBMS01.RaiseCanExecuteChanged();

                //组装报文
                byte[] sendBuffer = ModbusRTU.GetSendBytesByBMS01(SelectedMachineItem, SendingCommands);
                byte[] receive = new byte[] { 0 };
                // 模拟耗时操作（实际应用中替换为真实的写入逻辑）
                await Task.Run(() =>
                {
                    // 模拟耗时操作，例如写入文件或网络请求
                    System.Threading.Thread.Sleep(1000);
                    receive = SerialCommunicationService.SendCommandToBMS(sendBuffer, 8);
                    if (receive.Length != 8)
                    {

                        receive = SerialCommunicationService.SendCommandToBMS(ModbusRTU.BuildRead20Frame(1, 8, 1), 8);
                        if (receive.Length == 8)
                        {
                            OutIndex = receive[5];
                            MessageBox.Show($"写入失败，超出数据范围，索引：{OutIndex}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                });

                ButtonText = receive.Length == 8 ? "成功" : "失败";
                //ShowBoubleWithTime($"{ButtonText}", 1500);

                await Task.Delay(500); // 短暂显示"完成"状态
                ButtonText = "写入";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
                ButtonText = "写入";
            }
            finally
            {
                // 恢复按钮可点击状态
                IsButtonEnabled = true;
                WriteCommandByBMS01.RaiseCanExecuteChanged();
                //关闭串口
                //SerialCommunicationService.CloseCom();
            }
        }

        private bool AnalyseReceuve(string receive)
        {
            if (receive.Length >= 7)
            {
                if (receive.Substring(0, 4) == "(NAK")
                {
                    return false;
                }
                else if (receive.Substring(0, 4) == "(ACK")
                {
                    // 操作完成后更新按钮状态
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region 配置

        public SerialPortSettingViewModel SerialPort1 { get; set; }

        //密码
        private string password;

        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                this.OnPropertyChanged(nameof(Password));
            }
        }


        // 双击命令
        public ICommand DoubleClickCommand { get; }

        // 处理双击事件
        //private void OnDoubleClick()
        //{
        //    // 显示密码输入对话框

        //    var dialogResult = ShowDialog(this);

        //    if (dialogResult == true)
        //    {
        //        if (Password == "Tqf147258")
        //        {
        //            // 密码验证通过，打开新窗口
        //            OpenNewWindow(this);
        //        }
        //        else
        //        {
        //            MessageBox.Show("密码错误！", "验证失败",
        //           MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //    }
        //    else
        //    {

        //    }
        //}

        //// 显示对话框（可通过依赖注入替换为实际实现）
        //private bool? ShowDialog(SendingCommandSettingsViewModel viewModel)
        //{
        //    var dialog = new PasswordDialogWindow { DataContext = viewModel };
        //    return dialog.ShowDialog();
        //}

        //// 打开新窗口
        //private void OpenNewWindow(SendingCommandSettingsViewModel viewModel)
        //{
        //    var newWindow = new PropertyWindow { DataContext = viewModel };
        //    newWindow.Show();
        //}

        #endregion

        #region 下拉选择机型

        private string? _selectedItem = "UPSLB600";
        public string? SelectedMachineItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> MachineItems { get; } = new(){
        "UPSLB600",
        "VQ3024",
        "VDF"
         };

        ////切换指令
        //public ICommand SelectionChangedCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(OnSelectionChanged);
        //    }
        //}

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
        ///根据机型导入配置文件
        /// </summary>
        public void SwitchViewToVQorGB(string view)
        {

            if (view == "VQ3024")
            {
                filePathToSave = "VQ3024.xml";
                SendingCommands = LoadSettings();
                SelectedMachineItem = "VQ3024";
            }
            else if (view == "UPSLB600")
            {
                filePathToSave = "UPSLB600.xml";
                //重新加载配置文件
                SendingCommands = LoadSettings();
                //下拉框显示
                SelectedMachineItem = "UPSLB600";
            }
            else
            {
                filePathToSave = "VDF.xml";
                SendingCommands = LoadSettings();
                SelectedMachineItem = "VDF";
            }
            FileName = filePathToSave;
        }
        #endregion

    }
}
