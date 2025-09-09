using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.CustomMessageBox.Models
{
    public class InputDialogModel : MessageDialogModel
    {
        // 输入框默认值
        public string DefaultInput { get; set; } = string.Empty;

        // 输入类型
        public InputType InputType { get; set; } = InputType.Text;

        // 输入框标签
        public string InputLabel { get; set; } = "请输入:";

        // 输入验证函数
        public Func<string, bool> Validator { get; set; } = input => true;

        // 验证失败提示
        public string ValidationMessage { get; set; } = "输入无效，请重新输入";

        // 最大输入长度
        public int MaxLength { get; set; } = 255;

        // 多行输入高度
        public int MultilineHeight { get; set; } = 100;
    }
}
