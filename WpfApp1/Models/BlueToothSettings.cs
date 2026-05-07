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
    public class BlueToothSettings : BaseViewModel
    {
        public readonly IBluetoothService _bluetoothService;
        private BluetoothDeviceInfo? _selectedDevice;
        private string _statusMessage = "等待开启扫描";
        private string _SendData = string.Empty;
        private bool _isBusy;
        public BlueToothSettings(IBluetoothService bluetoothService) {
            _bluetoothService = bluetoothService;
            // 订阅蓝牙服务的事件
            _bluetoothService.DeviceDiscovered += OnDeviceDiscovered;
            _bluetoothService.ConnectionStatusChanged += OnConnectionStatusChanged;
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
        private bool _canConnect = true;
        public bool CanConnect
        {
            get => _canConnect;
            set => SetProperty(ref _canConnect, value);
        }
        #endregion

        #region 事件处理器（将服务层事件调度到 UI 线程）
        /// <summary>
        /// 设备连接事件处理器
        /// </summary>
        /// <param name="connected"></param>
        private void OnConnectionStatusChanged(bool connected)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                IsBusy = false;  // 连接操作结束，解除忙碌状态
                // 强制刷新所有命令的 CanExecute 状态
                CommandManager.InvalidateRequerySuggested();
            });
        }

        /// <summary>
        /// 设备发现事件处理器
        /// </summary>
        /// <param name="info"></param>
        private void OnDeviceDiscovered(BluetoothDeviceInfo device)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (!Devices.Any(d => d.BluetoothAddress == device.BluetoothAddress))
                {
                    Devices.Add(device);
                    // 如果是第一个设备，自动选中
                    if (Devices.Count == 1)
                    {
                        SelectedDevice = device;
                    }
                }
            });
        }
        #endregion

        #region 命令执行方法
        public async Task<bool> IsOpenBL()
        {
            // ⭐ 用 Radio 判断蓝牙状态
            bool isOn = await _bluetoothService.IsBluetoothOnAsync();

            if (!isOn)
            {
                MessageBox.Show("请先打开系统蓝牙!", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 开始扫描
        /// </summary>
        /// <returns></returns>
        public async Task StartScanAsync()
        {

            IsBusy = true;
            Devices.Clear();
            await _bluetoothService.StartScanningAsync();
            IsBusy = false;
        }
        /// <summary>
        /// 停止扫描
        /// </summary>
        public void StopScan()
        {
            _bluetoothService.StopScanning();
            IsBusy = false;
        }
        /// <summary>
        /// 延时，防止重复点击连接按钮
        /// </summary>
        /// <param name="milliseconds"></param>
        private async void StartCooldown(int milliseconds = 2000)
        {
            CanConnect = false;
            await Task.Delay(milliseconds);
            CanConnect = true;
            CommandManager.InvalidateRequerySuggested();
        }
        /// <summary>
        /// 连接蓝牙
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ConnectAsync()
        {
            IsBusy = true;
            bool success = await _bluetoothService.ConnectAsync(SelectedDevice);
            return success;
        }
        /// <summary>
        /// 断开蓝牙
        /// </summary>
        /// <returns></returns>
        public async Task DisconnectAsync()
        {

            await _bluetoothService.DisconnectAsync();
            StatusMessage = "正在断开连接...";
            StartCooldown();
            StatusMessage = "已断开连接";
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
        /// <summary>
        /// 获取蓝牙名称
        /// </summary>
        /// <returns></returns>
        public string getBluetoothName()
        {
            return _selectedDevice?.Name ?? string.Empty;
        }

        /// <summary>
        /// ASCLL指令数据发送
        /// </summary>
        /// <param name="command"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<string> SendBluetoothData(string command, int count)
        {
            return await _bluetoothService.SendBluetoothAscllcmd(command, count);
        }

        /// <summary>
        /// BMS指令数据发送
        /// </summary>
        /// <param name="command"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<byte[]> SendBluetoothBMS(byte[] command, int count, CancellationToken cancellationToken = default)
        {
            return await _bluetoothService.SendBluetoothToBMS(command, count, default);
        }
        #endregion

    }
}
