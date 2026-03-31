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
    public class HBAT_CG_ViewModel:BaseViewModel
    {
        //指令
        private string command = "HBAT\r";
        public string Command { get { return command; } }

        ManualResetEventSlim _pauseEvent;//线程的开启、暂停
        SemaphoreSlim _semaphore;        //异步竞争，资源锁
        Action<string> AddLog;           //添加日志委托
        Action<string> UpdateState;      //更新状态日志

        public HBAT_CG_ViewModel(ManualResetEventSlim pauseEvent, SemaphoreSlim semaphore, Action<string> addLog, Action<string> updateState)
        {
            _pauseEvent = pauseEvent;
            _semaphore = semaphore;
            AddLog = addLog;
            UpdateState = updateState;

            #region 初始化指令


            //母线电压
            Command_SetBusVolt = new RelayCommand(
                execute: () => BusVoltOperation(),
                canExecute: () => Validate(nameof(BusVolt_Inputs)) && !BusVolt_IsWorking // 增加处理状态检查
            );
            //NBUS电压
            Command_SetNBusVolt = new RelayCommand(
            execute: () => NBusVoltOperation(),
            canExecute: () => Validate(nameof(NBusVolt_Inputs)) && !NBusVolt_IsWorking // 增加处理状态检查
            );
            #endregion
        }

        #region 电池节数

        private string _BattCells;

        public string BattCells
        {
            get { return _BattCells; }
            set
            {
                _BattCells = Tools.RemoveLeadingZeros(value);
                this.RaiseProperChanged(nameof(BattCells));
            }
        }


        private bool BattCells_IsWorking;


        //设置值
        private string _BattCells_Inputs;

        public string BattCells_Inputs
        {
            get { return _BattCells_Inputs; }
            set
            {
                _BattCells_Inputs = value;
                this.RaiseProperChanged(nameof(BattCells_Inputs));
                Command_SetBattCells.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand Command_SetBattCells { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void BattCellsOperation()
        {
            try
            {
                BattCells_IsWorking = true;
                // 禁用按钮
                Command_SetBattCells.RaiseCanExecuteChanged();

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
                    string receive = SerialCommunicationService.SendSettingCommand("设置指令", BattCells_Inputs);

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
                BattCells_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetBattCells.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }


        #endregion

        #region 电池电压

        private string _BattVolt;

        public string BattVolt
        {
            get { return _BattVolt; }
            set
            {
                _BattVolt = Tools.RemoveLeadingZeros(value);
                this.RaiseProperChanged(nameof(BattVolt));
            }
        }
        #endregion

        #region 电池容量

        private string _BattCapacity;

        public string BattCapacity
        {
            get { return _BattCapacity; }
            set
            {
                _BattCapacity = Tools.RemoveLeadingZeros(value);
                this.RaiseProperChanged(nameof(BattCapacity));
            }
        }

        #endregion

        #region 母线电压
        private string _BusVolt;

        public string BusVolt
        {
            get { return _BusVolt; }
            set
            {
                _BusVolt = Tools.RemoveLeadingZeros(value) + "V";
                this.RaiseProperChanged(nameof(BusVolt));
            }
        }


        private bool BusVolt_IsWorking;


        //设置值
        private string _BusVolt_Inputs;

        public string BusVolt_Inputs
        {
            get { return _BusVolt_Inputs; }
            set
            {
                _BusVolt_Inputs = value;
                this.RaiseProperChanged(nameof(BusVolt_Inputs));
                Command_SetBusVolt.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand Command_SetBusVolt { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void BusVoltOperation()
        {
            try
            {
                BusVolt_IsWorking = true;
                // 禁用按钮
                Command_SetBusVolt.RaiseCanExecuteChanged();

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
                    string receive = SerialCommunicationService.SendSettingCommand("设置指令", BusVolt_Inputs);

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
                BusVolt_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetBusVolt.RaiseCanExecuteChanged();
                // 确保释放锁
                _semaphore.Release();
                UpdateState("设置指令已经执行完");
            }
        }

        #endregion

        #region N母线电压
        private string _NBusVolt;

        public string NBusVolt
        {
            get { return _NBusVolt; }
            set
            {
                _NBusVolt = Tools.RemoveLeadingZeros(value) + "V";
                this.RaiseProperChanged(nameof(NBusVolt));
            }
        }


        private bool NBusVolt_IsWorking;


        //设置值
        private string _NBusVolt_Inputs;

        public string NBusVolt_Inputs
        {
            get { return _NBusVolt_Inputs; }
            set
            {
                _NBusVolt_Inputs = value;
                this.RaiseProperChanged(nameof(NBusVolt_Inputs));
                Command_SetNBusVolt.RaiseCanExecuteChanged();
            }
        }


        public RelayCommand Command_SetNBusVolt { get; }

        /// <summary>
        /// 点击设置
        /// </summary>
        private async void NBusVoltOperation()
        {
            try
            {
                NBusVolt_IsWorking = true;
                // 禁用按钮
                Command_SetNBusVolt.RaiseCanExecuteChanged();

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
                    string receive = SerialCommunicationService.SendSettingCommand("设置指令", NBusVolt_Inputs);

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
                NBusVolt_IsWorking = false;
                //Status = "就绪";
                // 重新启用按钮
                Command_SetNBusVolt.RaiseCanExecuteChanged();
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
                //母线电压       
                case "BusVolt_Inputs":
                    return !string.IsNullOrWhiteSpace(BusVolt_Inputs);
                case "NBusVolt_Inputs":
                    return !string.IsNullOrWhiteSpace(NBusVolt_Inputs);
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
                //电池节数
                BattCells = Values[0].Substring(1,2);
                //电池电压
                BattVolt = Values[1];
                //电池容量
                BattCapacity = Values[2];
                //母线电压
                BusVolt = Values[3];
                //NBUS电压
                NBusVolt = Values[4];



            }
            catch (Exception)
            {
                ReceiveException("HBAT解析异常");
                AddLog($"{command}返回数据：{value}解析异常");
            }
        }

        /// <summary>
        /// 接收异常使用方法
        /// </summary>
        /// <param name="exceptionDescription"></param>
        private void ReceiveException(string exceptionDescription)
        {
            //电池节数
            BattCells = exceptionDescription;
            //电池电压
            BattVolt = exceptionDescription;
            //电池容量
            BattCapacity = exceptionDescription;
            //母线电压
            BusVolt = exceptionDescription;
            //NBUS电压
            NBusVolt = exceptionDescription;
        }
        #endregion
    }
}
