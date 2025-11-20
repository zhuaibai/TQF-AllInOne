using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Models;
using WpfApp1.Services;

namespace WpfApp1.Convert
{
    public static class ModbusRTU
    {

        public static Action<string>? showStatue;

        /// <summary>
        /// 生成 Modbus RTU 0x10 写多个寄存器帧
        /// </summary>
        /// <param name="devAddr">设备地址</param>
        /// <param name="startAddr">起始寄存器地址</param>
        /// <param name="values">要写入的寄存器值数组(int，高低位转换)</param>
        /// <returns>发送字节数组</returns>
        public static byte[] BuildWriteMultiRegisterFrame(byte devAddr, ushort startAddr, int[] values)
        {
            byte functionCode = 0x10;
            ushort registerCount = (ushort)values.Length;
            byte byteCount = (byte)(registerCount * 2);

            List<byte> frame = new List<byte>();
            frame.Add(devAddr);
            frame.Add(functionCode);
            frame.Add((byte)(startAddr >> 8));
            frame.Add((byte)(startAddr & 0xFF));
            frame.Add((byte)(registerCount >> 8));
            frame.Add((byte)(registerCount & 0xFF));
            frame.Add(byteCount);

            // 写入寄存器值（每个寄存器 2 字节，低字节在前）
            foreach (var value in values)
            {
                frame.Add((byte)(value & 0xFF)); // 低字节
                frame.Add((byte)(value >> 8));   // 高字节
            }

            // CRC16
            ushort crc = CRC16(frame.ToArray(), frame.Count);
            frame.Add((byte)(crc & 0xFF));      // CRC低字节
            frame.Add((byte)(crc >> 8));        // CRC高字节

            return frame.ToArray();
        }

        /// <summary>
        /// 解析 0x10 写寄存器返回帧
        /// </summary>
        /// <param name="response">串口收到的数据</param>
        /// <returns>写入成功的寄存器数量</returns>
        public static int ParseWriteMultiRegisterResponse(byte[] response)
        {
            if (response.Length < 8) throw new Exception("响应长度错误");

            byte devAddr = response[0];
            byte func = response[1];
            ushort startAddr = (ushort)((response[2] << 8) | response[3]);
            ushort regCount = (ushort)((response[4] << 8) | response[5]);

            // 校验 CRC
            ushort calcCRC = CRC16(response, response.Length - 2);
            ushort recvCRC = (ushort)((response[response.Length - 1] << 8) | response[response.Length - 2]);
            if (calcCRC != recvCRC) throw new Exception("CRC 校验失败");

            return regCount;
        }

        /// <summary>
        /// 生成 Modbus RTU 0x06 写单个寄存器指令
        /// </summary>
        /// <param name="devAddr">设备地址</param>
        /// <param name="regAddr">寄存器地址</param>
        /// <param name="value">写入值(int)</param>
        /// <returns>Modbus数据帧</returns>
        public static byte[] BuildWriteSingleRegisterFrame(byte devAddr, ushort regAddr, int value)
        {
            List<byte> frame = new List<byte>();

            frame.Add(devAddr);                // 设备地址
            frame.Add(0x06);                   // 功能码 06
            frame.Add((byte)(regAddr >> 8));   // 寄存器高位
            frame.Add((byte)(regAddr & 0xFF));// 寄存器低位

            frame.Add((byte)(value & 0xFF));   // 数据低位
            frame.Add((byte)(value >> 8));     // 数据高位

            ushort crc = CRC16(frame.ToArray(), frame.Count);
            frame.Add((byte)(crc & 0xFF));     // CRC低字节
            frame.Add((byte)(crc >> 8));       // CRC高字节

            return frame.ToArray();
        }


        /// <summary>
        /// 生成 Modbus 功能码 03 的发送帧
        /// </summary>
        /// <param name="slaveAddr">从站地址</param>
        /// <param name="startAddr">起始寄存器地址</param>
        /// <param name="regCount">读取的寄存器数量</param>
        /// <returns>完整的 Modbus RTU 帧（含 CRC）</returns>
        public static byte[] BuildRead03Frame(byte slaveAddr, ushort startAddr, ushort regCount)
        {
            List<byte> frame = new List<byte>();

            frame.Add(slaveAddr);                // 从站地址
            frame.Add(0x03);                     // 功能码 03
            frame.Add((byte)(startAddr >> 8));   // 起始地址高字节
            frame.Add((byte)(startAddr & 0xFF)); // 起始地址低字节
            frame.Add((byte)(regCount >> 8));    // 寄存器数量高字节
            frame.Add((byte)(regCount & 0xFF));  // 寄存器数量低字节

            // 计算 CRC
            ushort crc = CRC16(frame.ToArray(), frame.Count);
            frame.Add((byte)(crc & 0xFF));       // CRC 低字节
            frame.Add((byte)(crc >> 8));
            // CRC 高字节

            return frame.ToArray();
        }


        /// <summary>
        /// 生成 Modbus 功能码 20 的发送帧
        /// </summary>
        /// <param name="slaveAddr">从站地址</param>
        /// <param name="startAddr">功能选项</param>
        /// <param name="regCount">历史记录</param>
        /// <returns>完整的 Modbus RTU 帧（含 CRC）</returns>
        public static byte[] BuildRead20Frame(byte slaveAddr, ushort startAddr, ushort regCount)
        {
            List<byte> frame = new List<byte>();

            frame.Add(slaveAddr);                // 从站地址
            frame.Add(0x20);                     // 功能码 03
            frame.Add((byte)(startAddr >> 8));   // 起始地址高字节
            frame.Add((byte)(startAddr & 0xFF)); // 起始地址低字节
            frame.Add((byte)(regCount >> 8));    // 寄存器数量高字节
            frame.Add((byte)(regCount & 0xFF));  // 寄存器数量低字节

            // 计算 CRC
            ushort crc = CRC16(frame.ToArray(), frame.Count);
            frame.Add((byte)(crc & 0xFF));       // CRC 低字节
            frame.Add((byte)(crc >> 8));
            // CRC 高字节

            return frame.ToArray();
        }

        /// <summary>
        /// 解析功能码 20 返回帧，提取寄存器数据（小端在前）
        /// </summary>
        /// <param name="response">从机返回的完整字节数组</param>
        /// <returns>ushort 数组，每个元素代表一个寄存器值</returns>
        public static short[] ParseRead20Response(byte[] response)
        {
            if (response.Length != 133)
            {

                showStatue("响应异常!");
                return new short[] { 1 };

            }
                

            byte slaveAddr = response[0];
            byte funcCode = response[1];
            byte byteCount = response[2];

            if (funcCode != 0x20)
                throw new Exception("功能码错误，非 0x20 响应");

            if (response.Length < 3 + byteCount + 2)
                throw new Exception("响应帧长度与数据长度不匹配");

            // CRC 校验
            short recvCrc = (short)(response[response.Length - 2] | (response[response.Length - 1] << 8));
            short calcCrc = (short)CRC16(response, response.Length - 2);
            if (recvCrc != calcCrc)
                throw new Exception("CRC 校验错误");

            int regCount = byteCount / 2;
            short[] registers = new short[regCount];

            for (int i = 0; i < regCount; i++)
            {
                int dataIndex = 3 + i * 2;
                // 数据为大端格式（高字节在前），但用户希望小端在前返回
                short value = (short)((response[dataIndex + 1] << 8) | response[dataIndex]);
                registers[i] = value;
            }

            return registers;
        }

        /// <summary>
        /// 解析功能码 03 返回帧，提取寄存器数据（小端在前）
        /// </summary>
        /// <param name="response">从机返回的完整字节数组</param>
        /// <returns>ushort 数组，每个元素代表一个寄存器值</returns>
        public static short[] ParseRead03Response(byte[] response)
        {
            if(response == null)
            {
                showStatue("响应帧长度不足");
                return null;
            }
            

            if (response.Length < 5)
            {
                showStatue("响应帧长度不足");
                return null;
            }
                

            byte slaveAddr = response[0];
            byte funcCode = response[1];
            byte byteCount = response[2];

            if (funcCode != 0x03)
            {
                showStatue("功能码错误，非 0x03 响应");
                return null;             
            }
            if (response.Length < 3 + byteCount + 2)
            {
                showStatue("响应帧长度不足");
                return null;
            }




            // CRC 校验
            short recvCrc = (short)(response[response.Length - 2] | (response[response.Length - 1] << 8));
            short calcCrc = (short)CRC16(response, response.Length - 2);
            if (recvCrc != calcCrc)
            {
                showStatue("CRC 校验错误");
                return null;
            }
               

            int regCount = byteCount / 2;
            short[] registers = new short[regCount];

            for (int i = 0; i < regCount; i++)
            {
                int dataIndex = 3 + i * 2;
                // 数据为大端格式（高字节在前），但用户希望小端在前返回
                short value = (short)((response[dataIndex + 1] << 8) | response[dataIndex]);
                registers[i] = value;
            }
            showStatue("通讯正常");
            return registers;
        }

        /// <summary>
        /// Modbus CRC16 计算（多项式 0xA001）
        /// </summary>
        private static ushort CRC16(byte[] data, int length)
        {
            ushort crc = 0xFFFF;

            for (int i = 0; i < length; i++)
            {
                crc ^= data[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x0001) != 0)
                        crc = (ushort)((crc >> 1) ^ 0xA001);
                    else
                        crc >>= 1;
                }
            }

            return crc;
        }

        /// <summary>
        /// 判断字符串中是否包含  ℃
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool ContainsDegreeSymbol(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            return input.Contains("℃");
        }

        /// <summary>
        /// 判断字符串中是否包含  %
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool ContainsPercentSymbol(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            return input.Contains("%");
        }

        


        /// <summary>
        /// 自动生成发送包方法【8位帧头 + 一位数据长度(106) + 数据 + CRC校验(数据长度+数据)】
        /// </summary>
        /// <param name="HeadBytes">帧头字符串</param>
        /// <param name="sendingCommands">发送的数据</param>
        /// <returns>需要发送的一帧</returns>
        /// <exception cref="ArgumentException"></exception>
        public static byte[] GetSendBytes(string HeadBytes, ObservableCollection<SendingCommand> sendingCommands)
        {
            using var ms = new MemoryStream();//进行校验位的缓存
            ObservableCollection<SendingCommand> resultCommand = new ObservableCollection<SendingCommand>();//转换回原来的地址顺序
            

            //一、 帧头
            byte[] bytes = new byte[] { 0x01, 0x10, 0x00, 0x82, 0x00, 0x70, 0xE0};
            // msCRC.Write(bytes);

            //二、 数据长度  固定182
            ms.Write(bytes);


            //三、 数据

            //复原原来的数据地址
            //for (int i = 0; i < sendingCommands.Count; i++)
            //{

            //    if (i >= 22 && i <= 90)
            //    {
            //        //跳过四个
            //        resultCommand.Add(sendingCommands[i + 4]);
            //    }
            //    else if (i >= 91)
            //    {
            //        resultCommand.Add(sendingCommands[i - 69]);
            //    }
            //    else
            //        resultCommand.Add(sendingCommands[i]);
            //}

            foreach (var sendingCommand in sendingCommands)
            {
                
                if (ModbusRTU.ContainsDegreeSymbol(sendingCommand.Command))
                {
                    //包含℃符号，也就是温度，进行处理
                    if (double.TryParse(sendingCommand.ReturnCount, out double doubleValue))
                    {
                        short.TryParse((doubleValue * 10).ToString("F0"), out short valueE);
                        ms.WriteByte((byte)(valueE & 0xFF)); // 低字节
                        ms.WriteByte((byte)(valueE >> 8));  // 高字节
                    }
                }else if (ModbusRTU.ContainsPercentSymbol(sendingCommand.Command))
                {
                    //包含%符号，也就是百分比，进行处理
                    if (double.TryParse(sendingCommand.ReturnCount, out double doubleValue))
                    {
                        short.TryParse((doubleValue * 100).ToString("F0"), out short valueE);
                        ms.WriteByte((byte)(valueE & 0xFF)); // 低字节
                        ms.WriteByte((byte)(valueE >> 8));  // 高字节
                    }
                }
                else
                {
                    // 尝试解析字符串为整数
                    if (!short.TryParse(sendingCommand.ReturnCount, out short value))
                    {
                        throw new ArgumentException($"无法将 '{sendingCommand.ReturnCount}' 解析为整数。", sendingCommand.Command);
                    }
                    ms.WriteByte((byte)(value & 0xFF)); // 低字节
                    ms.WriteByte((byte)(value >> 8));  // 高字节
                }
                
            }

            //四、CRC校验
            //计算CRC校验值
            byte[] crc = SerialCommunicationService.getCRC16(ms.ToArray(), ms.ToArray().Length);
            ms.Write(crc);

            byte[] data = ms.ToArray();
            return data;
        }
 

        /// <summary>
        /// 把130-235(106个数据)的数据进行解析
        /// </summary>
        /// <param name="data">返回字节数组</param>
        /// <param name="sendingCommands">显示数据集合对象</param>
        public static void AnalyseSetReceive(short[] data, ObservableCollection<SendingCommand> sendingCommands)
        {
            if (data == null || sendingCommands == null || sendingCommands.Count == 0)
                return;

            if (data.Length != sendingCommands.Count)
            {
                return;
            }
            for (int i = 0; i < sendingCommands.Count; i++)
            {
                if (ModbusRTU.ContainsDegreeSymbol(sendingCommands[i].Command))
                {
                    //包含℃符号，也就是温度，进行处理
                    sendingCommands[i].Enable = (data[i] / 10.0).ToString("F1");
                }
                else if (ModbusRTU.ContainsPercentSymbol(sendingCommands[i].Command))
                {
                    //包含%符号，也就是百分比，进行处理
                    sendingCommands[i].Enable = (data[i] / 100.0).ToString("F1");
                }
                else
                {
                    sendingCommands[i].Enable = data[i].ToString();
                }
            }


            //for (int i = 0; i < sendingCommands.Count; i++)
            //{
            //    if (i == 4 || i == 9)
            //    {
            //        //对百分比进行处理
            //        sendingCommands[i].Enable = (data[i] / 100.0).ToString("F1");
            //        continue;
            //    }
            //    if (i == 37)
            //    {
            //        //对百分比进行处理
            //        sendingCommands[i+4].Enable = (data[i] / 100.0).ToString("F1");
            //        continue;
            //    }
            //    if (i >= 22 && i <= 90)
            //    {
            //        //跳过四个
            //        sendingCommands[i + 4].Enable = ModbusRTU.ContainsDegreeSymbol(sendingCommands[i+4].Command)? (data[i] / 10.0).ToString("F1"): data[i].ToString();
            //    }
            //    else if (i >= 91)
            //    {
            //        sendingCommands[i - 69].Enable = ModbusRTU.ContainsDegreeSymbol(sendingCommands[i-69].Command) ? (data[i] / 10.0).ToString("F1") : data[i].ToString();
            //    }
            //    else
            //        sendingCommands[i].Enable = ModbusRTU.ContainsDegreeSymbol(sendingCommands[i].Command) ? (data[i] / 10.0).ToString("F1") : data[i].ToString();
            //}
        }

        /// <summary>
        /// 把130-224(95个数据)的数据进行解析(第一次赋值，把设置值也赋成读取值)
        /// </summary>
        /// <param name="data">返回字节数组</param>
        /// <param name="sendingCommands">显示数据集合对象</param>
        public static void FirstSetReceive(ObservableCollection<SendingCommand> sendingCommands)
        {
            
            for (int i = 0; i < sendingCommands.Count; i++) {
                sendingCommands[i].ReturnCount = sendingCommands[i].Enable;
            }
        }


        //0 充电MOS 1 放电MOS 9 负载连接 4 充电 5 放电 
        /// <summary>
        /// 获取16个位的值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int[] GetBits(short value)
        {
            if (value == null)
            {
                return null;
            }
            int[] bits = new int[16]; // ushort 是 16 位

            for (int i = 0; i < 16; i++)
            {
                bits[i] = (value >> i) & 1; // 右移 i 位后取最低位
            }

            return bits;
        }

    }
}
