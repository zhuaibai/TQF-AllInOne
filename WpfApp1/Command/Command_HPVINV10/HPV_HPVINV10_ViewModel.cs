using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Convert;
using WpfApp1.Services;
using WpfApp1.ViewModels;

namespace WpfApp1.Command.Command_HPVINV10
{
    public class HPV_HPVINV10_ViewModel:BaseViewModel
    {
        //指令
        private string command = "HPV\r";
        public string Command { get { return command; } }

        ManualResetEventSlim _pauseEvent;//线程的开启、暂停
        SemaphoreSlim _semaphore;        //异步竞争，资源锁
        Action<string> AddLog;           //添加日志委托
        Action<string> UpdateState;      //更新状态日志

        public HPV_HPVINV10_ViewModel(ManualResetEventSlim pauseEvent, SemaphoreSlim semaphore, Action<string> addLog, Action<string> updateState)
        {
            _pauseEvent = pauseEvent;
            _semaphore = semaphore;
            AddLog = addLog;
            UpdateState = updateState;

            #region 初始化指令

            //PVA路电压
            Command_SetPVAVolt = new RelayCommand(
                execute: () => PVAVoltOperation(),
                canExecute: () => Validate(nameof(PVAVolt_Inputs)) && !PVAVolt_IsWorking // 增加处理状态检查
            );
            //PVA路电流
            Command_SetPVACurr = new RelayCommand(
               execute: () => PVACurrOperation(),
               canExecute: () => Validate(nameof(PVACurr_Inputs)) && !PVACurr_IsWorking // 增加处理状态检查
            );
            //PVA路功率
            Command_SetPVAPwr = new RelayCommand(
               execute: () => PVAPwrOperation(),
               canExecute: () => Validate(nameof(PVAPwr_Inputs)) && !PVAPwr_IsWorking // 增加处理状态检查
            );
            //PVB路电压
            Command_SetPVBVolt = new RelayCommand(
                execute: () => PVBVoltOperation(),
                canExecute: () => Validate(nameof(PVBVolt_Inputs)) && !PVBVolt_IsWorking // 增加处理状态检查
            );
            //PVB路电流
            Command_SetPVBCurr = new RelayCommand(
               execute: () => PVBCurrOperation(),
               canExecute: () => Validate(nameof(PVBCurr_Inputs)) && !PVBCurr_IsWorking // 增加处理状态检查
            );
            //PVB路功率
            Command_SetPVBPwr = new RelayCommand(
               execute: () => PVBPwrOperation(),
               canExecute: () => Validate(nameof(PVBPwr_Inputs)) && !PVBPwr_IsWorking // 增加处理状态检查
            );
            #endregion
        }


        #region PVA路电压

        //PV电压
        private string _PVAVolt;

        public string PVAVolt
        {
            get { return _PVAVolt; }
            set
            {
                _PVAVolt = Tools.RemoveLeadingZeros(value);
                this.RaiseProperChanged(nameof(PVAVolt));
            }
        }


        private bool PVAVolt_IsWorking;


        //设置值
        private string _PVAVolt_Inputs;

        public string PVAVolt_Inputs
        {
            get { return _PVAVolt_Inputs; }
            set
            {
                _PVAVolt_Inputs = value;
                this.RaiseProperChanged(nameof(PVAVolt_Inputs));
                Command_SetPVAVolt.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand Command_SetPVAVolt { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void PVAVoltOperation()
        {
            try
            {
                PVAVolt_IsWorking = true;
                // 禁用按钮
                Command_SetPVAVolt.RaiseCanExecuteChanged();

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
                    string receive = SerialCommunicationService.SendSettingCommand("设置指令", PVAVolt_Inputs);

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
                PVAVolt_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetPVAVolt.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }


        #endregion

        #region PVA路电流

        //PV电流
        private string _PVACurr;

        public string PVACurr
        {
            get { return _PVACurr; }
            set
            {
                _PVACurr = Tools.RemoveLeadingZeros(value);
                this.RaiseProperChanged(nameof(PVACurr));
            }
        }


        private bool PVACurr_IsWorking;


        //设置值
        private string _PVACurr_Inputs;

        public string PVACurr_Inputs
        {
            get { return _PVACurr_Inputs; }
            set
            {
                _PVACurr_Inputs = value;
                this.RaiseProperChanged(nameof(PVACurr_Inputs));
                Command_SetPVACurr.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand Command_SetPVACurr { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void PVACurrOperation()
        {
            try
            {
                PVACurr_IsWorking = true;
                // 禁用按钮
                Command_SetPVACurr.RaiseCanExecuteChanged();

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
                    string receive = SerialCommunicationService.SendSettingCommand("设置指令", PVACurr_Inputs);

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
                PVACurr_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetPVACurr.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }


        #endregion

        #region PVA路功率

        private string _PVAPwr;

        public string PVAPwr
        {
            get { return _PVAPwr; }
            set
            {
                _PVAPwr = Tools.RemoveLeadingZeros(value);
                this.RaiseProperChanged(nameof(PVAPwr));
            }
        }


        private bool PVAPwr_IsWorking;


        //设置值
        private string _PVAPwr_Inputs;

        public string PVAPwr_Inputs
        {
            get { return _PVAPwr_Inputs; }
            set
            {
                _PVAPwr_Inputs = value;
                this.RaiseProperChanged(nameof(PVAPwr_Inputs));
                Command_SetPVAPwr.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand Command_SetPVAPwr { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void PVAPwrOperation()
        {
            try
            {
                PVAPwr_IsWorking = true;
                // 禁用按钮
                Command_SetPVAPwr.RaiseCanExecuteChanged();

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
                    string receive = SerialCommunicationService.SendSettingCommand("设置指令", PVAPwr_Inputs);

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
                PVAPwr_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetPVAPwr.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }


        #endregion

        #region PVB路电压

        //PVB路电压
        private string _PVBVolt;

        public string PVBVolt
        {
            get { return _PVBVolt; }
            set
            {
                _PVBVolt = Tools.RemoveLeadingZeros(value);
                this.RaiseProperChanged(nameof(PVBVolt));
            }
        }


        private bool PVBVolt_IsWorking;


        //设置值
        private string _PVBVolt_Inputs;

        public string PVBVolt_Inputs
        {
            get { return _PVBVolt_Inputs; }
            set
            {
                _PVBVolt_Inputs = value;
                this.RaiseProperChanged(nameof(PVBVolt_Inputs));
                Command_SetPVBVolt.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand Command_SetPVBVolt { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void PVBVoltOperation()
        {
            try
            {
                PVBVolt_IsWorking = true;
                // 禁用按钮
                Command_SetPVBVolt.RaiseCanExecuteChanged();

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
                    string receive = SerialCommunicationService.SendSettingCommand("设置指令", PVBVolt_Inputs);

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
                PVAVolt_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetPVBVolt.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }


        #endregion

        #region PVB路电流

        //PVB路电流
        private string _PVBCurr;

        public string PVBCurr
        {
            get { return _PVBCurr; }
            set
            {
                _PVBCurr = Tools.RemoveLeadingZeros(value);
                this.RaiseProperChanged(nameof(PVBCurr));
            }
        }


        private bool PVBCurr_IsWorking;


        //设置值
        private string _PVBCurr_Inputs;
        public string PVBCurr_Inputs
        {
            get { return _PVBCurr_Inputs; }
            set
            {
                _PVBCurr_Inputs = value;
                this.RaiseProperChanged(nameof(PVBCurr_Inputs));
                Command_SetPVBCurr.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand Command_SetPVBCurr { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void PVBCurrOperation()
        {
            try
            {
                PVBCurr_IsWorking = true;
                // 禁用按钮
                Command_SetPVBCurr.RaiseCanExecuteChanged();

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
                    string receive = SerialCommunicationService.SendSettingCommand("设置指令", PVBCurr_Inputs);

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
                PVBCurr_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetPVBCurr.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }


        #endregion

        #region PVB路功率

        private string _PVBPwr;

        public string PVBPwr
        {
            get { return _PVBPwr; }
            set
            {
                _PVBPwr = Tools.RemoveLeadingZeros(value);
                this.RaiseProperChanged(nameof(PVBPwr));
            }
        }


        private bool PVBPwr_IsWorking;


        //设置值
        private string _PVBPwr_Inputs;
        public string PVBPwr_Inputs
        {
            get { return _PVBPwr_Inputs; }
            set
            {
                _PVBPwr_Inputs = value;
                this.RaiseProperChanged(nameof(PVBPwr_Inputs));
                Command_SetPVBPwr.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand Command_SetPVBPwr { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void PVBPwrOperation()
        {
            try
            {
                PVBPwr_IsWorking = true;
                // 禁用按钮
                Command_SetPVBPwr.RaiseCanExecuteChanged();

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
                    string receive = SerialCommunicationService.SendSettingCommand("设置指令", PVBPwr_Inputs);

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
                PVBPwr_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetPVBPwr.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }


        #endregion

        #region 通用方法

        private bool Validate(string value)
        {
            switch (value)
            {
                //PVA路电压    
                case "PVAVolt_Inputs":
                    return !string.IsNullOrWhiteSpace(PVAVolt_Inputs);
                //PVA路电流    
                case "PVACurr_Inputs":
                    return !string.IsNullOrWhiteSpace(PVACurr_Inputs);
                //PVA路功率  
                case "PVAPwr_Inputs":
                    return !string.IsNullOrWhiteSpace(PVAPwr_Inputs);
                //PVB路电压    
                case "PVBVolt_Inputs":
                    return !string.IsNullOrWhiteSpace(PVBVolt_Inputs);
                //PVB路电流    
                case "PVBCurr_Inputs":
                    return !string.IsNullOrWhiteSpace(PVBCurr_Inputs);
                //PVB路功率  
                case "PVBPwr_Inputs":
                    return !string.IsNullOrWhiteSpace(PVBPwr_Inputs);
                default:
                    return false;

            }
        }


        /// <summary>
        /// 对字符串进行解析
        /// </summary>
        /// <param name="value"></param>
        public void AnalysisStringToElement(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                ReceiveException("空");
                return;
            }
            if (value == "-1")
            {
                ReceiveException("CRC异常");
                AddLog(value);
                return;
            }
            string[] Values = value.Split(" ");

            try
            {
                //PVA路电压
                PVAVolt = Values[0].Substring(1, 5);
                //PVA路电流
                PVACurr = Values[1];
                //PVA路功率
                PVAPwr = Values[2];
                //PVB路电压
                PVBVolt = Values[3];
                //PVB路电流
                PVBCurr = Values[4];
                //PVB路功率
                PVBPwr = Values[5];

            }
            catch (Exception)
            {
                ReceiveException("HPV异常");
                AddLog($"{command}返回数据：{value}解析异常");
            }
        }


        /// <summary>
        /// 接收异常使用方法
        /// </summary>
        /// <param name="exceptionDescription"></param>
        private void ReceiveException(string exceptionDescription)
        {
            //PVA路电压
            PVAVolt = exceptionDescription;
            //PVA路电流
            PVACurr = exceptionDescription;
            //PVA路功率
            PVAPwr = exceptionDescription;
            //PVB路电压
            PVBVolt = exceptionDescription;
            //PVB路电流
            PVBCurr = exceptionDescription;
            //PVB路功率
            PVBPwr = exceptionDescription;

        }
        #endregion
    }
}
