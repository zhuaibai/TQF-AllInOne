using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    public class Common_Data
    {
        public object GetPropertyValue(string propertyName)
        {
            var property = GetType().GetProperty(propertyName);
            if (property != null)
            {
                return property.GetValue(this);
            }
            return null;
        }

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime DataNow { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public string Time => DataNow.ToString("HH:mm:ss");//时间

        #region 市电
        public string MainsVoltage { get; set; }//市电电压

        public string MainsFrequency { get; set; }//市电频率

        public string ACPower { get; set; }//市电功率
        #endregion

        #region 输出      
        public string OutVolt { get; set; }//输出电压

        public string OutFreq { get; set; }//输出频率

        public string ApparentPwr { get; set; }//视在功率

        public string ActivePwr { get; set; } //有功功率

        public string LoadPercent { get; set; }//负载百分比

        public string FullloadPwr { get; set; }//满载有功功率


        public string Outputpowerfactor { get; set; }//输出功率因数


        public string BypasshighDropout { get; set; }//旁路高退电压
        public string BypasslowDropout { get; set; }//旁路低退电压

        public string DCOffset { get; set; }//直流分量

        public string InductorCurr { get; set; }//电感电流

        public string InductorPwr { get; set; }//电感功率

        public string RatedPwr { get; set; }//满载有功功率
        #endregion

        #region 机器状态
        public string PVTemp { get; set; }//PV温度

        public string FaultCode { get; set; }//故障代码

        public string InvTemp { get; set; }//逆变温度

        public string BoostTemp { get; set; }//升压温度

        public string XfmrTemp { get; set; }//变压器温度

        public string AutoRestartAC { get; set; }//市电自动重启
        public string ECOMode { get; set; }//ECO模式
        public string PassFunctionEnable { get; set; }//旁路使能
        public string Frequencyrestrictionmode { get; set; }//频率限制模式

        public string MaxTemp { get; set; }//当前最高温度

        public string PFCStatus { get; set; }//PFC工作状态

        public string FanSpeed { get; set; }//风扇转速

        public string FanEnable { get; set; }//风扇使能

        public string Mode { get; set; } //模式

        public string PVToLoadAC { get; set; }//AC状态下PV馈能到负载

        public string BattDisconnected { get; set; }//电池未接

        public string OutputOverload { get; set; }//输出过载

        public string OverTemp { get; set; }//机器过温

        public string BattLowAlarm { get; set; }//电池低电报警

        public string EEPROM_DataErr { get; set; }//EEPROM数据异常

        public string EEPROM_IOErr { get; set; }//EEPROM读写异常

        public string InputOV { get; set; } //输入电压过高

        public string BattOV { get; set; } //电池电压过高

        public string FanSpeedFault { get; set; }//风扇转速异常

        public string OutputStatus { get; set; }//机器是否有输出

        public string PVLowPwrFault { get; set; }//PV功率过低异常

        public string AutoStartEnable { get; set; }//自动开机使能

        public string ChgStage { get; set; } //充电阶段

        public string ParallelUnits { get; set; } //并机系统里机器的总数

        public string ParallelRole { get; set; } //并机系统中角色

        public string GridTieFlag { get; set; } //并网标志

        public string MainRelayStat { get; set; } //主输出继电器状态

        public string SecOutStat { get; set; } //第二输出当前状态

        public string BMS_ComFault { get; set; } //BMS通讯异常

        public string TempSensorFault { get; set; } //温度传感器异常

        public string ACLED { get; set; } //市电灯状态

        public string InvLED { get; set; } //逆变灯状态

        public string ChgLED { get; set; } //充电灯状态

        public string AlarmLED { get; set; } //报警灯状态

        public string InvStatus { get; set; } //逆变器工作状态

        public string PVVoltStatus { get; set; } //PV电压状态

        public string InvBridgeStatus { get; set; } //逆变桥状态

        public string MPPTStatus { get; set; } //MPPT状态

        public string PLLStatus { get; set; } //锁相环状态
        #endregion

        #region PV
        public string PVVolt { get; set; }//PV电压

        public string PVVolt2 { get; set; }//PV2电压

        public string DailyGen { get; set; } //日发电量

        public string PVCurr { get; set; } //PV电流

        public string PVCurr2 { get; set; }//PV2电流

        public string MonthlyGen { get; set; }//月发电量

        public string PVPwr { get; set; }//PV功率

        public string PVPwr2 { get; set; }  //PV2功率

        public string AnnualGen { get; set; }//年发电量

        public string TotalGen { get; set; }//总发电量
        #endregion

        #region BMS
        public string ProtocolType { get; set; } //协议类型

        public string BMS_ComOK { get; set; }//BMS通信正常

        public string BMS_LowBattAlarm { get; set; }//BMS低电报警

        public string BMS_LowBattFault { get; set; }//BMS低电故障

        public string BMS_ChgEnable { get; set; }//BMS允许充电

        public string BMS_DisEnable { get; set; } //BMS允许放电

        public string BMS_ChgOC { get; set; }//BMS充电过流

        public string BMS_DisOC { get; set; } //BMS放电过流

        public string BMS_UnderTemp { get; set; }//BMS温度过低

        public string BMS_OverTemp { get; set; }//BMS温度过高

        public string BMS_ChgVoltLimit { get; set; }//BMS充电电压限制

        public string BMS_DisVoltLimit { get; set; }//BMS放电电压限制

        public string BMS_ChgCurrLimit { get; set; }//BMS充电电流限制

        public string BMS_SOC { get; set; }//BMS当前SOC

        public string BMS_AvgTemp { get; set; }//BMS平均温度

        public string BMS_ChgCurr { get; set; }//BMS充电电流

        public string BMS_DisCurr { get; set; } //BMS放电电流
        #endregion

        #region BAT       
        public string BatCurr { get; set; }//电池电流

        public string BattVolt { get; set; }//电池电压

        public string BattCapacity { get; set; }//电池容量

        public string BusVolt { get; set; }//母线电压
        public string NBusVolt { get; set; }//N母线电压

        public string BattCells { get; set; }//电池节数

        public string BatteryType { get; set; } //电池类型

        public string BattChgCurr { get; set; }//电池充电电流

        public string BattDisCurr { get; set; }//电池放电电流

        public string ChgMasterSW { get; set; }//充电总开关

        public string ACChgSW { get; set; }//AC充电开关

        public string SolarChgSW { get; set; }//太阳能充电开关
        #endregion

        #region 并网功能配置
        public string GridConnectedFunction { get; set; } //并网功能

        public string CT_Enable { get; set; }//CT功能开关

        public string PV_GridConnectionProtocol { get; set; }//并网协议

        public string GridCurrent { get; set; }//并网电流

        public string ZeroAdjPwr { get; set; } //调零功率

        public string CTCurr { get; set; } //CT电流

        public string CTPwr { get; set; }//CT功率

        public string MaxInvPower { get; set; }//当前允许最大逆变功率
        #endregion

        #region Time
        public string BoostTime { get; set; } //强充时间

        public string InvTime { get; set; }//逆变时间

        public string LoadTime { get; set; } //带载时间
        public string BatteryloadlimitTime { get; set; }//电池带载限制时间
        public string BatterydischargelimitTime { get; set; }//电池放电限制时间
        #endregion
    }
}
