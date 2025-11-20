using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.Win32;
using System.Windows;
using WpfApp1.Command.BMS;

namespace WpfApp1.Convert
{
    public static class ExcelExportHelper
    {
        public static void ExportHistoryToExcel(ObservableCollection<HistoryLodModel> historyLodModels)
        {
            if (historyLodModels == null || historyLodModels.Count == 0)
            {
                MessageBox.Show("没有数据可导出。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // 打开保存对话框
            var saveDialog = new SaveFileDialog
            {
                Title = "导出 Excel",
                Filter = "Excel 文件 (*.xlsx)|*.xlsx",
                FileName = "历史记录.xlsx"
            };

            if (saveDialog.ShowDialog() != true)
                return;

            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("HistoryLog");

                    var ws = workbook.Worksheets.Add("历史记录");
                    // 写表头
                    int col = 1;
                    ws.Cell(1, col++).Value = "历史记录数";
                    ws.Cell(1, col++).Value = "16节电芯电压(V)";
                    ws.Cell(1, col++).Value = "最高单体电压(V)";
                    ws.Cell(1, col++).Value = "最低单体电压(V)";
                    ws.Cell(1, col++).Value = "电芯压差(mV)";
                    ws.Cell(1, col++).Value = "电流(A)";
                    ws.Cell(1, col++).Value = "总电压(V)";
                    ws.Cell(1, col++).Value = "MOS温度(℃)";
                    ws.Cell(1, col++).Value = "累计充电容量(mAh)";
                    ws.Cell(1, col++).Value = "NTC温度(℃)";
                    ws.Cell(1, col++).Value = "SOC(%)";
                    ws.Cell(1, col++).Value = "SOH(%)";
                    ws.Cell(1, col++).Value = "剩余容量(mAh)";
                    ws.Cell(1, col++).Value = "满充容量(mAh)";
                    ws.Cell(1, col++).Value = "循环次数";
                    ws.Cell(1, col++).Value = "BMS告警";
                    ws.Cell(1, col++).Value = "BMS保护";
                    ws.Cell(1, col++).Value = "BMS错误";
                    ws.Cell(1, col++).Value = "BMS状态";

                    for (int i = 0; i < 16; i++)
                        ws.Cell(1, col++).Value = getElement(i);

                    ws.Cell(1, col++).Value = "AFE保护状态";

                    // 写数据
                    int row = 2;
                    foreach (var item in historyLodModels)
                    {
                        col = 1;
                        ws.Cell(row, col++).Value = item.Time;
                        ws.Cell(row, col++).Value = item.CellVoltage;
                        ws.Cell(row, col++).Value = item.MaxCellVolt;
                        ws.Cell(row, col++).Value = item.MinCellVolt;
                        ws.Cell(row, col++).Value = item.CellDiff;
                        ws.Cell(row, col++).Value = item.Current;
                        ws.Cell(row, col++).Value = item.PackVoltage;
                        ws.Cell(row, col++).Value = item.MOSTemp;
                        ws.Cell(row, col++).Value = item.TotalChgCap;
                        ws.Cell(row, col++).Value = item.NTCTemp;
                        ws.Cell(row, col++).Value = item.SOC;
                        ws.Cell(row, col++).Value = item.SOH;
                        ws.Cell(row, col++).Value = item.RemainCap;
                        ws.Cell(row, col++).Value = item.FullCap;
                        ws.Cell(row, col++).Value = item.CycleCount;
                        ws.Cell(row, col++).Value = item.BMS_Alarm;
                        ws.Cell(row, col++).Value = item.BMS_Protect;
                        ws.Cell(row, col++).Value = item.BMS_Error;
                        ws.Cell(row, col++).Value = item.BMS_Status;

                        for (int i = 0; i < 16; i++)
                            ws.Cell(row, col++).Value = item.ProtectCount[i];

                        ws.Cell(row, col++).Value = item.AFE_ProtStatus;
                        row++;
                    }

                    // 自动调整列宽
                    worksheet.Columns().AdjustToContents();

                    workbook.SaveAs(saveDialog.FileName);

                }

                MessageBox.Show("导出成功！", "完成", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static string getElement(int index)
        {
            switch (index)
            {
                case 0:
                    return "单体过压触发次数";
                case 1:
                    return "单体欠压触发次数";
                case 2:
                    return "总体过压触发次数";
                case 3:
                    return "总体欠压触发次数";
                case 4:
                    return "充电过流触发次数";
                case 5:
                    return "放电过流触发次数";
                case 6:
                    return "放电短路触发次数";
                case 7:
                    return "充电高温触发次数";
                case 8:
                    return "充电低温触发次数";
                case 9:
                    return "放电低温触发次数";
                case 11:
                    return "放电高温触发次数";
                case 13:
                    return "满充保护次数";
                default:
                    return "预留";
            }
        }
    }
}
