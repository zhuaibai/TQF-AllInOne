using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Radios;
using Windows.Storage.Streams;
using WpfApp1.Models;
using WpfApp1.ViewModels;

namespace WpfApp1.Services
{
   public class WindowsBluetoothService:IBluetoothService
    {
        // ---------- 蓝牙核心对象 ----------
        private BluetoothLEAdvertisementWatcher? _watcher;// 蓝牙扫描器
        private BluetoothLEDevice? _device;// 当前连接的蓝牙设备
        private GattCharacteristic? _txCharacteristic;// 用于发送数据的特征
        private GattCharacteristic? _rxCharacteristic;// 用于接收数据的特征

        // ---------- 线程控制与资源锁 ----------
        ManualResetEventSlim _pauseEvent;//线程的开启、暂停
        private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1, 1);//互斥锁
        private readonly SemaphoreSlim dataSignal = new SemaphoreSlim(0);//等待数据

        // ---------- 日志委托 ----------
        Action<string> AddLog;           //添加日志委托
        Action<string> UpdateState;      //更新状态日志

        // ---------- 事件 ----------
        public event Action<BluetoothDeviceInfo>? DeviceDiscovered;
        public event Action<string>? DataReceived;
        public event Action<bool>? ConnectionStatusChanged;
        public event Action<byte[]>? RawDataReceived;

        // ---------- 属性 ----------
        private bool _isConnected;
        public bool IsConnected => _isConnected;
        public bool IsScanning => _watcher?.Status == BluetoothLEAdvertisementWatcherStatus.Started;// 扫描状态
        public BluetoothDeviceInfo? ConnectedDevice { get; private set; }// 当前连接的设备信息

        // --------- CRC 校验标志 ----------
        private static bool Receive_CRC_Check = false;

        //配置蓝牙服务和特征的UUID
        private static readonly Guid UartServiceUuid = new("0000FF00-0000-1000-8000-00805F9B34FB");
        private static readonly Guid RxCharacteristicUuid = new("0000FF01-0000-1000-8000-00805F9B34FB");
        private static readonly Guid TxCharacteristicUuid = new("0000FF02-0000-1000-8000-00805F9B34FB");

        // --------- 构造函数 ----------
        public WindowsBluetoothService(ManualResetEventSlim pauseEvent,
            Action<string> addLog, Action<string> _updateState)
        {
            _pauseEvent = pauseEvent;
            AddLog = addLog;
            UpdateState = _updateState;
            _recentAddresses = new HashSet<ulong>();
            _lastCleanup = DateTime.Now;
        }

        #region 事件处理方法
        /// <summary>
        /// 处理收到的 BLE 广播包，从中提取设备信息并触发发现事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private HashSet<ulong> _recentAddresses = new HashSet<ulong>();
        private DateTime _lastCleanup = DateTime.Now;

        private void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            if (!_pauseEvent.IsSet) return;

            // 每 2 秒清理一次去重记录，防止内存膨胀
            if ((DateTime.Now - _lastCleanup).TotalSeconds > 2)
            {
                _recentAddresses.Clear();
                _lastCleanup = DateTime.Now;
            }

            // 同一设备 2 秒内只处理一次
            if (!_recentAddresses.Add(args.BluetoothAddress)) return;

            // 先检查广播名称，不符合的直接丢弃，不开 Task
            string broadcastName = args.Advertisement.LocalName;
            if (!string.IsNullOrEmpty(broadcastName) &&
                broadcastName.StartsWith("pg", StringComparison.OrdinalIgnoreCase))
            {
                // 只有名称符合的设备才开 Task 获取详细信息
                _ = Task.Run(async () =>
                {
                    try
                    {
                        var device = await BluetoothLEDevice.FromBluetoothAddressAsync(args.BluetoothAddress);
                        if (device == null) return;
                        var info = new BluetoothDeviceInfo
                        {
                            Name = string.IsNullOrEmpty(device.Name) ? "未知设备" : device.Name,
                            BluetoothAddress = args.BluetoothAddress,
                        };
                        DeviceDiscovered?.Invoke(info);
                    }
                    catch { }
                });
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
        #endregion

        #region 扫描和连接
        public async Task<bool> IsBluetoothOnAsync()
        {
            try
            {
                var radios = await Radio.GetRadiosAsync();

                foreach (var radio in radios)
                {
                    if (radio.Kind == RadioKind.Bluetooth)
                    {
                        return radio.State == RadioState.On;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 开始扫描附近的 BLE 设备
        /// 扫描结果通过 DeviceDiscovered 事件异步返回
        /// </summary>
        public async Task StartScanningAsync()
        {
            _pauseEvent.Wait();

            if (_watcher != null) return;

            try
            {
                _recentAddresses ??= new HashSet<ulong>();
                _recentAddresses.Clear();
                _lastCleanup = DateTime.Now;

                _watcher = new BluetoothLEAdvertisementWatcher
                {
                    ScanningMode = BluetoothLEScanningMode.Active
                };

                _watcher.Received += OnAdvertisementReceived;
                _watcher.Stopped += (s, e) => UpdateState("扫描已停止");

                _watcher.Start();

                UpdateState("正在扫描设备...");
                AddLog("开始扫描 BLE 设备");
            }
            catch (Exception ex)
            {
                AddLog($"启动扫描失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 停止扫描
        /// </summary>
        public void StopScanning()
        {
            _watcher?.Stop();
            _watcher = null;
            _recentAddresses?.Clear();  // 清理去重集合
            UpdateState("扫描已停止");
            AddLog("停止扫描");
        }

        /// <summary>
        /// 连接功能
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <returns></returns>
        public async Task<bool> ConnectAsync(BluetoothDeviceInfo deviceInfo)
        {
            try
            {
                UpdateState("正在连接...");
                AddLog($"尝试连接设备: {deviceInfo.Name} ({deviceInfo.BluetoothAddress:X})");

                _device = await BluetoothLEDevice.FromBluetoothAddressAsync(deviceInfo.BluetoothAddress);
                if (_device == null)
                {
                    return false;
                }
                

                //获取所有GATT服务
                var serivcesResult = await _device.GetGattServicesAsync();
                if (serivcesResult.Status != GattCommunicationStatus.Success)
                {
                    return false;
                }

                //查找 UART 服务
                var uartService = serivcesResult.Services.FirstOrDefault(s => s.Uuid == UartServiceUuid);
                if (uartService == null) return false;

                // 获取 UART 服务的特征
                GattCharacteristicsResult charResult = await uartService.GetCharacteristicsAsync(BluetoothCacheMode.Uncached); 
                if (charResult.Status != GattCommunicationStatus.Success)
                    return false;

                // 查找 Tx 和 Rx 特征
                _txCharacteristic = charResult.Characteristics.FirstOrDefault(c => c.Uuid == TxCharacteristicUuid);
                _rxCharacteristic = charResult.Characteristics.FirstOrDefault(c => c.Uuid == RxCharacteristicUuid);
                if (_txCharacteristic == null || _rxCharacteristic == null)
                {
                    var foundChars = string.Join(", ", charResult.Characteristics.Select(c => c.Uuid));
                    return false;
                }

                // 订阅 Rx 特征的通知
                await _rxCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                    GattClientCharacteristicConfigurationDescriptorValue.Notify);
                _rxCharacteristic.ValueChanged += OnValueChanged;

                _isConnected = true;
                ConnectedDevice = deviceInfo;
                ConnectionStatusChanged?.Invoke(true);
                UpdateState("连接成功");
                AddLog($"已连接到 {deviceInfo.Name}");
                return true;

            }
            catch (Exception e)
            {
                UpdateState($"连接失败: {e.Message}");
                await DisconnectAsync();
                return false;
            }
            finally
            {
                _isConnected = true;
            }
        }

        /// <summary>
        /// 断开连接并释放资源
        /// </summary>
        /// <returns></returns>
        public async Task DisconnectAsync()
        {
            try
            {
                if (_rxCharacteristic != null)
                {
                    _rxCharacteristic.ValueChanged -= OnValueChanged;
                    try
                    {
                        await _rxCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                            GattClientCharacteristicConfigurationDescriptorValue.None);
                    }
                    catch { }
                    _rxCharacteristic = null;
                }
                if (_device != null)
                {
                    _device.Dispose();
                    _device = null;
                }
                _txCharacteristic = null;
                _isConnected = false;
                ConnectedDevice = null;
                ConnectionStatusChanged?.Invoke(false);
            }
            finally
            {
                _sendLock.Release();
                _isConnected = false;
            }
        }
        #endregion

        #region 数据发送方法
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

        /// <summary>
        /// 发送字符串命令
        /// </summary>
        /// <param name="command">要发送的命令</param>
        /// <param name="returnCount">期望接收的字节数</param>
        /// <returns>返回接收到的响应字符串</returns>
        public async Task<string> SendBluetoothAscllcmd(string command, int returnCount)
        {
            _pauseEvent.Wait();
            await _sendLock.WaitAsync();
            try
            {
                //计算期望接收的总字节数
                int expectedTotal = returnCount + (Receive_CRC_Check ? 2 : 0);
                byte[] cmdBytes = Encoding.ASCII.GetBytes(command);
                //将基于事件的响应转换为可等待的 Task
                var responseTcs = new TaskCompletionSource<byte[]>();
                //数据累积缓冲区
                var receivedData = new List<byte>();
                //超时取消
                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMilliseconds(1000));
                //原始数据接收事件的处理委托
                Action<byte[]> onDataReceived = null;
                onDataReceived = (data) =>
                {
                    lock (receivedData)// 加锁保护缓冲区
                    {
                        receivedData.AddRange(data);
                        if (receivedData.Count >= expectedTotal)
                        {
                            byte[] result = receivedData.Take(expectedTotal).ToArray();
                            responseTcs.TrySetResult(result);
                        }
                    }
                };
                // 订阅原始数据接收事件
                RawDataReceived += onDataReceived;
                try
                {
                    // 发送命令
                    bool sent = await SendByteAsync(cmdBytes);
                    if (!sent) return string.Empty;

                    await Task.Delay(100); // 等待设备处理命令
                    var completed = await Task.WhenAny(responseTcs.Task,
                        Task.Delay(Timeout.Infinite, timeoutCts.Token));
                    if (completed != responseTcs.Task) return string.Empty; // 超时

                    // 获取接收到的字节数据
                    byte[] buffer = await responseTcs.Task;
                    if (buffer.Length == 0) return string.Empty;

                    //CRC 校验（如果启用）
                    if (Receive_CRC_Check)
                    {
                        byte[] origin = buffer;
                        byte[] crcori;
                        byte[] build;
                        bool CRC_Pass = CheckReceive_CRC(buffer, out crcori, out build);
                        if (!CRC_Pass)
                        {
                            //CRC校验不通过
                            return "-1   " + Encoding.ASCII.GetString(origin) + $"接收长度{origin.Length},期待长度{returnCount};\r收到的CRC:{crcori[0]},{crcori[1]};校验值:{build[0]},{build[1]}";

                        }
                    }
                    return Encoding.ASCII.GetString(buffer);
                }
                catch { return string.Empty; }
                finally
                {
                    // 无论成功或异常，都要取消事件订阅，避免内存泄漏和干扰下次调用
                    RawDataReceived -= onDataReceived;
                }
            }
            finally
            {
                _sendLock.Release();
            }
        }

        /// <summary>
        /// 发送BMS命令
        /// </summary>
        /// <param name="command">要发送的命令</param>
        /// <param name="returnCount">期望接收的字节数</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>返回接收到的响应字节数组</returns>
        public async Task<byte[]> SendBluetoothToBMS(byte[] command, int returnCount, CancellationToken cancellationToken = default)
        {
            byte[] timeoutFlag = new byte[] { 0xFF };
            await _sendLock.WaitAsync(cancellationToken);// 等待发送锁，确保同一时间只有一个发送操作

            try
            {
                int expectedTotal = returnCount + (Receive_CRC_Check ? 2 : 0);
                var receivedData = new List<byte>();
                var dataQueue = new Queue<byte[]>();
                // 设置超时
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                linkedCts.CancelAfter(TimeSpan.FromSeconds(5));
                var token = linkedCts.Token;

                void Handler(byte[] chunk)// 原始数据接收事件的处理委托
                {

                    if (chunk == null || chunk.Length == 0) return;
                    lock (dataQueue)
                    {
                        dataQueue.Enqueue(chunk);// 将接收到的数据块加入队列
                    }

                    dataSignal.Release();
                }

                RawDataReceived += Handler;// 订阅原始数据接收事件

                try
                {
                    bool sent = await SendByteAsync(command);// 发送数据
                    if (!sent)
                    {
                        AddLog("发送失败");
                        return timeoutFlag;
                    }
                    int frameLength = 0;
                    // 等待接收数据直到超时或接收到完整帧
                    while (!token.IsCancellationRequested)
                    {
                        try
                        {
                            await dataSignal.WaitAsync(token);
                        }
                        catch (OperationCanceledException)
                        {
                            return timeoutFlag;
                        }

                        byte[] chunk = null;

                        lock (dataQueue)
                        {
                            if (dataQueue.Count > 0)
                                chunk = dataQueue.Dequeue();// 从队列中取出数据块
                        }

                        if (chunk != null)
                        {
                            receivedData.AddRange(chunk);// 将数据块添加到接收缓冲区

                            if (TryGetFullFrame(receivedData, out frameLength))
                            {
                                break;
                            }
                        }
                    }

                    byte[] buffer = receivedData.ToArray();
                    if (Receive_CRC_Check)
                    {
                        byte[] origin = buffer;
                        byte[] crcori;
                        byte[] build;
                        bool CRC_Pass = CheckReceive_ModBus_CRC(buffer, out crcori, out build);
                        if (!CRC_Pass)
                        {
                            return buffer;
                        }
                    }
                    return buffer;

                }
                finally
                {
                    RawDataReceived -= Handler;
                }
            }
            catch (Exception ex)
            {
                AddLog($"整体异常: {ex.Message}");
                return timeoutFlag;
            }
            finally
            {
                _sendLock.Release();
            }
        }
        /// <summary>
        /// 验证接收到的数据是否包含完整的帧
        /// </summary>
        /// <param name="buffer">接收到的数据缓冲区</param>
        /// <param name="frameLength">完整帧的长度</param>
        /// <returns>如果包含完整帧则返回 true，否则返回 false</returns>
        private bool TryGetFullFrame(List<byte> buffer, out int frameLength)
        {
            frameLength = 0;

            if (buffer.Count < 5) // 最小长度
                return false;

            int byteCount = buffer[2];

            frameLength = 3 + byteCount + 2; // addr + func + len + data + CRC

            return buffer.Count >= frameLength;
        }
        #endregion

        #region 校验方法

        #region CRC 校验方法
        /// <summary>
        /// 检验接收到的CRC是否正确
        /// </summary>
        /// <param name="bytes">接收到的字节数组</param>
        /// <param name="crcOri">接收到的CRC值</param>
        /// <param name="crcGet">计算得到的CRC值</param>
        /// <returns>返回CRC校验是否通过</returns>
        private static bool CheckReceive_CRC(byte[] bytes, out byte[] crcOri, out byte[] crcGet)
        {
            crcOri = new byte[2];
            crcGet = new byte[2];
            if (bytes == null) return false;
            int CRC_length = bytes.Length - 3;
            // 创建新的字节数组进行 CRC 校验
            byte[] buffer = new byte[CRC_length];
            byte[] CRC_Receuve = new byte[2];

            // 复制除 CRC 校验码外的数据到 buffer 数组
            Array.Copy(bytes, 0, buffer, 0, CRC_length);

            // 从 bytes 数组中提取接收到的 CRC 校验码到 CRC_Receuve 数组
            Array.Copy(bytes, CRC_length, CRC_Receuve, 0, 2);

            // 获取 CRC 校验码
            byte[] CRC_Build = getCRC(buffer);
            crcOri = CRC_Receuve;
            crcGet = CRC_Build;
            // 判断两个校验码是否一致
            bool isEqual = CRC_Build.SequenceEqual(CRC_Receuve);
            return isEqual;
        }
        /// <summary>
        /// 获取 CRC 校验码
        /// </summary>
        /// <param name="data">要计算 CRC 的字节数组</param>
        /// <returns>返回计算得到的 CRC 校验码</returns>
        public static byte[] getCRC(byte[] data)
        {
            byte[] CRC = new byte[2];
            int value = cal_crc_half(data, data.Length);//从数据包中获取校验码
            CRC[0] = U16_MSB(value);//获取高位校验码
            CRC[1] = U16_LSB(value);//获取地位校验码
            return CRC;
        }
        static private int[] crc_ta = new int[16]
        {
                0x0000,0x1021,0x2042,0x3063,0x4084,0x50a5,0x60c6,0x70e7,

                0x8108,0x9129,0xa14a,0xb16b,0xc18c,0xd1ad,0xe1ce,0xf1ef
        };
        /// <summary>
        /// 计算 CRC 校验码
        /// </summary>
        /// <param name="pin">要计算 CRC 的字节数组</param>
        /// <param name="len">字节数组的长度</param>
        /// <returns>返回计算得到的 CRC 校验码</returns>
        static private int cal_crc_half(byte[] pin, int len)
        {
            int i = 0;
            int crc;

            byte da;
            byte[] ptr = new byte[len];
            byte bCRCHign;
            byte bCRCLow;

            for (i = 0; i < ptr.Length; i++)
            {
                ptr[i] = pin[i];
            }

            crc = 0;
            i = 0;

            while (len-- != 0)
            {
                da = (byte)(((byte)(crc >> 8)) >> 4); /* 暂存CRC的高四位 */

                crc <<= 4; /* CRC右移4位，相当于取CRC的低12位）*/

                crc ^= crc_ta[(da ^ (ptr[i] >> 4))]; /* CRC的高4位和本字节的前半字节相加后查表计算CRC，然后加上上一次CRC的余数 */

                da = (byte)(((byte)(crc >> 8)) >> 4); /* 暂存CRC的高4位 */

                crc <<= 4; /* CRC右移4位， 相当于CRC的低12位） */

                crc ^= crc_ta[(da ^ (ptr[i] & 0x0f))]; /* CRC的高4位和本字节的后半字节相加后查表计算CRC，然后再加上上一次CRC的余数 */

                i++;
            }

            bCRCLow = (byte)crc;

            bCRCHign = (byte)(crc >> 8);

            if (bCRCLow == 0x28 || bCRCLow == 0x0d || bCRCLow == 0x0a)
            {
                bCRCLow++;
            }
            if (bCRCHign == 0x28 || bCRCHign == 0x0d || bCRCHign == 0x0a)
            {
                bCRCHign++;
            }
            crc = (int)bCRCHign << 8;
            crc += bCRCLow;
            return (crc);
        }
        /// <summary>
        /// 获取 CRC 校验码的高位字节
        /// </summary>
        /// <param name="data">要计算 CRC 的值</param>
        /// <returns>返回 CRC 校验码的高位字节</returns>
        private static byte U16_MSB(int data)
        {
            return (byte)(data >> 8);
        }
        /// <summary>
        /// 获取CRC 校验码的低位字节
        /// </summary>
        /// <param name="data">要计算 CRC 的值</param>
        /// <returns>返回 CRC 校验码的低位字节</returns>
        static private byte U16_LSB(int data)
        {
            return (byte)(data & 0xff);
        }
        #endregion

        #region RTU CRC校验方法
        private static bool CheckReceive_ModBus_CRC(byte[] bytes, out byte[] crcOri, out byte[] crcGet)
        {

            crcOri = new byte[2];
            crcGet = new byte[2];
            if (bytes == null) return false;
            int CRC_length = bytes.Length - 2;
            // 创建新的字节数组进行 CRC 校验
            byte[] buffer = new byte[CRC_length];
            byte[] CRC_Receuve = new byte[2];

            // 复制除 CRC 校验码外的数据到 buffer 数组
            Array.Copy(bytes, 0, buffer, 0, CRC_length);

            // 从 bytes 数组中提取接收到的 CRC 校验码到 CRC_Receuve 数组
            Array.Copy(bytes, CRC_length, CRC_Receuve, 0, 2);

            // 获取 CRC 校验码
            byte[] CRC_Build = getCRC16(buffer, buffer.Length);
            crcOri = CRC_Receuve;
            crcGet = CRC_Build;
            // 判断两个校验码是否一致
            bool isEqual = CRC_Build.SequenceEqual(CRC_Receuve);
            return isEqual;
        }

        public static byte[] getCRC16(byte[] data, int length)
        {
            int value = RTU_CalCRC16(data, length);
            byte[] CRC = new byte[2];
            CRC[1] = U16_MSB(value);//获取高位校验码
            CRC[0] = U16_LSB(value);//获取地位校验码
            return CRC;
        }
        static int RTU_CalCRC16(byte[] pucFrame, int usDataLen)
        {
            byte ucCRCHi = 0xFF;     // 高CRC字节初始化
            byte ucCRCLo = 0xFF;     // 低CRC字节初始化
            int wIndex = 0;          // CRC循环中的索引
            int i = 0;

            while (usDataLen > 0)
            {
                usDataLen--;
                wIndex = ucCRCLo ^ (pucFrame[i]);
                ucCRCLo = (byte)(ucCRCHi ^ rtu_aucCRCHi[wIndex]);
                ucCRCHi = rtu_aucCRCLo[wIndex];
                i++;
            }

            return (int)(ucCRCHi << 8 | ucCRCLo);
        }

        public static byte[] rtu_aucCRCHi = new byte[256]
{
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40
};

        // CRC16 低位字节值表
        public static byte[] rtu_aucCRCLo = new byte[256]
        {
            0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06, 0x07, 0xC7,
            0x05, 0xC5, 0xC4, 0x04, 0xCC, 0x0C, 0x0D, 0xCD, 0x0F, 0xCF, 0xCE, 0x0E,
            0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09, 0x08, 0xC8, 0xD8, 0x18, 0x19, 0xD9,
            0x1B, 0xDB, 0xDA, 0x1A, 0x1E, 0xDE, 0xDF, 0x1F, 0xDD, 0x1D, 0x1C, 0xDC,
            0x14, 0xD4, 0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2, 0x12, 0x13, 0xD3,
            0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3, 0xF2, 0x32,
            0x36, 0xF6, 0xF7, 0x37, 0xF5, 0x35, 0x34, 0xF4, 0x3C, 0xFC, 0xFD, 0x3D,
            0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A, 0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38,
            0x28, 0xE8, 0xE9, 0x29, 0xEB, 0x2B, 0x2A, 0xEA, 0xEE, 0x2E, 0x2F, 0xEF,
            0x2D, 0xED, 0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26,
            0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0, 0xA0, 0x60, 0x61, 0xA1,
            0x63, 0xA3, 0xA2, 0x62, 0x66, 0xA6, 0xA7, 0x67, 0xA5, 0x65, 0x64, 0xA4,
            0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F, 0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB,
            0x69, 0xA9, 0xA8, 0x68, 0x78, 0xB8, 0xB9, 0x79, 0xBB, 0x7B, 0x7A, 0xBA,
            0xBE, 0x7E, 0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C, 0xB4, 0x74, 0x75, 0xB5,
            0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71, 0x70, 0xB0,
            0x50, 0x90, 0x91, 0x51, 0x93, 0x53, 0x52, 0x92, 0x96, 0x56, 0x57, 0x97,
            0x55, 0x95, 0x94, 0x54, 0x9C, 0x5C, 0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E,
            0x5A, 0x9A, 0x9B, 0x5B, 0x99, 0x59, 0x58, 0x98, 0x88, 0x48, 0x49, 0x89,
            0x4B, 0x8B, 0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C,
            0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42, 0x43, 0x83,
            0x41, 0x81, 0x80, 0x40
        };
        #endregion

        #endregion

        public async Task<string> SendBLCommand(byte[] command, int returnCount)
        {
           // int? totalBytesRead = 0;

            //_pauseEvent.Wait();
            await _sendLock.WaitAsync();
            try
            {
                //计算期望接收的总字节数
                int expectedTotal = returnCount + (Receive_CRC_Check ? 2 : 0);
                byte[] cmdBytes = command;
                //将基于事件的响应转换为可等待的 Task
                var responseTcs = new TaskCompletionSource<byte[]>();
                //数据累积缓冲区
                var receivedData = new List<byte>();
                //超时取消
                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMilliseconds(1000));
                //原始数据接收事件的处理委托
                Action<byte[]> onDataReceived = null;
                onDataReceived = (data) =>
                {
                    lock (receivedData)// 加锁保护缓冲区
                    {
                        receivedData.AddRange(data);
                        if (receivedData.Count >= expectedTotal)
                        {
                            byte[] result = receivedData.Take(expectedTotal).ToArray();
                            responseTcs.TrySetResult(result);
                        }
                    }
                };
                // 订阅原始数据接收事件
                RawDataReceived += onDataReceived;
                try
                {
                    // 发送命令
                    bool sent = await SendByteAsync(cmdBytes);
                    if (!sent) return string.Empty;

                    await Task.Delay(100); // 等待设备处理命令
                    var completed = await Task.WhenAny(responseTcs.Task,
                        Task.Delay(Timeout.Infinite, timeoutCts.Token));
                    if (completed != responseTcs.Task) return string.Empty; // 超时

                    // 获取接收到的字节数据
                    byte[] buffer = await responseTcs.Task;
                    if (buffer.Length == 0) return string.Empty;

                    //CRC 校验（如果启用）
                    if (Receive_CRC_Check)
                    {
                        byte[] origin = buffer;
                        byte[] crcori;
                        byte[] build;
                        bool CRC_Pass = CheckReceive_CRC(buffer, out crcori, out build);
                        if (!CRC_Pass)
                        {
                            //CRC校验不通过
                            return "-1   " + Encoding.ASCII.GetString(origin) + $"接收长度{origin.Length},期待长度{returnCount};\r收到的CRC:{crcori[0]},{crcori[1]};校验值:{build[0]},{build[1]}";

                        }
                    }
                    return Encoding.ASCII.GetString(buffer);
                }
                catch { return string.Empty; }
                finally
                {
                    // 无论成功或异常，都要取消事件订阅，避免内存泄漏和干扰下次调用
                    RawDataReceived -= onDataReceived;
                }
            }
            finally
            {
                _sendLock.Release();
            }


        }
    }
}


