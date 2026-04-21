using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Windows.Devices.Bluetooth;
using WpfApp1.Command;
using WpfApp1.ViewModels;

namespace WpfApp1.Models
{
    public class BlueToothSettings: BaseViewModel
    {
        private readonly IBluetoothService _bluetoothService;
        private BluetoothDeviceInfo? _selectedDevice;
        private string _statusMessage = "等待开启扫描";
        private string _SendData = string.Empty;
        private bool _isBusy;
        public BlueToothSettings(IBluetoothService bluetoothService) {
            _bluetoothService = bluetoothService;
            // 订阅蓝牙服务的事件
            _bluetoothService.DeviceDiscovered += OnDeviceDiscovered;
            _bluetoothService.StatusChanged += OnStatusChanged;
            _bluetoothService.DataReceived += OnDataReceived;
            _bluetoothService.ConnectionStatusChanged += OnConnectionStatusChanged;
            SendDataCommand = new RelayCommand(async _ => await SendDataAsync(), _ => _bluetoothService.IsConnected && !string.IsNullOrWhiteSpace(SendData));
        }

        #region 属性和命令
        public ObservableCollection<BluetoothDeviceInfo> Devices { get; } = new();// 设备列表
        public ObservableCollection<string> Messages { get; } = new();// 消息列表

        public BluetoothDeviceInfo? SelectedDevice
        {
            get => _selectedDevice;
            set => SetProperty(ref _selectedDevice, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public string SendData
        {
            get => _SendData;
            set => SetProperty(ref _SendData, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }


        public ICommand SendDataCommand { get; }


        #endregion

        #region 事件处理器（将服务层事件调度到 UI 线程）
        private void OnConnectionStatusChanged(bool connected)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                IsBusy = false;  // 连接操作结束，解除忙碌状态
                // 强制刷新所有命令的 CanExecute 状态
                CommandManager.InvalidateRequerySuggested();
            });
        }

        private void OnDataReceived(string data)
        {
            Application.Current.Dispatcher.Invoke(() =>
                Messages.Insert(0, $"[{DateTime.Now:HH:mm:ss}] 收到: {data}"));
        }

        private void OnStatusChanged(string status)
        {
            Application.Current.Dispatcher.Invoke(() => StatusMessage = status);

        }

        /// <summary>
        /// 调度UI线程并实现蓝牙地址去重
        /// </summary>
        /// <param name="info"></param>
        private void OnDeviceDiscovered(BluetoothDeviceInfo device)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (!Devices.Any(d => d.BluetoothAddress == device.BluetoothAddress))
                {
                    Devices.Add(device);
                }
            });
        }
        #endregion

        #region 命令执行方法
        public async Task StartScanAsync()
        {
            await _bluetoothService.StartScanningAsync();
        }

        public void StopScan()
        {
            _bluetoothService.StopScanning();
            IsBusy = false;
        }
        public async Task<bool> ConnextAsync()
        {
            bool success = await _bluetoothService.ConnectAsync(SelectedDevice!);
            return success;
        }

        public async Task DisconnectAsync()
        {
            await _bluetoothService.DisconnectAsync();
        }

        private async Task SendDataAsync()
        {
            if (string.IsNullOrEmpty(SendData)) return;
            bool success = await _bluetoothService.SendDataAsync(SendData);
            if (success)
            {
                Messages.Insert(0, $"[{DateTime.Now:HH:mm:ss}] 发送: {SendData}");
                SendData = string.Empty;
            }
            else
            {
                StatusMessage = "发送失败";
            }
        }

        /// <summary>
        /// 判断蓝牙扫描是否打开
        /// </summary>
        /// <returns></returns>
        public bool IsScanningOpen()
        {
            return _bluetoothService.IsScanning;
        }
        /// <summary>
        /// 判断蓝牙连接是否打开
        /// </summary>
        /// <returns></returns>
        public bool IsConnected()
        {
            return _bluetoothService.IsConnected;
        }

        public string getBluetoothName()
        {
            return _selectedDevice?.Name ?? string.Empty;
        }
        #endregion

    }
}
