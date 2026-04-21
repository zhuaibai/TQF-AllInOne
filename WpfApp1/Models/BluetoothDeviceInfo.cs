using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    public class BluetoothDeviceInfo
    {
        public string Id { get; set; } = string.Empty;// 设备唯一标识符
        public string Name { get; set; } = string.Empty;// 设备名称
        public ulong BluetoothAddress { get; set; }// 蓝牙地址

        public override string ToString()
        {
            return $"{Name} ({BluetoothAddress:X}) ";
        }

    }
}
