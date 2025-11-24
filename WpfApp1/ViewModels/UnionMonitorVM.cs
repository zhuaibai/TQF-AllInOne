using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Models;

namespace WpfApp1.ViewModels
{
    public class UnionMonitorVM :BaseViewModel
    {
        private ObservableCollection<UnionMonitorM> unionList;

        public ObservableCollection<UnionMonitorM> UnionList
        {
            get { return unionList; }
            set
            {
                unionList = value;
                this.RaiseProperChanged(nameof(UnionList));
            }
        }

        public UnionMonitorVM()
        {
            UnionList = new ObservableCollection<UnionMonitorM>();
            for(int i = 0; i < 16; i++)
            {
                UnionList.Add(new UnionMonitorM() { pack = i + 1 });
            }
        }

        /// <summary>
        /// 设置第i项属性值
        /// </summary>
        /// <param name="unionMonitorM"></param>
        /// <param name="index"></param>
        public void setElement(short[] data,int index)
        {
            if (data == null || data.Length < 33) 
            {
                return;
            }
            //十六节电芯电压
            UnionList[index].Cell1 = data[0];
            UnionList[index].Cell2 = data[1];
            UnionList[index].Cell3 = data[2];
            UnionList[index].Cell4 = data[3];
            UnionList[index].Cell5 = data[4];
            UnionList[index].Cell6 = data[5];
            UnionList[index].Cell7 = data[6];
            UnionList[index].Cell8 = data[7];
            UnionList[index].Cell9 = data[8];
            UnionList[index].Cell10 = data[9];
            UnionList[index].Cell11 = data[10];
            UnionList[index].Cell12 = data[11];
            UnionList[index].Cell13 = data[12];
            UnionList[index].Cell14 = data[13];
            UnionList[index].Cell15 = data[14];
            UnionList[index].Cell16 = data[15];

            //
            UnionList[index].MaxVolt = data[16];
            UnionList[index].MinVolt = data[17];
            UnionList[index].AvgVolt = data[18];
            UnionList[index].TotalVolt = data[19];
            UnionList[index].Current = data[21];

            //
            UnionList[index].CellTemp1 = data[23].ToString();
            UnionList[index].CellTemp2 = data[24].ToString();
            UnionList[index].CellTemp3 = data[25].ToString();
            UnionList[index].CellTemp4 = data[26].ToString();

            //
            UnionList[index].MosTemp = data[27];
            UnionList[index].BatBackTemp = data[28].ToString();
            UnionList[index].SOC = data[29].ToString();
            UnionList[index].SOH = data[30].ToString();
            UnionList[index].RemainCap = data[31].ToString();
            UnionList[index].FullCap = data[32].ToString();





        }

    }
}
