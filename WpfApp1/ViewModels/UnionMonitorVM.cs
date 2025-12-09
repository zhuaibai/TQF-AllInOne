using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Models;

namespace WpfApp1.ViewModels
{
    public class UnionMonitorVM : BaseViewModel
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
            for (int i = 0; i < 16; i++)
            {
                UnionList.Add(new UnionMonitorM() { pack = i + 1 });
            }
        }

        /// <summary>
        /// 设置第i项属性值
        /// </summary>
        /// <param name="unionMonitorM"></param>
        /// <param name="index"></param>
        public void setElement(short[] data, int index)
        {
            index = index - 1;
            if (data == null || data.Length < 33)
            {
                //十六节电芯电压
                UnionList[index].Cell1 = 0;
                UnionList[index].Cell2 = 0;
                UnionList[index].Cell3 = 0;
                UnionList[index].Cell4 = 0;
                UnionList[index].Cell5 = 0;
                UnionList[index].Cell6 = 0;
                UnionList[index].Cell7 = 0;
                UnionList[index].Cell8 = 0;
                UnionList[index].Cell9 = 0;
                UnionList[index].Cell10 = 0;
                UnionList[index].Cell11 = 0;
                UnionList[index].Cell12 = 0;
                UnionList[index].Cell13 = 0;
                UnionList[index].Cell14 = 0;
                UnionList[index].Cell15 = 0;
                UnionList[index].Cell16 = 0;

                //
                UnionList[index].MaxVolt = 0;
                UnionList[index].MinVolt = 0;
                UnionList[index].AvgVolt = 0;
                UnionList[index].TotalVolt = 0;
                UnionList[index].Current = 0;

                //
                UnionList[index].CellTemp1 = "0";
                UnionList[index].CellTemp2 = "0";
                UnionList[index].CellTemp3 = "0";
                UnionList[index].CellTemp4 = "0";

                //
                UnionList[index].MosTemp = 0;
                UnionList[index].BatBackTemp = 0;
                UnionList[index].SOC = 0;
                UnionList[index].SOH = 0;
                UnionList[index].RemainCap = 0;
                UnionList[index].FullCap = 0;
                return;
            }

            //十六节电芯电压
            UnionList[index].Cell1 = data[0] / 1000.0;
            UnionList[index].Cell2 = data[1] / 1000.0;
            UnionList[index].Cell3 = data[2] / 1000.0;
            UnionList[index].Cell4 = data[3] / 1000.0;
            UnionList[index].Cell5 = data[4] / 1000.0;
            UnionList[index].Cell6 = data[5] / 1000.0;
            UnionList[index].Cell7 = data[6] / 1000.0;
            UnionList[index].Cell8 = data[7] / 1000.0;
            UnionList[index].Cell9 = data[8] / 1000.0;
            UnionList[index].Cell10 = data[9] / 1000.0;
            UnionList[index].Cell11 = data[10] / 1000.0;
            UnionList[index].Cell12 = data[11] / 1000.0;
            UnionList[index].Cell13 = data[12] / 1000.0;
            UnionList[index].Cell14 = data[13] / 1000.0;
            UnionList[index].Cell15 = data[14] / 1000.0;
            UnionList[index].Cell16 = data[15] / 1000.0;

            //
            UnionList[index].MaxVolt = data[16] / 1000.0;
            UnionList[index].MinVolt = data[17] / 1000.0;
            UnionList[index].AvgVolt = data[18];
            UnionList[index].TotalVolt = data[19] / 100.0;
            UnionList[index].Current = data[21];

            //
            UnionList[index].CellTemp1 = (data[23] / 10.0).ToString("F1");
            UnionList[index].CellTemp2 = (data[24] / 10.0).ToString("F1");
            UnionList[index].CellTemp3 = (data[25] / 10.0).ToString("F1");
            UnionList[index].CellTemp4 = (data[26] / 10.0).ToString("F1");

            //
            UnionList[index].MosTemp = data[27] / 10.0;
            UnionList[index].BatBackTemp = data[28] / 10.0;
            UnionList[index].SOC = data[29] / 100.0;
            UnionList[index].SOH = data[30] / 100.0;
            UnionList[index].RemainCap = data[31]/100.0;
            UnionList[index].FullCap = data[32]/100.0;
        }

    }
}
