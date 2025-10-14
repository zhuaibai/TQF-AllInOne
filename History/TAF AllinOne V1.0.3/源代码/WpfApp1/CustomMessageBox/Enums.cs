using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.CustomMessageBox
{
    public enum MessageIcon
    {
        Information,
        Warning,
        Error,
        Question,
        Pass // 新增通过图标
    }


    public enum MessageResult
    {
        None,
        OK,
        Cancel
    }

    public enum InputType
    {
        Text,       // 单行文本
        Multiline,  // 多行文本
        Number,     // 数字输入
        Password,   // 密码输入
        Date        // 日期输入
    }
}
