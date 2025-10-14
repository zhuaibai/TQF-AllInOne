using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.CustomMessageBox.Service
{
    public interface IMessageDialogService
    {
        MessageResult Show(string message, string title = "系统提示",
                          MessageIcon icon = MessageIcon.Information,
                          double fontSize = 18);

        string ShowInputDialog(
       string message,
      string title = "请输入",
      InputType inputType = InputType.Text,
      string defaultInput = "",
      string inputLabel = "请输入:",
      Func<string, bool> validator = null,
      string validationMessage = "输入无效",
      int maxLength = 255,
      int multilineHeight = 100,
      double fontSize = 18);
    }
}
