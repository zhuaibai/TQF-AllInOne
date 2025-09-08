using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp1.ViewModels;

namespace WpfApp1.Command.GB_General
{
    public class HSTS2_HPVINV08_ViewModel:BaseViewModel
    {
        //指令
        private string command = "HSTS2\r"; //返回40字节 (00 B0010001 10701000 00000000000000000

        public string Command { get { return command; } }

        public string MachineType;

        ManualResetEventSlim _pauseEvent;//线程的开启、暂停 
        SemaphoreSlim _semaphore;        //异步竞争，资源锁
        Action<string> AddLog;           //添加日志委托
        Action<string> UpdateState;      //更新状态日志

        public HSTS2_HPVINV08_ViewModel(ManualResetEventSlim pauseEvent, SemaphoreSlim semaphore, Action<string> addLog, Action<string> _updateState)
        {
            _pauseEvent = pauseEvent;
            _semaphore = semaphore;
            AddLog = addLog;
            UpdateState = _updateState;
        }

        #region PV输入状态

        private Visibility _PVStatus;

        public Visibility PVStatus
        {
            get { return _PVStatus; }
            set
            {
                _PVStatus = value;
                this.RaiseProperChanged(nameof(PVStatus));
            }
        }

        #endregion

        #region 市电输入状态

        //流入逆变器
        private Visibility _ACStatusIn;

        public Visibility ACStatusIn
        {
            get { return _ACStatusIn; }
            set
            {
                _ACStatusIn = value;
                this.RaiseProperChanged(nameof(ACStatusIn));
            }
        }

        //流出逆变器
        private Visibility _ACStatusOut;

        public Visibility ACStatusOut
        {
            get { return _ACStatusOut; }
            set
            {
                _ACStatusOut = value;
                this.RaiseProperChanged(nameof(ACStatusOut));
            }
        }

        #endregion

        #region 逆变输出

        private Visibility _InvOutput;

        public Visibility InvOutput
        {
            get { return _InvOutput; }
            set
            {
                _InvOutput = value;
                this.RaiseProperChanged(nameof(InvOutput));
            }
        }

        #endregion

        #region 并网标志

        private Visibility _GridStatus;

        public Visibility GridStatus
        {
            get { return _GridStatus; }
            set
            {
                _GridStatus = value;
                this.RaiseProperChanged(nameof(GridStatus));
            }
        }

        #endregion

        #region 能量供给负载

        private Visibility _EnergyToLoad;

        public Visibility EnergyToLoad
        {
            get { return _EnergyToLoad; }
            set
            {
                _EnergyToLoad = value;
                this.RaiseProperChanged(nameof(EnergyToLoad));
            }
        }

        #endregion

        #region 电池充电标志

        private Visibility _BattChgFlag;

        public Visibility BattChgFlag
        {
            get { return _BattChgFlag; }
            set
            {
                _BattChgFlag = value;
                this.RaiseProperChanged(nameof(BattChgFlag));
            }
        }

        #endregion

        #region 电池放电标志

        private Visibility _BattDisFlag;

        public Visibility BattDisFlag
        {
            get { return _BattDisFlag; }
            set
            {
                _BattDisFlag = value;
                this.RaiseProperChanged(nameof(BattDisFlag));
            }
        }

        #endregion

        #region 逆变器工作状态

        private string _InvStatus;

        public string InvStatus
        {
            get { return _InvStatus; }
            set
            {
                if (value == "1")
                {
                    _InvStatus = App.GetText("开机");
                }else
                _InvStatus = _InvStatus = App.GetText("关机");
                this.RaiseProperChanged(nameof(InvStatus));
            }
        }


        #endregion

        #region PV电压状态
        private string _PVVoltStatus;

        public string PVVoltStatus
        {
            get { return _PVVoltStatus; }
            set
            {
                if (value == "1")
                {
                    _PVVoltStatus = App.GetText("正常");
                }else
                _PVVoltStatus = App.GetText("异常");
                this.RaiseProperChanged(nameof(PVVoltStatus));
            }
        }


        #endregion

        #region 逆变桥状态

        private string _InvBridgeStatus;

        public string InvBridgeStatus
        {
            get { return _InvBridgeStatus; }
            set
            {
                _InvBridgeStatus = value;
                this.RaiseProperChanged(nameof(InvBridgeStatus));
            }
        }

        #endregion

        #region MPPT状态

        private string _MPPTStatus;

        public string MPPTStatus
        {
            get { return _MPPTStatus; }
            set
            {
                _MPPTStatus = value;
                this.RaiseProperChanged(nameof(MPPTStatus));
            }
        }

        #endregion

        #region 锁相环状态

        private string _PLLStatus;

        public string PLLStatus
        {
            get { return _PLLStatus; }
            set
            {
                _PLLStatus = value;
                this.RaiseProperChanged(nameof(PLLStatus));
            }
        }


        #endregion


        /// <summary>
        /// 对字符串进行解析
        /// </summary>
        /// <param name="value"></param>
        public void AnalyseStringToElement(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                ReceiveException("null");
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
                //PV输入状态
                PVStatus = Values[1].Substring(1, 1)=="1" ? Visibility.Visible:Visibility.Hidden;
                //并网标志
                GridStatus = Values[1].Substring(4, 1) == "1" ? Visibility.Visible : Visibility.Hidden;
                //市电输入状态
                if (Values[1].Substring(4, 1) == "0")
                {
                    ACStatusIn = Visibility.Hidden;
                    ACStatusOut = Visibility.Hidden;
                }else if(Values[1].Substring(2, 1) == "1")
                {
                    ACStatusOut = Visibility.Visible;
                    ACStatusIn = Visibility.Hidden;
                }
                else
                {
                    ACStatusOut = Visibility.Hidden;
                    ACStatusIn = Visibility.Visible;
                }
                //逆变输出
                InvOutput = Values[1].Substring(3, 1) == "1" ? Visibility.Visible : Visibility.Hidden;
                //能量供给负载
                EnergyToLoad = Values[1].Substring(5, 1) == "1" ? Visibility.Visible : Visibility.Hidden;
                //电池充电标志
                BattChgFlag = Values[1].Substring(6, 1) == "1" ? Visibility.Visible : Visibility.Hidden;
                //电池放电标志
                BattDisFlag = Values[1].Substring(7, 1) == "1" ? Visibility.Visible : Visibility.Hidden;
                //逆变器工作状态
                InvStatus = Values[2].Substring(0, 1);
                //PV电压状态
                PVVoltStatus = Values[2].Substring(1, 1);
                //逆变桥状态
                InvBridgeStatus = Values[2].Substring(2, 1);
                //MPPT状态
                MPPTStatus = Values[2].Substring(3, 1);
                //锁相环状态
                PLLStatus = Values[2].Substring(4, 1);
                
            }
            catch (Exception ex)
            {
                //异常
                ReceiveException("HEEP1_EX");
                AddLog($"{command}返回数据：{value}解析异常");
            }

        }

        /// <summary>
        /// 接收异常使用方法
        /// </summary>
        /// <param name="exceptionDescription"></param>
        private void ReceiveException(string exceptionDescription)
        {
            //PV输入状态
            PVStatus = Visibility.Hidden;
            //市电输入状态
            ACStatusIn = Visibility.Hidden;
            ACStatusOut = Visibility.Hidden;
            //逆变输出
            InvOutput = Visibility.Hidden;
            //并网标志
            GridStatus = Visibility.Hidden;
            //能量供给负载
            EnergyToLoad = Visibility.Hidden;
            //电池充电标志
            BattChgFlag = Visibility.Hidden;
            //PV馈能优先级
            BattDisFlag = Visibility.Hidden;
            //逆变器工作状态
            InvStatus = exceptionDescription;
            //PV电压状态
            PVVoltStatus = exceptionDescription;
            //逆变桥状态
            InvBridgeStatus = exceptionDescription;
            //MPPT状态
            MPPTStatus = exceptionDescription;
            //锁相环状态
            PLLStatus = exceptionDescription;

        }
    }
}
