using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;

namespace WpfApp1.Convert
{
    public class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                return intValue < 0 ? Visibility.Visible : Visibility.Hidden;
            }

            // 如果绑定类型不是int，也返回Hidden避免异常
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IntToHiddenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                return intValue > 0 ? Visibility.Visible : Visibility.Hidden;
            }

            // 如果绑定类型不是int，也返回Hidden避免异常
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IntToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                // 1 → #FF0000（红色）  你可以换成自己的颜色
                if (intValue == 1)
                    return (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF0000"));

                // 0 → #1677FF（蓝色）
                return (SolidColorBrush)(new BrushConverter().ConvertFrom("#1677FF"));
            }

            // 非 int 类型时，返回蓝色防止异常
            return new SolidColorBrush(Colors.Blue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IntToColorConverter2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                // 1 → #FF0000（红色）  你可以换成自己的颜色
                if (intValue == 0)
                    return (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF0000"));

                // 0 → #1677FF（蓝色）
                return (SolidColorBrush)(new BrushConverter().ConvertFrom("#1677FF"));
            }

            // 非 int 类型时，返回蓝色防止异常
            return new SolidColorBrush(Colors.Blue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
