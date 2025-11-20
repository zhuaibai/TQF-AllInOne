using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Convert;
using WpfApp1.ViewModels;

namespace WpfApp1.Command.BMS
{
    public class HistoryLodModel:BaseViewModel
    {
        public HistoryLodModel(short[] data, int cellNum)
        {
            
            //进行解析数据
            //历史记录数
            Time = data[1].ToString();
            //16节电芯
            for (int i = 4; i < 4+cellNum; i++)
            {
                CellVoltage += data[i].ToString() + ";";
            }
            //单体最高电压
            MaxCellVolt = data[20].ToString();
            //单体最低电压
            MinCellVolt = data[21].ToString();
            //电芯压差
            CellDiff = data[22].ToString();
            //电流
            Current = (data[23]/100.0).ToString("F2");
            //电池包总电压
            PackVoltage = (data[24]/100.0).ToString("F2");
            //OSFET温度
            MOSTemp = data[25].ToString();
            //累计充电容量
            TotalChgCap = (data[26]/100.0).ToString("F2");
            //NTC温度传感器值
            NTCTemp = ((data[27] * 1000 + data[28] * 100 + data[29] * 10 + data[30])/10000.0).ToString("F1");
            //当前电量SOC
            SOC = (data[31]/100.0).ToString("F2");
            //电池健康SOH
            SOH = (data[32]/100.0).ToString("F2");
            //剩余容量(mAh)
            RemainCap = (data[33] / 100.0).ToString("F2");
            //满充容量(mAh)
            FullCap = (data[34] / 100.0).ToString("F2");
            //充电循环次数
            CycleCount = data[35].ToString();
            //BMS告警
            BMS_Alarm = MOD_WARN_STATE_Set(ModbusRTU.GetBits(data[36]));
            //BMS保护
            BMS_Protect = MOD_PROT_STATE_Set(ModbusRTU.GetBits(data[37]));
            //BMS错误
            BMS_Error = MOD_ERROR_STATE_Set(ModbusRTU.GetBits(data[38]));
            //BMS状态
            int[] flags = ModbusRTU.GetBits((short)data[39]);
            BMS_Status = getBMS_Status(flags);
            //保护次数
            for (int i = 0; i < 16; i++)
            {
                ProtectCount    [i] = data[i + 40];
            }
            //前端芯片保护状态
            AFE_ProtStatus = getAFE_ProtStatus(ModbusRTU.GetBits(data[56]));
        }

        /// <summary>
        /// 获取BMS状态
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string getBMS_Status(int[] data)
        {
            string result = string.Empty;
            if (data[0]==1)
            {
                result += "充电MOS开 ";
            }
            if (data[0]==0)
            {
                result += "充电MOS关 ";
            }
            if (data[1] == 1)
            {
                result += "放电MOS开 ";
            }
            if (data[1] == 0)
            {
                result += "放电MOS关 ";
            }
            result = data[4] == 1 ? result + "正在充电 " : result;
            result = data[5] == 1 ? result + "正在放电 " : result;
            result = data[7] == 1 ? result + "历史低电 " : result;
            result = data[8] == 1 ? result + "充电器连接 " : result;
            result = data[9] == 1 ? result + "负载连接 " : result;
            result = data[10] == 1 ? result + "容量学习 " : result;
            return result;
        }

        /// <summary>
        /// 获取前端芯片状态
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string getAFE_ProtStatus(int[] data)
        {
            string result = string.Empty;
            
            result = data[0] == 1 ? result + "前端芯片触发保护 " : result;
            result = data[1] == 1 ? result + "前端芯片告警下拉 " : result;
            result = data[2] == 1 ? result + "前端芯片中断 " : result;
            result = data[3] == 1 ? result + "AFE过充保护 " : result;
            result = data[4] == 1 ? result + "AFE过放保护 " : result;
            result = data[5] == 1 ? result + "充电过流保护 " : result;
            result = data[6] == 1 ? result + "放电过流保护 " : result;
            result = data[7] == 1 ? result + "短路保护 " : result;
           
            return result;
        }

        private string _Time;
        /// <summary>
        /// 时间
        /// </summary>
        public string Time
        {
            get { return _Time; }
            set
            {
                _Time = value;
                this.RaiseProperChanged(nameof(Time));
            }
        }

        private string _CellVoltage=string.Empty;
        /// <summary>
        /// 单体电芯电压
        /// </summary>
        public string CellVoltage
        {
            get { return _CellVoltage; }
            set
            {
                _CellVoltage = value;
                this.RaiseProperChanged(nameof(CellVoltage));
            }
        }

        private string _MaxCellVolt;
        /// <summary>
        /// 单体最高电压
        /// </summary>
        public string MaxCellVolt
        {
            get { return _MaxCellVolt; }
            set
            {
                _MaxCellVolt = value;
                this.RaiseProperChanged(nameof(MaxCellVolt));
            }
        }

        private string _MinCellVolt;
        /// <summary>
        /// 单体最低电压
        /// </summary>
        public string MinCellVolt
        {
            get { return _MinCellVolt; }
            set
            {
                _MinCellVolt = value;
                this.RaiseProperChanged(nameof(MinCellVolt));
            }
        }

        private string _CellDiff;
        /// <summary>
        /// 电芯压差
        /// </summary>
        public string CellDiff
        {
            get { return _CellDiff; }
            set
            {
                _CellDiff = value;
                this.RaiseProperChanged(nameof(CellDiff));
            }
        }

        private string _Current;
        /// <summary>
        /// 电流(mA)
        /// </summary>
        public string Current
        {
            get { return _Current; }
            set
            {
                _Current = value;
                this.RaiseProperChanged(nameof(Current));
            }
        }

        private string _PackVoltage;
        /// <summary>
        /// 电池包总电压
        /// </summary>
        public string PackVoltage
        {
            get { return _PackVoltage; }
            set
            {
                _PackVoltage = value;
                this.RaiseProperChanged(nameof(PackVoltage));
            }
        }

        private string _MOSTemp;
        /// <summary>
        /// MOSFET温度
        /// </summary>
        public string MOSTemp
        {
            get { return _MOSTemp; }
            set
            {
                _MOSTemp = value;
                this.RaiseProperChanged(nameof(MOSTemp));
            }
        }

        private string _TotalChgCap;
        /// <summary>
        /// 累计充电容量
        /// </summary>
        public string TotalChgCap
        {
            get { return _TotalChgCap; }
            set
            {
                _TotalChgCap = value;
                this.RaiseProperChanged(nameof(TotalChgCap));
            }
        }

        private string _NTCTemp;
        /// <summary>
        /// NTC温度传感器值
        /// </summary>
        public string NTCTemp
        {
            get { return _NTCTemp; }
            set
            {
                _NTCTemp = value;
                this.RaiseProperChanged(nameof(NTCTemp));
            }
        }

        private string _SOC;
        /// <summary>
        /// 当前电量SOC
        /// </summary>
        public string SOC
        {
            get { return _SOC; }
            set
            {
                _SOC = value;
                this.RaiseProperChanged(nameof(SOC));
            }
        }

        private string _SOH;
        /// <summary>
        /// 电池健康SOH
        /// </summary>
        public string SOH
        {
            get { return _SOH; }
            set
            {
                _SOH = value;
                this.RaiseProperChanged(nameof(SOH));
            }
        }

        private string _RemainCap;
        /// <summary>
        /// 剩余容量(mAh)
        /// </summary>
        public string RemainCap
        {
            get { return _RemainCap; }
            set
            {
                _RemainCap = value;
                this.RaiseProperChanged(nameof(RemainCap));
            }
        }

        private string _FullCap;
        /// <summary>
        /// 满充容量(mAh)
        /// </summary>
        public string FullCap
        {
            get { return _FullCap; }
            set
            {
                _FullCap = value;
                this.RaiseProperChanged(nameof(FullCap));
            }
        }

        /// <summary>
        /// 循环次数
        /// </summary>
        private string _CycleCount;

        public string CycleCount
        {
            get { return _CycleCount; }
            set
            {
                _CycleCount = value;
                this.RaiseProperChanged(nameof(CycleCount));
            }
        }


        private string _BMS_Alarm;
        /// <summary>
        /// BMS告警
        /// </summary>
        public string BMS_Alarm
        {
            get { return _BMS_Alarm; }
            set
            {
                _BMS_Alarm = value;
                this.RaiseProperChanged(nameof(BMS_Alarm));
            }
        }

        private string _BMS_Protect;
        /// <summary>
        /// BMS保护
        /// </summary>
        public string BMS_Protect
        {
            get { return _BMS_Protect; }
            set
            {
                _BMS_Protect = value;
                this.RaiseProperChanged(nameof(BMS_Protect));
            }
        }

        private string _BMS_Error;
        /// <summary>
        /// BMS错误
        /// </summary>
        public string BMS_Error
        {
            get { return _BMS_Error; }
            set
            {
                _BMS_Error = value;
                this.RaiseProperChanged(nameof(BMS_Error));
            }
        }

        private string _BMS_Status;
        /// <summary>
        /// BMS状态
        /// </summary>
        public string BMS_Status
        {
            get { return _BMS_Status; }
            set
            {
                _BMS_Status = value;
                this.RaiseProperChanged(nameof(BMS_Status));
            }
        }

        private int[] _ProtectCount = new int[16];
        /// <summary>
        /// 保护次数
        /// </summary>
        public int[] ProtectCount
        {
            get { return _ProtectCount; }
            set
            {
                _ProtectCount = value;
                this.RaiseProperChanged(nameof(ProtectCount));
            }
        }

        private string _AFE_ProtStatus;
        /// <summary>
        /// 前端芯片保护状态
        /// </summary>
        public string AFE_ProtStatus
        {
            get { return _AFE_ProtStatus; }
            set
            {
                _AFE_ProtStatus = value;
                this.RaiseProperChanged(nameof(AFE_ProtStatus));
            }
        }

        /// <summary>
        /// 告警
        /// </summary>
        /// <param name="data"></param>
        public string MOD_WARN_STATE_Set(int[] data)
        {
            string MOD_WARN_STATE = "";
            if (data == null)
            {
                MOD_WARN_STATE = "和上位机通讯异常";
                return "";
            }

            //根据标志位获取告警信息并拼接
            for (int i = 0; i < data.Length; i++)
            {
                MOD_WARN_STATE = data[i] == 1 ? MOD_WARN_STATE + getWarning(i) : MOD_WARN_STATE;
            }
            return MOD_WARN_STATE;

        }

        /// <summary>
        /// 保护
        /// </summary>
        /// <param name="data"></param>
        public string MOD_PROT_STATE_Set(int[] data)
        {
            string MOD_PROT_STATE = "";
            if (data == null)
            {
                MOD_PROT_STATE = "和上位机通讯异常";
                return "";
            }

            //根据标志位获取告警信息并拼接
            for (int i = 0; i < data.Length; i++)
            {
                MOD_PROT_STATE = data[i] == 1 ? MOD_PROT_STATE + getProtect(i) : MOD_PROT_STATE;
            }
            return MOD_PROT_STATE;
        }

        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="data"></param>
        public string MOD_ERROR_STATE_Set(int[] data)
        {
            string MOD_ERROR_STATE = "";
            if (data == null)
            {
                MOD_ERROR_STATE = "和上位机通讯异常";
                return "";
            }

            //根据标志位获取告警信息并拼接
            for (int i = 0; i < data.Length; i++)
            {
                MOD_ERROR_STATE = data[i] == 1 ? MOD_ERROR_STATE + getERROR(i) : MOD_ERROR_STATE;
            }
            return MOD_ERROR_STATE;

        }

        /// <summary>
        /// 获取对应标志位的告警信息
        /// </summary>
        /// <param name="i">索引</param>
        private string getWarning(int i)
        {
            switch (i)
            {
                case 0:
                    return "单体过压 ";
                case 1:
                    return "单体欠压 ";
                case 2:
                    return "总体过压 ";
                case 3:
                    return "总体欠压 ";
                case 4:
                    return "电芯压差过大 ";
                case 5:
                    return "充电过流 ";
                case 6:
                    return "放电过流 ";
                case 7:
                    return "充电高温 ";
                case 8:
                    return "";
                case 9:
                    return "充电低温 ";
                case 10:
                    return "放电低温 ";
                case 11:
                    return "电芯温差过大 ";
                case 12:
                    return "";
                case 13:
                    return "";
                case 14:
                    return "放电高温 ";
                case 15:
                    return "低电量 ";
                default:
                    return "索引错误 ";

            }
        }

        /// <summary>
        /// 获取对应标志位的保护信息
        /// </summary>
        /// <param name="i">索引</param>
        private string getProtect(int i)
        {
            switch (i)
            {
                case 0:
                    return "单体过压 ";
                case 1:
                    return "单体欠压 ";
                case 2:
                    return "总体过压 ";
                case 3:
                    return "总体欠压 ";//
                case 4:
                    return "充电过流 ";
                case 5:
                    return "放电过流 ";
                case 6:
                    return "";
                case 7:
                    return "满充保护 ";
                case 8:
                    return "";
                case 9:
                    return "";
                case 10:
                    return "充电高温 ";//
                case 11:
                    return "充电低温 ";
                case 12:
                    return "放电低温 ";
                case 13:
                    return "";
                case 14:
                    return "放电高温 ";
                case 15:
                    return "";
                default:
                    return "";

            }
        }

        /// <summary>
        /// 获取对应标志位的错误信息
        /// </summary>
        /// <param name="i">索引</param>
        private string getERROR(int i)
        {
            switch (i)
            {
                case 0:
                    return "充电MOS故障 ";
                case 1:
                    return "放电MOS故障 ";
                case 2:
                    return "";
                case 3:
                    return "";
                case 4:
                    return "电芯失效故障 ";
                case 5:
                    return "前端芯片通信故障 ";
                case 6:
                    return "电芯温差过大故障 ";
                case 7:
                    return "前端芯片电流传感器故障 ";
                case 8:
                    return "前端芯片电压传感器故障 ";
                case 9:
                    return "NTC故障 ";
                case 10:
                    return "FLASH读写故障 ";
                case 11:
                    return "";
                case 12:
                    return "";
                case 13:
                    return "";
                case 14:
                    return "";
                case 15:
                    return "";
                default:
                    return "";

            }
        }

    }
}
