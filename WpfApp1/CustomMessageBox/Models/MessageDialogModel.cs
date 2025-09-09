using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.CustomMessageBox.Models
{
    public class MessageDialogModel
    {
        public string Title { get; set; } = "系统提示";
        public string Message { get; set; } = string.Empty;
        public MessageIcon Icon { get; set; } = MessageIcon.Information;
        public double FontSize { get; set; } = 18;
    }
}
