using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{

    public class ConfigWrapper
    {
        public string machineName { get; set; }             //机器类型

        public string Hash { get; set; }                    //哈希校验

        public byte[] DataBytes { get; set; }               // 改为存储字节数组 配置文件数据

    }

}
