using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Convert;
using WpfApp1.ViewModels;

namespace WpfApp1.Command.BMS
{
    public class ModbusAddr : BaseViewModel
    {

        public ModbusAddr() { MOD_INST_STATE = new int[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; }

        /// <summary>
        /// 16个电池电压的赋值
        /// </summary>
        /// <param name="data"></param>
        public void MOD_CELL1_VOL_1_16(short[] data)
        {
            if (data == null || data.Length == 0) return;
            MOD_CELL1_VOL = data[0];                 //2 电池电压
            MOD_CELL2_VOL = data[1];                 //3
            MOD_CELL3_VOL = data[2];                 //4
            MOD_CELL4_VOL = data[3];                 //5
            MOD_CELL5_VOL = data[4];                 //6
            MOD_CELL6_VOL = data[5];                 //7
            MOD_CELL7_VOL = data[6];                 //8
            MOD_CELL8_VOL = data[7];                 //9
            MOD_CELL9_VOL = data[8];                 //10
            MOD_CELL10_VOL = data[9];                //11
            MOD_CELL11_VOL = data[10];               //12
            MOD_CELL12_VOL = data[11];               //13
            MOD_CELL13_VOL = data[12];               //14
            MOD_CELL14_VOL = data[13];               //15
            MOD_CELL15_VOL = data[14];               //16
            MOD_CELL16_VOL = data[15];               //17
        }




        /// <summary>
        /// 16位进行赋值(状态)
        /// </summary>
        public void MOD_INST_STATE_Set(int[] data)
        {
            if (data == null)
            {
                return;
            }
            MOD_INST_STATE = data;
        }

        /// <summary>
        /// 告警
        /// </summary>
        /// <param name="data"></param>
        public  void MOD_WARN_STATE_Set(int[] data)
        {
            MOD_WARN_STATE = "";
            if (data == null)
            {
                MOD_WARN_STATE = "和上位机通讯异常";
                return;
            }

            //根据标志位获取告警信息并拼接
            for (int i = 0; i < data.Length; i++)
            {
                MOD_WARN_STATE = data[i] == 1 ? MOD_WARN_STATE + getWarning(i) : MOD_WARN_STATE;
            }

        }

        /// <summary>
        /// 告警
        /// </summary>
        /// <param name="data"></param>
        public void MOD_WARN_STATE_Set_BMS01(int[] data)
        {
            MOD_WARN_STATE = "";
            if (data == null)
            {
                MOD_WARN_STATE = "和上位机通讯异常";
                return;
            }

            //根据标志位获取告警信息并拼接
            for (int i = 0; i < data.Length; i++)
            {
                MOD_WARN_STATE = data[i] == 1 ? MOD_WARN_STATE + getWarningByBMS01(i) : MOD_WARN_STATE;
            }

        }


        /// <summary>
        /// 保护
        /// </summary>
        /// <param name="data"></param>
        public void MOD_PROT_STATE_Set(int[] data)
        {
            MOD_PROT_STATE = "";
            if (data == null)
            {
                MOD_PROT_STATE = "和上位机通讯异常";
                return;
            }

            //根据标志位获取告警信息并拼接
            for (int i = 0; i < data.Length; i++)
            {
                MOD_PROT_STATE = data[i] == 1 ? MOD_PROT_STATE + getProtect(i) : MOD_PROT_STATE;
            }
        }

        /// <summary>
        /// 保护
        /// </summary>
        /// <param name="data"></param>
        public void MOD_PROT_STATE_Set_BMS01(int[] data)
        {
            MOD_PROT_STATE = "";
            if (data == null)
            {
                MOD_PROT_STATE = "和上位机通讯异常";
                return;
            }

            //根据标志位获取告警信息并拼接
            for (int i = 0; i < data.Length; i++)
            {
                MOD_PROT_STATE = data[i] == 1 ? MOD_PROT_STATE + getProtectByBMS01(i) : MOD_PROT_STATE;
            }
        }

        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="data"></param>
        public void MOD_ERROR_STATE_Set(int[] data)
        {
            MOD_ERROR_STATE = "";
            if (data == null)
            {
                MOD_ERROR_STATE = "和上位机通讯异常";
                return;
            }

            //根据标志位获取告警信息并拼接
            for (int i = 0; i < data.Length; i++)
            {
                MOD_ERROR_STATE = data[i] == 1 ? MOD_ERROR_STATE + getERROR(i) : MOD_ERROR_STATE;
            }

        }

        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="data"></param>
        public void MOD_ERROR_STATE_Set_BMS01(int[] data)
        {
            MOD_ERROR_STATE = "";
            if (data == null)
            {
                MOD_ERROR_STATE = "和上位机通讯异常";
                return;
            }

            //根据标志位获取告警信息并拼接
            for (int i = 0; i < data.Length; i++)
            {
                MOD_ERROR_STATE = data[i] == 1 ? MOD_ERROR_STATE + getERRORByBMS01(i) : MOD_ERROR_STATE;
            }

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
        /// 获取对应标志位的告警信息
        /// </summary>
        /// <param name="i">索引</param>
        private string getWarningByBMS01(int i)
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
                    return "充电高温1 ";
                case 8:
                    return "充电高温2";
                case 9:
                    return "充电低温 ";
                case 10:
                    return "放电低温 ";
                case 11:
                    return "电芯温差过大 ";
                case 12:
                    return "MOS高温";
                case 13:
                    return "环境高温";
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
        /// 获取对应标志位的保护信息
        /// </summary>
        /// <param name="i">索引</param>
        private string getProtectByBMS01(int i)
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
                    return "满充保护 ";
                case 5:
                    return "充电过流 ";
                case 6:
                    return "放电过流 ";
                case 7:
                    return "短路保护 ";
                case 8:
                    return "充电高温1 ";
                case 9:
                    return "充电高温2 ";
                case 10:
                    return "充电低温 ";//
                case 11:
                    return "放电低温 ";
                case 12:
                    return "MOS高温 ";
                case 13:
                    return "请立即充电";
                case 14:
                    return "放电高温 ";
                case 15:
                    return "电芯压差过大 ";
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

        /// <summary>
        /// 获取对应标志位的错误信息
        /// </summary>
        /// <param name="i">索引</param>
        private string getERRORByBMS01(int i)
        {
            switch (i)
            {
                case 0:
                    return "充电MOS故障 ";
                case 1:
                    return "放电MOS故障 ";
                case 2:
                    return "限流板故障 ";
                case 3:
                    return "ADC采集故障 ";
                case 4:
                    return "电芯失效故障 ";
                case 5:
                    return "前端芯片通信故障 ";
                case 6:
                    return "电池电压故障 ";
                case 7:
                    return "预充电阻故障 ";
                case 8:
                    return "3V3辅源故障 ";
                case 9:
                    return "NTC故障 ";
                case 10:
                    return "脱扣器故障 ";
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

        /// <summary>
        /// 系统信息赋值
        /// </summary>
        /// <param name="data"></param>
        public void SystemInfoSet(short[] data)
        {
            if (data == null)
            {
                return;
            }
            MOD_SP_SOFTVER = (data[0] + (float)data[1]/100).ToString("F2");         //软件版本
            MOD_SP_HARDVER = (data[2] + (float)data[3]/100).ToString("F2");         //硬件版本
            MOD_SP_SN = (int)data[4] << 16 | (int)data[5];              //序列号
            MOD_SP_FACTDATE = (int)data[6] << 16 | (int)data[7];        //出厂日期 
            MOD_SP_FANUFACTURER = data[8] << 8 | data[9];    //厂商
            MOD_SP_DEVICESN = data[10] << 8 | data[11];      //设备型号
            MOD_SP_CELLSN = data[12] << 8 | data[13];        //电芯型号
        }



        /// <summary>
        /// 总览
        /// </summary>
        /// <param name="data"></param>
        public void OverViewSet(short[] data)
        {
            if (data == null)
            {
                return;
            }
            MOD_MAXCELL_VOL = data[0];    //最大电芯电压
            MOD_MINCELL_VOL = data[1];    //最小电芯电压
            MOD_CELL_VOLDIFF = data[2];   //最大电芯压差
            MOD_AFECOL_PACKVOL = data[3]; //电压
            MOD_AFECOL_CUR = data[5];     //电流
            MOD_GROUD1_TEMP = data[7];    //电芯温度1
            MOD_GROUD2_TEMP = data[8];    //电芯温度2
            MOD_GROUD3_TEMP = data[9];    //电芯温度3
            MOD_GROUD4_TEMP = data[10];    //电芯温度4
            MOD_MOS_TEMP = data[11];       //MOS温度
            MOD_ENV_TEMP = data[12];       //环境温度
            MOD_SOC = data[13];           //SOC
            SOC = MOD_SOC / 100;          //整数显示百分比
            MOD_SOH = data[14];           //SOH
            MOD_RES_CAP = data[15];       //剩余容量
            MOD_FULL_CAP = data[16];      //充满容量
            MOD_CYCLECNT = data[17];      //循环次数
        }

        private int _SOC;

        public int SOC
        {
            get { return _SOC; }
            set
            {
                _SOC = value;
                this.RaiseProperChanged(nameof(SOC));
            }
        }








        /************************************** 模拟量参数读取区(Read Only) **************************************/


        private int[] AFE_protect;

        public int[] AFE_Protect
        {
            get { return AFE_protect; }
            set
            {
                AFE_protect = value;
                this.RaiseProperChanged(nameof(AFE_Protect));
            }
        }



        private int _MOD_LOCAL_ADDR;
        public int MOD_LOCAL_ADDR
        {
            get { return _MOD_LOCAL_ADDR; }
            set
            {
                _MOD_LOCAL_ADDR = value;
                this.RaiseProperChanged(nameof(MOD_LOCAL_ADDR));

            }
        }

        //2 电池电压
        private int _MOD_CELL1_VOL;
        public int MOD_CELL1_VOL
        {
            get { return _MOD_CELL1_VOL; }
            set
            {
                _MOD_CELL1_VOL = value;
                this.RaiseProperChanged(nameof(MOD_CELL1_VOL));
            }
        }

        private int _MOD_CELL2_VOL;
        public int MOD_CELL2_VOL
        {
            get { return _MOD_CELL2_VOL; }
            set
            {
                _MOD_CELL2_VOL = value;
                this.RaiseProperChanged(nameof(MOD_CELL2_VOL));
            }
        }

        private int _MOD_CELL3_VOL;
        public int MOD_CELL3_VOL
        {
            get { return _MOD_CELL3_VOL; }
            set
            {
                _MOD_CELL3_VOL = value;
                this.RaiseProperChanged(nameof(MOD_CELL3_VOL));
            }
        }

        private int _MOD_CELL4_VOL;
        public int MOD_CELL4_VOL
        {
            get { return _MOD_CELL4_VOL; }
            set
            {
                _MOD_CELL4_VOL = value;
                this.RaiseProperChanged(nameof(MOD_CELL4_VOL));
            }
        }

        private int _MOD_CELL5_VOL;
        public int MOD_CELL5_VOL
        {
            get { return _MOD_CELL5_VOL; }
            set
            {
                _MOD_CELL5_VOL = value;
                this.RaiseProperChanged(nameof(MOD_CELL5_VOL));
            }
        }

        private int _MOD_CELL6_VOL;
        public int MOD_CELL6_VOL
        {
            get { return _MOD_CELL6_VOL; }
            set
            {
                _MOD_CELL6_VOL = value;
                this.RaiseProperChanged(nameof(MOD_CELL6_VOL));
            }
        }

        private int _MOD_CELL7_VOL;
        public int MOD_CELL7_VOL
        {
            get { return _MOD_CELL7_VOL; }
            set
            {
                _MOD_CELL7_VOL = value;
                this.RaiseProperChanged(nameof(MOD_CELL7_VOL));
            }
        }

        private int _MOD_CELL8_VOL;
        public int MOD_CELL8_VOL
        {
            get { return _MOD_CELL8_VOL; }
            set
            {
                _MOD_CELL8_VOL = value;
                this.RaiseProperChanged(nameof(MOD_CELL8_VOL));
            }
        }

        private int _MOD_CELL9_VOL;
        public int MOD_CELL9_VOL
        {
            get { return _MOD_CELL9_VOL; }
            set
            {
                _MOD_CELL9_VOL = value;
                this.RaiseProperChanged(nameof(MOD_CELL9_VOL));
            }
        }

        private int _MOD_CELL10_VOL;
        public int MOD_CELL10_VOL
        {
            get { return _MOD_CELL10_VOL; }
            set
            {
                _MOD_CELL10_VOL = value;
                this.RaiseProperChanged(nameof(MOD_CELL10_VOL));
            }
        }

        private int _MOD_CELL11_VOL;
        public int MOD_CELL11_VOL
        {
            get { return _MOD_CELL11_VOL; }
            set
            {
                _MOD_CELL11_VOL = value;
                this.RaiseProperChanged(nameof(MOD_CELL11_VOL));
            }
        }

        private int _MOD_CELL12_VOL;
        public int MOD_CELL12_VOL
        {
            get { return _MOD_CELL12_VOL; }
            set
            {
                _MOD_CELL12_VOL = value;
                this.RaiseProperChanged(nameof(MOD_CELL12_VOL));
            }
        }

        private int _MOD_CELL13_VOL;
        public int MOD_CELL13_VOL
        {
            get { return _MOD_CELL13_VOL; }
            set
            {
                _MOD_CELL13_VOL = value;
                this.RaiseProperChanged(nameof(MOD_CELL13_VOL));
            }
        }

        private int _MOD_CELL14_VOL;
        public int MOD_CELL14_VOL
        {
            get { return _MOD_CELL14_VOL; }
            set
            {
                _MOD_CELL14_VOL = value;
                this.RaiseProperChanged(nameof(MOD_CELL14_VOL));
            }
        }

        private int _MOD_CELL15_VOL;
        public int MOD_CELL15_VOL
        {
            get { return _MOD_CELL15_VOL; }
            set
            {
                _MOD_CELL15_VOL = value;
                this.RaiseProperChanged(nameof(MOD_CELL15_VOL));
            }
        }

        private int _MOD_CELL16_VOL;
        public int MOD_CELL16_VOL
        {
            get { return _MOD_CELL16_VOL; }
            set
            {
                _MOD_CELL16_VOL = value;
                this.RaiseProperChanged(nameof(MOD_CELL16_VOL));
            }
        }

        private int _MOD_MAXCELL_VOL;
        public int MOD_MAXCELL_VOL
        {
            get { return _MOD_MAXCELL_VOL; }
            set
            {
                _MOD_MAXCELL_VOL = value;
                this.RaiseProperChanged(nameof(MOD_MAXCELL_VOL));
            }
        }

        private int _MOD_MINCELL_VOL;
        public int MOD_MINCELL_VOL
        {
            get { return _MOD_MINCELL_VOL; }
            set
            {
                _MOD_MINCELL_VOL = value;
                this.RaiseProperChanged(nameof(MOD_MINCELL_VOL));
            }
        }

        private int _MOD_CELL_VOLDIFF;
        public int MOD_CELL_VOLDIFF
        {
            get { return _MOD_CELL_VOLDIFF; }
            set
            {
                _MOD_CELL_VOLDIFF = value;
                this.RaiseProperChanged(nameof(MOD_CELL_VOLDIFF));
            }
        }

        private int _MOD_AFECOL_PACKVOL;
        public int MOD_AFECOL_PACKVOL
        {
            get { return _MOD_AFECOL_PACKVOL; }
            set
            {
                _MOD_AFECOL_PACKVOL = value;
                this.RaiseProperChanged(nameof(MOD_AFECOL_PACKVOL));
            }
        }

        private int _MOD_MCUCOL_PACKVOL;
        public int MOD_MCUCOL_PACKVOL
        {
            get { return _MOD_MCUCOL_PACKVOL; }
            set
            {
                _MOD_MCUCOL_PACKVOL = value;
                this.RaiseProperChanged(nameof(MOD_MCUCOL_PACKVOL));
            }
        }

        private int _MOD_AFECOL_CUR;
        public int MOD_AFECOL_CUR
        {
            get { return _MOD_AFECOL_CUR; }
            set
            {
                _MOD_AFECOL_CUR = value;
                this.RaiseProperChanged(nameof(MOD_AFECOL_CUR));
            }
        }

        private int _MOD_MCUCOL_CUR;
        public int MOD_MCUCOL_CUR
        {
            get { return _MOD_MCUCOL_CUR; }
            set
            {
                _MOD_MCUCOL_CUR = value;
                this.RaiseProperChanged(nameof(MOD_MCUCOL_CUR));
            }
        }

        private int _MOD_GROUD1_TEMP;
        public int MOD_GROUD1_TEMP
        {
            get { return _MOD_GROUD1_TEMP; }
            set
            {
                _MOD_GROUD1_TEMP = value;
                this.RaiseProperChanged(nameof(MOD_GROUD1_TEMP));
            }
        }

        private int _MOD_GROUD2_TEMP;
        public int MOD_GROUD2_TEMP
        {
            get { return _MOD_GROUD2_TEMP; }
            set
            {
                _MOD_GROUD2_TEMP = value;
                this.RaiseProperChanged(nameof(MOD_GROUD2_TEMP));
            }
        }

        private int _MOD_GROUD3_TEMP;
        public int MOD_GROUD3_TEMP
        {
            get { return _MOD_GROUD3_TEMP; }
            set
            {
                _MOD_GROUD3_TEMP = value;
                this.RaiseProperChanged(nameof(MOD_GROUD3_TEMP));
            }
        }

        private int _MOD_GROUD4_TEMP;
        public int MOD_GROUD4_TEMP
        {
            get { return _MOD_GROUD4_TEMP; }
            set
            {
                _MOD_GROUD4_TEMP = value;
                this.RaiseProperChanged(nameof(MOD_GROUD4_TEMP));
            }
        }

        private int _MOD_MOS_TEMP;
        public int MOD_MOS_TEMP
        {
            get { return _MOD_MOS_TEMP; }
            set
            {
                _MOD_MOS_TEMP = value;
                this.RaiseProperChanged(nameof(MOD_MOS_TEMP));
            }
        }

        private int _MOD_ENV_TEMP;
        public int MOD_ENV_TEMP
        {
            get { return _MOD_ENV_TEMP; }
            set
            {
                _MOD_ENV_TEMP = value;
                this.RaiseProperChanged(nameof(MOD_ENV_TEMP));
            }
        }

        private int _MOD_SOC;               //31 SOC
        public int MOD_SOC
        {
            get { return _MOD_SOC; }
            set
            {
                _MOD_SOC = value;
                this.RaiseProperChanged(nameof(MOD_SOC));
            }
        }

        private int _MOD_SOH;               //32 SOC
        public int MOD_SOH
        {
            get { return _MOD_SOH; }
            set
            {
                _MOD_SOH = value;
                this.RaiseProperChanged(nameof(MOD_SOH));
            }
        }

        private int _MOD_RES_CAP;           //33 剩余容量
        public int MOD_RES_CAP
        {
            get { return _MOD_RES_CAP; }
            set
            {
                _MOD_RES_CAP = value;
                this.RaiseProperChanged(nameof(MOD_RES_CAP));
            }
        }

        private int _MOD_FULL_CAP;          //34 充满容量
        public int MOD_FULL_CAP
        {
            get { return _MOD_FULL_CAP; }
            set
            {
                _MOD_FULL_CAP = value;
                this.RaiseProperChanged(nameof(MOD_FULL_CAP));
            }
        }

        private int _MOD_CYCLECNT;          //35 循环次数
        public int MOD_CYCLECNT
        {
            get { return _MOD_CYCLECNT; }
            set
            {
                _MOD_CYCLECNT = value;
                this.RaiseProperChanged(nameof(MOD_CYCLECNT));
            }
        }

        private int _MOD_BASIC_MSG_RESERVED1;   //36
        public int MOD_BASIC_MSG_RESERVED1
        {
            get { return _MOD_BASIC_MSG_RESERVED1; }
            set
            {
                _MOD_BASIC_MSG_RESERVED1 = value;
                this.RaiseProperChanged(nameof(MOD_BASIC_MSG_RESERVED1));
            }
        }

        private int _MOD_BASIC_MSG_RESERVED2;   //37
        public int MOD_BASIC_MSG_RESERVED2
        {
            get { return _MOD_BASIC_MSG_RESERVED2; }
            set
            {
                _MOD_BASIC_MSG_RESERVED2 = value;
                this.RaiseProperChanged(nameof(MOD_BASIC_MSG_RESERVED2));
            }
        }

        private int _MOD_BASIC_MSG_RESERVED3;   //38
        public int MOD_BASIC_MSG_RESERVED3
        {
            get { return _MOD_BASIC_MSG_RESERVED3; }
            set
            {
                _MOD_BASIC_MSG_RESERVED3 = value;
                this.RaiseProperChanged(nameof(MOD_BASIC_MSG_RESERVED3));
            }
        }

        private int _MOD_BASIC_MSG_RESERVED4;   //39
        public int MOD_BASIC_MSG_RESERVED4
        {
            get { return _MOD_BASIC_MSG_RESERVED4; }
            set
            {
                _MOD_BASIC_MSG_RESERVED4 = value;
                this.RaiseProperChanged(nameof(MOD_BASIC_MSG_RESERVED4));
            }
        }

        private int _MOD_BASIC_MSG_RESERVED5;   //40
        public int MOD_BASIC_MSG_RESERVED5
        {
            get { return _MOD_BASIC_MSG_RESERVED5; }
            set
            {
                _MOD_BASIC_MSG_RESERVED5 = value;
                this.RaiseProperChanged(nameof(MOD_BASIC_MSG_RESERVED5));
            }
        }

        private int _MOD_BASIC_MSG_RESERVED6;   //41
        public int MOD_BASIC_MSG_RESERVED6
        {
            get { return _MOD_BASIC_MSG_RESERVED6; }
            set
            {
                _MOD_BASIC_MSG_RESERVED6 = value;
                this.RaiseProperChanged(nameof(MOD_BASIC_MSG_RESERVED6));
            }
        }

        private int _MOD_BASIC_MSG_RESERVED7;   //42
        public int MOD_BASIC_MSG_RESERVED7
        {
            get { return _MOD_BASIC_MSG_RESERVED7; }
            set
            {
                _MOD_BASIC_MSG_RESERVED7 = value;
                this.RaiseProperChanged(nameof(MOD_BASIC_MSG_RESERVED7));
            }
        }

        private int _MOD_BASIC_MSG_RESERVED8;   //43
        public int MOD_BASIC_MSG_RESERVED8
        {
            get { return _MOD_BASIC_MSG_RESERVED8; }
            set
            {
                _MOD_BASIC_MSG_RESERVED8 = value;
                this.RaiseProperChanged(nameof(MOD_BASIC_MSG_RESERVED8));
            }
        }

        private int _MOD_BASIC_MSG_RESERVED9;   //44
        public int MOD_BASIC_MSG_RESERVED9
        {
            get { return _MOD_BASIC_MSG_RESERVED9; }
            set
            {
                _MOD_BASIC_MSG_RESERVED9 = value;
                this.RaiseProperChanged(nameof(MOD_BASIC_MSG_RESERVED9));
            }
        }

        private int _MOD_BASIC_MSG_RESERVED10;  //45
        public int MOD_BASIC_MSG_RESERVED10
        {
            get { return _MOD_BASIC_MSG_RESERVED10; }
            set
            {
                _MOD_BASIC_MSG_RESERVED10 = value;
                this.RaiseProperChanged(nameof(MOD_BASIC_MSG_RESERVED10));
            }
        }

        private int _MOD_BASIC_MSG_RESERVED11;  //46
        public int MOD_BASIC_MSG_RESERVED11
        {
            get { return _MOD_BASIC_MSG_RESERVED11; }
            set
            {
                _MOD_BASIC_MSG_RESERVED11 = value;
                this.RaiseProperChanged(nameof(MOD_BASIC_MSG_RESERVED11));
            }
        }

        private int _MOD_BASIC_MSG_RESERVED12;  //47
        public int MOD_BASIC_MSG_RESERVED12
        {
            get { return _MOD_BASIC_MSG_RESERVED12; }
            set
            {
                _MOD_BASIC_MSG_RESERVED12 = value;
                this.RaiseProperChanged(nameof(MOD_BASIC_MSG_RESERVED12));
            }
        }

        private int _MOD_BASIC_MSG_RESERVED13;  //48
        public int MOD_BASIC_MSG_RESERVED13
        {
            get { return _MOD_BASIC_MSG_RESERVED13; }
            set
            {
                _MOD_BASIC_MSG_RESERVED13 = value;
                this.RaiseProperChanged(nameof(MOD_BASIC_MSG_RESERVED13));
            }
        }

        private int _MOD_BASIC_MSG_RESERVED14;  //49
        public int MOD_BASIC_MSG_RESERVED14
        {
            get { return _MOD_BASIC_MSG_RESERVED14; }
            set
            {
                _MOD_BASIC_MSG_RESERVED14 = value;
                this.RaiseProperChanged(nameof(MOD_BASIC_MSG_RESERVED14));
            }
        }

        private int _MOD_BASIC_MSG_RESERVED15;  //50
        public int MOD_BASIC_MSG_RESERVED15
        {
            get { return _MOD_BASIC_MSG_RESERVED15; }
            set
            {
                _MOD_BASIC_MSG_RESERVED15 = value;
                this.RaiseProperChanged(nameof(MOD_BASIC_MSG_RESERVED15));
            }
        }

        private int _MOD_BASIC_MSG_RESERVED16;  //51
        public int MOD_BASIC_MSG_RESERVED16
        {
            get { return _MOD_BASIC_MSG_RESERVED16; }
            set
            {
                _MOD_BASIC_MSG_RESERVED16 = value;
                this.RaiseProperChanged(nameof(MOD_BASIC_MSG_RESERVED16));
            }
        }

        private int _MOD_BASIC_MSG_RESERVED17;  //52
        public int MOD_BASIC_MSG_RESERVED17
        {
            get { return _MOD_BASIC_MSG_RESERVED17; }
            set
            {
                _MOD_BASIC_MSG_RESERVED17 = value;
                this.RaiseProperChanged(nameof(MOD_BASIC_MSG_RESERVED17));
            }
        }

        private int _MOD_BASIC_MSG_RESERVED18;  //53
        public int MOD_BASIC_MSG_RESERVED18
        {
            get { return _MOD_BASIC_MSG_RESERVED18; }
            set
            {
                _MOD_BASIC_MSG_RESERVED18 = value;
                this.RaiseProperChanged(nameof(MOD_BASIC_MSG_RESERVED18));
            }
        }

        private int _MOD_BASIC_MSG_RESERVED19;  //54
        public int MOD_BASIC_MSG_RESERVED19
        {
            get { return _MOD_BASIC_MSG_RESERVED19; }
            set
            {
                _MOD_BASIC_MSG_RESERVED19 = value;
                this.RaiseProperChanged(nameof(MOD_BASIC_MSG_RESERVED19));
            }
        }

        private int _MOD_BASIC_MSG_RESERVED20;  //55
        public int MOD_BASIC_MSG_RESERVED20
        {
            get { return _MOD_BASIC_MSG_RESERVED20; }
            set
            {
                _MOD_BASIC_MSG_RESERVED20 = value;
                this.RaiseProperChanged(nameof(MOD_BASIC_MSG_RESERVED20));
            }
        }

        private int _MOD_BASIC_MSG_RESERVED21;  //56
        public int MOD_BASIC_MSG_RESERVED21
        {
            get { return _MOD_BASIC_MSG_RESERVED21; }
            set
            {
                _MOD_BASIC_MSG_RESERVED21 = value;
                this.RaiseProperChanged(nameof(MOD_BASIC_MSG_RESERVED21));
            }
        }

        private int _MOD_BASIC_MSG_RESERVED22;  //57
        public int MOD_BASIC_MSG_RESERVED22
        {
            get { return _MOD_BASIC_MSG_RESERVED22; }
            set
            {
                _MOD_BASIC_MSG_RESERVED22 = value;
                this.RaiseProperChanged(nameof(MOD_BASIC_MSG_RESERVED22));
            }
        }

        private int _MOD_BASIC_MSG_RESERVED23;  //58
        public int MOD_BASIC_MSG_RESERVED23
        {
            get { return _MOD_BASIC_MSG_RESERVED23; }
            set
            {
                _MOD_BASIC_MSG_RESERVED23 = value;
                this.RaiseProperChanged(nameof(MOD_BASIC_MSG_RESERVED23));
            }
        }

        private int _MOD_BASIC_MSG_RESERVED24;  //59
        public int MOD_BASIC_MSG_RESERVED24
        {
            get { return _MOD_BASIC_MSG_RESERVED24; }
            set
            {
                _MOD_BASIC_MSG_RESERVED24 = value;
                this.RaiseProperChanged(nameof(MOD_BASIC_MSG_RESERVED24));
            }
        }


        /************************************ 告警量参数读取区(Read Only) **************************************/

        private int _MOD_CELL1VOL_STATE;
        public int MOD_CELL1VOL_STATE
        {
            get { return _MOD_CELL1VOL_STATE; }
            set
            {
                _MOD_CELL1VOL_STATE = value;
                this.RaiseProperChanged(nameof(MOD_CELL1VOL_STATE));
            }
        }

        private int _MOD_CELL2VOL_STATE;
        public int MOD_CELL2VOL_STATE
        {
            get { return _MOD_CELL2VOL_STATE; }
            set
            {
                _MOD_CELL2VOL_STATE = value;
                this.RaiseProperChanged(nameof(MOD_CELL2VOL_STATE));
            }
        }

        private int _MOD_CELL3VOL_STATE;
        public int MOD_CELL3VOL_STATE
        {
            get { return _MOD_CELL3VOL_STATE; }
            set
            {
                _MOD_CELL3VOL_STATE = value;
                this.RaiseProperChanged(nameof(MOD_CELL3VOL_STATE));
            }
        }

        private int _MOD_CELL4VOL_STATE;
        public int MOD_CELL4VOL_STATE
        {
            get { return _MOD_CELL4VOL_STATE; }
            set
            {
                _MOD_CELL4VOL_STATE = value;
                this.RaiseProperChanged(nameof(MOD_CELL4VOL_STATE));
            }
        }

        private int _MOD_CELL5VOL_STATE;
        public int MOD_CELL5VOL_STATE
        {
            get { return _MOD_CELL5VOL_STATE; }
            set
            {
                _MOD_CELL5VOL_STATE = value;
                this.RaiseProperChanged(nameof(MOD_CELL5VOL_STATE));
            }
        }

        private int _MOD_CELL6VOL_STATE;
        public int MOD_CELL6VOL_STATE
        {
            get { return _MOD_CELL6VOL_STATE; }
            set
            {
                _MOD_CELL6VOL_STATE = value;
                this.RaiseProperChanged(nameof(MOD_CELL6VOL_STATE));
            }
        }

        private int _MOD_CELL7VOL_STATE;
        public int MOD_CELL7VOL_STATE
        {
            get { return _MOD_CELL7VOL_STATE; }
            set
            {
                _MOD_CELL7VOL_STATE = value;
                this.RaiseProperChanged(nameof(MOD_CELL7VOL_STATE));
            }
        }

        private int _MOD_CELL8VOL_STATE;
        public int MOD_CELL8VOL_STATE
        {
            get { return _MOD_CELL8VOL_STATE; }
            set
            {
                _MOD_CELL8VOL_STATE = value;
                this.RaiseProperChanged(nameof(MOD_CELL8VOL_STATE));
            }
        }

        private int _MOD_CELL9VOL_STATE;
        public int MOD_CELL9VOL_STATE
        {
            get { return _MOD_CELL9VOL_STATE; }
            set
            {
                _MOD_CELL9VOL_STATE = value;
                this.RaiseProperChanged(nameof(MOD_CELL9VOL_STATE));
            }
        }

        private int _MOD_CELL10VOL_STATE;
        public int MOD_CELL10VOL_STATE
        {
            get { return _MOD_CELL10VOL_STATE; }
            set
            {
                _MOD_CELL10VOL_STATE = value;
                this.RaiseProperChanged(nameof(MOD_CELL10VOL_STATE));
            }
        }

        private int _MOD_CELL11VOL_STATE;
        public int MOD_CELL11VOL_STATE
        {
            get { return _MOD_CELL11VOL_STATE; }
            set
            {
                _MOD_CELL11VOL_STATE = value;
                this.RaiseProperChanged(nameof(MOD_CELL11VOL_STATE));
            }
        }

        private int _MOD_CELL12VOL_STATE;
        public int MOD_CELL12VOL_STATE
        {
            get { return _MOD_CELL12VOL_STATE; }
            set
            {
                _MOD_CELL12VOL_STATE = value;
                this.RaiseProperChanged(nameof(MOD_CELL12VOL_STATE));
            }
        }

        private int _MOD_CELL13VOL_STATE;
        public int MOD_CELL13VOL_STATE
        {
            get { return _MOD_CELL13VOL_STATE; }
            set
            {
                _MOD_CELL13VOL_STATE = value;
                this.RaiseProperChanged(nameof(MOD_CELL13VOL_STATE));
            }
        }

        private int _MOD_CELL14VOL_STATE;
        public int MOD_CELL14VOL_STATE
        {
            get { return _MOD_CELL14VOL_STATE; }
            set
            {
                _MOD_CELL14VOL_STATE = value;
                this.RaiseProperChanged(nameof(MOD_CELL14VOL_STATE));
            }
        }

        private int _MOD_CELL15VOL_STATE;
        public int MOD_CELL15VOL_STATE
        {
            get { return _MOD_CELL15VOL_STATE; }
            set
            {
                _MOD_CELL15VOL_STATE = value;
                this.RaiseProperChanged(nameof(MOD_CELL15VOL_STATE));
            }
        }

        private int _MOD_CELL16VOL_STATE;
        public int MOD_CELL16VOL_STATE
        {
            get { return _MOD_CELL16VOL_STATE; }
            set
            {
                _MOD_CELL16VOL_STATE = value;
                this.RaiseProperChanged(nameof(MOD_CELL16VOL_STATE));
            }
        }

        private int _MOD_GROUD1TEMP_STATE;
        public int MOD_GROUD1TEMP_STATE
        {
            get { return _MOD_GROUD1TEMP_STATE; }
            set
            {
                _MOD_GROUD1TEMP_STATE = value;
                this.RaiseProperChanged(nameof(MOD_GROUD1TEMP_STATE));
            }
        }

        private int _MOD_GROUD2TEMP_STATE;
        public int MOD_GROUD2TEMP_STATE
        {
            get { return _MOD_GROUD2TEMP_STATE; }
            set
            {
                _MOD_GROUD2TEMP_STATE = value;
                this.RaiseProperChanged(nameof(MOD_GROUD2TEMP_STATE));
            }
        }

        private int _MOD_GROUD3TEMP_STATE;
        public int MOD_GROUD3TEMP_STATE
        {
            get { return _MOD_GROUD3TEMP_STATE; }
            set
            {
                _MOD_GROUD3TEMP_STATE = value;
                this.RaiseProperChanged(nameof(MOD_GROUD3TEMP_STATE));
            }
        }

        private int _MOD_GROUD4TEMP_STATE;
        public int MOD_GROUD4TEMP_STATE
        {
            get { return _MOD_GROUD4TEMP_STATE; }
            set
            {
                _MOD_GROUD4TEMP_STATE = value;
                this.RaiseProperChanged(nameof(MOD_GROUD4TEMP_STATE));
            }
        }

        private string _MOD_WARN_STATE;
        public string MOD_WARN_STATE
        {
            get { return _MOD_WARN_STATE; }
            set
            {
                _MOD_WARN_STATE = value;
                this.RaiseProperChanged(nameof(MOD_WARN_STATE));
            }
        }

        private string _MOD_PROT_STATE;
        public string MOD_PROT_STATE
        {
            get { return _MOD_PROT_STATE; }
            set
            {
                _MOD_PROT_STATE = value;
                this.RaiseProperChanged(nameof(MOD_PROT_STATE));
            }
        }

        private string _MOD_ERROR_STATE;
        public string MOD_ERROR_STATE
        {
            get { return _MOD_ERROR_STATE; }
            set
            {
                _MOD_ERROR_STATE = value;
                this.RaiseProperChanged(nameof(MOD_ERROR_STATE));
            }
        }

        private int[] _MOD_INST_STATE;
        public int[] MOD_INST_STATE
        {
            get { return _MOD_INST_STATE; }
            set
            {
                _MOD_INST_STATE = value;
                this.RaiseProperChanged(nameof(MOD_INST_STATE));
            }
        }

        private int _MOD_BALANCE_STATE;
        public int MOD_BALANCE_STATE
        {
            get { return _MOD_BALANCE_STATE; }
            set
            {
                _MOD_BALANCE_STATE = value;
                this.RaiseProperChanged(nameof(MOD_BALANCE_STATE));
            }
        }

        private int _MOD_STATE_RESERVED1;
        public int MOD_STATE_RESERVED1
        {
            get { return _MOD_STATE_RESERVED1; }
            set
            {
                _MOD_STATE_RESERVED1 = value;
                this.RaiseProperChanged(nameof(MOD_STATE_RESERVED1));
            }
        }

        private int _MOD_STATE_RESERVED2;
        public int MOD_STATE_RESERVED2
        {
            get { return _MOD_STATE_RESERVED2; }
            set
            {
                _MOD_STATE_RESERVED2 = value;
                this.RaiseProperChanged(nameof(MOD_STATE_RESERVED2));
            }
        }

        private int _MOD_STATE_RESERVED3;
        public int MOD_STATE_RESERVED3
        {
            get { return _MOD_STATE_RESERVED3; }
            set
            {
                _MOD_STATE_RESERVED3 = value;
                this.RaiseProperChanged(nameof(MOD_STATE_RESERVED3));
            }
        }

        private int _MOD_STATE_RESERVED4;
        public int MOD_STATE_RESERVED4
        {
            get { return _MOD_STATE_RESERVED4; }
            set
            {
                _MOD_STATE_RESERVED4 = value;
                this.RaiseProperChanged(nameof(MOD_STATE_RESERVED4));
            }
        }

        private int _MOD_STATE_RESERVED5;
        public int MOD_STATE_RESERVED5
        {
            get { return _MOD_STATE_RESERVED5; }
            set
            {
                _MOD_STATE_RESERVED5 = value;
                this.RaiseProperChanged(nameof(MOD_STATE_RESERVED5));
            }
        }

        private int _MOD_STATE_RESERVED6;
        public int MOD_STATE_RESERVED6
        {
            get { return _MOD_STATE_RESERVED6; }
            set
            {
                _MOD_STATE_RESERVED6 = value;
                this.RaiseProperChanged(nameof(MOD_STATE_RESERVED6));
            }
        }

        private int _MOD_STATE_RESERVED7;
        public int MOD_STATE_RESERVED7
        {
            get { return _MOD_STATE_RESERVED7; }
            set
            {
                _MOD_STATE_RESERVED7 = value;
                this.RaiseProperChanged(nameof(MOD_STATE_RESERVED7));
            }
        }

        private int _MOD_STATE_RESERVED8;
        public int MOD_STATE_RESERVED8
        {
            get { return _MOD_STATE_RESERVED8; }
            set
            {
                _MOD_STATE_RESERVED8 = value;
                this.RaiseProperChanged(nameof(MOD_STATE_RESERVED8));
            }
        }

        private int _MOD_STATE_RESERVED9;
        public int MOD_STATE_RESERVED9
        {
            get { return _MOD_STATE_RESERVED9; }
            set
            {
                _MOD_STATE_RESERVED9 = value;
                this.RaiseProperChanged(nameof(MOD_STATE_RESERVED9));
            }
        }

        private int _MOD_STATE_RESERVED10;
        public int MOD_STATE_RESERVED10
        {
            get { return _MOD_STATE_RESERVED10; }
            set
            {
                _MOD_STATE_RESERVED10 = value;
                this.RaiseProperChanged(nameof(MOD_STATE_RESERVED10));
            }
        }

        private int _MOD_STATE_RESERVED11;
        public int MOD_STATE_RESERVED11
        {
            get { return _MOD_STATE_RESERVED11; }
            set
            {
                _MOD_STATE_RESERVED11 = value;
                this.RaiseProperChanged(nameof(MOD_STATE_RESERVED11));
            }
        }

        private int _MOD_STATE_RESERVED12;
        public int MOD_STATE_RESERVED12
        {
            get { return _MOD_STATE_RESERVED12; }
            set
            {
                _MOD_STATE_RESERVED12 = value;
                this.RaiseProperChanged(nameof(MOD_STATE_RESERVED12));
            }
        }

        private int _MOD_STATE_RESERVED13;
        public int MOD_STATE_RESERVED13
        {
            get { return _MOD_STATE_RESERVED13; }
            set
            {
                _MOD_STATE_RESERVED13 = value;
                this.RaiseProperChanged(nameof(MOD_STATE_RESERVED13));
            }
        }

        private int _MOD_STATE_RESERVED14;
        public int MOD_STATE_RESERVED14
        {
            get { return _MOD_STATE_RESERVED14; }
            set
            {
                _MOD_STATE_RESERVED14 = value;
                this.RaiseProperChanged(nameof(MOD_STATE_RESERVED14));
            }
        }

        private int _MOD_STATE_RESERVED15;
        public int MOD_STATE_RESERVED15
        {
            get { return _MOD_STATE_RESERVED15; }
            set
            {
                _MOD_STATE_RESERVED15 = value;
                this.RaiseProperChanged(nameof(MOD_STATE_RESERVED15));
            }
        }

        /************************************* 运行时间读取区(Read Only) **************************************/

        private int _MOD_SYSWORKTIME;
        public int MOD_SYSWORKTIME
        {
            get { return _MOD_SYSWORKTIME; }
            set
            {
                _MOD_SYSWORKTIME = value;
                this.RaiseProperChanged(nameof(MOD_SYSWORKTIME));
            }
        }

        private int _MOD_SYS_LIFECYCLE_RESERVED1;
        public int MOD_SYS_LIFECYCLE_RESERVED1
        {
            get { return _MOD_SYS_LIFECYCLE_RESERVED1; }
            set
            {
                _MOD_SYS_LIFECYCLE_RESERVED1 = value;
                this.RaiseProperChanged(nameof(MOD_SYS_LIFECYCLE_RESERVED1));
            }
        }

        private int _MOD_SYS_LIFECYCLE_RESERVED2;
        public int MOD_SYS_LIFECYCLE_RESERVED2
        {
            get { return _MOD_SYS_LIFECYCLE_RESERVED2; }
            set
            {
                _MOD_SYS_LIFECYCLE_RESERVED2 = value;
                this.RaiseProperChanged(nameof(MOD_SYS_LIFECYCLE_RESERVED2));
            }
        }

        private int _MOD_SYS_LIFECYCLE_RESERVED3;
        public int MOD_SYS_LIFECYCLE_RESERVED3
        {
            get { return _MOD_SYS_LIFECYCLE_RESERVED3; }
            set
            {
                _MOD_SYS_LIFECYCLE_RESERVED3 = value;
                this.RaiseProperChanged(nameof(MOD_SYS_LIFECYCLE_RESERVED3));
            }
        }

        private int _MOD_SYS_LIFECYCLE_RESERVED4;
        public int MOD_SYS_LIFECYCLE_RESERVED4
        {
            get { return _MOD_SYS_LIFECYCLE_RESERVED4; }
            set
            {
                _MOD_SYS_LIFECYCLE_RESERVED4 = value;
                this.RaiseProperChanged(nameof(MOD_SYS_LIFECYCLE_RESERVED4));
            }
        }

        private int _MOD_SYS_LIFECYCLE_RESERVED5;
        public int MOD_SYS_LIFECYCLE_RESERVED5
        {
            get { return _MOD_SYS_LIFECYCLE_RESERVED5; }
            set
            {
                _MOD_SYS_LIFECYCLE_RESERVED5 = value;
                this.RaiseProperChanged(nameof(MOD_SYS_LIFECYCLE_RESERVED5));
            }
        }

        private int _MOD_SYS_LIFECYCLE_RESERVED6;
        public int MOD_SYS_LIFECYCLE_RESERVED6
        {
            get { return _MOD_SYS_LIFECYCLE_RESERVED6; }
            set
            {
                _MOD_SYS_LIFECYCLE_RESERVED6 = value;
                this.RaiseProperChanged(nameof(MOD_SYS_LIFECYCLE_RESERVED6));
            }
        }

        private int _MOD_SYS_LIFECYCLE_RESERVED7;
        public int MOD_SYS_LIFECYCLE_RESERVED7
        {
            get { return _MOD_SYS_LIFECYCLE_RESERVED7; }
            set
            {
                _MOD_SYS_LIFECYCLE_RESERVED7 = value;
                this.RaiseProperChanged(nameof(MOD_SYS_LIFECYCLE_RESERVED7));
            }
        }

        private int _MOD_SYS_LIFECYCLE_RESERVED8;
        public int MOD_SYS_LIFECYCLE_RESERVED8
        {
            get { return _MOD_SYS_LIFECYCLE_RESERVED8; }
            set
            {
                _MOD_SYS_LIFECYCLE_RESERVED8 = value;
                this.RaiseProperChanged(nameof(MOD_SYS_LIFECYCLE_RESERVED8));
            }
        }

        private int _MOD_SYS_LIFECYCLE_RESERVED9;
        public int MOD_SYS_LIFECYCLE_RESERVED9
        {
            get { return _MOD_SYS_LIFECYCLE_RESERVED9; }
            set
            {
                _MOD_SYS_LIFECYCLE_RESERVED9 = value;
                this.RaiseProperChanged(nameof(MOD_SYS_LIFECYCLE_RESERVED9));
            }
        }
        /************************************* 时间读写区(Read And Write) **************************************/

        private int _MOD_YEAR_MONTH;
        public int MOD_YEAR_MONTH
        {
            get { return _MOD_YEAR_MONTH; }
            set
            {
                _MOD_YEAR_MONTH = value;
                this.RaiseProperChanged(nameof(MOD_YEAR_MONTH));
            }
        }

        private int _MOD_DAY_HOUR;
        public int MOD_DAY_HOUR
        {
            get { return _MOD_DAY_HOUR; }
            set
            {
                _MOD_DAY_HOUR = value;
                this.RaiseProperChanged(nameof(MOD_DAY_HOUR));
            }
        }

        private int _MOD_MINUTE_SECOND;
        public int MOD_MINUTE_SECOND
        {
            get { return _MOD_MINUTE_SECOND; }
            set
            {
                _MOD_MINUTE_SECOND = value;
                this.RaiseProperChanged(nameof(MOD_MINUTE_SECOND));
            }
        }

        private int _MOD_TIMER_RESERVED1;
        public int MOD_TIMER_RESERVED1
        {
            get { return _MOD_TIMER_RESERVED1; }
            set
            {
                _MOD_TIMER_RESERVED1 = value;
                this.RaiseProperChanged(nameof(MOD_TIMER_RESERVED1));
            }
        }

        private int _MOD_TIMER_RESERVED2;
        public int MOD_TIMER_RESERVED2
        {
            get { return _MOD_TIMER_RESERVED2; }
            set
            {
                _MOD_TIMER_RESERVED2 = value;
                this.RaiseProperChanged(nameof(MOD_TIMER_RESERVED2));
            }
        }

        private int _MOD_TIMER_RESERVED3;
        public int MOD_TIMER_RESERVED3
        {
            get { return _MOD_TIMER_RESERVED3; }
            set
            {
                _MOD_TIMER_RESERVED3 = value;
                this.RaiseProperChanged(nameof(MOD_TIMER_RESERVED3));
            }
        }

        private int _MOD_TIMER_RESERVED4;
        public int MOD_TIMER_RESERVED4
        {
            get { return _MOD_TIMER_RESERVED4; }
            set
            {
                _MOD_TIMER_RESERVED4 = value;
                this.RaiseProperChanged(nameof(MOD_TIMER_RESERVED4));
            }
        }

        private int _MOD_TIMER_RESERVED5;
        public int MOD_TIMER_RESERVED5
        {
            get { return _MOD_TIMER_RESERVED5; }
            set
            {
                _MOD_TIMER_RESERVED5 = value;
                this.RaiseProperChanged(nameof(MOD_TIMER_RESERVED5));
            }
        }

        private int _MOD_TIMER_RESERVED6;
        public int MOD_TIMER_RESERVED6
        {
            get { return _MOD_TIMER_RESERVED6; }
            set
            {
                _MOD_TIMER_RESERVED6 = value;
                this.RaiseProperChanged(nameof(MOD_TIMER_RESERVED6));
            }
        }

        private int _MOD_TIMER_RESERVED7;
        public int MOD_TIMER_RESERVED7
        {
            get { return _MOD_TIMER_RESERVED7; }
            set
            {
                _MOD_TIMER_RESERVED7 = value;
                this.RaiseProperChanged(nameof(MOD_TIMER_RESERVED7));
            }
        }

        /****************************  强制控制写入区(Write Only) **************************************/

        private int _MOD_FORCEACT_SWITCH;
        public int MOD_FORCEACT_SWITCH
        {
            get { return _MOD_FORCEACT_SWITCH; }
            set
            {
                _MOD_FORCEACT_SWITCH = value;
                this.RaiseProperChanged(nameof(MOD_FORCEACT_SWITCH));
            }
        }

        private int _MOD_FORCEACT_BALANCE;
        public int MOD_FORCEACT_BALANCE
        {
            get { return _MOD_FORCEACT_BALANCE; }
            set
            {
                _MOD_FORCEACT_BALANCE = value;
                this.RaiseProperChanged(nameof(MOD_FORCEACT_BALANCE));
            }
        }

        private int _MOD_FORCEACT_RESERVED1;
        public int MOD_FORCEACT_RESERVED1
        {
            get { return _MOD_FORCEACT_RESERVED1; }
            set
            {
                _MOD_FORCEACT_RESERVED1 = value;
                this.RaiseProperChanged(nameof(MOD_FORCEACT_RESERVED1));
            }
        }

        private int _MOD_FORCEACT_RESERVED2;
        public int MOD_FORCEACT_RESERVED2
        {
            get { return _MOD_FORCEACT_RESERVED2; }
            set
            {
                _MOD_FORCEACT_RESERVED2 = value;
                this.RaiseProperChanged(nameof(MOD_FORCEACT_RESERVED2));
            }
        }

        private int _MOD_FORCEACT_RESERVED3;
        public int MOD_FORCEACT_RESERVED3
        {
            get { return _MOD_FORCEACT_RESERVED3; }
            set
            {
                _MOD_FORCEACT_RESERVED3 = value;
                this.RaiseProperChanged(nameof(MOD_FORCEACT_RESERVED3));
            }
        }

        private int _MOD_FORCEACT_RESERVED4;
        public int MOD_FORCEACT_RESERVED4
        {
            get { return _MOD_FORCEACT_RESERVED4; }
            set
            {
                _MOD_FORCEACT_RESERVED4 = value;
                this.RaiseProperChanged(nameof(MOD_FORCEACT_RESERVED4));
            }
        }

        private int _MOD_FORCEACT_RESERVED5;
        public int MOD_FORCEACT_RESERVED5
        {
            get { return _MOD_FORCEACT_RESERVED5; }
            set
            {
                _MOD_FORCEACT_RESERVED5 = value;
                this.RaiseProperChanged(nameof(MOD_FORCEACT_RESERVED5));
            }
        }

        private int _MOD_FORCEACT_RESERVED6;
        public int MOD_FORCEACT_RESERVED6
        {
            get { return _MOD_FORCEACT_RESERVED6; }
            set
            {
                _MOD_FORCEACT_RESERVED6 = value;
                this.RaiseProperChanged(nameof(MOD_FORCEACT_RESERVED6));
            }
        }

        private int _MOD_FORCEACT_RESERVED7;
        public int MOD_FORCEACT_RESERVED7
        {
            get { return _MOD_FORCEACT_RESERVED7; }
            set
            {
                _MOD_FORCEACT_RESERVED7 = value;
                this.RaiseProperChanged(nameof(MOD_FORCEACT_RESERVED7));
            }
        }

        private int _MOD_FORCEACT_RESERVED8;
        public int MOD_FORCEACT_RESERVED8
        {
            get { return _MOD_FORCEACT_RESERVED8; }
            set
            {
                _MOD_FORCEACT_RESERVED8 = value;
                this.RaiseProperChanged(nameof(MOD_FORCEACT_RESERVED8));
            }
        }

        /************************************** 用户参数读写区(Read And Write) **************************************/

        // 单体过充保护参数
        private int _MOD_PP_CCOV_WARNVOL;
        public int MOD_PP_CCOV_WARNVOL
        {
            get { return _MOD_PP_CCOV_WARNVOL; }
            set
            {
                _MOD_PP_CCOV_WARNVOL = value;
                this.RaiseProperChanged(nameof(MOD_PP_CCOV_WARNVOL));
            }
        }

        private int _MOD_PP_CCOV_PROTVOL;
        public int MOD_PP_CCOV_PROTVOL
        {
            get { return _MOD_PP_CCOV_PROTVOL; }
            set
            {
                _MOD_PP_CCOV_PROTVOL = value;
                this.RaiseProperChanged(nameof(MOD_PP_CCOV_PROTVOL));
            }
        }

        private int _MOD_PP_CCOV_PROTDELAY;
        public int MOD_PP_CCOV_PROTDELAY
        {
            get { return _MOD_PP_CCOV_PROTDELAY; }
            set
            {
                _MOD_PP_CCOV_PROTDELAY = value;
                this.RaiseProperChanged(nameof(MOD_PP_CCOV_PROTDELAY));
            }
        }

        private int _MOD_PP_CCOV_RELEASEVOL;
        public int MOD_PP_CCOV_RELEASEVOL
        {
            get { return _MOD_PP_CCOV_RELEASEVOL; }
            set
            {
                _MOD_PP_CCOV_RELEASEVOL = value;
                this.RaiseProperChanged(nameof(MOD_PP_CCOV_RELEASEVOL));
            }
        }

        private int _MOD_PP_CCOV_RELEASECAP;
        public int MOD_PP_CCOV_RELEASECAP
        {
            get { return _MOD_PP_CCOV_RELEASECAP; }
            set
            {
                _MOD_PP_CCOV_RELEASECAP = value;
                this.RaiseProperChanged(nameof(MOD_PP_CCOV_RELEASECAP));
            }
        }

        // 总体过充保护参数
        private int _MOD_PP_TCOV_WARNVOL;
        public int MOD_PP_TCOV_WARNVOL
        {
            get { return _MOD_PP_TCOV_WARNVOL; }
            set
            {
                _MOD_PP_TCOV_WARNVOL = value;
                this.RaiseProperChanged(nameof(MOD_PP_TCOV_WARNVOL));
            }
        }

        private int _MOD_PP_TCOV_PROTVOL;
        public int MOD_PP_TCOV_PROTVOL
        {
            get { return _MOD_PP_TCOV_PROTVOL; }
            set
            {
                _MOD_PP_TCOV_PROTVOL = value;
                this.RaiseProperChanged(nameof(MOD_PP_TCOV_PROTVOL));
            }
        }

        private int _MOD_PP_TCOV_PROTDELAY;
        public int MOD_PP_TCOV_PROTDELAY
        {
            get { return _MOD_PP_TCOV_PROTDELAY; }
            set
            {
                _MOD_PP_TCOV_PROTDELAY = value;
                this.RaiseProperChanged(nameof(MOD_PP_TCOV_PROTDELAY));
            }
        }

        private int _MOD_PP_TCOV_RELEASEVOL;
        public int MOD_PP_TCOV_RELEASEVOL
        {
            get { return _MOD_PP_TCOV_RELEASEVOL; }
            set
            {
                _MOD_PP_TCOV_RELEASEVOL = value;
                this.RaiseProperChanged(nameof(MOD_PP_TCOV_RELEASEVOL));
            }
        }

        private int _MOD_PP_TCOV_RELEASECAP;
        public int MOD_PP_TCOV_RELEASECAP
        {
            get { return _MOD_PP_TCOV_RELEASECAP; }
            set
            {
                _MOD_PP_TCOV_RELEASECAP = value;
                this.RaiseProperChanged(nameof(MOD_PP_TCOV_RELEASECAP));
            }
        }

        // 单体过放保护参数
        private int _MOD_PP_CDCUV_WARNVOL;
        public int MOD_PP_CDCUV_WARNVOL
        {
            get { return _MOD_PP_CDCUV_WARNVOL; }
            set
            {
                _MOD_PP_CDCUV_WARNVOL = value;
                this.RaiseProperChanged(nameof(MOD_PP_CDCUV_WARNVOL));
            }
        }

        private int _MOD_PP_CDCUV_PROTVOL;
        public int MOD_PP_CDCUV_PROTVOL
        {
            get { return _MOD_PP_CDCUV_PROTVOL; }
            set
            {
                _MOD_PP_CDCUV_PROTVOL = value;
                this.RaiseProperChanged(nameof(MOD_PP_CDCUV_PROTVOL));
            }
        }

        private int _MOD_PP_CDCUV_PROTDELAY;
        public int MOD_PP_CDCUV_PROTDELAY
        {
            get { return _MOD_PP_CDCUV_PROTDELAY; }
            set
            {
                _MOD_PP_CDCUV_PROTDELAY = value;
                this.RaiseProperChanged(nameof(MOD_PP_CDCUV_PROTDELAY));
            }
        }

        private int _MOD_PP_CDCUV_RELEASEVOL;
        public int MOD_PP_CDCUV_RELEASEVOL
        {
            get { return _MOD_PP_CDCUV_RELEASEVOL; }
            set
            {
                _MOD_PP_CDCUV_RELEASEVOL = value;
                this.RaiseProperChanged(nameof(MOD_PP_CDCUV_RELEASEVOL));
            }
        }

        // 总体过放保护参数
        private int _MOD_PP_TDCUV_WARNVOL;
        public int MOD_PP_TDCUV_WARNVOL
        {
            get { return _MOD_PP_TDCUV_WARNVOL; }
            set
            {
                _MOD_PP_TDCUV_WARNVOL = value;
                this.RaiseProperChanged(nameof(MOD_PP_TDCUV_WARNVOL));
            }
        }

        private int _MOD_PP_TDCUV_PROTVOL;
        public int MOD_PP_TDCUV_PROTVOL
        {
            get { return _MOD_PP_TDCUV_PROTVOL; }
            set
            {
                _MOD_PP_TDCUV_PROTVOL = value;
                this.RaiseProperChanged(nameof(MOD_PP_TDCUV_PROTVOL));
            }
        }

        private int _MOD_PP_TDCUV_PROTDELAY;
        public int MOD_PP_TDCUV_PROTDELAY
        {
            get { return _MOD_PP_TDCUV_PROTDELAY; }
            set
            {
                _MOD_PP_TDCUV_PROTDELAY = value;
                this.RaiseProperChanged(nameof(MOD_PP_TDCUV_PROTDELAY));
            }
        }

        private int _MOD_PP_TDCUV_RELEASEVOL;
        public int MOD_PP_TDCUV_RELEASEVOL
        {
            get { return _MOD_PP_TDCUV_RELEASEVOL; }
            set
            {
                _MOD_PP_TDCUV_RELEASEVOL = value;
                this.RaiseProperChanged(nameof(MOD_PP_TDCUV_RELEASEVOL));
            }
        }

        // 充电过流保护参数
        private int _MOD_PP_COC_WARNCUR;
        public int MOD_PP_COC_WARNCUR
        {
            get { return _MOD_PP_COC_WARNCUR; }
            set
            {
                _MOD_PP_COC_WARNCUR = value;
                this.RaiseProperChanged(nameof(MOD_PP_COC_WARNCUR));
            }
        }

        private int _MOD_PP_COC_PROTCUR;
        public int MOD_PP_COC_PROTCUR
        {
            get { return _MOD_PP_COC_PROTCUR; }
            set
            {
                _MOD_PP_COC_PROTCUR = value;
                this.RaiseProperChanged(nameof(MOD_PP_COC_PROTCUR));
            }
        }

        private int _MOD_PP_COC_PROTDELAY;
        public int MOD_PP_COC_PROTDELAY
        {
            get { return _MOD_PP_COC_PROTDELAY; }
            set
            {
                _MOD_PP_COC_PROTDELAY = value;
                this.RaiseProperChanged(nameof(MOD_PP_COC_PROTDELAY));
            }
        }

        private int _MOD_PP_COC_RELEASDELAY;
        public int MOD_PP_COC_RELEASDELAY
        {
            get { return _MOD_PP_COC_RELEASDELAY; }
            set
            {
                _MOD_PP_COC_RELEASDELAY = value;
                this.RaiseProperChanged(nameof(MOD_PP_COC_RELEASDELAY));
            }
        }

        // 放电过流保护参数
        private int _MOD_PP_DCOC_WARNCUR;
        public int MOD_PP_DCOC_WARNCUR
        {
            get { return _MOD_PP_DCOC_WARNCUR; }
            set
            {
                _MOD_PP_DCOC_WARNCUR = value;
                this.RaiseProperChanged(nameof(MOD_PP_DCOC_WARNCUR));
            }
        }

        private int _MOD_PP_DCOC_PROTCUR1;
        public int MOD_PP_DCOC_PROTCUR1
        {
            get { return _MOD_PP_DCOC_PROTCUR1; }
            set
            {
                _MOD_PP_DCOC_PROTCUR1 = value;
                this.RaiseProperChanged(nameof(MOD_PP_DCOC_PROTCUR1));
            }
        }

        private int _MOD_PP_DCOC_PROTCUR2;
        public int MOD_PP_DCOC_PROTCUR2
        {
            get { return _MOD_PP_DCOC_PROTCUR2; }
            set
            {
                _MOD_PP_DCOC_PROTCUR2 = value;
                this.RaiseProperChanged(nameof(MOD_PP_DCOC_PROTCUR2));
            }
        }

        private int _MOD_PP_DCOC_PROTDELAY1;
        public int MOD_PP_DCOC_PROTDELAY1
        {
            get { return _MOD_PP_DCOC_PROTDELAY1; }
            set
            {
                _MOD_PP_DCOC_PROTDELAY1 = value;
                this.RaiseProperChanged(nameof(MOD_PP_DCOC_PROTDELAY1));
            }
        }

        private int _MOD_PP_DCOC_PROTDELAY2;
        public int MOD_PP_DCOC_PROTDELAY2
        {
            get { return _MOD_PP_DCOC_PROTDELAY2; }
            set
            {
                _MOD_PP_DCOC_PROTDELAY2 = value;
                this.RaiseProperChanged(nameof(MOD_PP_DCOC_PROTDELAY2));
            }
        }

        private int _MOD_PP_DCOC_RELEASDELAY;
        public int MOD_PP_DCOC_RELEASDELAY
        {
            get { return _MOD_PP_DCOC_RELEASDELAY; }
            set
            {
                _MOD_PP_DCOC_RELEASDELAY = value;
                this.RaiseProperChanged(nameof(MOD_PP_DCOC_RELEASDELAY));
            }
        }

        // 短路保护参数
        private int _MOD_PP_DCSC_RELEASDELAY;
        public int MOD_PP_DCSC_RELEASDELAY
        {
            get { return _MOD_PP_DCSC_RELEASDELAY; }
            set
            {
                _MOD_PP_DCSC_RELEASDELAY = value;
                this.RaiseProperChanged(nameof(MOD_PP_DCSC_RELEASDELAY));
            }
        }

        // 均衡保护参数
        private int _MOD_PP_BALANCE_TRUNONVOL;
        public int MOD_PP_BALANCE_TRUNONVOL
        {
            get { return _MOD_PP_BALANCE_TRUNONVOL; }
            set
            {
                _MOD_PP_BALANCE_TRUNONVOL = value;
                this.RaiseProperChanged(nameof(MOD_PP_BALANCE_TRUNONVOL));
            }
        }

        private int _MOD_PP_BALANCE_TRUNOFFVOL;
        public int MOD_PP_BALANCE_TRUNOFFVOL
        {
            get { return _MOD_PP_BALANCE_TRUNOFFVOL; }
            set
            {
                _MOD_PP_BALANCE_TRUNOFFVOL = value;
                this.RaiseProperChanged(nameof(MOD_PP_BALANCE_TRUNOFFVOL));
            }
        }

        private int _MOD_PP_BALANCE_TRUNONDIFFVOL;
        public int MOD_PP_BALANCE_TRUNONDIFFVOL
        {
            get { return _MOD_PP_BALANCE_TRUNONDIFFVOL; }
            set
            {
                _MOD_PP_BALANCE_TRUNONDIFFVOL = value;
                this.RaiseProperChanged(nameof(MOD_PP_BALANCE_TRUNONDIFFVOL));
            }
        }

        private int _MOD_PP_BALANCE_TRUNOFFDIFFVOL;
        public int MOD_PP_BALANCE_TRUNOFFDIFFVOL
        {
            get { return _MOD_PP_BALANCE_TRUNOFFDIFFVOL; }
            set
            {
                _MOD_PP_BALANCE_TRUNOFFDIFFVOL = value;
                this.RaiseProperChanged(nameof(MOD_PP_BALANCE_TRUNOFFDIFFVOL));
            }
        }

        // 休眠保护参数
        private int _MOD_PP_SLEEP_CELLVOL;
        public int MOD_PP_SLEEP_CELLVOL
        {
            get { return _MOD_PP_SLEEP_CELLVOL; }
            set
            {
                _MOD_PP_SLEEP_CELLVOL = value;
                this.RaiseProperChanged(nameof(MOD_PP_SLEEP_CELLVOL));
            }
        }

        private int _MOD_PP_SLEEP_DELAY;
        public int MOD_PP_SLEEP_DELAY
        {
            get { return _MOD_PP_SLEEP_DELAY; }
            set
            {
                _MOD_PP_SLEEP_DELAY = value;
                this.RaiseProperChanged(nameof(MOD_PP_SLEEP_DELAY));
            }
        }

        // 满充保护参数
        private int _MOD_PP_FC_VOL;
        public int MOD_PP_FC_VOL
        {
            get { return _MOD_PP_FC_VOL; }
            set
            {
                _MOD_PP_FC_VOL = value;
                this.RaiseProperChanged(nameof(MOD_PP_FC_VOL));
            }
        }

        private int _MOD_PP_FC_CUR;
        public int MOD_PP_FC_CUR
        {
            get { return _MOD_PP_FC_CUR; }
            set
            {
                _MOD_PP_FC_CUR = value;
                this.RaiseProperChanged(nameof(MOD_PP_FC_CUR));
            }
        }

        // 低电保护参数
        private int _MOD_PP_LP_SOC;
        public int MOD_PP_LP_SOC
        {
            get { return _MOD_PP_LP_SOC; }
            set
            {
                _MOD_PP_LP_SOC = value;
                this.RaiseProperChanged(nameof(MOD_PP_LP_SOC));
            }
        }

        // 电芯温度保护参数（前段）
        private int _MOD_PP_CELLTEMP_CHTWARNTEMP;
        public int MOD_PP_CELLTEMP_CHTWARNTEMP
        {
            get { return _MOD_PP_CELLTEMP_CHTWARNTEMP; }
            set
            {
                _MOD_PP_CELLTEMP_CHTWARNTEMP = value;
                this.RaiseProperChanged(nameof(MOD_PP_CELLTEMP_CHTWARNTEMP));
            }
        }

        private int _MOD_PP_CELLTEMP_COTPROTTEMP;
        public int MOD_PP_CELLTEMP_COTPROTTEMP
        {
            get { return _MOD_PP_CELLTEMP_COTPROTTEMP; }
            set
            {
                _MOD_PP_CELLTEMP_COTPROTTEMP = value;
                this.RaiseProperChanged(nameof(MOD_PP_CELLTEMP_COTPROTTEMP));
            }
        }

        private int _MOD_PP_CELLTEMP_COTPROTDELAY;
        public int MOD_PP_CELLTEMP_COTPROTDELAY
        {
            get { return _MOD_PP_CELLTEMP_COTPROTDELAY; }
            set
            {
                _MOD_PP_CELLTEMP_COTPROTDELAY = value;
                this.RaiseProperChanged(nameof(MOD_PP_CELLTEMP_COTPROTDELAY));
            }
        }

        private int _MOD_PP_CELLTEMP_COTRELEASETEMP;
        public int MOD_PP_CELLTEMP_COTRELEASETEMP
        {
            get { return _MOD_PP_CELLTEMP_COTRELEASETEMP; }
            set
            {
                _MOD_PP_CELLTEMP_COTRELEASETEMP = value;
                this.RaiseProperChanged(nameof(MOD_PP_CELLTEMP_COTRELEASETEMP));
            }
        }

        private int _MOD_PP_CELLTEMP_DUTWARNTEMP;
        public int MOD_PP_CELLTEMP_DUTWARNTEMP
        {
            get { return _MOD_PP_CELLTEMP_DUTWARNTEMP; }
            set
            {
                _MOD_PP_CELLTEMP_DUTWARNTEMP = value;
                this.RaiseProperChanged(nameof(MOD_PP_CELLTEMP_DUTWARNTEMP));
            }
        }

        private int _MOD_PP_CELLTEMP_DOTPROTTEMP;
        public int MOD_PP_CELLTEMP_DOTPROTTEMP
        {
            get { return _MOD_PP_CELLTEMP_DOTPROTTEMP; }
            set
            {
                _MOD_PP_CELLTEMP_DOTPROTTEMP = value;
                this.RaiseProperChanged(nameof(MOD_PP_CELLTEMP_DOTPROTTEMP));
            }
        }

        private int _MOD_PP_CELLTEMP_DOTPROTDELAY;
        public int MOD_PP_CELLTEMP_DOTPROTDELAY
        {
            get { return _MOD_PP_CELLTEMP_DOTPROTDELAY; }
            set
            {
                _MOD_PP_CELLTEMP_DOTPROTDELAY = value;
                this.RaiseProperChanged(nameof(MOD_PP_CELLTEMP_DOTPROTDELAY));
            }
        }

        private int _MOD_PP_CELLTEMP_DOTRELEASETEMP;
        public int MOD_PP_CELLTEMP_DOTRELEASETEMP
        {
            get { return _MOD_PP_CELLTEMP_DOTRELEASETEMP; }
            set
            {
                _MOD_PP_CELLTEMP_DOTRELEASETEMP = value;
                this.RaiseProperChanged(nameof(MOD_PP_CELLTEMP_DOTRELEASETEMP));
            }
        }

        private int _MOD_PP_CELLTEMP_DUTPROTTEMP;
        public int MOD_PP_CELLTEMP_DUTPROTTEMP
        {
            get { return _MOD_PP_CELLTEMP_DUTPROTTEMP; }
            set
            {
                _MOD_PP_CELLTEMP_DUTPROTTEMP = value;
                this.RaiseProperChanged(nameof(MOD_PP_CELLTEMP_DUTPROTTEMP));
            }
        }

        private int _MOD_PP_CELLTEMP_DUTPROTDELAY;
        public int MOD_PP_CELLTEMP_DUTPROTDELAY
        {
            get { return _MOD_PP_CELLTEMP_DUTPROTDELAY; }
            set
            {
                _MOD_PP_CELLTEMP_DUTPROTDELAY = value;
                this.RaiseProperChanged(nameof(MOD_PP_CELLTEMP_DUTPROTDELAY));
            }
        }

        private int _MOD_PP_CELLTEMP_DUTRELEASETEMP;
        public int MOD_PP_CELLTEMP_DUTRELEASETEMP
        {
            get { return _MOD_PP_CELLTEMP_DUTRELEASETEMP; }
            set
            {
                _MOD_PP_CELLTEMP_DUTRELEASETEMP = value;
                this.RaiseProperChanged(nameof(MOD_PP_CELLTEMP_DUTRELEASETEMP));
            }
        }

        // 环境温度保护参数
        private int _MOD_PP_ENVTEMP_COTPROTTEMP;
        public int MOD_PP_ENVTEMP_COTPROTTEMP
        {
            get { return _MOD_PP_ENVTEMP_COTPROTTEMP; }
            set
            {
                _MOD_PP_ENVTEMP_COTPROTTEMP = value;
                this.RaiseProperChanged(nameof(MOD_PP_ENVTEMP_COTPROTTEMP));
            }
        }

        private int _MOD_PP_ENVTEMP_CUTPROTTEMP;
        public int MOD_PP_ENVTEMP_CUTPROTTEMP
        {
            get { return _MOD_PP_ENVTEMP_CUTPROTTEMP; }
            set
            {
                _MOD_PP_ENVTEMP_CUTPROTTEMP = value;
                this.RaiseProperChanged(nameof(MOD_PP_ENVTEMP_CUTPROTTEMP));
            }
        }

        private int _MOD_PP_ENVTEMP_DOTPROTTEMP;
        public int MOD_PP_ENVTEMP_DOTPROTTEMP
        {
            get { return _MOD_PP_ENVTEMP_DOTPROTTEMP; }
            set
            {
                _MOD_PP_ENVTEMP_DOTPROTTEMP = value;
                this.RaiseProperChanged(nameof(MOD_PP_ENVTEMP_DOTPROTTEMP));
            }
        }

        private int _MOD_PP_ENVTEMP_DUTPROTTEMP;
        public int MOD_PP_ENVTEMP_DUTPROTTEMP
        {
            get { return _MOD_PP_ENVTEMP_DUTPROTTEMP; }
            set
            {
                _MOD_PP_ENVTEMP_DUTPROTTEMP = value;
                this.RaiseProperChanged(nameof(MOD_PP_ENVTEMP_DUTPROTTEMP));
            }
        }

        // MOS 管温度保护参数
        private int _MOD_PP_MOSTEMP_COTPROTTEMP;
        public int MOD_PP_MOSTEMP_COTPROTTEMP
        {
            get { return _MOD_PP_MOSTEMP_COTPROTTEMP; }
            set
            {
                _MOD_PP_MOSTEMP_COTPROTTEMP = value;
                this.RaiseProperChanged(nameof(MOD_PP_MOSTEMP_COTPROTTEMP));
            }
        }

        private int _MOD_PP_MOSTEMP_DOTPROTTEMP;
        public int MOD_PP_MOSTEMP_DOTPROTTEMP
        {
            get { return _MOD_PP_MOSTEMP_DOTPROTTEMP; }
            set
            {
                _MOD_PP_MOSTEMP_DOTPROTTEMP = value;
                this.RaiseProperChanged(nameof(MOD_PP_MOSTEMP_DOTPROTTEMP));
            }
        }

        private int _MOD_PP_MOSTEMP_COTRELEASETEMP;
        public int MOD_PP_MOSTEMP_COTRELEASETEMP
        {
            get { return _MOD_PP_MOSTEMP_COTRELEASETEMP; }
            set
            {
                _MOD_PP_MOSTEMP_COTRELEASETEMP = value;
                this.RaiseProperChanged(nameof(MOD_PP_MOSTEMP_COTRELEASETEMP));
            }
        }

        private int _MOD_PP_MOSTEMP_DOTRELEASETEMP;
        public int MOD_PP_MOSTEMP_DOTRELEASETEMP
        {
            get { return _MOD_PP_MOSTEMP_DOTRELEASETEMP; }
            set
            {
                _MOD_PP_MOSTEMP_DOTRELEASETEMP = value;
                this.RaiseProperChanged(nameof(MOD_PP_MOSTEMP_DOTRELEASETEMP));
            }
        }

        // 设备参数
        private int _MOD_PP_TOTAL_NTC;
        public int MOD_PP_TOTAL_NTC
        {
            get { return _MOD_PP_TOTAL_NTC; }
            set
            {
                _MOD_PP_TOTAL_NTC = value;
                this.RaiseProperChanged(nameof(MOD_PP_TOTAL_NTC));
            }
        }

        private int _MOD_PP_NTC_TYPE;
        public int MOD_PP_NTC_TYPE
        {
            get { return _MOD_PP_NTC_TYPE; }
            set
            {
                _MOD_PP_NTC_TYPE = value;
                this.RaiseProperChanged(nameof(MOD_PP_NTC_TYPE));
            }
        }

        private int _MOD_PP_CELL_CNT;
        public int MOD_PP_CELL_CNT
        {
            get { return _MOD_PP_CELL_CNT; }
            set
            {
                _MOD_PP_CELL_CNT = value;
                this.RaiseProperChanged(nameof(MOD_PP_CELL_CNT));
            }
        }

        private int _MOD_PP_CAPACITY;
        public int MOD_PP_CAPACITY
        {
            get { return _MOD_PP_CAPACITY; }
            set
            {
                _MOD_PP_CAPACITY = value;
                this.RaiseProperChanged(nameof(MOD_PP_CAPACITY));
            }
        }

        private int _MOD_PP_FIRMWARE_VERSION;
        public int MOD_PP_FIRMWARE_VERSION
        {
            get { return _MOD_PP_FIRMWARE_VERSION; }
            set
            {
                _MOD_PP_FIRMWARE_VERSION = value;
                this.RaiseProperChanged(nameof(MOD_PP_FIRMWARE_VERSION));
            }
        }

        private int _MOD_PP_HARDWARE_VERSION;
        public int MOD_PP_HARDWARE_VERSION
        {
            get { return _MOD_PP_HARDWARE_VERSION; }
            set
            {
                _MOD_PP_HARDWARE_VERSION = value;
                this.RaiseProperChanged(nameof(MOD_PP_HARDWARE_VERSION));
            }
        }

        private int _MOD_PP_SERIAL_NUM;
        public int MOD_PP_SERIAL_NUM
        {
            get { return _MOD_PP_SERIAL_NUM; }
            set
            {
                _MOD_PP_SERIAL_NUM = value;
                this.RaiseProperChanged(nameof(MOD_PP_SERIAL_NUM));
            }
        }

        private int _MOD_PP_DEVICE_ADDR;
        public int MOD_PP_DEVICE_ADDR
        {
            get { return _MOD_PP_DEVICE_ADDR; }
            set
            {
                _MOD_PP_DEVICE_ADDR = value;
                this.RaiseProperChanged(nameof(MOD_PP_DEVICE_ADDR));
            }
        }

        private int _MOD_PP_BAUD_RATE;
        public int MOD_PP_BAUD_RATE
        {
            get { return _MOD_PP_BAUD_RATE; }
            set
            {
                _MOD_PP_BAUD_RATE = value;
                this.RaiseProperChanged(nameof(MOD_PP_BAUD_RATE));
            }
        }

        private int _MOD_PP_RESERVED1;
        public int MOD_PP_RESERVED1
        {
            get { return _MOD_PP_RESERVED1; }
            set
            {
                _MOD_PP_RESERVED1 = value;
                this.RaiseProperChanged(nameof(MOD_PP_RESERVED1));
            }
        }

        private int _MOD_PP_RESERVED2;
        public int MOD_PP_RESERVED2
        {
            get { return _MOD_PP_RESERVED2; }
            set
            {
                _MOD_PP_RESERVED2 = value;
                this.RaiseProperChanged(nameof(MOD_PP_RESERVED2));
            }
        }

        private int _MOD_PP_RESERVED3;
        public int MOD_PP_RESERVED3
        {
            get { return _MOD_PP_RESERVED3; }
            set
            {
                _MOD_PP_RESERVED3 = value;
                this.RaiseProperChanged(nameof(MOD_PP_RESERVED3));
            }
        }

        private int _MOD_PP_RESERVED4;
        public int MOD_PP_RESERVED4
        {
            get { return _MOD_PP_RESERVED4; }
            set
            {
                _MOD_PP_RESERVED4 = value;
                this.RaiseProperChanged(nameof(MOD_PP_RESERVED4));
            }
        }

        private int _MOD_PP_RESERVED5;
        public int MOD_PP_RESERVED5
        {
            get { return _MOD_PP_RESERVED5; }
            set
            {
                _MOD_PP_RESERVED5 = value;
                this.RaiseProperChanged(nameof(MOD_PP_RESERVED5));
            }
        }

        private int _MOD_PP_RESERVED6;
        public int MOD_PP_RESERVED6
        {
            get { return _MOD_PP_RESERVED6; }
            set
            {
                _MOD_PP_RESERVED6 = value;
                this.RaiseProperChanged(nameof(MOD_PP_RESERVED6));
            }
        }

        private int _MOD_PP_RESERVED7;
        public int MOD_PP_RESERVED7
        {
            get { return _MOD_PP_RESERVED7; }
            set
            {
                _MOD_PP_RESERVED7 = value;
                this.RaiseProperChanged(nameof(MOD_PP_RESERVED7));
            }
        }

        private int _MOD_PP_RESERVED8;
        public int MOD_PP_RESERVED8
        {
            get { return _MOD_PP_RESERVED8; }
            set
            {
                _MOD_PP_RESERVED8 = value;
                this.RaiseProperChanged(nameof(MOD_PP_RESERVED8));
            }
        }

        private int _MOD_PP_RESERVED9;
        public int MOD_PP_RESERVED9
        {
            get { return _MOD_PP_RESERVED9; }
            set
            {
                _MOD_PP_RESERVED9 = value;
                this.RaiseProperChanged(nameof(MOD_PP_RESERVED9));
            }
        }

        private int _MOD_PP_RESERVED10;
        public int MOD_PP_RESERVED10
        {
            get { return _MOD_PP_RESERVED10; }
            set
            {
                _MOD_PP_RESERVED10 = value;
                this.RaiseProperChanged(nameof(MOD_PP_RESERVED10));
            }
        }

        private string _MOD_SP_SOFTVER;            //282 软件版本
        public string MOD_SP_SOFTVER
        {
            get { return _MOD_SP_SOFTVER; }
            set
            {
                _MOD_SP_SOFTVER = value;
                this.RaiseProperChanged(nameof(MOD_SP_SOFTVER));
            }
        }

        private string _MOD_SP_HARDVER;            //284 硬件版本
        public string MOD_SP_HARDVER
        {
            get { return _MOD_SP_HARDVER; }
            set
            {
                _MOD_SP_HARDVER = value;
                this.RaiseProperChanged(nameof(MOD_SP_HARDVER));
            }
        }

        private int _MOD_SP_SN;                 //286 序列号
        public int MOD_SP_SN
        {
            get { return _MOD_SP_SN; }
            set
            {
                _MOD_SP_SN = value;
                this.RaiseProperChanged(nameof(MOD_SP_SN));
            }
        }

        private int _MOD_SP_FACTDATE;           //288 出厂日期
        public int MOD_SP_FACTDATE
        {
            get { return _MOD_SP_FACTDATE; }
            set
            {
                _MOD_SP_FACTDATE = value;
                this.RaiseProperChanged(nameof(MOD_SP_FACTDATE));
            }
        }

        private int _MOD_SP_FANUFACTURER;       //290 厂商
        public int MOD_SP_FANUFACTURER
        {
            get { return _MOD_SP_FANUFACTURER; }
            set
            {
                _MOD_SP_FANUFACTURER = value;
                this.RaiseProperChanged(nameof(MOD_SP_FANUFACTURER));
            }
        }

        private int _MOD_SP_DEVICESN;           //292 设备型号
        public int MOD_SP_DEVICESN
        {
            get { return _MOD_SP_DEVICESN; }
            set
            {
                _MOD_SP_DEVICESN = value;
                this.RaiseProperChanged(nameof(MOD_SP_DEVICESN));
            }
        }

        private int _MOD_SP_CELLSN;             //294 设备型号
        public int MOD_SP_CELLSN
        {
            get { return _MOD_SP_CELLSN; }
            set
            {
                _MOD_SP_CELLSN = value;
                this.RaiseProperChanged(nameof(MOD_SP_CELLSN));
            }
        }

    }
}




