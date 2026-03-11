using Microsoft.Win32;
using OfficeOpenXml;
using System;
<<<<<<< HEAD
using System.Diagnostics.CodeAnalysis;
=======
>>>>>>> new
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
<<<<<<< HEAD
// removed unused usings
=======
using System.Text;
using System.Threading.Tasks;
>>>>>>> new
using System.Windows;
using System.Windows.Input;
using WpfApp1.Command;
using WpfApp1.Models;

namespace WpfApp1.ViewModels
{
    public class DataRecrodingVM : BaseViewModel
    {
        #region 基础配置
        /// <summary>
        /// 设置数据列表
        /// </summary>
<<<<<<< HEAD
        private ObservableCollection<Common_Data> _CommonDataList = new ObservableCollection<Common_Data>();
=======
        private ObservableCollection<Common_Data> _CommonDataList;
>>>>>>> new
        public ObservableCollection<Common_Data> CommonDataList
        {
            get { return _CommonDataList; }
            set
            {
                _CommonDataList = value;
                this.RaiseProperChanged(nameof(CommonDataList));
            }
        }

        /// <summary>
        /// 获取通行证
        /// </summary>
        public DataRecrodingVM() => ExcelPackage.License.SetNonCommercialPersonal("LFQApp");
        #endregion

        #region 配置类定义
<<<<<<< HEAD
        // can be null when not saving
        public string? _savePath;
        private bool _isSaving = false;
        private bool _isFileLock = false;
        private int _retryCount = 0;
        // MAX_RETRY_COUNT removed (unused)
        // 设备Excel配置类
        public class DeviceExcelConfig
        {
            public string DeviceName { get; set; } = string.Empty;
            public List<ColumnGroup> ColumnGroups { get; set; } = new List<ColumnGroup>();
=======
        public string _savePath;
        private bool _isSaving = false;
        private bool _isFileLock = false;
        private int _retryCount = 0;
        private const int MAX_RETRY_COUNT = 2;
        // 设备Excel配置类
        public class DeviceExcelConfig
        {
            public string DeviceName { get; set; }
            public List<ColumnGroup> ColumnGroups { get; set; }
>>>>>>> new
        }

        // 列分组类
        public class ColumnGroup
        {
<<<<<<< HEAD
            public string GroupName { get; set; } = string.Empty;
            public System.Drawing.Color GroupColor { get; set; }
            public List<ColumnInfo> Columns { get; set; } = new List<ColumnInfo>();
=======
            public string GroupName { get; set; }
            public System.Drawing.Color GroupColor { get; set; }
            public List<ColumnInfo> Columns { get; set; }
>>>>>>> new
        }

        // 列信息类
        public class ColumnInfo
        {
<<<<<<< HEAD
            public string HeaderName { get; set; } = string.Empty;     // 表头名称
            public string DataProperty { get; set; } = string.Empty;   // 数据属性名
            public int ColumnWidth { get; set; } = 15;       // 列宽
            public string? Format { get; set; }         // 数据格式

            public ColumnInfo(string headerName, string dataProperty, int columnWidth = 15, string? format = null)
=======
            public string HeaderName { get; set; }     // 表头名称
            public string DataProperty { get; set; }   // 数据属性名
            public int ColumnWidth { get; set; }       // 列宽
            public string Format { get; set; }         // 数据格式

            public ColumnInfo(string headerName, string dataProperty, int columnWidth = 15, string format = null)
>>>>>>> new
            {
                HeaderName = headerName;
                DataProperty = dataProperty;
                ColumnWidth = columnWidth;
                Format = format;
            }
        }
        #endregion

        #region HPVINV02/06的保存命令和配置
<<<<<<< HEAD
        private ICommand? _HPVINVSaveCommand;
        public ICommand HPVINVSaveCommand => _HPVINVSaveCommand ??= new RelayCommand(() => StartSavingWithConfig(GetHPVINVConfig()));
=======
        private ICommand _HPVINVSaveCommand;
        public ICommand HPVINVSaveCommand
        {
            get
            {
                return _HPVINVSaveCommand ?? (_HPVINVSaveCommand = new RelayCommand(() => StartSavingWithConfig(GetHPVINVConfig())));
            }
        }
>>>>>>> new

        /// <summary>
        /// 配置数据和头部
        /// </summary>
        /// <returns></returns>
        private static DeviceExcelConfig GetHPVINVConfig()
        {
            var config = new DeviceExcelConfig
            {
                DeviceName = "HPVINV",
                ColumnGroups = new List<ColumnGroup>
        {
            new ColumnGroup
            {
                GroupName = "基本信息",
                GroupColor = System.Drawing.Color.LightGray,
                Columns = new List<ColumnInfo>
                {
                    new ("日期", "DataNow", 12),
                    new ("时间", "Time", 10)
                }
            },
            new ColumnGroup
            {
                GroupName = "市电",
                GroupColor = System.Drawing.Color.LightBlue,
                Columns = new List<ColumnInfo>
                {
                    new ("市电电压(V)", "MainsVoltage", 15, "0.0"),
                    new ("市电频率(Hz)", "MainsFrequency", 15, "0.0"),
                    new ("市电功率(W)", "ACPower", 15, "0")
                }
            },
            new ColumnGroup
            {
                GroupName = "输出",
                GroupColor = System.Drawing.Color.LightGreen,
                Columns = new List<ColumnInfo>
                {
                    new ("输出电压(V)", "OutVolt", 15, "0.0"),
                    new ("输出频率(Hz)", "OutFreq", 15, "0.0"),
                    new ("视在功率(W)", "ApparentPwr", 15, "0"),
                    new ("有功功率(W)", "ActivePwr", 15, "0"),
                    new ("负载百分比(%)", "LoadPercent", 15, "0%")
                }
            },
            new ColumnGroup
            {
                GroupName = "PV",
                GroupColor = System.Drawing.Color.LightYellow,
                Columns = new List<ColumnInfo>
                {
                    new ("PV电压(V)", "PVVolt", 15, "0.0"),
                    new ("PV功率(W)", "PVPwr", 15, "0"),
                    new ("PV电流(A)", "PVCurr", 15, "0.0"),
                    new ("日发电量(kW·h)", "DailyGen", 15, "0.00"),
                    new ("月发电量(kW·h)", "MonthlyGen", 15, "0.00"),
                    new ("年发电量(kW·h)", "AnnualGen", 15, "0.00"),
                    new ("总发电量(kW·h)", "TotalGen", 15, "0.00")
                }
            },
            new ColumnGroup
            {
                GroupName = "机器状态",
                GroupColor = System.Drawing.Color.LightSlateGray,
                Columns = new List<ColumnInfo>
                {
                    new ("故障代码", "FaultCode", 15),
                    new ("PV温度(℃)", "PVTemp", 15, "0.0"),
                    new ("逆变温度(℃)", "InvTemp", 15, "0.0"),
                    new ("升压温度(℃)", "BoostTemp", 15, "0.0"),
                    new ("变压器温度(℃)", "XfmrTemp", 15, "0.0"),
                    new ("当前最高温度(℃)", "MaxTemp", 15,"0.0"),
                    new ("风扇转速", "FanSpeed", 15),
                    new ("风扇使能", "FanEnable", 15),
                    new ("模式", "Mode", 15),
                    new ("AC状态下PV馈能到负载", "PVToLoadAC", 15),
                    new ("机器是否有输出", "OutputStatus", 15),
                    new ("电池低电报警", "BattLowAlarm", 15),
                    new ("电池未接", "BattDisconnected", 15),
                    new ("输出过载", "OutputOverload", 15),
                    new ("机器过温", "OverTemp", 15),
                    new ("EEPROM数据异常", "EEPROM_DataErr", 15),
                    new ("EEPROM读写异常", "EEPROM_IOErr", 15),
                    new ("PV功率过低异常", "PVLowPwrFault", 15),
                    new ("输入电压过高", "InputOV", 15),
                    new ("电池电压过高", "BattOV", 15),
                    new ("风扇转速异常", "FanSpeedFault", 15),
                    new ("并机系统里机器的总数", "ParallelUnits", 15),
                    new("并网标志", "GridTieFlag", 15),
                    new ("并机系统中角色", "ParallelRole", 15),
                    new ("主输出继电器状态", "MainRelayStat", 15),
                    new ("第二输出当前状态", "SecOutStat", 15),
                    new ("BMS通讯异常", "BMS_ComFault", 15),
                    new ("温度传感器异常", "TempSensorFault", 15),
                    new ("市电灯状态", "ACLED", 15),
                    new ("逆变灯状态", "InvLED", 15),
                    new ("充电灯状态", "ChgLED", 15),
                    new ("报警灯状态", "AlarmLED", 15)
                }
            },
            new ColumnGroup
            {
                GroupName = "BMS",
                GroupColor = System.Drawing.Color.LightSeaGreen,
                Columns = new List<ColumnInfo>
                {
                    new ("协议类型", "ProtocolType", 15),
                    new ("BMS通信正常", "BMS_ComOK", 15),
                    new ("BMS低电报警", "BMS_LowBattAlarm", 15),
                    new ("BMS低电故障", "BMS_LowBattFault", 15),
                    new ("BMS允许充电", "BMS_ChgEnable", 15),
                    new ("BMS允许放电", "BMS_DisEnable", 15),
                    new ("BMS充电过流", "BMS_ChgOC", 15),
                    new ("BMS放电过流", "BMS_DisOC", 15),
                    new ("BMS温度过低", "BMS_UnderTemp", 15),
                    new ("BMS温度过高", "BMS_OverTemp", 15),
                    new ("BMS平均温度", "BMS_AvgTemp", 15),
                    new ("BMS充电电流限制", "BMS_ChgCurrLimit", 15),
                    new ("BMS当前SOC", "BMS_SOC", 15),
                    new ("BMS充电电压限制", "BMS_ChgVoltLimit", 15),
                    new ("BMS放电电压限制", "BMS_DisVoltLimit", 15)
                }
            },
            new ColumnGroup
            {
                GroupName = "电池",
                GroupColor = System.Drawing.Color.LightPink,
                Columns = new List<ColumnInfo>
                {
                    new ("电池电压(V)", "BattVolt", 15, "0.0"),
                    new ("电池容量(%)", "BattCapacity", 15, "0%"),
                    new ("电池电流(A)", "BatCurr", 15, "0.0"),
                    new ("母线电压(V)", "BusVolt", 15, "0.0")
                }
            }
        }
            };

            return config;
        }
        #endregion

        #region HPVINV04的保存命令和配置
<<<<<<< HEAD
        private ICommand? _HPVINV04SaveCommand;
        public ICommand HPVINV04SaveCommand => _HPVINV04SaveCommand ??= new RelayCommand(() => StartSavingWithConfig(GetHPVINV04Config()));
=======
        private ICommand _HPVINV04SaveCommand;
        public ICommand HPVINV04SaveCommand
        {
            get
            {
                return _HPVINV04SaveCommand ?? (_HPVINV04SaveCommand = new RelayCommand(() => StartSavingWithConfig(GetHPVINV04Config())));
            }
        }
>>>>>>> new

        private static DeviceExcelConfig GetHPVINV04Config()
        {
            var config = new DeviceExcelConfig
            {
                DeviceName = "HPVINV04",
                ColumnGroups = new List<ColumnGroup>
        {
            new ColumnGroup
            {
                GroupName = "基本信息",
                GroupColor = System.Drawing.Color.LightGray,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("日期", "DataNow", 12),
                    new ColumnInfo("时间", "Time", 10)
                }
            },
            new ColumnGroup
            {
                GroupName = "市电",
                GroupColor = System.Drawing.Color.LightBlue,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("市电电压(V)", "MainsVoltage", 15, "0.0"),
                    new ColumnInfo("市电频率(Hz)", "MainsFrequency", 15, "0.0"),
                    new ColumnInfo("市电功率(W)", "ACPower", 15, "0")
                }
            },
            new ColumnGroup
            {
                GroupName = "输出",
                GroupColor = System.Drawing.Color.LightGreen,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("输出电压(V)", "OutVolt", 15, "0.0"),
                    new ColumnInfo("输出频率(Hz)", "OutFreq", 15, "0.0"),
                    new ColumnInfo("视在功率(W)", "ApparentPwr", 15, "0"),
                    new ColumnInfo("有功功率(W)", "ActivePwr", 15, "0"),
                    new ColumnInfo("负载百分比(%)", "LoadPercent", 15, "0%")
                }
            },

            new ColumnGroup
            {
                GroupName = "机器状态",
                GroupColor = System.Drawing.Color.LightGray,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("故障代码", "FaultCode", 15),
                    new ColumnInfo("PV温度(℃)", "PVTemp", 15, "0.0"),
                    new ColumnInfo("逆变温度(℃)", "InvTemp", 15, "0.0"),
                    new ColumnInfo("升压温度(℃)", "BoostTemp", 15, "0.0"),
                    new ColumnInfo("变压器温度(℃)", "XfmrTemp", 15, "0.0"),
                    new ColumnInfo("当前最高温度(℃)", "MaxTemp", 15,"0.0"),
                    new ColumnInfo("风扇转速", "FanSpeed", 15),
                    new ColumnInfo("风扇使能", "FanEnable", 15),
                    new ColumnInfo("模式", "Mode", 15),
                    new ColumnInfo("AC状态下PV馈能到负载", "PVToLoadAC", 15),
                    new ColumnInfo("机器是否有输出", "OutputStatus", 15),
                    new ColumnInfo("电池低电报警", "BattLowAlarm", 15),
                    new ColumnInfo("电池未接", "BattDisconnected", 15),
                    new ColumnInfo("输出过载", "OutputOverload", 15),
                    new ColumnInfo("机器过温", "OverTemp", 15),
                    new ColumnInfo("EEPROM数据异常", "EEPROM_DataErr", 15),
                    new ColumnInfo("EEPROM读写异常", "EEPROM_IOErr", 15),
                    new ColumnInfo("PV功率过低异常", "PVLowPwrFault", 15),
                    new ColumnInfo("输入电压过高", "InputOV", 15),
                    new ColumnInfo("电池电压过高", "BattOV", 15),
                    new ColumnInfo("风扇转速异常", "FanSpeedFault", 15),
                    new ColumnInfo("并机系统里机器的总数", "ParallelUnits", 15),
                    new ColumnInfo("并网标志", "GridTieFlag", 15),
                    new ColumnInfo("并机系统中角色", "ParallelRole", 15),
                    new ColumnInfo("主输出继电器状态", "MainRelayStat", 15),
                    new ColumnInfo("第二输出当前状态", "SecOutStat", 15),
                    new ColumnInfo("BMS通讯异常", "BMS_ComFault", 15),
                    new ColumnInfo("温度传感器异常", "TempSensorFault", 15),
                    new ColumnInfo("市电灯状态", "ACLED", 15),
                    new ColumnInfo("逆变灯状态", "InvLED", 15),
                    new ColumnInfo("充电灯状态", "ChgLED", 15),
                    new ColumnInfo("报警灯状态", "AlarmLED", 15)
                }
            },
            new ColumnGroup
            {
                GroupName = "PV",
                GroupColor = System.Drawing.Color.LightYellow,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("PV电压(V)", "PVVolt", 15, "0.0"),
                    new ColumnInfo("PV功率(W)", "PVPwr", 15, "0"),
                    new ColumnInfo("PV电流(A)", "PVCurr", 15, "0.0"),
                    new ColumnInfo("PV2电压(V)", "PV2Volt", 15, "0.0"),
                    new ColumnInfo("PV2功率(W)", "PV2Pwr", 15, "0"),
                    new ColumnInfo("PV2电流(A)", "PV2Curr", 15, "0.0"),
                    new ColumnInfo("日发电量(kW·h)", "DailyGen", 15, "0.00"),
                    new ColumnInfo("月发电量(kW·h)", "MonthlyGen", 15, "0.00"),
                    new ColumnInfo("年发电量(kW·h)", "AnnualGen", 15, "0.00"),
                    new ColumnInfo("总发电量(kW·h)", "TotalGen", 15, "0.00")
                }
            },
            new ColumnGroup
            {
                GroupName = "BMS",
                GroupColor = System.Drawing.Color.LightSeaGreen,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("协议类型", "ProtocolType", 15),
                    new ColumnInfo("BMS通信正常", "BMS_ComOK", 15),
                    new ColumnInfo("BMS低电报警", "BMS_LowBattAlarm", 15),
                    new ColumnInfo("BMS低电故障", "BMS_LowBattFault", 15),
                    new ColumnInfo("BMS允许充电", "BMS_ChgEnable", 15),
                    new ColumnInfo("BMS允许放电", "BMS_DisEnable", 15),
                    new ColumnInfo("BMS充电过流", "BMS_ChgOC", 15),
                    new ColumnInfo("BMS放电过流", "BMS_DisOC", 15),
                    new ColumnInfo("BMS温度过低", "BMS_UnderTemp", 15),
                    new ColumnInfo("BMS温度过高", "BMS_OverTemp", 15),
                    new ColumnInfo("BMS平均温度", "BMS_AvgTemp", 15),
                    new ColumnInfo("BMS充电电流限制", "BMS_ChgCurrLimit", 15),
                    new ColumnInfo("BMS当前SOC", "BMS_SOC", 15),
                    new ColumnInfo("BMS充电电压限制", "BMS_ChgVoltLimit", 15),
                    new ColumnInfo("BMS放电电压限制", "BMS_DisVoltLimit", 15),
                }
            },
           new ColumnGroup
            {
                GroupName = "电池",
                GroupColor = System.Drawing.Color.LightPink,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("电池电压(V)", "BattVolt", 15, "0.0"),
                    new ColumnInfo("电池容量(%)", "BattCapacity", 15, "0%"),
                    new ColumnInfo("电池电流(A)", "BatCurr", 15, "0.0"),
                    new ColumnInfo("母线电压(V)", "BusVolt", 15, "0.0")
                }
            }
        }
            };

            return config;
        }
        #endregion

        #region HPVINV07的保存命令和配置
<<<<<<< HEAD
        private ICommand? _HPVINV07SaveCommand;
        public ICommand HPVINV07SaveCommand => _HPVINV07SaveCommand ??= new RelayCommand(() => StartSavingWithConfig(GetHPVINV07Config()));
=======
        private ICommand _HPVINV07SaveCommand;
        public ICommand HPVINV07SaveCommand
        {
            get
            {
                return _HPVINV07SaveCommand ?? (_HPVINV07SaveCommand = new RelayCommand(() => StartSavingWithConfig(GetHPVINV07Config())));
            }
        }
>>>>>>> new

        private static DeviceExcelConfig GetHPVINV07Config()
        {
            var config = new DeviceExcelConfig
            {
                DeviceName = "HPVINV07",
                ColumnGroups = new List<ColumnGroup>
        {
            new ColumnGroup
            {
                GroupName = "基本信息",
                GroupColor = System.Drawing.Color.LightGray,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("日期", "DataNow", 12),
                    new ColumnInfo("时间", "Time", 10)
                }
            },
            new ColumnGroup
            {
                GroupName = "市电",
                GroupColor = System.Drawing.Color.LightBlue,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("市电电压(V)", "MainsVoltage", 15, "0.0"),
                    new ColumnInfo("市电频率(Hz)", "MainsFrequency", 15, "0.0"),
                    new ColumnInfo("市电功率(W)", "ACPower", 15, "0"),
                    new ColumnInfo("并网电流(A)", "GridCurrent", 15, "0.0"),
                    new ColumnInfo("调零功率(W)", "ZeroAdjPwr", 15, "0"),
                    new ColumnInfo("CT电流(A)", "CTCurr", 15, "0.0"),
                    new ColumnInfo("CT功率(W)", "CTPwr", 15, "0"),
                    new ColumnInfo("当前允许最大逆变功率(W)", "MaxInvPower", 15, "0")
                }
            },
            new ColumnGroup
            {
                GroupName = "输出",
                GroupColor = System.Drawing.Color.LightGreen,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("输出电压(V)", "OutVolt", 15, "0.0"),
                    new ColumnInfo("输出频率(Hz)", "OutFreq", 15, "0.0"),
                    new ColumnInfo("视在功率(W)", "ApparentPwr", 15, "0"),
                    new ColumnInfo("有功功率(W)", "ActivePwr", 15, "0"),
                    new ColumnInfo("负载百分比(%)", "LoadPercent", 15, "0%"),
                    new ColumnInfo("电感电流(A)", "DCOffset", 15, "0.0"),
                    new ColumnInfo("直流分量(A)", "InductorCurr", 15, "0.0"),
                    new ColumnInfo("电感功率(W)", "InductorPwr", 15, "0")
                }
            },
            new ColumnGroup
            {
                GroupName = "PV",
                GroupColor = System.Drawing.Color.LightYellow,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("PV电压(V)", "PVVolt", 15, "0.0"),
                    new ColumnInfo("PV功率(W)", "PVPwr", 15, "0"),
                    new ColumnInfo("PV电流(A)", "PVCurr", 15, "0.0"),
                    new ColumnInfo("日发电量(kW·h)", "DailyGen", 15, "0.00"),
                    new ColumnInfo("月发电量(kW·h)", "MonthlyGen", 15, "0.00"),
                    new ColumnInfo("年发电量(kW·h)", "AnnualGen", 15, "0.00"),
                    new ColumnInfo("总发电量(kW·h)", "TotalGen", 15, "0.00")
                }
            },
            new ColumnGroup
            {
                GroupName = "并网功能配置",
                GroupColor = System.Drawing.Color.LightPink,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("并网功能", "GridConnectedFunction", 15),
                    new ColumnInfo("CT功能开关", "CT_Enable", 15),
                    new ColumnInfo("并网协议", "PV_GridConnectionProtocol", 15)
                }
            },
            new ColumnGroup
            {
                GroupName = "机器状态",
                GroupColor = System.Drawing.Color.LightGray,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("母线电压(V)", "BusVolt", 15, "0.0"),
                    new ColumnInfo("逆变温度(℃)", "InvTemp", 15, "0.0"),
                    new ColumnInfo("PFC工作状态", "PFCStatus", 15),
                    new ColumnInfo("当前最高温度(℃)", "MaxTemp", 15, "0.0"),
                    new ColumnInfo("风扇转速", "FanSpeed", 15),
                    new ColumnInfo("风扇使能", "FanEnable", 15)
                }
            }
        }
            };

            return config;
        }
        #endregion

        #region HPVINV08的保存命令和配置
<<<<<<< HEAD
        private ICommand? _HPVINV08SaveCommand;
        public ICommand HPVINV08SaveCommand => _HPVINV08SaveCommand ??= new RelayCommand(() => StartSavingWithConfig(GetHPVINV08Config()));
=======
        private ICommand _HPVINV08SaveCommand;
        public ICommand HPVINV08SaveCommand
        {
            get
            {
                return _HPVINV08SaveCommand ?? (_HPVINV08SaveCommand = new RelayCommand(() => StartSavingWithConfig(GetHPVINV08Config())));
            }
        }
>>>>>>> new

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static DeviceExcelConfig GetHPVINV08Config()
        {
            var config = new DeviceExcelConfig
            {
                DeviceName = "HPVINV08",
                ColumnGroups = new List<ColumnGroup>
        {
            new ColumnGroup
            {
                GroupName = "基本信息",
                GroupColor = System.Drawing.Color.LightGray,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("日期", "DataNow", 12),
                    new ColumnInfo("时间", "Time", 10)
                }
            },
            new ColumnGroup
            {
                GroupName = "市电",
                GroupColor = System.Drawing.Color.LightBlue,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("市电电压(V)", "MainsVoltage", 15, "0.0"),
                    new ColumnInfo("市电频率(Hz)", "MainsFrequency", 15, "0.0"),
                    new ColumnInfo("市电功率(W)", "ACPower", 15, "0")
                }
            },
            new ColumnGroup
            {
                GroupName = "输出",
                GroupColor = System.Drawing.Color.LightGreen,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("输出电压(V)", "OutVolt", 15, "0.0"),
                    new ColumnInfo("输出频率(Hz)", "OutFreq", 15, "0.0"),
                    new ColumnInfo("视在功率(W)", "ApparentPwr", 15, "0"),
                    new ColumnInfo("有功功率(W)", "ActivePwr", 15, "0"),
                    new ColumnInfo("负载百分比(%)", "LoadPercent", 15, "0%"),
                    new ColumnInfo("调零功率(W)", "ZeroAdjPwr", 15, "0"),
                    new ColumnInfo("CT电流(A)", "CTCurr", 15, "0.0"),
                    new ColumnInfo("CT功率(W)", "CTPwr", 15, "0"),
                }
            },
            new ColumnGroup
            {
                GroupName = "PV",
                GroupColor = System.Drawing.Color.LightYellow,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("PV电压(V)", "PVVolt", 15, "0.0"),
                    new ColumnInfo("PV功率(W)", "PVPwr", 15, "0"),
                    new ColumnInfo("PV电流(A)", "PVCurr", 15, "0.0"),
                    new ColumnInfo("日发电量(kW·h)", "DailyGen", 15, "0.00"),
                    new ColumnInfo("月发电量(kW·h)", "MonthlyGen", 15, "0.00"),
                    new ColumnInfo("年发电量(kW·h)", "AnnualGen", 15, "0.00"),
                    new ColumnInfo("总发电量(kW·h)", "TotalGen", 15, "0.00")
                }
            },
            new ColumnGroup
            {
                GroupName = "BMS",
                GroupColor = System.Drawing.Color.LightSeaGreen,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("协议类型", "ProtocolType", 15),
                    new ColumnInfo("BMS通信正常", "BMS_ComOK", 15),
                    new ColumnInfo("BMS低电报警", "BMS_LowBattAlarm", 15),
                    new ColumnInfo("BMS低电故障", "BMS_LowBattFault", 15),
                    new ColumnInfo("BMS允许充电", "BMS_ChgEnable", 15),
                    new ColumnInfo("BMS允许放电", "BMS_DisEnable", 15),
                    new ColumnInfo("BMS充电过流", "BMS_ChgOC", 15),
                    new ColumnInfo("BMS放电过流", "BMS_DisOC", 15),
                    new ColumnInfo("BMS温度过低", "BMS_UnderTemp", 15),
                    new ColumnInfo("BMS温度过高", "BMS_OverTemp", 15),
                    new ColumnInfo("BMS平均温度", "BMS_AvgTemp", 15),
                    new ColumnInfo("BMS充电电流限制", "BMS_ChgCurrLimit", 15),
                    new ColumnInfo("BMS当前SOC", "BMS_SOC", 15),
                    new ColumnInfo("BMS充电电压限制", "BMS_ChgVoltLimit", 15),
                    new ColumnInfo("BMS放电电压限制", "BMS_DisVoltLimit", 15)
                }
            },
            new ColumnGroup
            {
                GroupName = "电池",
                GroupColor = System.Drawing.Color.LightPink,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("电池电压(V)", "BattVolt", 15, "0.0"),
                    new ColumnInfo("电池容量(%)", "BattCapacity", 15, "0%"),
                    new ColumnInfo("电池电流(A)", "BatCurr", 15, "0.0"),
                    new ColumnInfo("母线电压(V)", "BusVolt", 15, "0.0")
                }
            },
            new ColumnGroup
            {
                GroupName = "机器状态",
                GroupColor = System.Drawing.Color.LightGray,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("PV温度(℃)", "PVTemp", 15, "0.0"),
                    new ColumnInfo("故障代码", "FaultCode", 15),
                    new ColumnInfo("逆变温度(℃)", "InvTemp", 15, "0.0"),
                    new ColumnInfo("升压温度(℃)", "BoostTemp", 15, "0.0"),
                    new ColumnInfo("变压器温度(℃)", "XfmrTemp", 15, "0.0"),
                    new ColumnInfo("当前最高温度(℃)", "MaxTemp", 15, "0.0"),
                    new ColumnInfo("风扇转速", "FanSpeed", 15, "0"),
                    new ColumnInfo("风扇使能", "FanEnable", 15),
                    new ColumnInfo("模式", "Mode", 15),
                    new ColumnInfo("AC状态下PV馈能到负载", "PVToLoadAC", 15),
                    new ColumnInfo("机器是否有输出", "OutputStatus", 15),
                    new ColumnInfo("电池低电报警", "BattLowAlarm", 15),
                    new ColumnInfo("电池未接", "BattDisconnected", 15),
                    new ColumnInfo("输出过载", "OutputOverload", 15),
                    new ColumnInfo("机器过温(℃)", "OverTemp", 15,"0.0"),
                    new ColumnInfo("EEPROM数据异常", "EEPROM_DataErr", 15),
                    new ColumnInfo("EEPROM读写异常", "EEPROM_IOErr", 15),
                    new ColumnInfo("PV功率过低异常", "PVLowPwrFault", 15),
                    new ColumnInfo("输入电压过高", "InputOV", 15),
                    new ColumnInfo("电池电压过高", "BattOV", 15),
                    new ColumnInfo("风扇转速异常", "FanSpeedFault", 15),
                    new ColumnInfo("并机系统里机器的总数", "ParallelUnits", 15),
                    new ColumnInfo("并网标志", "GridTieFlag", 15),
                    new ColumnInfo("并机系统中角色", "ParallelRole", 15),
                    new ColumnInfo("主输出继电器状态", "MainRelayStat", 15),
                    new ColumnInfo("第二输出当前状态", "SecOutStat", 15),
                    new ColumnInfo("BMS通讯异常", "BMS_ComFault", 15),
                    new ColumnInfo("温度传感器异常", "TempSensorFault", 15),
                    new ColumnInfo("市电灯状态", "ACLED", 15),
                    new ColumnInfo("逆变灯状态", "InvLED", 15),
                    new ColumnInfo("充电灯状态", "ChgLED", 15),
                    new ColumnInfo("报警灯状态", "AlarmLED", 15),
                    new ColumnInfo("逆变器工作状态", "InvStatus", 15),
                    new ColumnInfo("PV电压状态", "PVVoltStatus", 15),
                    new ColumnInfo("逆变桥状态", "InvBridgeStatus", 15),
                    new ColumnInfo("MPPT状态", "MPPTStatus", 15),
                    new ColumnInfo("锁相环状态", "PLLStatus", 15)
                }

            }
        }
            };

            return config;
        }
        #endregion

        #region UPSCYX01的保存命令和配置
<<<<<<< HEAD
        private ICommand? _UPSCYX01SaveCommand;
        public ICommand UPSCYX01SaveCommand => _UPSCYX01SaveCommand ??= new RelayCommand(() => StartSavingWithConfig(GetUPSCYX01Config()));
=======
        private ICommand _UPSCYX01SaveCommand;
        public ICommand UPSCYX01SaveCommand
        {
            get
            {
                return _UPSCYX01SaveCommand ?? (_UPSCYX01SaveCommand = new RelayCommand(() => StartSavingWithConfig(GetUPSCYX01Config())));
            }
        }
>>>>>>> new

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static DeviceExcelConfig GetUPSCYX01Config()
        {
            var config = new DeviceExcelConfig
            {
                DeviceName = "UPSCYX01",
                ColumnGroups = new List<ColumnGroup>
        {
            new ColumnGroup
            {
                GroupName = "基本信息",
                GroupColor = System.Drawing.Color.LightGray,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("日期", "DataNow", 12),
                    new ColumnInfo("时间", "Time", 10)
                }
            },
            new ColumnGroup
            {
                GroupName = "市电",
                GroupColor = System.Drawing.Color.LightBlue,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("市电电压(V)", "MainsVoltage", 15, "0.0"),
                    new ColumnInfo("市电频率(Hz)", "MainsFrequency", 15, "0.0"),
                }
            },
            new ColumnGroup
            {
                GroupName = "输出",
                GroupColor = System.Drawing.Color.LightGreen,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("输出电压(V)", "OutVolt", 15, "0.0"),
                    new ColumnInfo("输出频率(Hz)", "OutFreq", 15, "0.0"),
                    new ColumnInfo("视在功率(W)", "ApparentPwr", 15, "0"),
                    new ColumnInfo("有功功率(W)", "ActivePwr", 15, "0"),
                    new ColumnInfo("负载百分比(%)", "LoadPercent", 15, "0%"),
                    new ColumnInfo("满载有功功率(W)", "RatedPwr", 15, "0"),
                    new ColumnInfo("电感电流(A)", "InductorCurr", 15, "0.0")
                }
            },
            new ColumnGroup
            {
                GroupName = "Time",
                GroupColor = System.Drawing.Color.LightYellow,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("强充时间", "BoostTime", 15),
                    new ColumnInfo("逆变时间", "InvTime", 15),
                    new ColumnInfo("带载时间", "LoadTime", 15),
                }
            },
            new ColumnGroup
            {
                GroupName = "电池",
                GroupColor = System.Drawing.Color.LightPink,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("电池电压(V)", "BattVolt", 15, "0.0"),
                    new ColumnInfo("电池容量(%)", "BattCapacity", 15, "0%"),
                    new ColumnInfo("电池节数", "BattCells", 15, "0")
                }
            },
            new ColumnGroup
            {
                GroupName = "机器状态",
                GroupColor = System.Drawing.Color.LightGray,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("故障代码", "FaultCode", 15),
                    new ColumnInfo("模式", "Mode", 15),
                    new ColumnInfo("逆变温度(℃)", "InvTemp", 15, "0.0"),
                    new ColumnInfo("当前最高温度(℃)", "MaxTemp", 15,"0.0"),
                    new ColumnInfo("电池未接", "BattDisconnected", 15),
                    new ColumnInfo("输出过载", "OutputOverload", 15),
                    new ColumnInfo("机器过温", "OverTemp", 15),
                    new ColumnInfo("电池低电报警", "BattLowAlarm", 15),
                    new ColumnInfo("EEPROM数据异常", "EEPROM_DataErr", 15),
                    new ColumnInfo("EEPROM读写异常", "EEPROM_IOErr", 15),
                    new ColumnInfo("输入电压过高", "InputOV", 15),
                    new ColumnInfo("电池电压过高", "BattOV", 15),
                    new ColumnInfo("风扇转速异常", "FanSpeedFault", 15),
                    new ColumnInfo("机器是否有输出", "OutputStatus", 15)
                }

            }
        }
            };

            return config;
        }
        #endregion

        #region LB6的保存命令和配置
<<<<<<< HEAD
        private ICommand? _LB6SaveCommand;
        public ICommand LB6SaveCommand => _LB6SaveCommand ??= new RelayCommand(() => StartSavingWithConfig(GetLB6Config()));
=======
        private ICommand _LB6SaveCommand;
        public ICommand LB6SaveCommand
        {
            get
            {
                return _LB6SaveCommand ?? (_LB6SaveCommand = new RelayCommand(() => StartSavingWithConfig(GetLB6Config())));
            }
        }
>>>>>>> new

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static DeviceExcelConfig GetLB6Config()
        {
            var config = new DeviceExcelConfig
            {
                DeviceName = "LB6",
                ColumnGroups = new List<ColumnGroup>
        {
            new ColumnGroup
            {
                GroupName = "基本信息",
                GroupColor = System.Drawing.Color.LightGray,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("日期", "DataNow", 12),
                    new ColumnInfo("时间", "Time", 10)
                }
            },
            new ColumnGroup
            {
                GroupName = "市电",
                GroupColor = System.Drawing.Color.LightBlue,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("市电电压(V)", "MainsVoltage", 15, "0.0"),
                    new ColumnInfo("市电频率(Hz)", "MainsFrequency", 15, "0.0"),
                }
            },
            new ColumnGroup
            {
                GroupName = "输出",
                GroupColor = System.Drawing.Color.LightGreen,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("输出电压(V)", "OutVolt", 15, "0.0"),
                    new ColumnInfo("输出频率(Hz)", "OutFreq", 15, "0.0"),
                    new ColumnInfo("视在功率(W)", "ApparentPwr", 15, "0"),
                    new ColumnInfo("有功功率(W)", "ActivePwr", 15, "0"),
                    new ColumnInfo("负载百分比(%)", "LoadPercent", 15, "0%"),
                    new ColumnInfo("满载有功功率(W)", "RatedPwr", 15, "0"),
                    new ColumnInfo("电感电流(A)", "InductorCurr", 15, "0.0")
                }
            },
            new ColumnGroup
            {
                GroupName = "Time",
                GroupColor = System.Drawing.Color.LightYellow,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("强充时间", "BoostTime", 15),
                    new ColumnInfo("逆变时间", "InvTime", 15)
                }
            },
            new ColumnGroup
            {
                GroupName = "电池",
                GroupColor = System.Drawing.Color.LightPink,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("电池电压(V)", "BattVolt", 15, "0.0"),
                    new ColumnInfo("电池容量(%)", "BattCapacity", 15, "0%"),
                    new ColumnInfo("电池节数", "BattCells", 15, "0"),
                    new ColumnInfo("电池类型", "BatteryType", 15),
                    new ColumnInfo("电池充电电流(A)", "V", 15, "0.0")
                }
            },
            new ColumnGroup
            {
                GroupName = "机器状态",
                GroupColor = System.Drawing.Color.LightGray,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("故障代码", "FaultCode", 15),
                    new ColumnInfo("逆变温度(℃)", "InvTemp", 15, "0.0"),
                    new ColumnInfo("模式", "Mode", 15),
                    new ColumnInfo("当前最高温度(℃)", "MaxTemp", 15, "0.0"),
                    new ColumnInfo("电池未接", "BattDisconnected", 15),
                    new ColumnInfo("输出过载", "OutputOverload", 15),
                    new ColumnInfo("机器过温(℃)", "OverTemp", 15,"0.0"),
                    new ColumnInfo("电池低电报警", "BattLowAlarm", 15),
                    new ColumnInfo("EEPROM数据异常", "EEPROM_DataErr", 15),
                    new ColumnInfo("EEPROM读写异常", "EEPROM_IOErr", 15),
                    new ColumnInfo("输入电压过高", "InputOV", 15),
                    new ColumnInfo("电池电压过高", "BattOV", 15),
                    new ColumnInfo("风扇转速异常", "FanSpeedFault", 15),
                    new ColumnInfo("充电阶段", "ChgStage", 15),
                    new ColumnInfo("自动开机使能", "AutoStartEnable", 15),
                    new ColumnInfo("机器是否有输出", "OutputStatus", 15)
                }

            }
        }
            };

            return config;
        }
        #endregion

        #region LPVINV02的保存命令和配置
<<<<<<< HEAD
        private ICommand? _LPVINV02SaveCommand;
        public ICommand LPVINV02SaveCommand => _LPVINV02SaveCommand ??= new RelayCommand(() => StartSavingWithConfig(GetLPVINV02Config()));
=======
        private ICommand _LPVINV02SaveCommand;
        public ICommand LPVINV02SaveCommand
        {
            get
            {
                return _LPVINV02SaveCommand ?? (_LPVINV02SaveCommand = new RelayCommand(() => StartSavingWithConfig(GetLPVINV02Config())));
            }
        }
>>>>>>> new

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static DeviceExcelConfig GetLPVINV02Config()
        {
            var config = new DeviceExcelConfig
            {
                DeviceName = "LPVINV02",
                ColumnGroups = new List<ColumnGroup>
        {
            new ColumnGroup
            {
                GroupName = "基本信息",
                GroupColor = System.Drawing.Color.LightGray,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("日期", "DataNow", 12),
                    new ColumnInfo("时间", "Time", 10)
                }
            },
            new ColumnGroup
            {
                GroupName = "市电",
                GroupColor = System.Drawing.Color.LightBlue,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("市电电压(V)", "MainsVoltage", 15, "0.0"),
                    new ColumnInfo("市电频率(Hz)", "MainsFrequency", 15, "0.0")
                }
            },
            new ColumnGroup
            {
                GroupName = "输出",
                GroupColor = System.Drawing.Color.LightGreen,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("输出电压(V)", "OutVolt", 15, "0.0"),
                    new ColumnInfo("输出频率(Hz)", "OutFreq", 15, "0.0"),
                    new ColumnInfo("视在功率(W)", "ApparentPwr", 15, "0"),
                    new ColumnInfo("有功功率(W)", "ActivePwr", 15, "0"),
                    new ColumnInfo("负载百分比(%)", "LoadPercent", 15, "0%")
                }
            },
            new ColumnGroup
            {
                GroupName = "BMS",
                GroupColor = System.Drawing.Color.LightYellow,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("协议类型", "ProtocolType", 15),
                    new ColumnInfo("BMS通信正常", "BMS_ComOK", 15),
                    new ColumnInfo("BMS低电报警", "BMS_LowBattAlarm", 15),
                    new ColumnInfo("BMS低电故障", "BMS_LowBattFault", 15),
                    new ColumnInfo("BMS允许充电", "BMS_ChgEnable", 15),
                    new ColumnInfo("BMS允许放电", "BMS_DisEnable", 15),
                    new ColumnInfo("BMS充电过流", "BMS_ChgOC", 15),
                    new ColumnInfo("BMS放电过流", "BMS_DisOC", 15),
                    new ColumnInfo("BMS温度过低", "BMS_UnderTemp", 15),
                    new ColumnInfo("BMS温度过高", "BMS_OverTemp", 15),
                    new ColumnInfo("BMS平均温度", "BMS_AvgTemp", 15),
                    new ColumnInfo("BMS充电电流限制", "BMS_ChgCurrLimit", 15),
                    new ColumnInfo("BMS当前SOC", "BMS_SOC", 15),
                    new ColumnInfo("BMS充电电压限制", "BMS_ChgVoltLimit", 15),
                    new ColumnInfo("BMS放电电压限制", "BMS_DisVoltLimit", 15),
                    new ColumnInfo("BMS充电电流(A)", "BMS_ChgCurr", 15, "0.0"),
                    new ColumnInfo("BMS放电电流(A)", "BMS_DisCurr", 15, "0.0"),
                }
            },
            new ColumnGroup
            {
                GroupName = "电池",
                GroupColor = System.Drawing.Color.LightPink,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("电池电压(V)", "BattVolt", 15, "0.0"),
                    new ColumnInfo("电池容量(%)", "BattCapacity", 15, "0%"),
                    new ColumnInfo("电池节数", "BattCells", 15, "0"),
                    new ColumnInfo("电池充电电流(A)", "BattChgCurr", 15, "0.0"),
                    new ColumnInfo("母线电压(V)", "BusVolt", 15, "0.0"),
                    new ColumnInfo("电池放电电流(A)", "BattDisCurr", 15,"0.0"),
                    new ColumnInfo("充电总开关", "ChgMasterSW", 15),
                    new ColumnInfo("AC充电开关", "ACChgSW", 15),
                    new ColumnInfo("太阳能充电开关", "SolarChgSW", 15),
                }
            },
            new ColumnGroup
            {
                GroupName = "机器状态",
                GroupColor = System.Drawing.Color.LightGray,
                Columns = new List<ColumnInfo>
                {
                    new ColumnInfo("逆变温度(℃)", "InvTemp", 15, "0.0"),
                    new ColumnInfo("当前最高温度(℃)", "MaxTemp", 15, "0.0"),
                    new ColumnInfo("PV温度(℃)", "PVTemp", 15,"0.0"),
                    new ColumnInfo("升压温度", "BoostTemp", 15,"0.0"),
                    new ColumnInfo("变压器温度(℃)", "XfmrTemp", 15,"0.0"),
                    new ColumnInfo("风扇转速", "FanSpeed", 15, "0.0"),
                    new ColumnInfo("风扇使能", "FanEnable", 15)
                }

            }
        }
            };

            return config;
        }
        #endregion

        #region 通用保存方法
        // 当前设备配置
<<<<<<< HEAD
        private DeviceExcelConfig? _currentDeviceConfig;
=======
        private DeviceExcelConfig _currentDeviceConfig;
>>>>>>> new

        /// <summary>
        /// 开始保存
        /// </summary>
<<<<<<< HEAD
        [MemberNotNull(nameof(_currentDeviceConfig))]
=======
>>>>>>> new
        private void StartSavingWithConfig(DeviceExcelConfig config)
        {
            _currentDeviceConfig = config;

            if (_currentDeviceConfig == null)
            {
                MessageBox.Show("设备配置错误");
<<<<<<< HEAD
                return ;
=======
                return;
>>>>>>> new
            }

            // 重置状态
            _isFileLock = false;
            _retryCount = 0;

            var dialog = new SaveFileDialog();
            dialog.Filter = "Excel 文件 (*.xlsx)|*.xlsx";
            dialog.FileName = $"{_currentDeviceConfig.DeviceName}_数据记录_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            if (dialog.ShowDialog() == true)
            {
                _savePath = dialog.FileName;

                try
                {
                    // 尝试创建Excel表头
                    CreateExcelHeader();
                    IsSaving = true;
                    Console.WriteLine($"开始保存到: {_savePath}");
                }
                catch (Exception ex)
                {
                    // 如果创建表头失败（可能是因为文件被占用），显示错误但不停止
                    MessageBox.Show($"创建Excel文件失败: {ex.Message}\n请关闭Excel后重试。");
                    // 不设置IsSaving = true，让用户重试
                }
            }
        }

        /// <summary>
        /// 停止保存
        /// </summary>
        private void StopSaving()
        {
            IsSaving = false;
            _savePath = null;
            _isFileLock = false;
            _retryCount = 0;
            _currentDeviceConfig = null;
        }

        /// <summary>
        /// 设置保存属性
        /// </summary>
        public bool IsSaving
        {
            get => _isSaving;
            set
            {
                if (_isSaving != value)
                {
                    _isSaving = value;
                    OnPropertyChanged(nameof(IsSaving));
<<<<<<< HEAD
                    
=======

>>>>>>> new
                    if (!_isSaving)
                    {
                        StopSaving();
                    }
                }
            }
        }

        /// <summary>
        /// 文件占用检测
        /// </summary>
        private static bool IsFileLock(string filePath)
        {
            if (!File.Exists(filePath))
                return false;

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    // 如果可以打开文件，说明没有被占用
                }
                return false;
            }
            catch (IOException)
            {
                return true; // 文件被占用
            }
            catch (Exception)
            {
                return false;
            }
        }
<<<<<<< HEAD
        
=======

>>>>>>> new
        /// <summary>
        /// 处理文件被占用的情况
        /// </summary>
        private void HandleFileLocked()
        {
            if (!_isFileLock)
            {
                _isFileLock = true;
                _retryCount = 0;

                // 只在第一次检测到锁定时提示
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("Excel 正在打开，数据将暂时跳过保存。关闭Excel后会自动恢复保存。",
                                  "文件被占用",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Warning);
                });
                Console.WriteLine($"文件被占用，跳过保存: {_savePath}");
            }
            else
            {
                _retryCount++;
                Console.WriteLine($"文件仍被占用，重试次数: {_retryCount}");

                // 如果连续多次失败，可以提示用户
                if (_retryCount % 10 == 0) // 每10次提示一次
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"文件已被长时间占用，已跳过{_retryCount}次保存。\n请关闭Excel文件以恢复自动保存。",
                                      "文件长时间占用",
                                      MessageBoxButton.OK,
                                      MessageBoxImage.Information);
                    });
                }
            }
        }

        /// <summary>
        /// 创建表头
        /// </summary>
        private void CreateExcelHeader()
        {
            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Data");

            int currentColumn = 1;

            // 创建分组表头
<<<<<<< HEAD
            var cfg = _currentDeviceConfig!;
            foreach (var group in cfg.ColumnGroups)
=======
            foreach (var group in _currentDeviceConfig.ColumnGroups)
>>>>>>> new
            {
                int groupStartColumn = currentColumn;
                int groupEndColumn = currentColumn + group.Columns.Count - 1;

                // 合并分组标题单元格
                sheet.Cells[1, groupStartColumn, 1, groupEndColumn].Merge = true;
                sheet.Cells[1, groupStartColumn].Value = group.GroupName;
                sheet.Cells[1, groupStartColumn].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells[1, groupStartColumn].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                sheet.Cells[1, groupStartColumn].Style.Font.Bold = true;
                sheet.Cells[1, groupStartColumn].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                sheet.Cells[1, groupStartColumn].Style.Fill.BackgroundColor.SetColor(group.GroupColor);

                // 设置分组边框
                var groupBorder = sheet.Cells[1, groupStartColumn, 2, groupEndColumn];
                groupBorder.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                groupBorder.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                groupBorder.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                groupBorder.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                // 写入分组内的列标题
                foreach (var column in group.Columns)
                {
                    sheet.Cells[2, currentColumn].Value = column.HeaderName;
                    sheet.Cells[2, currentColumn].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    sheet.Cells[2, currentColumn].Style.Font.Bold = true;

                    // 设置列宽
                    sheet.Column(currentColumn).Width = column.ColumnWidth;

                    currentColumn++;
                }
            }

            // 设置行高
            sheet.Row(1).Height = 30;
            sheet.Row(2).Height = 25;

            // 冻结前两行（表头）
            sheet.View.FreezePanes(3, 1);

            // 保存文件（这里可能会抛出异常，由调用者处理）
<<<<<<< HEAD
            package.SaveAs(new FileInfo(_savePath!));
=======
            package.SaveAs(new FileInfo(_savePath));
>>>>>>> new
        }

        /// <summary>
        /// 尝试保存数据到Excel
        /// </summary>
        private void TrySaveData(Common_Data data)
        {
            try
            {
                var file = new FileInfo(_savePath);

                // 如果文件不存在，尝试重新创建表头
                if (!file.Exists)
                {
                    try
                    {
                        CreateExcelHeader();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"重新创建表头失败: {ex.Message}");
                        return;
                    }
                }

                using var package = new ExcelPackage(file);
                var sheet = package.Workbook.Worksheets[0];

                int row = sheet.Dimension?.End?.Row + 1 ?? 1;

                // 保存数据
                int columnIndex = 1;
                foreach (var group in _currentDeviceConfig.ColumnGroups)
                {
                    foreach (var column in group.Columns)
                    {
                        var cell = sheet.Cells[row, columnIndex];

                        // 获取属性值
                        object value = null;

                        // 特殊处理日期格式
                        if (column.DataProperty == "DataNow")
                        {
                            value = data.DataNow.ToString("yyyy-MM-dd");
                        }
                        else if (column.DataProperty == "Time")
                        {
                            value = data.Time;
                        }
                        else
                        {
                            // 使用反射获取属性值
                            value = data.GetPropertyValue(column.DataProperty);
                        }

                        cell.Value = value;

                        // 应用格式
                        if (!string.IsNullOrEmpty(column.Format))
                        {
                            cell.Style.Numberformat.Format = column.Format;
                        }

                        columnIndex++;
                    }
                }

                // 数据居中
                SetDataAlignment(sheet, row);

                //数据边框
                AddDataRowBorder(sheet, row);

                // 保存文件
                package.Save();

                Console.WriteLine($"数据保存成功，行号: {row}, 时间: {data.Time}");
            }
            catch (Exception ex)
            {
                // 记录错误，但不停止保存
                Console.WriteLine($"保存失败: {ex.Message}");

                // 检查是否是文件占用异常
                if (ex is IOException &&
                    (ex.Message.Contains("另一个进程") ||
                     ex.Message.Contains("being used") ||
                     ex.Message.ToLower().Contains("locked")))
                {
                    // 如果是文件占用异常，设置文件锁定状态
                    HandleFileLocked();
                }
            }
        }

        // <summary>
        /// 保存到Excel表
        /// </summary>
        public void SaveToExcel(Common_Data data)
        {
            if (!IsSaving || string.IsNullOrEmpty(_savePath) || _currentDeviceConfig == null)
                return;

            // 2. 检查文件是否被占用
            if (IsFileLock(_savePath))
            {
                HandleFileLocked();
                return;
            }

            // 3. 如果文件之前被占用，现在可用了，重置状态
            if (_isFileLock)
            {
                _isFileLock = false;
                _retryCount = 0;
            }

            // 4. 尝试保存数据
            TrySaveData(data);
        }

        /// <summary>
        /// 添加边框
        /// </summary>
        private void AddDataRowBorder(ExcelWorksheet sheet, int row)
        {
            if (_currentDeviceConfig == null) return;

            int totalColumns = _currentDeviceConfig.ColumnGroups.Sum(g => g.Columns.Count);

            for (int col = 1; col <= totalColumns; col++)
            {
                var cell = sheet.Cells[row, col];

                // 设置黑色细边框
                cell.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                cell.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);

                cell.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                cell.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);

                cell.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                cell.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);

                cell.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                cell.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
            }
        }

        /// <summary>
        /// 设置数据行的对齐方式
        /// </summary>
        private void SetDataAlignment(ExcelWorksheet sheet, int row)
        {
            int totalColumns = _currentDeviceConfig.ColumnGroups.Sum(g => g.Columns.Count);
            // 设置所有数据居中对齐
            sheet.Cells[row, 1, row, totalColumns].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            sheet.Cells[row, 1, row, totalColumns].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            // 设置日期列左对齐
            sheet.Cells[row, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

            // 设置时间列居中对齐
            sheet.Cells[row, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        }
        #endregion 
    }
}
