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

namespace WpfApp1.UserControls
{
    /// <summary>
    /// BatteryControl.xaml 的交互逻辑
    /// </summary>
    public partial class BatteryControl : UserControl
    {
        public BatteryControl()
        {
            InitializeComponent();
            
        }
        // 电量百分比 (0~100)
        public int BatteryValue
        {
            get { return (int)GetValue(BatteryValueProperty); }
            set { SetValue(BatteryValueProperty, value); }
        }

        public static readonly DependencyProperty BatteryValueProperty =
            DependencyProperty.Register("BatteryValue", typeof(int), typeof(BatteryControl),
                new PropertyMetadata(0, OnBatteryValueChanged));

        private static void OnBatteryValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (BatteryControl)d;
            control.Dispatcher.InvokeAsync(() => control.UpdateFillWidth());
        }

        private void UpdateFillWidth()
        {
            double width = Math.Max(0, ActualWidth);
            double percent = Math.Clamp(BatteryValue, 0, 100) / 100.0;
            FillRect.Width = width * percent;

            // 根据电量调整颜色
            if (BatteryValue > 20)
                FillRect.Fill = new SolidColorBrush(Colors.LimeGreen);
            else if (BatteryValue > 10)
                FillRect.Fill = new SolidColorBrush(Colors.Gold);
            else
                FillRect.Fill = new SolidColorBrush(Colors.Red);
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateFillWidth();
        }

        //public double BatteryFillWidth => (ActualWidth - 10) * Math.Clamp(BatteryValue, 0, 100) / 100.0;


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateFillWidth(); // 控件布局完成，这时 ActualWidth 有效
        }

        
    }
}
