using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.CustomMessageBox.ViewModels;

namespace WpfApp1.CustomMessageBox.Views
{
    /// <summary>
    /// InputDialogView.xaml 的交互逻辑
    /// </summary>
    public partial class InputDialogView : Window
    {
        public InputDialogView()
        {
            InitializeComponent();
            Loaded += InputDialogView_Loaded;
        }

        private void InputDialogView_Loaded(object sender, RoutedEventArgs e)
        {
            // 根据输入类型设置焦点
            if (DataContext is InputDialogViewModel vm)
            {
                switch (vm.InputType)
                {
                    case InputType.Text:
                        TextInput.Focus();
                        TextInput.SelectAll();
                        break;
                    case InputType.Password:
                        PasswordInput.Focus();
                        break;
                    case InputType.Multiline:
                        MultilineInput.Focus();
                        MultilineInput.SelectAll();
                        break;
                    case InputType.Number:
                        NumberInput.Focus();
                        NumberInput.SelectAll();
                        break;
                    case InputType.Date:
                        DateInput.Focus();
                        break;
                }
            }
        }

        private void NumberInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 只允许输入数字和小数点
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c) && c != '.' && c != '-')
                {
                    e.Handled = true;
                    return;
                }
            }

            // 检查是否已经是小数
            var textBox = (TextBox)sender;
            if (e.Text == "." && textBox.Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        // 密码框绑定处理
        private void PasswordInput_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is InputDialogViewModel vm)
            {
                vm.UserInput = PasswordInput.Password;
            }
        }
    
    }
}
