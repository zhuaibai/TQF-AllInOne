using DocumentFormat.OpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Convert;
using WpfApp1.Services;
using WpfApp1.ViewModels;

namespace WpfApp1.Command.Command_CG
{
    public class HEEP1_CG_ViewModel:BaseViewModel
    {
        private string command  = "HEEP1\r";
        public string Command { get { return command; } }

        public string MachineType;

        ManualResetEventSlim _pauseEvent;//线程的开启、暂停
        SemaphoreSlim _semaphore;        //异步竞争，资源锁
        Action<string> AddLog;           //添加日志委托
        Action<string> UpdateState;      //更新状态日志

        public HEEP1_CG_ViewModel(ManualResetEventSlim pauseEvent, SemaphoreSlim semaphore, Action<string> addLog, Action<string> _updateState)
        {
            _pauseEvent = pauseEvent;
            _semaphore = semaphore;
            AddLog = addLog;
            UpdateState = _updateState;

            #region 初始化命令

            //设置低电锁机电压
            Command_SetLowPowerLock = new RelayCommand(
                execute: () => LowPowerLockOperation(),
                canExecute: () => Validate(nameof(LowPowerLock_Inputs)) && !_LowPowerLockIsWorking
            );

            //旁路低退电压
            Command_SetBypasslowDropout = new RelayCommand(
                execute: () => BypasslowDropoutOperation(),
                canExecute: () => Validate(nameof(BypasslowDropout_Inputs)) && !_BypasslowDropoutIsWorking
            );

            //旁路高退电压
            Command_SetBypasshighDropout = new RelayCommand(
                execute: () => BypasshighDropoutOperation(),
                canExecute: () => Validate(nameof(BypasshighDropout_Inputs)) && !_BypasshighDropoutIsWorking
            );



            //强充电压
            Command_SetStrongChargeVoltage = new RelayCommand(
                execute: () => StrongChargeVoltageOperation(),
                canExecute: () => Validate(nameof(StrongChargeVoltage_Inputs)) && !StrongChargeVoltage_IsWorking
            );


            //浮充电压
            Command_SetFloatChargeVolage = new RelayCommand(
                execute: () => FloatChargeVolageOperation(),
                canExecute: () => Validate(nameof(FloatChargeVolage_Inputs)) && !FloatChargeVolage_IsWorking
            );

            //输出设定电压
            Command_SetOutSetVolt = new RelayCommand(
              execute: () => OutSetVoltOperation(),
              canExecute: () => Validate(nameof(OutSetVolt_Inputs)) && !OutSetVolt_IsWorking
             );

            //蜂鸣器状态
            Command_SetBuzzerStatus = new RelayCommand(
                execute: () => BuzzerStatusOperation(),
                canExecute: () => Validate(nameof(BuzzerStatus_Inputs)) && !BuzzerStatus_IsWorking
            );

            //LCD背光
            Command_SetLCD_Backlight = new RelayCommand(
                execute: () => LCD_BacklightOperation(),
                canExecute: () => Validate(nameof(LCD_Backlight_Inputs)) && !LCD_Backlight_IsWorking
            );

            //系统频率
            Command_SetOutputSettingFrequency = new RelayCommand(
               execute: () => OutputSettingFrequencyOperation(),
               canExecute: () => Validate(nameof(OutputSettingFrequency_Inputs)) && !OutputSettingFrequency_IsWorking
            );

            //故障记录
            Command_SetFaultLog = new RelayCommand(
              execute: () => FaultLogOperation(),
              canExecute: () => Validate(nameof(FaultLog_Inputs)) && !FaultLog_IsWorking
             );

            //自动开机
            Command_SetAutoStartEnable = new RelayCommand(
              execute: () => AutoStartEnableOperation(),
              canExecute: () => Validate(nameof(AutoStartEnable_Inputs)) && !AutoStartEnable_IsWorking
             );

            //低电告警电压
            Command_SetBattLowAlarmVolt = new RelayCommand(
              execute: () => BattLowAlarmVoltOperation(),
              canExecute: () => Validate(nameof(BattLowAlarmVolt_Inputs)) && !BattLowAlarmVolt_IsWorking
             );

            //ECO模式
            Command_ECOMode = new RelayCommand(
              execute: () => ECOModeOperation(),
              canExecute: () => Validate(nameof(ECOMode_Inputs)) && !ECOMode_IsWorking
             );
            //市电自动重启
            Command_AutoRestartAC = new RelayCommand(
              execute: () => AutoRestartACOperation(),
              canExecute: () => Validate(nameof(AutoRestartAC_Inputs)) && !AutoRestartAC_IsWorking
             );
            //旁路使能
            Command_PassFunctionEnable = new RelayCommand(
              execute: () => PassFunctionEnableOperation(),
              canExecute: () => Validate(nameof(PassFunctionEnable_Inputs)) && !PassFunctionEnable_IsWorking
             );
            //频率限制模式
            Command_Frequencyrestrictionmode = new RelayCommand(
              execute: () => FrequencyrestrictionmodeOperation(),
              canExecute: () => Validate(nameof(Frequencyrestrictionmode_Inputs)) && !Frequencyrestrictionmode_IsWorking
             );
            //功率因数
            Command_Outputpowerfactor = new RelayCommand(
              execute: () => OutputpowerfactorOperation(),
              canExecute: () => Validate(nameof(Outputpowerfactor_Inputs)) && !Outputpowerfactor_IsWorking
             );
            //电池带载限制时间
            Command_BatteryloadlimitTime = new RelayCommand(
              execute: () => BatteryloadlimitTimeOperation(),
              canExecute: () => Validate(nameof(BatteryloadlimitTime_Inputs)) && !BatteryloadlimitTime_IsWorking
             );
            //电池放电限制时间
            Command_BatterydischargelimitTime = new RelayCommand(
              execute: () => BatterydischargelimitTimeOperation(),
              canExecute: () => Validate(nameof(BatterydischargelimitTime_Inputs)) && !BatterydischargelimitTime_IsWorking
             );
            #endregion
        }

        #region 蜂鸣器状态

        private string _BuzzerStatus;

        public string BuzzerStatus
        {
            get { return _BuzzerStatus; }
            set
            {
                if (value == "1")
                {

                    _BuzzerStatus = App.GetText("开启");
                }
                else if (value == "0")
                {
                    _BuzzerStatus = App.GetText("关闭");
                }
                else
                {
                    _BuzzerStatus = "";
                }
                RaiseProperChanged(nameof(BuzzerStatus));
            }
        }


        private bool BuzzerStatus_IsWorking;


        //设置值
        private string _BuzzerStatus_Inputs;

        public string BuzzerStatus_Inputs
        {
            get { return _BuzzerStatus_Inputs; }
            set
            {
                _BuzzerStatus_Inputs = value;
                RaiseProperChanged(nameof(BuzzerStatus_Inputs));
                Command_SetBuzzerStatus.RaiseCanExecuteChanged();
            }
        }

        //下拉选项
        private List<string> _BuzzerStatusOptions = new List<string> { "开启/On", "关闭/Off" };

        public List<string> BuzzerStatusOptions
        {
            get { return _BuzzerStatusOptions; }
            set
            {
                _BuzzerStatusOptions = value;
                RaiseProperChanged(nameof(BuzzerStatusOptions));
            }
        }


        public RelayCommand Command_SetBuzzerStatus { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void BuzzerStatusOperation()
        {
            try
            {
                BuzzerStatus_IsWorking = true;
                // 禁用按钮
                Command_SetBuzzerStatus.RaiseCanExecuteChanged();

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
                    Thread.Sleep(1000);
                    string receive = SerialCommunicationService.SendSettingCommand("", getSelectedToCommad(nameof(BuzzerStatus_Inputs)));

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
                BuzzerStatus_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetBuzzerStatus.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }


        #endregion

        #region LCD背光开启

        private string _LCD_Backlight;

        public string LCD_Backlight
        {
            get { return _LCD_Backlight; }
            set
            {
                if (value == "0")
                {
                    _LCD_Backlight = App.GetText("关闭");
                }
                else if (value == "1")
                {
                    _LCD_Backlight = App.GetText("开启");
                }
                else
                    _LCD_Backlight = value;
                RaiseProperChanged(nameof(LCD_Backlight));
            }
        }


        private bool LCD_Backlight_IsWorking;


        //设置值
        private string _LCD_Backlight_Inputs;

        public string LCD_Backlight_Inputs
        {
            get { return _LCD_Backlight_Inputs; }
            set
            {
                _LCD_Backlight_Inputs = value;
                RaiseProperChanged(nameof(LCD_Backlight_Inputs));
                Command_SetLCD_Backlight.RaiseCanExecuteChanged();
            }
        }

        //下拉选项
        private List<string> _LCD_BacklightOptions = new List<string> { "开启/On", "关闭/Off" };

        public List<string> LCD_BacklightOptions
        {
            get { return _LCD_BacklightOptions; }
            set
            {
                _LCD_BacklightOptions = value;
                RaiseProperChanged(nameof(LCD_BacklightOptions));
            }
        }

        public RelayCommand Command_SetLCD_Backlight { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void LCD_BacklightOperation()
        {
            try
            {
                LCD_Backlight_IsWorking = true;
                // 禁用按钮
                Command_SetLCD_Backlight.RaiseCanExecuteChanged();

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
                    string receive = SerialCommunicationService.SendSettingCommand("", getSelectedToCommad(nameof(LCD_Backlight_Inputs)));

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
                LCD_Backlight_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetLCD_Backlight.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }



        #endregion

        #region 故障记录

        private string _FaultLog;

        public string FaultLog
        {
            get { return _FaultLog; }
            set
            {
                if (value == "0")
                {
                    _FaultLog = App.GetText("关闭");
                }
                else if (value == "1")
                {
                    _FaultLog = App.GetText("开启");
                }
                else
                    _FaultLog = value;
                this.RaiseProperChanged(nameof(FaultLog));
            }
        }


        private bool FaultLog_IsWorking;


        //设置值
        private string _FaultLog_Inputs;

        public string FaultLog_Inputs
        {
            get { return _FaultLog_Inputs; }
            set
            {
                _FaultLog_Inputs = value;
                this.RaiseProperChanged(nameof(FaultLog_Inputs));
                Command_SetFaultLog.RaiseCanExecuteChanged();
            }
        }

        //下拉选项
        private List<string> _FaultLogOptions = new List<string> { "开启/On", "关闭/Off" };

        public List<string> FaultLogOptions
        {
            get { return _FaultLogOptions; }
            set
            {
                _FaultLogOptions = value;
                this.RaiseProperChanged(nameof(FaultLogOptions));
            }
        }

        public RelayCommand Command_SetFaultLog { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void FaultLogOperation()
        {
            try
            {
                FaultLog_IsWorking = true;
                // 禁用按钮
                Command_SetFaultLog.RaiseCanExecuteChanged();

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
                    string receive = SerialCommunicationService.SendSettingCommand("", getSelectedToCommad(nameof(FaultLog_Inputs)));

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
                FaultLog_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetFaultLog.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }


        #endregion

        #region 自动开机使能
        //自动开机使能
        private string _AutoStartEnable;

        public string AutoStartEnable
        {
            get { return _AutoStartEnable; }
            set
            {
                if (value == "0")
                {
                    _AutoStartEnable = App.GetText("关闭");
                }
                else if (value == "1")
                {
                    _AutoStartEnable = App.GetText("开启");
                }
                else
                    _AutoStartEnable = value;
                this.RaiseProperChanged(nameof(AutoStartEnable));
            }
        }


        private bool AutoStartEnable_IsWorking;


        //设置值
        private string _AutoStartEnable_Inputs;

        public string AutoStartEnable_Inputs
        {
            get { return _AutoStartEnable_Inputs; }
            set
            {
                _AutoStartEnable_Inputs = value;
                this.RaiseProperChanged(nameof(AutoStartEnable_Inputs));
                Command_SetAutoStartEnable.RaiseCanExecuteChanged();
            }
        }

        //下拉选项
        private List<string> _AutoStartEnableOptions = new List<string> { "开启/On" , "关闭/Off"};

        public List<string> AutoStartEnableOptions
        {
            get { return _AutoStartEnableOptions; }
            set
            {
                _AutoStartEnableOptions = value;
                this.RaiseProperChanged(nameof(AutoStartEnableOptions));
            }
        }

        public RelayCommand Command_SetAutoStartEnable { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void AutoStartEnableOperation()
        {
            try
            {
                AutoStartEnable_IsWorking = true;
                // 禁用按钮
                Command_SetAutoStartEnable.RaiseCanExecuteChanged();

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
                    string receive = SerialCommunicationService.SendCommand(getSelectedToCommad(nameof(AutoStartEnable_Inputs)), 1);

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
                AutoStartEnable_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetAutoStartEnable.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }



        #endregion

        #region 市电自动重启
        //市电自动重启
        private string _AutoRestartAC;

        public string AutoRestartAC
        {
            get { return _AutoRestartAC; }
            set
            {
                if (value == "0")
                {
                    _AutoRestartAC = App.GetText("关闭");
                }
                else if (value == "1")
                {
                    _AutoRestartAC = App.GetText("开启");
                }
                else
                    _AutoRestartAC = value;
                this.RaiseProperChanged(nameof(AutoRestartAC));
            }
        }


        private bool AutoRestartAC_IsWorking;


        //设置值
        private string _AutoRestartAC_Inputs;

        public string AutoRestartAC_Inputs
        {
            get { return _AutoStartEnable_Inputs; }
            set
            {
                _AutoRestartAC_Inputs = value;
                this.RaiseProperChanged(nameof(AutoRestartAC_Inputs));
                Command_AutoRestartAC.RaiseCanExecuteChanged();
            }
        }

        //下拉选项
        private List<string> _AutoRestartACOptions = new List<string> { "开启/On", "关闭/Off"};

        public List<string> AutoRestartACOptions
        {
            get { return _AutoRestartACOptions; }
            set
            {
                _AutoRestartACOptions = value;
                this.RaiseProperChanged(nameof(AutoRestartACOptions));
            }
        }

        public RelayCommand Command_AutoRestartAC { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void AutoRestartACOperation()
        {
            try
            {
                AutoRestartAC_IsWorking = true;
                // 禁用按钮
                Command_AutoRestartAC.RaiseCanExecuteChanged();

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
                    string receive = SerialCommunicationService.SendSettingCommand("", getSelectedToCommad(nameof(AutoRestartAC_Inputs)));

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
                AutoRestartAC_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_AutoRestartAC.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }



        #endregion

        #region 系统频率

        private string _OutputSettingFrequency2;

        public string OutputSettingFrequency2
        {
            get { return _OutputSettingFrequency2; }
            set
            {
                _OutputSettingFrequency2 = Tools.RemoveLeadingZeros(value) + "Hz";
                this.RaiseProperChanged(nameof(OutputSettingFrequency2));
            }
        }


        //系统频率
        private string _OutputSettingFrequency;

        public string OutputSettingFrequency
        {
            get { return _OutputSettingFrequency; }
            set
            {
                if (value == "0")
                {
                    _OutputSettingFrequency = "50";
                    OutputSettingFrequency2 = _OutputSettingFrequency;
                }
                else if (value == "1")
                {
                    _OutputSettingFrequency = "60";
                    OutputSettingFrequency2 = _OutputSettingFrequency;
                }
                else
                {
                    _OutputSettingFrequency = value;
                    OutputSettingFrequency2 = _OutputSettingFrequency;
                }

                RaiseProperChanged(nameof(OutputSettingFrequency));
            }
        }


        private bool OutputSettingFrequency_IsWorking;


        //设置值
        private string _OutputSettingFrequency_Inputs;

        public string OutputSettingFrequency_Inputs
        {
            get { return _OutputSettingFrequency_Inputs; }
            set
            {
                _OutputSettingFrequency_Inputs = value;
                RaiseProperChanged(nameof(OutputSettingFrequency_Inputs));
                Command_SetOutputSettingFrequency.RaiseCanExecuteChanged();
            }
        }

        //下拉选项
        private List<string> _OutputSettingFrequencyOptions = new List<string> { "50", "60" };

        public List<string> OutputSettingFrequencyOptions
        {
            get { return _OutputSettingFrequencyOptions; }
            set
            {
                _OutputSettingFrequencyOptions = value;
                RaiseProperChanged(nameof(OutputSettingFrequencyOptions));
            }
        }

        public RelayCommand Command_SetOutputSettingFrequency { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void OutputSettingFrequencyOperation()
        {
            try
            {
                OutputSettingFrequency_IsWorking = true;
                // 禁用按钮
                Command_SetOutputSettingFrequency.RaiseCanExecuteChanged();

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
                    string receive = SerialCommunicationService.SendSettingCommand("F", OutputSettingFrequency_Inputs);

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
                OutputSettingFrequency_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetOutputSettingFrequency.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }


        #endregion

        #region ECO模式
        //ECO模式
        private string _ECOMode;

        public string ECOMode
        {
            get { return _ECOMode; }
            set
            {
                if (value == "0")
                {
                    _ECOMode = App.GetText("关闭");
                }
                else if (value == "1")
                {
                    _ECOMode = App.GetText("开启");
                }
                else
                    _ECOMode = value;
                this.RaiseProperChanged(nameof(ECOMode));
            }
        }


        private bool ECOMode_IsWorking;


        //设置值
        private string _ECOMode_Inputs;

        public string ECOMode_Inputs
        {
            get { return _ECOMode_Inputs; }
            set
            {
                _ECOMode_Inputs = value;
                this.RaiseProperChanged(nameof(ECOMode_Inputs));
                Command_ECOMode.RaiseCanExecuteChanged();
            }
        }

        //下拉选项
        private List<string> _ECOModeOptions = new List<string> { "开启/On" , "关闭/Off"};

        public List<string> ECOModeOptions
        {
            get { return _ECOModeOptions; }
            set
            {
                _ECOModeOptions = value;
                this.RaiseProperChanged(nameof(ECOModeOptions));
            }
        }

        public RelayCommand Command_ECOMode { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void ECOModeOperation()
        {
            try
            {
                ECOMode_IsWorking = true;
                // 禁用按钮
                Command_ECOMode.RaiseCanExecuteChanged();

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
                    string receive = SerialCommunicationService.SendSettingCommand("", getSelectedToCommad(nameof(ECOMode_Inputs)));

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
                ECOMode_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_ECOMode.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }



        #endregion

        #region 旁路使能

        //旁路使能
        private string _PassFunctionEnable;

        public string PassFunctionEnable
        {
            get { return _PassFunctionEnable; }
            set
            {
                if (value == "0")
                {
                    _PassFunctionEnable = App.GetText("关闭");
                }
                else if (value == "1")
                {
                    _PassFunctionEnable = App.GetText("开启");
                }
                else
                    _PassFunctionEnable = value;
                RaiseProperChanged(nameof(PassFunctionEnable));
            }
        }


        private bool PassFunctionEnable_IsWorking;


        //设置值
        private string _PassFunctionEnable_Inputs;

        public string PassFunctionEnable_Inputs
        {
            get { return _PassFunctionEnable_Inputs; }
            set
            {
                _PassFunctionEnable_Inputs = value;
                RaiseProperChanged(nameof(PassFunctionEnable_Inputs));
                Command_PassFunctionEnable.RaiseCanExecuteChanged();
            }
        }

        //下拉选项
        private List<string> _PassFunctionEnableOptions = new List<string> { "开启/On", "关闭/Off" };

        public List<string> PassFunctionEnableOptions
        {
            get { return _PassFunctionEnableOptions; }
            set
            {
                _PassFunctionEnableOptions = value;
                RaiseProperChanged(nameof(PassFunctionEnableOptions));
            }
        }

        public RelayCommand Command_PassFunctionEnable { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void PassFunctionEnableOperation()
        {
            try
            {
                PassFunctionEnable_IsWorking = true;
                // 禁用按钮
                Command_PassFunctionEnable.RaiseCanExecuteChanged();

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
                    string receive = SerialCommunicationService.SendSettingCommand("", getSelectedToCommad(nameof(PassFunctionEnable_Inputs)));

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
                PassFunctionEnable_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_PassFunctionEnable.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        #endregion

        #region 频率限制模式

        //频率限制模式
        private string _Frequencyrestrictionmode;

        public string Frequencyrestrictionmode
        {
            get { return _Frequencyrestrictionmode; }
            set
            {
                if (value == "0")
                {
                    _Frequencyrestrictionmode = App.GetText("关闭");
                }
                else if (value == "1")
                {
                    _Frequencyrestrictionmode = App.GetText("开启");
                }
                else
                    _Frequencyrestrictionmode = value;
                RaiseProperChanged(nameof(Frequencyrestrictionmode));
            }
        }


        private bool Frequencyrestrictionmode_IsWorking;


        //设置值
        private string _Frequencyrestrictionmode_Inputs;

        public string Frequencyrestrictionmode_Inputs
        {
            get { return _Frequencyrestrictionmode_Inputs; }
            set
            {
                _Frequencyrestrictionmode_Inputs = value;
                RaiseProperChanged(nameof(Frequencyrestrictionmode_Inputs));
                Command_Frequencyrestrictionmode.RaiseCanExecuteChanged();
            }
        }

        //下拉选项
        private List<string> _FrequencyrestrictionmodeOptions = new List<string> { "开启/On", "关闭/Off" };

        public List<string> FrequencyrestrictionmodeOptions
        {
            get { return _FrequencyrestrictionmodeOptions; }
            set
            {
                _FrequencyrestrictionmodeOptions = value;
                RaiseProperChanged(nameof(FrequencyrestrictionmodeOptions));
            }
        }

        public RelayCommand Command_Frequencyrestrictionmode { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void FrequencyrestrictionmodeOperation()
        {
            try
            {
                Frequencyrestrictionmode_IsWorking = true;
                // 禁用按钮
                Command_Frequencyrestrictionmode.RaiseCanExecuteChanged();

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
                    string receive = SerialCommunicationService.SendSettingCommand("", getSelectedToCommad(nameof(Frequencyrestrictionmode_Inputs)));

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
                Frequencyrestrictionmode_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_Frequencyrestrictionmode.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        #endregion

        #region 输出设定电压


        //输出设定电压
        private string _OutSetVolt;

        public string OutSetVolt
        {
            get { return _OutSetVolt; }
            set
            {
                _OutSetVolt = value + "V";
                this.RaiseProperChanged(nameof(OutSetVolt));
            }
        }


        private bool OutSetVolt_IsWorking;


        //设置值
        private string _OutSetVolt_Inputs;

        public string OutSetVolt_Inputs
        {
            get { return _OutSetVolt_Inputs; }
            set
            {
                _OutSetVolt_Inputs = value;
                this.RaiseProperChanged(nameof(OutSetVolt_Inputs));
                Command_SetOutSetVolt.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand Command_SetOutSetVolt { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void OutSetVoltOperation()
        {
            try
            {
                OutSetVolt_IsWorking = true;
                // 禁用按钮
                Command_SetOutSetVolt.RaiseCanExecuteChanged();

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
                    string receive = SerialCommunicationService.SendSettingCommand("V", OutSetVolt_Inputs);

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
                OutSetVolt_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetOutSetVolt.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }



        #endregion

        #region 电池类型

        //电池类型
        private string _BatteryType;

        public string BatteryType
        {
            get { return _BatteryType; }
            set
            {
                if (value == "0") { _BatteryType = "AGM"; }
                else if (value == "1") { _BatteryType = "FLD"; }
                else if (value == "2") { _BatteryType = "USER"; }
                else if (value == "3") { _BatteryType = "LIA"; }
                else if (value == "4") { _BatteryType = "PYL"; }
                else if (value == "5") { _BatteryType = "TQF"; }
                else if (value == "6") { _BatteryType = "GRO"; }
                else if (value == "7") { _BatteryType = "LIB"; }
                else if (value == "8") { _BatteryType = "LIC"; }


                else
                    _BatteryType = value;
                RaiseProperChanged(nameof(BatteryType));
            }
        }
        private bool _BatteryIsWorking;


        //电池类型可选项
        private List<string> _BatteryTypeOptions = new List<string> { "AGM", "FLD", "USE", "LIA", "PYL", "TQF", "GRO", "LIB", "LIC" };

        public List<string> BatteryTypeOptions
        {
            get { return _BatteryTypeOptions; }
            set
            {
                _BatteryTypeOptions = value;
                RaiseProperChanged(nameof(BatteryTypeOptions));
            }
        }

        //电池类型已选项
        private string? _BatteryTypeSelectedOption;

        public string? BatteryTypeSelectedOption
        {
            get { return _BatteryTypeSelectedOption; }
            set
            {
                _BatteryTypeSelectedOption = value;
                RaiseProperChanged(nameof(BatteryTypeSelectedOption));
                Command_SetBatteryType.RaiseCanExecuteChanged();
            }
        }
        //设置命令
        public RelayCommand Command_SetBatteryType { get; set; }

        /// <summary>
        /// 设置电池类型
        /// </summary>
        private async void BatteryTypeOperation()
        {
            try
            {
                _BatteryIsWorking = true;
                // 禁用按钮
                Command_SetBatteryType.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState($"正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    Thread.Sleep(1000);
                    string receive = SerialCommunicationService.SendSettingCommand("PBT", getSelectedToCommad(nameof(BatteryTypeSelectedOption)));

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
                _BatteryIsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetBatteryType.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        #endregion

        #region 强充电压

        //强充电压
        private string _StrongChargeVoltage;

        public string StrongChargeVoltage
        {
            get { return _StrongChargeVoltage; }
            set
            {
                _StrongChargeVoltage = Tools.RemoveLeadingZeros(value) + "V";
                RaiseProperChanged(nameof(StrongChargeVoltage));
            }
        }


        private bool StrongChargeVoltage_IsWorking;


        //设置值
        private string _StrongChargeVoltage_Inputs;

        public string StrongChargeVoltage_Inputs
        {
            get { return _StrongChargeVoltage_Inputs; }
            set
            {
                _StrongChargeVoltage_Inputs = value;
                RaiseProperChanged(nameof(StrongChargeVoltage_Inputs));
                Command_SetStrongChargeVoltage.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand Command_SetStrongChargeVoltage { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void StrongChargeVoltageOperation()
        {
            try
            {
                StrongChargeVoltage_IsWorking = true;
                // 禁用按钮
                Command_SetStrongChargeVoltage.RaiseCanExecuteChanged();

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
                    Thread.Sleep(1000);
                    string receive = SerialCommunicationService.SendSettingCommand("PCVV", Tools.FormatToXxx(StrongChargeVoltage_Inputs));
                    if (receive.StartsWith("(ACK"))
                    {
                        AddLog("设置强充电压成功！");
                    }
                    else
                    {
                        AddLog("设置强充电压失败！");
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
                StrongChargeVoltage_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetStrongChargeVoltage.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }


        #endregion

        #region 浮充电压

        //浮充电压
        private string _FloatChargeVoltage;

        public string FloatChargeVolage
        {
            get { return _FloatChargeVoltage; }
            set
            {
                _FloatChargeVoltage = Tools.RemoveLeadingZeros(value) + "V";
                RaiseProperChanged(nameof(FloatChargeVolage));
            }
        }


        private bool FloatChargeVolage_IsWorking;


        //设置值
        private string _FloatChargeVolage_Inputs;

        public string FloatChargeVolage_Inputs
        {
            get { return _FloatChargeVolage_Inputs; }
            set
            {
                _FloatChargeVolage_Inputs = value;
                RaiseProperChanged(nameof(FloatChargeVolage_Inputs));
                Command_SetFloatChargeVolage.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand Command_SetFloatChargeVolage { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void FloatChargeVolageOperation()
        {
            try
            {
                FloatChargeVolage_IsWorking = true;
                // 禁用按钮
                Command_SetFloatChargeVolage.RaiseCanExecuteChanged();

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
                    Thread.Sleep(1000);
                    string receive = SerialCommunicationService.SendSettingCommand("PBFT", Tools.FormatToXxx(FloatChargeVolage_Inputs));
                    if (receive.StartsWith("(ACK"))
                    {
                        AddLog("设置浮充电压成功！");
                    }
                    else
                    {
                        AddLog("设置浮充电压失败！");
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
                FloatChargeVolage_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetFloatChargeVolage.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }


        #endregion

        #region 低电告警电压
        //低电告警电压
        private string _BattLowAlarmVolt;

        public string BattLowAlarmVolt
        {
            get { return _BattLowAlarmVolt; }
            set
            {
                _BattLowAlarmVolt = Tools.RemoveLeadingZeros(value) + "V";
                this.RaiseProperChanged(nameof(BattLowAlarmVolt));
            }
        }


        private bool BattLowAlarmVolt_IsWorking;


        //设置值
        private string _BattLowAlarmVolt_Inputs;

        public string BattLowAlarmVolt_Inputs
        {
            get { return _BattLowAlarmVolt_Inputs; }
            set
            {
                _BattLowAlarmVolt_Inputs = value;
                this.RaiseProperChanged(nameof(BattLowAlarmVolt_Inputs));
                Command_SetBattLowAlarmVolt.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand Command_SetBattLowAlarmVolt { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void BattLowAlarmVoltOperation()
        {
            try
            {
                BattLowAlarmVolt_IsWorking = true;
                // 禁用按钮
                Command_SetBattLowAlarmVolt.RaiseCanExecuteChanged();

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
                    string receive = SerialCommunicationService.SendSettingCommand("PSLV", Tools.FormatToXxx(BattLowAlarmVolt_Inputs));

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
                BattLowAlarmVolt_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetBattLowAlarmVolt.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        #endregion

        #region 低电锁机电压

        //低电锁机电压
        private string _LowPowerLock;

        public string LowPowerLock
        {
            get { return _LowPowerLock; }
            set
            {
                _LowPowerLock = Tools.RemoveLeadingZeros(value) + "V";
                RaiseProperChanged(nameof(LowPowerLock));
            }
        }

        private bool _LowPowerLockIsWorking;


        //设置值
        private string _LowPowerLock_Inputs;

        public string LowPowerLock_Inputs
        {
            get { return _LowPowerLock_Inputs; }
            set
            {
                _LowPowerLock_Inputs = value;
                RaiseProperChanged(nameof(LowPowerLock_Inputs));
                Command_SetLowPowerLock.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand Command_SetLowPowerLock { get; }

        /// <summary>
        /// 点击工作模式设置
        /// </summary>
        private async void LowPowerLockOperation()
        {
            try
            {
                _LowPowerLockIsWorking = true;
                // 禁用按钮
                Command_SetLowPowerLock.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState($"正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    //Thread.Sleep(2000);
                    string receive = SerialCommunicationService.SendSettingCommand("PSDV", Tools.FormatToXxx(LowPowerLock_Inputs));

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
                _LowPowerLockIsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetLowPowerLock.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }


        #endregion

        #region 输出功率因数

        //输出功率因数
        private string _Outputpowerfactor;

        public string Outputpowerfactor
        {
            get { return _Outputpowerfactor; }
            set
            {
                _Outputpowerfactor = Tools.RemoveLeadingZeros(value);
                this.RaiseProperChanged(nameof(Outputpowerfactor));
            }
        }


        private bool Outputpowerfactor_IsWorking;


        //设置值
        private string _Outputpowerfactor_Inputs;

        public string Outputpowerfactor_Inputs
        {
            get { return _Outputpowerfactor_Inputs; }
            set
            {
                _Outputpowerfactor_Inputs = value;
                this.RaiseProperChanged(nameof(Outputpowerfactor_Inputs));
                Command_Outputpowerfactor.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand Command_Outputpowerfactor { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void OutputpowerfactorOperation()
        {
            try
            {
                Outputpowerfactor_IsWorking = true;
                // 禁用按钮
                Command_Outputpowerfactor.RaiseCanExecuteChanged();

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
                    string receive = SerialCommunicationService.SendSettingCommand("设置指令", Outputpowerfactor_Inputs);

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
                Outputpowerfactor_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_Outputpowerfactor.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        #endregion

        #region 电池带载限制时间
        //电池带载限制时间
        private string _BatteryloadlimitTime;

        public string BatteryloadlimitTime
        {
            get { return _BatteryloadlimitTime; }
            set
            {
                _BatteryloadlimitTime = Tools.RemoveLeadingZeros(value);
                this.RaiseProperChanged(nameof(BatteryloadlimitTime));
            }
        }


        private bool BatteryloadlimitTime_IsWorking;


        //设置值
        private string _BatteryloadlimitTime_Inputs;

        public string BatteryloadlimitTime_Inputs
        {
            get { return _BatteryloadlimitTime_Inputs; }
            set
            {
                _BatteryloadlimitTime_Inputs = value;
                this.RaiseProperChanged(nameof(BatteryloadlimitTime_Inputs));
                Command_BatteryloadlimitTime.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand Command_BatteryloadlimitTime { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void BatteryloadlimitTimeOperation()
        {
            try
            {
                BatteryloadlimitTime_IsWorking = true;
                // 禁用按钮
                Command_BatteryloadlimitTime.RaiseCanExecuteChanged();

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
                    string receive = SerialCommunicationService.SendSettingCommand("设置指令", BatteryloadlimitTime_Inputs);

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
                BatteryloadlimitTime_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_BatteryloadlimitTime.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        #endregion

        #region 电池放电限制时间
        //电池放电限制时间
        private string _BatterydischargelimitTime;

        public string BatterydischargelimitTime
        {
            get { return _BatterydischargelimitTime; }
            set
            {
                _BatterydischargelimitTime = Tools.RemoveLeadingZeros(value);
                this.RaiseProperChanged(nameof(BatterydischargelimitTime));
            }
        }


        private bool BatterydischargelimitTime_IsWorking;


        //设置值
        private string _BatterydischargelimitTime_Inputs;

        public string BatterydischargelimitTime_Inputs
        {
            get { return _BatterydischargelimitTime_Inputs; }
            set
            {
                _BatterydischargelimitTime_Inputs = value;
                this.RaiseProperChanged(nameof(BatterydischargelimitTime_Inputs));
                Command_BatterydischargelimitTime.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand Command_BatterydischargelimitTime { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void BatterydischargelimitTimeOperation()
        {
            try
            {
                BatterydischargelimitTime_IsWorking = true;
                // 禁用按钮
                Command_BatterydischargelimitTime.RaiseCanExecuteChanged();

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
                    string receive = SerialCommunicationService.SendSettingCommand("设置指令", BatterydischargelimitTime_Inputs);

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
                BatterydischargelimitTime_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_BatterydischargelimitTime.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        #endregion

        #region 旁路高退电压

        //旁路高退电压
        private string _BypasshighDropout;

        public string BypasshighDropout
        {
            get { return _BypasshighDropout; }
            set
            {
                _BypasshighDropout = Tools.RemoveLeadingZeros(value);
                RaiseProperChanged(nameof(BypasshighDropout));
            }
        }

        private bool _BypasshighDropoutIsWorking;


        //设置值
        private string _BypasshighDropout_Inputs;

        public string BypasshighDropout_Inputs
        {
            get { return _BypasshighDropout_Inputs; }
            set
            {
                _BypasshighDropout_Inputs = value;
                RaiseProperChanged(nameof(BypasshighDropout_Inputs));
                Command_SetBypasshighDropout.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand Command_SetBypasshighDropout { get; }

        /// <summary>
        /// 点击工作模式设置
        /// </summary>
        private async void BypasshighDropoutOperation()
        {
            try
            {
                _BypasshighDropoutIsWorking = true;
                // 禁用按钮
                Command_SetBypasshighDropout.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState($"正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    //Thread.Sleep(2000);
                    string receive = SerialCommunicationService.SendSettingCommand("V", BypasshighDropout_Inputs);

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
                _BypasshighDropoutIsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetBypasshighDropout.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }


        #endregion

        #region 旁路低退电压

        //旁路低退电压
        private string _BypasslowDropout;

        public string BypasslowDropout
        {
            get { return _BypasslowDropout; }
            set
            {
                _BypasslowDropout = Tools.RemoveLeadingZeros(value);
                RaiseProperChanged(nameof(BypasslowDropout));
            }
        }

        private bool _BypasslowDropoutIsWorking;


        //设置值
        private string _BypasslowDropout_Inputs;

        public string BypasslowDropout_Inputs
        {
            get { return _BypasslowDropout_Inputs; }
            set
            {
                _BypasslowDropout_Inputs = value;
                RaiseProperChanged(nameof(BypasslowDropout_Inputs));
                Command_SetBypasslowDropout.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand Command_SetBypasslowDropout { get; }

        /// <summary>
        /// 点击工作模式设置
        /// </summary>
        private async void BypasslowDropoutOperation()
        {
            try
            {
                _BypasslowDropoutIsWorking = true;
                // 禁用按钮
                Command_SetBypasslowDropout.RaiseCanExecuteChanged();

                // 异步等待锁
                await _semaphore.WaitAsync();
                UpdateState($"正在执行设置命令");
                //Status = "正在执行特殊操作...";

                // 暂停后台线程
                _pauseEvent.Reset();
                AddLog("已暂停后台通信");

                // 执行特殊操作（带超时保护）
                using var timeoutCts = new CancellationTokenSource(5000);
                await Task.Run(new Action(() =>
                {
                    //执行设置指令
                    //Thread.Sleep(2000);
                    string receive = SerialCommunicationService.SendSettingCommand("V", BypasslowDropout_Inputs);

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
                _BypasslowDropoutIsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetBypasslowDropout.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }


        #endregion

        #region 通用方法
        // 输入验证&选择验证
        private bool Validate(string value)
        {
            switch (value)
            {
                case "LowPowerLock_Inputs":
                    return !string.IsNullOrWhiteSpace(LowPowerLock_Inputs);                      //低电锁机电压
                case "BattLowAlarmVolt_Inputs":
                    return !string.IsNullOrWhiteSpace(BattLowAlarmVolt_Inputs);                  //低电告警电压
                case "StrongChargeVoltage_Inputs":
                    return !string.IsNullOrWhiteSpace(StrongChargeVoltage_Inputs);               //强充电压
                case "FloatChargeVolage_Inputs":
                    return !string.IsNullOrWhiteSpace(FloatChargeVolage_Inputs);                 //浮充电压
                case "BuzzerStatus_Inputs":
                    return !string.IsNullOrWhiteSpace(BuzzerStatus_Inputs);                      //蜂鸣器使能
                case "LCD_Backlight_Inputs":
                    return !string.IsNullOrWhiteSpace(LCD_Backlight_Inputs);                     //背光开启使能
                case "OutputSettingFrequency_Inputs":
                    return !string.IsNullOrWhiteSpace(OutputSettingFrequency_Inputs);            //输出频率
                case "OutSetVolt_Inputs":
                    return !string.IsNullOrWhiteSpace(OutSetVolt_Inputs);                        //输出电压
                case "AutoStartEnable_Inputs":
                    return !string.IsNullOrWhiteSpace(AutoStartEnable_Inputs);                   //自动开机使能
                case "AutoRestartAC_Inputs":
                    return !string.IsNullOrWhiteSpace(AutoRestartAC_Inputs);                     //市电自动重启
                case "ECOMode_Inputs":
                    return !string.IsNullOrWhiteSpace(ECOMode_Inputs);                           //ECO模式
                case "PassFunctionEnable_Inputs":
                    return !string.IsNullOrWhiteSpace(PassFunctionEnable_Inputs);                 //旁路使能
                case "Frequencyrestrictionmode_Inputs":
                    return !string.IsNullOrWhiteSpace(Frequencyrestrictionmode_Inputs);            //频率限制模式
                case "BypasshighDropout_Inputs":
                    return !string.IsNullOrWhiteSpace(BypasshighDropout_Inputs);                   //旁路高退电压
                case "BypasslowDropout_Inputs":
                    return !string.IsNullOrWhiteSpace(BypasslowDropout_Inputs);                          //旁路低退电压
                case "Outputpowerfactor_Inputs":
                    return !string.IsNullOrWhiteSpace(Outputpowerfactor_Inputs);                          //功率因数
                case "BatteryloadlimitTime_Inputs":
                    return !string.IsNullOrWhiteSpace(BatteryloadlimitTime_Inputs);                          //电池带载限制时间
                case "BatterydischargelimitTime_Inputs":
                    return !string.IsNullOrWhiteSpace(BatterydischargelimitTime_Inputs);                          //电池放电限制时间
                default:
                    return false;
            }

        }


        /// <summary>
        /// 对字符串进行解析
        /// </summary>
        /// <param name="value"></param>
        public void AnalyseStringToElement(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                ReceiveException("空");
                return;
            }
            if (value.StartsWith("-1"))
            {
                ReceiveException("CRC异常");
                AddLog(value);
                return;
            }
            string[] Values = value.Split(" ");
            try
            {
                //蜂鸣器状态
                BuzzerStatus = Values[0].Substring(1, 1);
                //LCD背光
                LCD_Backlight = Values[0].Substring(2, 1);
                //故障记录
                FaultLog = Values[0].Substring(3, 1);
                //自动开机使能
                AutoStartEnable = Values[1].Substring(0, 1);
                //市电自动重启
                AutoRestartAC = Values[1].Substring(1, 1);
                //系统(输出)频率
                OutputSettingFrequency = Values[1].Substring(2, 1);
                //ECO模式
                ECOMode = Values[1].Substring(3, 1);
                //旁路使能
                PassFunctionEnable = Values[1].Substring(4, 1);
                //频率限制模式
                Frequencyrestrictionmode = Values[1].Substring(5, 1);
                //输出设定电压
                OutSetVolt = Values[2];
                //电池类型
                BatteryType = Values[3];
                //强充电压
                StrongChargeVoltage = Values[4];
                //浮充电压
                FloatChargeVolage = Values[5];
                //电池低电告警电压
                BattLowAlarmVolt = Values[6];
                //低电锁机电压
                LowPowerLock = Values[7];
                //输出功率因数
                Outputpowerfactor = Values[8];
                //电池带载限制时间
                BatteryloadlimitTime = Values[9];
                //电池放电限制时间
                BatterydischargelimitTime = Values[10];
                //旁路高退电压
                BypasshighDropout = Values[13];
                //旁路低退电压
                BypasslowDropout = Values[14];
            }
            catch (Exception)
            {
                //异常
                ReceiveException("HEEP1异常");
                AddLog($"{command}返回数据：{value}解析异常");
            }

        }

        /// <summary>
        /// 对属性值进行二次转换，换成命令指令符号
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string getSelectedToCommad(string value)
        {
            switch (value)
            {
                //设置蜂鸣器状态
                case "BuzzerStatus_Inputs":
                    if (string.IsNullOrWhiteSpace(BuzzerStatus_Inputs)) { return string.Empty; }
                    else if (BuzzerStatus_Inputs == "开启/On") { return "PEa"; }
                    else if (BuzzerStatus_Inputs == "关闭/Off") { return "PDa"; }
                    else
                        return "";

                //LCD背光开启
                case "LCD_Backlight_Inputs":
                    if (string.IsNullOrWhiteSpace(LCD_Backlight_Inputs)) { return string.Empty; }
                    else if (LCD_Backlight_Inputs == "开启/On") { return "PEx"; }
                    else if (LCD_Backlight_Inputs == "关闭/Off") { return "PDx"; }
                    else
                        return "";

                //故障记录
                case "FaultLog_Inputs":
                    if (FaultLog_Inputs == "开启/On"){ return "PEz";}
                    else if (FaultLog_Inputs == "关闭/Off"){  return "PDz";}
                    else return FaultLog_Inputs;

                //自动开机使能
                case "AutoStartEnable_Inputs":
                    if (AutoStartEnable_Inputs == "开启/On") { return "1"; }
                    else if (AutoStartEnable_Inputs == "关闭/Off") { return "0"; }
                    else return AutoStartEnable_Inputs;

                //市电自动重启
                case "AutoRestartAC_Inputs":
                    if (AutoRestartAC_Inputs == "开启/On") { return "1"; }
                    else if (AutoRestartAC_Inputs == "关闭/Off") { return "0"; }
                    else return AutoRestartAC_Inputs;

                //ECO模式
                case "ECOMode_Inputs":
                    if (ECOMode_Inputs == "开启/On") { return "PEj"; }
                    else if (ECOMode_Inputs == "关闭/Off") { return "PDj"; }
                    else return ECOMode_Inputs;

                //旁路使能
                case "PassFunctionEnable_Inputs":
                    if (PassFunctionEnable_Inputs == "开启/On") { return "1"; }
                    else if (PassFunctionEnable_Inputs == "关闭/Off") { return "0"; }
                    else return PassFunctionEnable_Inputs;

                //频率限制模式
                case "Frequencyrestrictionmode_Inputs":
                    if (Frequencyrestrictionmode_Inputs == "开启/On") { return "1"; }
                    else if (Frequencyrestrictionmode_Inputs == "关闭/Off") { return "0"; }
                    else return Frequencyrestrictionmode_Inputs;

                default:
                    return "";
            }
        }

        /// <summary>
        /// 接收异常使用方法
        /// </summary>
        /// <param name="exceptionDescription"></param>
        private void ReceiveException(string exceptionDescription)
        {
            //蜂鸣器状态
            BuzzerStatus = exceptionDescription;
            //LCD背光
            LCD_Backlight = exceptionDescription;
            //故障记录
            FaultLog = exceptionDescription;
            //自动开机使能
            AutoStartEnable = exceptionDescription;
            //市电自动重启
            AutoRestartAC = exceptionDescription;
            //系统(输出)频率
            OutputSettingFrequency = exceptionDescription;
            //ECO模式
            ECOMode = exceptionDescription;
            //旁路使能
            PassFunctionEnable = exceptionDescription;
            //频率限制模式
            Frequencyrestrictionmode = exceptionDescription;
            //输出设定电压
            OutSetVolt = exceptionDescription;
            //强充电压
            StrongChargeVoltage = exceptionDescription;
            //浮充电压
            FloatChargeVolage = exceptionDescription;
            //电池低电告警电压
            BattLowAlarmVolt = exceptionDescription;
            //低电锁机电压
            LowPowerLock = exceptionDescription;
            //输出功率因数
            Outputpowerfactor = exceptionDescription;
            //电池带载限制时间
            BatteryloadlimitTime = exceptionDescription;
            //电池放电限制时间
            BatterydischargelimitTime = exceptionDescription;
            //旁路高退电压
            BypasshighDropout = exceptionDescription;
            //旁路低退电压
            BypasslowDropout = exceptionDescription;
        }
        #endregion
    }
}
