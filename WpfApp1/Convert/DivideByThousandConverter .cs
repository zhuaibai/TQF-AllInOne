using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WpfApp1.Convert
{
    public class DivideByThousandConverter : IValueConverter
    {
        // 从 ViewModel -> UI
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                double result = intValue / 1000.0;
                return result.ToString("F3")+"V"; // 保留三位小数
            }
            return "0.000V";
        }

        // 从 UI -> ViewModel（如果需要反向绑定）
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (double.TryParse(value as string, out double doubleValue))
            {
                return (int)(doubleValue * 1000);
            }
            return 0;
        }
    }
}
