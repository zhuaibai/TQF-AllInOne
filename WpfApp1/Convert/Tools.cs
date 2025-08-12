using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace WpfApp1.Convert
{
    public class Tools
    {
        /// <summary>
        /// 根据0/1返回开关状态
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static string ConvertState(String state)
        {
            if (state == null) return "";
            if (state == "1")
            {
                return App.GetText("开启");
            }
            else if (state == "0")
            {
                return App.GetText("关闭");
            }
            else
                return state;
        }


        /// <summary>
        /// 带符号的数字字符串转换成数字
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveLeadingZeros(string input)
        {
            //检查是否含有+、-符号
            if (input.Contains("+") || input.Contains("-")){
                string tag = input.Substring(0, 1);
                input = input.Substring(1);
                string outPut = removeLeadingZeros(input);
                return $"{tag}{outPut}";
            }
            else
            {
                return removeLeadingZeros(input);
            }

           
        }

        /// <summary>
        /// 不带符号的数字字符串转换成数字
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string removeLeadingZeros(string input)
        {
            // 先检查是否为纯数字或者包含小数点
            if (input.Contains('.'))
            {
                // 分割整数部分和小数部分
                string[] parts = input.Split('.');
                string integerPart = parts[0].TrimStart('0');
                string decimalPart = parts[1];

                // 如果整数部分为空，说明整数部分全是 0，置为 "0"
                if (string.IsNullOrEmpty(integerPart))
                {
                    integerPart = "0";
                }

                // 重新组合整数部分和小数部分
                return $"{integerPart}.{decimalPart}";
            }
            else
            {
                // 不包含小数点，使用原逻辑
                string result = input.TrimStart('0');
                return string.IsNullOrEmpty(result) ? "0" : result;
            }
        }

        /// <summary>
        /// 三位数补零【60/6 → 060】
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string PadToThreeDigits(string input)
        {
            if (int.TryParse(input, out int number))
            {
                return number.ToString("D3"); // D3格式 = 三位数补零
            }
            return "";
            //throw new ArgumentException("输入必须是有效数字字符串");
        }

        /// <summary>
        /// 格式化为两位整数+一位小数【 XX/X → XX.X】
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string FormatToXxx(string input)
        {
            // 处理整数输入（X 或 XX）
            if (int.TryParse(input, out int integerValue))
            {
                return (integerValue).ToString("F1", System.Globalization.CultureInfo.InvariantCulture)
                       .PadLeft(4, '0'); // 统一补零至XX.X格式
            }

            // 处理带小数点的输入（如1.2）
            if (input.Contains('.'))
            {
                string[] parts = input.Split('.');
                if (parts.Length == 2 &&
                    int.TryParse(parts[0], out int whole) &&
                    int.TryParse(parts[1], out int fraction))
                {
                    return $"{whole:D2}.{fraction}0".Substring(0, 4); // 格式化为XX.X
                }
            }
            return "";
            //throw new ArgumentException("输入格式无效，示例：'5'→'05.0', '1.2'→'01.2'");
        }


    }
    public static class WatermarkService
    {
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.RegisterAttached("Watermark", typeof(string), typeof(WatermarkService),
                new PropertyMetadata(string.Empty, OnWatermarkChanged));

        private static readonly DependencyProperty IsWatermarkDisplayedProperty =
            DependencyProperty.RegisterAttached("IsWatermarkDisplayed", typeof(bool), typeof(WatermarkService),
                new PropertyMetadata(false));

        public static string GetWatermark(DependencyObject obj) => (string)obj.GetValue(WatermarkProperty);
        public static void SetWatermark(DependencyObject obj, string value) => obj.SetValue(WatermarkProperty, value);

        private static bool GetIsWatermarkDisplayed(DependencyObject obj) => (bool)obj.GetValue(IsWatermarkDisplayedProperty);
        private static void SetIsWatermarkDisplayed(DependencyObject obj, bool value) => obj.SetValue(IsWatermarkDisplayedProperty, value);

        private static void OnWatermarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                // 移除旧事件处理程序
                textBox.Loaded -= TextBox_Loaded;
                textBox.GotFocus -= TextBox_GotFocus;
                textBox.LostFocus -= TextBox_LostFocus;
                textBox.TextChanged -= TextBox_TextChanged;

                // 添加新事件处理程序
                textBox.Loaded += TextBox_Loaded;
                textBox.GotFocus += TextBox_GotFocus;
                textBox.LostFocus += TextBox_LostFocus;
                textBox.TextChanged += TextBox_TextChanged;

                // 初始水印状态
                CheckWatermarkState(textBox);
            }
        }

        private static void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                CheckWatermarkState(textBox);
            }
        }

        private static void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (GetIsWatermarkDisplayed(textBox))
                {
                    // 清除水印但不会触发TextChanged事件
                    textBox.Text = string.Empty;
                    textBox.Foreground = SystemColors.ControlTextBrush;
                    SetIsWatermarkDisplayed(textBox, false);
                }
            }
        }

        private static void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                CheckWatermarkState(textBox);
            }
        }

        private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                // 当用户输入时确保水印状态正确
                if (GetIsWatermarkDisplayed(textBox) && textBox.Text != GetWatermark(textBox))
                {
                    SetIsWatermarkDisplayed(textBox, false);
                    textBox.Foreground = SystemColors.ControlTextBrush;
                }
            }
        }

        private static void CheckWatermarkState(TextBox textBox)
        {
            // 如果已经显示水印，不需要再次设置
            if (GetIsWatermarkDisplayed(textBox))
                return;

            var watermark = GetWatermark(textBox);

            // 当文本框没有焦点且内容为空时显示水印
            if (!textBox.IsFocused && string.IsNullOrEmpty(textBox.Text))
            {
                // 设置标志位，避免递归
                SetIsWatermarkDisplayed(textBox, true);

                // 设置水印文本
                textBox.Text = watermark;
                textBox.Foreground = Brushes.Gray;
            }
        }
    }
}
