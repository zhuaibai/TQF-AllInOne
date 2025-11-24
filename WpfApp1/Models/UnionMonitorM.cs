using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    public class UnionMonitorM
    {
        public int pack { get; set; }
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
        public string RemainCap { get; set; }
        public string FullCap { get; set; }
        public double RatedCap { get; set; }
        public double MosTemp { get; set; }
        public string BatBackTemp { get; set; }
        public string CellTemp1 { get; set; }
        public string CellTemp2 { get; set; }
        public string CellTemp3 { get; set; }
        public string CellTemp4 { get; set; }
        //SOC
        public string SOC { get; set; }
        //SOH
        public string SOH { get; set; }
    }
}
