using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;
using OfficeOpenXml;
using WpfApp1.Command;
using WpfApp1.Command.BMS;
using WpfApp1.Models;


namespace WpfApp1.ViewModels
{
    public class RealTimeMonitorVM : BaseViewModel
    {
       

        private ObservableCollection<PollingData> pllingList;

        public ObservableCollection<PollingData> PollingList
        {
            get { return pllingList; }
            set
            {
                pllingList = value;
                this.RaiseProperChanged(nameof(PollingList));
            }
        }


        private DispatcherTimer timer;
        public bool _isSaving = false;
        public string _savePath = null;

        public bool IsSaving
        {
            get => _isSaving;
            set
            {
                _isSaving = value;
                OnPropertyChanged(nameof(IsSaving));

                if (_isSaving)
                    SelectSavePath();
                else
                    _savePath = null;
            }
        }

        public RealTimeMonitorVM()
        {
            ExcelPackage.License.SetNonCommercialPersonal("LFQApp");

        }

        public RelayCommand ClearData { get
            {
                return new RelayCommand(clearData);
            } }
        private void clearData()
        {
            App.Current.Dispatcher.Invoke(()=> PollingList.Clear());
            
        }

        /// <summary>
        /// 选择保存复选框
        /// </summary>
        private void SelectSavePath()
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "Excel 文件 (*.xlsx)|*.xlsx";
            dialog.FileName = "实时监控记录.xlsx";

            if (dialog.ShowDialog() == true)
            {
                _savePath = dialog.FileName;
                CreateExcelHeader();
            }
            else
            {
                IsSaving = false; // 用户取消保存
            }
        }

        //创建Excel表头
        private void CreateExcelHeader()
        {
            if (File.Exists(_savePath) && IsFileLocked(_savePath))
            {
                MessageBox.Show("Excel 正在打开，请先关闭它再继续保存。");
                return;
            }
            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Data");

            string[] headers = new[]
            {
            "日期","时间","总电压(V)","电流(A)","SOC(%)","SOH(%)","满充容量(mAH)","剩余容量(mAH)",  "循环次数",
            "Cell1(V)","Cell2(V)","Cell3(V)","Cell4(V)",
            "最大电芯压差(V)","最高电压(V)","最低电压(V)",
            "温度1(℃)","充电MOS(C_FET)","放电MOS(D_FET)","充电状态","放电状态",
            "AFE过充保护","AFE过放保护","充电过流保护","放电过流保护","前端芯片中断","短路保护","前端芯片触发保护","前端芯片告警下拉",
            "BalanceStatus","AlarmStatus","ProtectStatus", "ErrorStatus"
            };

            for (int i = 0; i < headers.Length; i++)
                sheet.Cells[1, i + 1].Value = headers[i];

            package.SaveAs(new FileInfo(_savePath));
        }

        /// <summary>
        /// 文件是否正在被使用
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private bool IsFileLocked(string filePath)
        {
            try
            {
                using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    return false; // 没有被占用
                }
            }
            catch (IOException)
            {
                return true; // 被占用
            }
        }

        /// <summary>
        /// 保存到Excel中
        /// </summary>
        /// <param name="d"></param>
        public void SaveToExcel(PollingData d)
        {
            if (File.Exists(_savePath) && IsFileLocked(_savePath))
            {
                MessageBox.Show("Excel 正在打开，请先关闭它再继续保存。");
                return;
            }
            var file = new FileInfo(_savePath);

            using var package = new ExcelPackage(file);
            var sheet = package.Workbook.Worksheets[0];

            int row = sheet.Dimension.End.Row + 1;

            sheet.Cells[row, 1].Value = d.Date.ToString("yyyy-MM-dd");
            sheet.Cells[row, 2].Value = d.Time;
            sheet.Cells[row, 3].Value = d.TotalVolt;
            sheet.Cells[row, 4].Value = d.Current;
            sheet.Cells[row, 5].Value = d.SOC;
            sheet.Cells[row, 6].Value = d.SOH;
            sheet.Cells[row, 7].Value = d.FullCap;
            sheet.Cells[row, 8].Value = d.FullRemainCap;
            sheet.Cells[row, 9].Value = d.CycleCount;
            sheet.Cells[row, 10].Value = d.Cell1;
            sheet.Cells[row, 11].Value = d.Cell2;
            sheet.Cells[row, 12].Value = d.Cell3;
            sheet.Cells[row, 13].Value = d.Cell4;
            sheet.Cells[row, 14].Value = d.AvgVolt;
            sheet.Cells[row, 15].Value = d.MaxVolt;
            sheet.Cells[row, 16].Value = d.MinVolt;
            sheet.Cells[row, 17].Value = d.Temp1;
            sheet.Cells[row, 18].Value = d.Chg_MOS;
            sheet.Cells[row, 19].Value = d.Dis_MOS;
            sheet.Cells[row, 20].Value = d.Chg_Statues;
            sheet.Cells[row, 21].Value = d.Dis_Statues;
            sheet.Cells[row, 22].Value = d.AFE_OverChg_Pro;
            sheet.Cells[row, 23].Value = d.AFE_OverDis_Pro;
            sheet.Cells[row, 24].Value = d.Chg_Current_Pro;
            sheet.Cells[row, 25].Value = d.Dis_Current_Pro;
            sheet.Cells[row, 26].Value = d.AFE_Interrupt;
            sheet.Cells[row, 27].Value = d.ShortProtect;
            sheet.Cells[row, 28].Value = d.AFE_TriggerProt;
            sheet.Cells[row, 29].Value = d.AFE_AlertPull;
            sheet.Cells[row, 30].Value = d.BalanceStatus;
            sheet.Cells[row, 31].Value = d.AlarmStatus;
            sheet.Cells[row, 32].Value = d.ProtectStatus;
            sheet.Cells[row, 33].Value = d.ErrorStatus;

            package.Save();
        }
    }
}
