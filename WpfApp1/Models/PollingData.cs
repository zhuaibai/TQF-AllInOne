
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    public class PollingData
    {
        //日期
        public DateTime Date { get; set; }
        //时间
        public string Time => Date.ToString("HH:mm:ss");
        //总压
        public double TotalVolt { get; set; }
        //电流
        public double Current { get; set; }
        //电芯电压
        public double Cell1 { get; set; }
        public double Cell2 { get; set; }
        public double Cell3 { get; set; }
        public double Cell4 { get; set; }
        public double Cell5 { get; set; }
        public double Cell6 { get; set; }
        public double Cell7 { get; set; }
        public double Cell8 { get; set; }
        public double Cell9 { get; set; }
        public double Cell10 { get; set; }
        public double Cell11 { get; set; }
        public double Cell12 { get; set; }
        public double Cell13 { get; set; }
        public double Cell14 { get; set; }
        public double Cell15 { get; set; }
        public double Cell16 { get; set; }
        public double AvgVolt { get; set; }
        public double MaxVolt { get; set; }
        public double MinVolt { get; set; }
        public double RSOC { get; set; }
        public double RemainCap { get; set; }
        public double RatedCap { get; set; }
        public int CycleCount { get; set; }
        public double Temp1 { get; set; }
        public double Temp2 { get; set; }
        public double Temp3 { get; set; }
        public double Temp4 { get; set; }
        public string CFET { get; set; }
        public string DFET { get; set; }
        public string ProtectStatus { get; set; }
        public string BalanceStatus { get; set; }
        public string FullCap { get; set; }
        public string FullRemainCap { get; set; }
        public double BalanceCurrent { get; set; }
        public string AlarmStatus { get; set; }
        //SOC
        public string SOC { get; set; }
        //SOH
        public string SOH { get; set; }
        //充电MOS
        public string Chg_MOS { get; set; }
        //放电MOS
        public string Dis_MOS { get; set; }
        //充电
        public string Chg_Statues { get; set; }
        //放电
        public string Dis_Statues { get; set; }
        //AFE过充保护
        public string AFE_OverChg_Pro { get; set; }
        //AFE过放保护
        public string AFE_OverDis_Pro { get; set; }
        //充电过流保护
        public string Chg_Current_Pro { get; set; }
        //放电过流保护
        public string Dis_Current_Pro { get; set; }
        //前端芯片中断
        public string AFE_Interrupt { get; set; }
        //短路保护
        public string ShortProtect { get; set; }
        //前端芯片触发保护
        public string AFE_TriggerProt { get; set; }
        //前端芯片告警下拉
        public string AFE_AlertPull { get; set; }
        //错误信息
        public string ErrorStatus { get; set; }








    }
}
