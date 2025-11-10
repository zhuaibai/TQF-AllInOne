using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WpfApp1.Convert
{
    internal class DivideByTenConverter : IValueConverter
    {
        // 从 ViewModel -> UI
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                double result = intValue / 10.0;
                return result.ToString("F1"); //保留两位小数
            }
            return "0.0";
        }

        // 从 UI -> ViewModel（如果需要反向绑定）
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (double.TryParse(value as string, out double doubleValue))
            {
                return (int)(doubleValue * 100);
            }
            return 0;
        }
    }
}
