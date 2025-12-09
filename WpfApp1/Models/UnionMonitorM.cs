using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    //public class UnionMonitorM
    //{
    //    public int pack { get; set; }
    //    //日期
    //    public DateTime Date { get; set; }
    //    //时间
    //    public string Time => Date.ToString("HH:mm:ss");
    //    //总压
    //    public double TotalVolt { get; set; }
    //    //电流
    //    public double Current { get; set; }
    //    //电芯电压
    //    public double Cell1 { get; set; }
    //    public double Cell2 { get; set; }
    //    public double Cell3 { get; set; }
    //    public double Cell4 { get; set; }
    //    public double Cell5 { get; set; }
    //    public double Cell6 { get; set; }
    //    public double Cell7 { get; set; }
    //    public double Cell8 { get; set; }
    //    public double Cell9 { get; set; }
    //    public double Cell10 { get; set; }
    //    public double Cell11 { get; set; }
    //    public double Cell12 { get; set; }
    //    public double Cell13 { get; set; }
    //    public double Cell14 { get; set; }
    //    public double Cell15 { get; set; }
    //    public double Cell16 { get; set; }
    //    public double AvgVolt { get; set; }
    //    public double MaxVolt { get; set; }
    //    public double MinVolt { get; set; }
    //    public string RemainCap { get; set; }
    //    public string FullCap { get; set; }
    //    public double RatedCap { get; set; }
    //    public double MosTemp { get; set; }
    //    public string BatBackTemp { get; set; }
    //    public string CellTemp1 { get; set; }
    //    public string CellTemp2 { get; set; }
    //    public string CellTemp3 { get; set; }
    //    public string CellTemp4 { get; set; }
    //    //SOC
    //    public string SOC { get; set; }
    //    //SOH
    //    public string SOH { get; set; }
    //}
    public class UnionMonitorM : INotifyPropertyChanged
    {
        private int _pack;
        public int pack
        {
            get => _pack;
            set { _pack = value; OnPropertyChanged(nameof(pack)); }
        }

        //日期
        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged(nameof(Date));
                OnPropertyChanged(nameof(Time)); // Date 变了，Time 也要刷新
            }
        }

        //时间（只读）
        public string Time => Date.ToString("HH:mm:ss");

        private double _totalVolt;
        public double TotalVolt
        {
            get => _totalVolt;
            set { _totalVolt = value; OnPropertyChanged(nameof(TotalVolt)); }
        }

        private double _current;
        public double Current
        {
            get => _current;
            set { _current = value; OnPropertyChanged(nameof(Current)); }
        }

        private double _cell1;
        public double Cell1 { get => _cell1; set { _cell1 = value; OnPropertyChanged(nameof(Cell1)); } }
        private double _cell2;
        public double Cell2 { get => _cell2; set { _cell2 = value; OnPropertyChanged(nameof(Cell2)); } }
        private double _cell3;
        public double Cell3 { get => _cell3; set { _cell3 = value; OnPropertyChanged(nameof(Cell3)); } }
        private double _cell4;
        public double Cell4 { get => _cell4; set { _cell4 = value; OnPropertyChanged(nameof(Cell4)); } }
        private double _cell5;
        public double Cell5 { get => _cell5; set { _cell5 = value; OnPropertyChanged(nameof(Cell5)); } }
        private double _cell6;
        public double Cell6 { get => _cell6; set { _cell6 = value; OnPropertyChanged(nameof(Cell6)); } }
        private double _cell7;
        public double Cell7 { get => _cell7; set { _cell7 = value; OnPropertyChanged(nameof(Cell7)); } }
        private double _cell8;
        public double Cell8 { get => _cell8; set { _cell8 = value; OnPropertyChanged(nameof(Cell8)); } }
        private double _cell9;
        public double Cell9 { get => _cell9; set { _cell9 = value; OnPropertyChanged(nameof(Cell9)); } }
        private double _cell10;
        public double Cell10 { get => _cell10; set { _cell10 = value; OnPropertyChanged(nameof(Cell10)); } }
        private double _cell11;
        public double Cell11 { get => _cell11; set { _cell11 = value; OnPropertyChanged(nameof(Cell11)); } }
        private double _cell12;
        public double Cell12 { get => _cell12; set { _cell12 = value; OnPropertyChanged(nameof(Cell12)); } }
        private double _cell13;
        public double Cell13 { get => _cell13; set { _cell13 = value; OnPropertyChanged(nameof(Cell13)); } }
        private double _cell14;
        public double Cell14 { get => _cell14; set { _cell14 = value; OnPropertyChanged(nameof(Cell14)); } }
        private double _cell15;
        public double Cell15 { get => _cell15; set { _cell15 = value; OnPropertyChanged(nameof(Cell15)); } }
        private double _cell16;
        public double Cell16 { get => _cell16; set { _cell16 = value; OnPropertyChanged(nameof(Cell16)); } }

        private double _avgVolt;
        public double AvgVolt { get => _avgVolt; set { _avgVolt = value; OnPropertyChanged(nameof(AvgVolt)); } }

        private double _maxVolt;
        public double MaxVolt { get => _maxVolt; set { _maxVolt = value; OnPropertyChanged(nameof(MaxVolt)); } }

        private double _minVolt;
        public double MinVolt { get => _minVolt; set { _minVolt = value; OnPropertyChanged(nameof(MinVolt)); } }

        private double _remainCap;
        public double RemainCap { get => _remainCap; set { _remainCap = value; OnPropertyChanged(nameof(RemainCap)); } }

        private double _fullCap;
        public double FullCap { get => _fullCap; set { _fullCap = value; OnPropertyChanged(nameof(FullCap)); } }

        private double _ratedCap;
        public double RatedCap { get => _ratedCap; set { _ratedCap = value; OnPropertyChanged(nameof(RatedCap)); } }

        private double _mosTemp;
        public double MosTemp { get => _mosTemp; set { _mosTemp = value; OnPropertyChanged(nameof(MosTemp)); } }

        private double _batBackTemp;
        public double BatBackTemp { get => _batBackTemp; set { _batBackTemp = value; OnPropertyChanged(nameof(BatBackTemp)); } }

        private string _cellTemp1;
        public string CellTemp1 { get => _cellTemp1; set { _cellTemp1 = value; OnPropertyChanged(nameof(CellTemp1)); } }
        private string _cellTemp2;
        public string CellTemp2 { get => _cellTemp2; set { _cellTemp2 = value; OnPropertyChanged(nameof(CellTemp2)); } }
        private string _cellTemp3;
        public string CellTemp3 { get => _cellTemp3; set { _cellTemp3 = value; OnPropertyChanged(nameof(CellTemp3)); } }
        private string _cellTemp4;
        public string CellTemp4 { get => _cellTemp4; set { _cellTemp4 = value; OnPropertyChanged(nameof(CellTemp4)); } }

        private double _soc;
        public double SOC { get => _soc; set { _soc = value; OnPropertyChanged(nameof(SOC)); } }

        private double _soh;
        public double SOH { get => _soh; set { _soh = value; OnPropertyChanged(nameof(SOH)); } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
