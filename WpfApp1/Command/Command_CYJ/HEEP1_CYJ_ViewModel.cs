using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Convert;
using WpfApp1.Services;
using WpfApp1.ViewModels;

namespace WpfApp1.Command.Command_CYJ
{
    public class HEEP1_CYJ_ViewModel : BaseViewModel
    {
        //指令
        private string command = "HEEP1\r";
        public string Command { get { return command; } }

        public string MachineType;

        ManualResetEventSlim _pauseEvent;//线程的开启、暂停
        SemaphoreSlim _semaphore;        //异步竞争，资源锁
        Action<string> AddLog;           //添加日志委托
        Action<string> UpdateState;      //更新状态日志
        public HEEP1_CYJ_ViewModel(ManualResetEventSlim pauseEvent, SemaphoreSlim semaphore, Action<string> addLog, Action<string> _updateState)
        {
            _pauseEvent = pauseEvent;
            _semaphore = semaphore;
            AddLog = addLog;
            UpdateState = _updateState;

            #region 初始化命令
            //充电电流
            Command_SetChargeCur = new RelayCommand(
                execute: () => ChargeCurOperation(),
                canExecute: () => Validate(nameof(ChargeCur_Inputs)) && !ChargeCur_IsWorking
            );

            //设置低电锁机电压
            Command_SetLowPowerLock = new RelayCommand(
                execute: () => LowPowerLockOperation(),
                canExecute: () => Validate(nameof(LowPowerLock_Inputs)) && !_LowPowerLockIsWorking
            );

            //强充电压
            Command_SetStrongChargeVoltage = new RelayCommand(
                execute: () => StrongChargeVoltageOperation(),
                canExecute: () => Validate(nameof(StrongChargeVoltage_Inputs)) && !StrongChargeVoltage_IsWorking
            );

            //强充电压
            Command_SetStrongChargeVoltage = new RelayCommand(
                execute: () => StrongChargeVoltageOperation(),
                canExecute: () => Validate(nameof(StrongChargeVoltage_Inputs)) && !StrongChargeVoltage_IsWorking
            );

            //强充电压
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

            //自动返回首页
            Command_SetAutoReturnHome = new RelayCommand(
              execute: () => AutoReturnHomeOperation(),
              canExecute: () => Validate(nameof(AutoReturnHome_Inputs)) && !AutoReturnHome_IsWorking
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

            //定时关机
            Command_SetTimeToDown = new RelayCommand(
              execute: () => TimeToDownOperation(),
              canExecute: () => Validate(nameof(TimeToDown_Inputs)) && !TimeToDown_IsWorking
             );
            //模式选择
            Command_SetMode = new RelayCommand(
              execute: () => ModeOperation(),
              canExecute: () => Validate(nameof(Mode_Inputs)) && !Mode_IsWorking
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
        private List<string> _AutoStartEnableOptions = new List<string> { "开启/On" };

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
                    string receive = SerialCommunicationService.SendSettingCommand("SWON", "");

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

        #region 强充电压

        //强充电压
        private string _StrongChargeVoltage;

        public string StrongChargeVoltage
        {
            get { return _StrongChargeVoltage; }
            set
            {
                _StrongChargeVoltage = Tools.RemoveLeadingZeros(value)+"V";
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
                _FloatChargeVoltage = Tools.RemoveLeadingZeros(value)+"V";
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
                    string receive = SerialCommunicationService.SendSettingCommand("PSLV",Tools.FormatToXxx(BattLowAlarmVolt_Inputs));

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

        #region 高压锁机电压
        //高压锁机电压
        private string _BattHvLockVolt;

        public string BattHvLockVolt
        {
            get { return _BattHvLockVolt; }
            set
            {
                _BattHvLockVolt = Tools.RemoveLeadingZeros(value)+ "V";
                this.RaiseProperChanged(nameof(BattHvLockVolt));
            }
        }


        private bool BattHvLockVolt_IsWorking;


        //设置值
        private string _BattHvLockVolt_Inputs;

        public string BattHvLockVolt_Inputs
        {
            get { return _BattHvLockVolt_Inputs; }
            set
            {
                _BattHvLockVolt_Inputs = value;
                this.RaiseProperChanged(nameof(BattHvLockVolt_Inputs));
                Command_SetBattHvLockVolt.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand Command_SetBattHvLockVolt { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void BattHvLockVoltOperation()
        {
            try
            {
                BattHvLockVolt_IsWorking = true;
                // 禁用按钮
                Command_SetBattHvLockVolt.RaiseCanExecuteChanged();

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
                    string receive = SerialCommunicationService.SendSettingCommand("设置指令", BattHvLockVolt_Inputs);

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
                BattHvLockVolt_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetBattHvLockVolt.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        #endregion

        #region 强充时间
        //强充时间
        private string _BoostTime;

        public string BoostTime
        {
            get { return _BoostTime; }
            set
            {
                _BoostTime = Tools.RemoveLeadingZeros(value);
                this.RaiseProperChanged(nameof(BoostTime));
            }
        }


        private bool BoostTime_IsWorking;


        //设置值
        private string _BoostTime_Inputs;

        public string BoostTime_Inputs
        {
            get { return _BoostTime_Inputs; }
            set
            {
                _BoostTime_Inputs = value;
                this.RaiseProperChanged(nameof(BoostTime_Inputs));
                Command_SetBoostTime.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand Command_SetBoostTime { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void BoostTimeOperation()
        {
            try
            {
                BoostTime_IsWorking = true;
                // 禁用按钮
                Command_SetBoostTime.RaiseCanExecuteChanged();

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
                    string receive = SerialCommunicationService.SendSettingCommand("设置指令", BoostTime_Inputs);

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
                BoostTime_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetBoostTime.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        #endregion

        #region 逆变时间
        //逆变时间
        private string _InvTime;

        public string InvTime
        {
            get { return _InvTime; }
            set
            {
                _InvTime = Tools.RemoveLeadingZeros(value);
                this.RaiseProperChanged(nameof(InvTime));
            }
        }


        private bool InvTime_IsWorking;


        //设置值
        private string _InvTime_Inputs;

        public string InvTime_Inputs
        {
            get { return _InvTime_Inputs; }
            set
            {
                _InvTime_Inputs = value;
                this.RaiseProperChanged(nameof(InvTime_Inputs));
                Command_SetInvTime.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand Command_SetInvTime { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void InvTimeOperation()
        {
            try
            {
                InvTime_IsWorking = true;
                // 禁用按钮
                Command_SetInvTime.RaiseCanExecuteChanged();

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
                    string receive = SerialCommunicationService.SendSettingCommand("设置指令", InvTime_Inputs);

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
                InvTime_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetInvTime.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        #endregion

        #region 带载时间
        //带载时间
        private string _LoadTime;

        public string LoadTime
        {
            get { return _LoadTime; }
            set
            {
                _LoadTime = Tools.RemoveLeadingZeros(value);
                this.RaiseProperChanged(nameof(LoadTime));
            }
        }


        private bool LoadTime_IsWorking;


        //设置值
        private string _LoadTime_Inputs;

        public string LoadTime_Inputs
        {
            get { return _LoadTime_Inputs; }
            set
            {
                _LoadTime_Inputs = value;
                this.RaiseProperChanged(nameof(LoadTime_Inputs));
                Command_SetLoadTime.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand Command_SetLoadTime { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void LoadTimeOperation()
        {
            try
            {
                LoadTime_IsWorking = true;
                // 禁用按钮
                Command_SetLoadTime.RaiseCanExecuteChanged();

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
                    string receive = SerialCommunicationService.SendSettingCommand("设置指令", LoadTime_Inputs);

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
                LoadTime_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetLoadTime.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        #endregion

        #region 自动返回首页

        private string _AutoReturnHome;

        public string AutoReturnHome
        {
            get { return _AutoReturnHome; }
            set
            {
                if (value == "0")
                {
                    _AutoReturnHome = App.GetText("关闭");
                }
                else if (value == "1")
                {
                    _AutoReturnHome = App.GetText("开启");
                }
                else
                    _AutoReturnHome = value;
                this.RaiseProperChanged(nameof(AutoReturnHome));
            }
        }


        private bool AutoReturnHome_IsWorking;


        //设置值
        private string _AutoReturnHome_Inputs;

        public string AutoReturnHome_Inputs
        {
            get { return _AutoReturnHome_Inputs; }
            set
            {
                _AutoReturnHome_Inputs = value;
                this.RaiseProperChanged(nameof(AutoReturnHome_Inputs));
                Command_SetAutoReturnHome.RaiseCanExecuteChanged();
            }
        }

        //下拉选项
        private List<string> _AutoReturnHomeOptions = new List<string> { "开启/On", "关闭/Off" };

        public List<string> AutoReturnHomeOptions
        {
            get { return _AutoReturnHomeOptions; }
            set
            {
                _AutoReturnHomeOptions = value;
                this.RaiseProperChanged(nameof(AutoReturnHomeOptions));
            }
        }

        public RelayCommand Command_SetAutoReturnHome { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void AutoReturnHomeOperation()
        {
            try
            {
                AutoReturnHome_IsWorking = true;
                // 禁用按钮
                Command_SetAutoReturnHome.RaiseCanExecuteChanged();

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
                    string receive = SerialCommunicationService.SendSettingCommand("", getSelectedToCommad(nameof(AutoReturnHome_Inputs)));

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
                AutoReturnHome_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetAutoReturnHome.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }


        #endregion

        #region 定时关机

        private string _TimeToDown;

        public string TimeToDown
        {
            get { return _TimeToDown; }
            set
            {
                _TimeToDown = value;
                this.RaiseProperChanged(nameof(TimeToDown));
            }
        }


        private bool TimeToDown_IsWorking;


        //设置值
        private string _TimeToDown_Inputs;

        public string TimeToDown_Inputs
        {
            get { return _TimeToDown_Inputs; }
            set
            {
                _TimeToDown_Inputs = value;
                this.RaiseProperChanged(nameof(TimeToDown_Inputs));
                Command_SetTimeToDown.RaiseCanExecuteChanged();
            }
        }

        //下拉选项
        private List<string> _TimeToDownOptions = new List<string> { "取消/Cancel", "关机/ShutDown","10min","15min","20min","30min" };

        public List<string> TimeToDownOptions
        {
            get { return _TimeToDownOptions; }
            set
            {
                _TimeToDownOptions = value;
                this.RaiseProperChanged(nameof(TimeToDownOptions));
            }
        }

        public RelayCommand Command_SetTimeToDown { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void TimeToDownOperation()
        {
            try
            {
                TimeToDown_IsWorking = true;
                // 禁用按钮
                Command_SetTimeToDown.RaiseCanExecuteChanged();

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
                    string receive = SerialCommunicationService.SendSettingCommand("SWOFF", getSelectedToCommad("TimeToDown_Inputs"));

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
                TimeToDown_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetTimeToDown.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }


        #endregion

        #region 模式选择

        private string _Mode;

        public string Mode
        {
            get { return _Mode; }
            set
            {
                if(value=="0")
                _Mode = "UPS";
                else
                {
                    _Mode = "GEN";
                }
                this.RaiseProperChanged(nameof(Mode));
            }
        }


        private bool Mode_IsWorking;


        //设置值
        private string _Mode_Inputs;

        public string Mode_Inputs
        {
            get { return _Mode_Inputs; }
            set
            {
                _Mode_Inputs = value;
                this.RaiseProperChanged(nameof(Mode_Inputs));
                Command_SetMode.RaiseCanExecuteChanged();
            }
        }

        //下拉选项
        private List<string> _ModeOptions = new List<string> { "UPS", "GEN" };

        public List<string> ModeOptions
        {
            get { return _ModeOptions; }
            set
            {
                _ModeOptions = value;
                this.RaiseProperChanged(nameof(ModeOptions));
            }
        }

        public RelayCommand Command_SetMode { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void ModeOperation()
        {
            try
            {
                Mode_IsWorking = true;
                // 禁用按钮
                Command_SetMode.RaiseCanExecuteChanged();

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
                    string receive = SerialCommunicationService.SendSettingCommand("PGR",getSelectedToCommad("Mode_Inputs"));

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
                Mode_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetMode.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        #endregion

        #region 充电电流

        private string _ChargeCur;

        public string ChargeCur
        {
            get { return _ChargeCur; }
            set
            {
                _ChargeCur = value;
                this.RaiseProperChanged(nameof(ChargeCur));
            }
        }


        private bool ChargeCur_IsWorking;


        //设置值
        private string _ChargeCur_Inputs;

        public string ChargeCur_Inputs
        {
            get { return _ChargeCur_Inputs; }
            set
            {
                _ChargeCur_Inputs = value;
                this.RaiseProperChanged(nameof(ChargeCur_Inputs));
                Command_SetChargeCur.RaiseCanExecuteChanged();
            }
        }

        //下拉选项
        private List<string> _ChargeCurOptions = new List<string> {"02", "03", "04", "05", "06", "07", "08", "09" ,"10"};

        public List<string> ChargeCurOptions
        {
            get { return _ChargeCurOptions; }
            set
            {
                _ChargeCurOptions = value;
                this.RaiseProperChanged(nameof(ChargeCurOptions));
            }
        }

        public RelayCommand Command_SetChargeCur { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void ChargeCurOperation()
        {
            try
            {
                ChargeCur_IsWorking = true;
                // 禁用按钮
                Command_SetChargeCur.RaiseCanExecuteChanged();

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
                    ChargeCur = "正在写入...";
                    //执行设置指令
                    Thread.Sleep(2000);//没有这个延时会报错
                    string receive = SerialCommunicationService.SendSettingCommand("MUCHGC0", ChargeCur_Inputs);
                    if (receive.StartsWith("(ACK"))
                    {
                        ChargeCur = "写入成功";
                    }
                    else
                        ChargeCur = "写入失败";

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
                ChargeCur_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetChargeCur.RaiseCanExecuteChanged();
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
                case "ChargeCur_Inputs":
                    return !String.IsNullOrWhiteSpace(ChargeCur_Inputs);
                case "LowPowerLock_Inputs":
                    return !string.IsNullOrWhiteSpace(LowPowerLock_Inputs);                      //低电锁机电压
                case "BattLowAlarmVolt_Inputs":
                    return !string.IsNullOrWhiteSpace(BattLowAlarmVolt_Inputs);                  //低电告警电压
                case "BattHvLockVolt_Inputs":
                    return !string.IsNullOrWhiteSpace(BattHvLockVolt_Inputs);                    //高压锁机电压
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
                case "AutoReturnHome_Inputs":
                    return !string.IsNullOrWhiteSpace(AutoReturnHome_Inputs);                    //自动返回首页
                case "OutSetVolt_Inputs":
                    return !string.IsNullOrWhiteSpace(OutSetVolt_Inputs);                        //输出电压
                case "AutoStartEnable_Inputs":
                    return !string.IsNullOrWhiteSpace(AutoStartEnable_Inputs);                   //自动开机使能
                case "FaultLog_Inputs":
                    return !string.IsNullOrWhiteSpace(FaultLog_Inputs);                          //故障记录
                case "TimeToDown_Inputs":
                    return !string.IsNullOrWhiteSpace(TimeToDown_Inputs);                        //定时关机
                case "Mode_Inputs":
                    return !string.IsNullOrWhiteSpace(Mode_Inputs);                              //模式
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
                AutoStartEnable = Values[0].Substring(6, 1);
                //系统(输出)频率
                OutputSettingFrequency = Values[1];
                //输出设定电压
                OutSetVolt = Values[2];
                //强充电压
                StrongChargeVoltage = Values[3];
                //浮充电压
                FloatChargeVolage = Values[4];
                //电池低电告警电压
                BattLowAlarmVolt = Values[5];
                //低电锁机电压
                LowPowerLock = Values[6];
                //电池高压锁机电压
                BattHvLockVolt = Values[7];
                //强充时间
                BoostTime = Values[8];
                //逆变时间
                InvTime = Values[9];
                //带载时间
                LoadTime = Values[10];
                //自动返回首页
                AutoReturnHome = Values[11];
                //模式
                Mode = Values[12];

            }
            catch (Exception)
            {
                //异常
                ReceiveException("HEEP1_Exe");
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

                //系统频率
                case "OutputSettingFrequency_Inputs":
                    if (string.IsNullOrWhiteSpace(OutputSettingFrequency_Inputs)) { return string.Empty; }
                    else if (OutputSettingFrequency_Inputs == "50") { return "1"; }
                    else if (OutputSettingFrequency_Inputs == "60") { return "0"; }
                    else
                        return OutputSettingFrequency_Inputs;

                //自动返回首页
                case "AutoReturnHome_Inputs":
                    if (AutoReturnHome_Inputs == "开启/On")
                    {
                        return "PEk";
                    }
                    else if (AutoReturnHome_Inputs == "关闭/Off")
                    {
                        return "PDk";
                    }
                    else
                        return AutoReturnHome_Inputs;
                //故障记录
                case "FaultLog_Inputs":
                    if (FaultLog_Inputs == "开启/On")
                    {
                        return "PEz";
                    }
                    else if (FaultLog_Inputs == "关闭/Off")
                    {
                        return "PDz";
                    }
                    else
                        return FaultLog_Inputs;
                    
                case "TimeToDown_Inputs":
                    if (TimeToDown_Inputs == "取消/Cancel")
                    {
                        return "00";
                    }
                    else if (TimeToDown_Inputs == "关机/ShutDown")
                    {
                        return "01";
                    }else if(TimeToDown_Inputs == "10min")
                    {
                        return "02";
                    }else if(TimeToDown_Inputs == "15min")
                    {
                        return "03";
                    }else if (TimeToDown_Inputs == "20min")
                    {
                        return "04";
                    }else if(TimeToDown_Inputs == "30min")
                    {
                        return "05";
                    }else 
                    return TimeToDown_Inputs;
                case "Mode_Inputs":
                    if (Mode_Inputs == "UPS")
                    {
                        return "00";
                    }
                    else
                    {
                        return "01";
                    }
                    
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
            //系统(输出)频率
            OutputSettingFrequency = exceptionDescription;
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
            //电池高压锁机电压
            BattHvLockVolt = exceptionDescription;
            //强充时间
            BoostTime = exceptionDescription;
            //逆变时间
            InvTime = exceptionDescription;
            //带载时间
            LoadTime = exceptionDescription;
            //自动返回首页
            AutoReturnHome = exceptionDescription;
        }
        #endregion
    }
}
