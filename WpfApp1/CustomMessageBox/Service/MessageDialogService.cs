using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using WpfApp1.CustomMessageBox.Models;
using WpfApp1.CustomMessageBox.ViewModels;
using WpfApp1.CustomMessageBox.Views;

namespace WpfApp1.CustomMessageBox.Service
{
    public class MessageDialogService : IMessageDialogService
    {
        public MessageResult Show(string message, string title = "系统提示",
                                MessageIcon icon = MessageIcon.Information,
                                double fontSize = 18)
        {
            var model = new MessageDialogModel
            {
                Message = message,
                Title = title,
                Icon = icon,
                FontSize = fontSize
            };

            var view = new MessageDialogView();
            var viewModel = new MessageDialogViewModel(model, view);
            view.DataContext = viewModel;

            view.ShowDialog();

            return viewModel.Result;
        }

       public string ShowInputDialog(
       string message,
       string title = "请输入",
       InputType inputType = InputType.Text,
       string defaultInput = "",
       string inputLabel = "请输入:",
       Func<string, bool> validator = null,
       string validationMessage = "输入无效",
       int maxLength = 255,
       int multilineHeight = 100,
       double fontSize = 18)
        {
            var model = new InputDialogModel
            {
                Message = message,
                Title = title,
                InputType = inputType,
                DefaultInput = defaultInput,
                InputLabel = inputLabel,
                Validator = validator,
                ValidationMessage = validationMessage,
                MaxLength = maxLength,
                MultilineHeight = multilineHeight,
                FontSize = fontSize
            };

            var view = new InputDialogView();
            //var window = new Window
            //{
            //    WindowStyle = WindowStyle.None,
            //    WindowStartupLocation = WindowStartupLocation.CenterScreen,
            //    SizeToContent = SizeToContent.WidthAndHeight,
            //    ResizeMode = ResizeMode.NoResize,
            //    Content = view
            //};

            var viewModel = new InputDialogViewModel(model, view);
            view.DataContext = viewModel;

            // 绑定密码框
            if (inputType == InputType.Password)
            {
                var passwordBox = view.FindName("PasswordInput") as PasswordBox;
                if (passwordBox != null)
                {
                    passwordBox.PasswordChanged += (s, e) =>
                    {
                        viewModel.UserInput = passwordBox.Password;
                    };
                }
            }

            view.ShowDialog();

            return viewModel.ResultInput;
        }
    }
}
