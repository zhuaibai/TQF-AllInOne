using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    public interface IBluetoothService
    {
        event Action<BluetoothDeviceInfo> DeviceDiscovered;// 设备发现事件
                                                           // event Action<string> StatusChanged;// 状态更新事件
        event Action<string> DataReceived;// 数据接收事件
        event Action<bool> ConnectionStatusChanged;// 连接状态变化事件
        event Action<byte[]> RawDataReceived;
        Task StartScanningAsync();// 开始扫描设备
        void StopScanning();// 停止扫描设备
        Task<bool> ConnectAsync(BluetoothDeviceInfo deviceInfo);// 连接设备
        Task DisconnectAsync();// 断开连接
        Task<bool> SendDataAsync(string data);// 发送数据
        Task<bool> SendByteAsync(byte[] data);
        Task<string> SendBluetoothAscllcmd(string command, int returnCount);
        Task<byte[]> SendBluetoothToBMS(byte[] command, int returnCount, CancellationToken cancellationToken = default);
        bool IsConnected { get; }// 连接状态属性
        bool IsScanning { get; }// 扫描状态属性
        BluetoothDeviceInfo? ConnectedDevice { get; }// 当前连接的设备信息属性
    }
}
