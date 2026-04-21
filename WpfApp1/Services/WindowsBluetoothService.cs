using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using WpfApp1.Models;

namespace WpfApp1.Services
{
   public class WindowsBluetoothService:IBluetoothService
    {
        private BluetoothLEAdvertisementWatcher? _watcher;
        private BluetoothLEDevice? _device;// 当前连接的蓝牙设备
        private GattCharacteristic? _txCharacteristic;// 用于发送数据的特征
        private GattCharacteristic? _rxCharacteristic;// 用于接收数据的特征

        public event Action<BluetoothDeviceInfo>? DeviceDiscovered;
        public event Action<string>? StatusChanged;
        public event Action<string>? DataReceived;
        public event Action<bool>? ConnectionStatusChanged;
        public event Action<byte[]>? RawDataReceived;

        public bool IsScanning => _watcher?.Status == BluetoothLEAdvertisementWatcherStatus.Started;// 扫描状态
        public bool IsConnected => _device?.ConnectionStatus == BluetoothConnectionStatus.Connected;// 连接状态
        public BluetoothDeviceInfo? ConnectedDevice { get; private set; }// 当前连接的设备信息

        //配置蓝牙服务和特征的UUID
        private static readonly Guid UartServiceUuid = new("0000FF00-0000-1000-8000-00805F9B34FB");
        private static readonly Guid RxCharacteristicUuid = new("0000FF01-0000-1000-8000-00805F9B34FB");
        private static readonly Guid TxCharacteristicUuid = new("0000FF02-0000-1000-8000-00805F9B34FB");


        ManualResetEventSlim _pauseEvent;//线程的开启、暂停
        SemaphoreSlim _semaphore;        //异步竞争，资源锁
        Action<string> AddLog;           //添加日志委托
        Action<string> UpdateState;      //更新状态日志
        public WindowsBluetoothService(ManualResetEventSlim pauseEvent, SemaphoreSlim semaphore, Action<string> addLog, Action<string> _updateState)
        {
            _pauseEvent = pauseEvent;
            _semaphore = semaphore;
            AddLog = addLog;
            UpdateState = _updateState;
            // 初始化蓝牙服务
        }

        /// <summary>
        /// 开始扫描附近的 BLE 设备
        /// 扫描结果通过 DeviceDiscovered 事件异步返回
        /// </summary>
        public async Task StartScanningAsync()
        {
            if (_watcher != null) return;// 已经在扫描中
            _watcher = new BluetoothLEAdvertisementWatcher
            {
                ScanningMode = BluetoothLEScanningMode.Active// 主动扫描模式，可以获取更多设备信息
            };
            // 订阅事件
            _watcher.Received += OnAdvertisementReceived;// 订阅设备发现事件
            _watcher.Stopped += (s, e) => StatusChanged?.Invoke("未发现设备");// 订阅扫描停止事件
            _watcher.Start();// 开始扫描
            StatusChanged?.Invoke("正在扫描设备...");
            await Task.CompletedTask;
        }

        /// <summary>
        /// 处理收到的 BLE 广播包，从中提取设备信息并触发发现事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            // 广播回调在非 UI 线程，使用 Task.Run 避免阻塞扫描器内部线程
            _ = Task.Run(async () =>
            {
                try
                {
                    var device = await BluetoothLEDevice.FromBluetoothAddressAsync(args.BluetoothAddress);// 获取设备对象
                    if (device == null) return;
                    var info = new BluetoothDeviceInfo
                    {
                        Name = string.IsNullOrEmpty(device.Name) ? "未知设备" : device.Name,
                        Id = device.DeviceId,
                        BluetoothAddress = args.BluetoothAddress,
                    };
                    DeviceDiscovered?.Invoke(info);// 通过事件通知 UI 层有新设备被发现
                }
                catch
                {
                }
            });
        }
        /// <summary>
        /// 停止扫描
        /// </summary>
        public void StopScanning()
        {
            _watcher?.Stop();
            _watcher = null;
            StatusChanged?.Invoke("扫描已停止");
        }

        public async Task<bool> ConnectAsync(BluetoothDeviceInfo deviceInfo)
        {
            try
            {
                StatusChanged?.Invoke("正在连接...");
                _device = await BluetoothLEDevice.FromIdAsync(deviceInfo.Id);
                if (_device == null)
                {
                    StatusChanged?.Invoke("连接失败: 无法从 ID 创建设备对象");
                    return false;
                }


                //获取所有GATT服务
                var serivcesResult = await _device.GetGattServicesAsync();
                if (serivcesResult.Status != GattCommunicationStatus.Success)
                {
                    StatusChanged?.Invoke("连接失败: 无法获取GATT服务");
                    return false;
                }

                //查找 UART 服务
                var uartService = serivcesResult.Services.FirstOrDefault(s => s.Uuid == UartServiceUuid);
                if (uartService == null) return false;

                // 获取 UART 服务的特征
                var charResult = await uartService.GetCharacteristicsAsync(BluetoothCacheMode.Uncached);
                if (charResult.Status != GattCommunicationStatus.Success)
                {
                    StatusChanged?.Invoke("连接失败: 无法获取UART服务");
                    return false;
                }

                // 查找 Tx 和 Rx 特征
                _txCharacteristic = charResult.Characteristics.FirstOrDefault(c => c.Uuid == TxCharacteristicUuid);
                _rxCharacteristic = charResult.Characteristics.FirstOrDefault(c => c.Uuid == RxCharacteristicUuid);
                if (_txCharacteristic == null || _rxCharacteristic == null)
                {
                    var foundChars = string.Join(", ", charResult.Characteristics.Select(c => c.Uuid));
                    StatusChanged?.Invoke($"未找到 TX/RX 特征。实际特征: {foundChars}");
                    return false;
                }

                // 订阅 Rx 特征的通知
                await _rxCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                    GattClientCharacteristicConfigurationDescriptorValue.Notify);
                _rxCharacteristic.ValueChanged += OnValueChanged;

                ConnectedDevice = deviceInfo;
                ConnectionStatusChanged?.Invoke(true);
                StatusChanged?.Invoke("连接成功");
                return true;

            }
            catch (Exception e)
            {
                StatusChanged?.Invoke($"连接失败: {e.Message}");
                await DisconnectAsync();
                return false;
            }
        }
        /// <summary>
        /// RX 特征的值变化回调
        /// 将原始字节转换为字符串并通过事件上报
        /// </summary>
        private void OnValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var reader = DataReader.FromBuffer(args.CharacteristicValue);
            byte[] data = new byte[reader.UnconsumedBufferLength];
            reader.ReadBytes(data);
            RawDataReceived?.Invoke(data);
            string receivedData = Encoding.UTF8.GetString(data);
            DataReceived?.Invoke(receivedData);
        }

        /// <summary>
        /// 断开连接并释放资源
        /// </summary>
        /// <returns></returns>
        public async Task DisconnectAsync()
        {
            _rxCharacteristic?.Service?.Device?.Dispose();
            _device?.Dispose();
            _device = null;
            ConnectedDevice = null;

            ConnectionStatusChanged?.Invoke(false);
            StatusChanged?.Invoke("已断开连接");
            await Task.CompletedTask;
        }

        /// <summary>
        /// 向设备发送字符串数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<bool> SendDataAsync(string data)
        {
            if (_txCharacteristic == null) return false;
            var writer = new DataWriter();
            writer.WriteString(data);
            var result = await _txCharacteristic.WriteValueAsync(writer.DetachBuffer());
            return result == GattCommunicationStatus.Success;
        }

        /// <summary>
        /// 向设备发送字节数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<bool> SendByteAsync(byte[] data)
        {
            if (_txCharacteristic == null) return false;
            try
            {
                var writer = new DataWriter();
                writer.WriteBytes(data);
                var result = await _txCharacteristic.WriteValueAsync(writer.DetachBuffer());
                return result == GattCommunicationStatus.Success;
            }
            catch { return false; }
        }
    }
}


