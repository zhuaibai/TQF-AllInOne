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
using System.Windows.Shapes;

namespace WpfApp1.UserControls
{
    /// <summary>
    /// BMS03_UserControl.xaml 的交互逻辑
    /// </summary>
    public partial class BMS03_UserControl : UserControl
    {
        public BMS03_UserControl()
        {
            InitializeComponent();
        }

        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = UIElement.MouseWheelEvent,
                Source = sender
            };

            // 把滚轮事件传递给父级 ScrollViewer
            var parent = ((Control)sender).Parent as UIElement;
            parent?.RaiseEvent(eventArg);
        }

        /// <summary>
        /// 点击概览图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ElementView_Click(object sender, RoutedEventArgs e)
        {
            SettingView.Visibility = Visibility.Collapsed;
            SystemSettingView.Visibility = Visibility.Collapsed;
            HistoryLogView.Visibility = Visibility.Collapsed;
            AdminView.Visibility = Visibility.Collapsed;
            RealTimeMonitor.Visibility = Visibility.Collapsed;
            UnionMonitor.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 参数设置界面显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ElementSetting_Click(object sender, RoutedEventArgs e)
        {
            SettingView.Visibility = Visibility.Visible;
            SystemSettingView.Visibility = Visibility.Collapsed;
            HistoryLogView.Visibility = Visibility.Collapsed;
            AdminView.Visibility = Visibility.Collapsed;
            RealTimeMonitor.Visibility = Visibility.Collapsed;
            UnionMonitor.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 系统设置界面显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ElementSystemSetting_Click(object sender, RoutedEventArgs e)
        {
            SettingView.Visibility = Visibility.Collapsed;
            SystemSettingView.Visibility = Visibility.Visible;
            HistoryLogView.Visibility = Visibility.Collapsed;
            AdminView.Visibility = Visibility.Collapsed;
            RealTimeMonitor.Visibility = Visibility.Collapsed;
            UnionMonitor.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 历史记录界面显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HistoryLog_Click(object sender, RoutedEventArgs e)
        {
            SettingView.Visibility = Visibility.Collapsed;
            SystemSettingView.Visibility = Visibility.Collapsed;
            HistoryLogView.Visibility = Visibility.Visible;
            AdminView.Visibility = Visibility.Collapsed;
            RealTimeMonitor.Visibility = Visibility.Collapsed;
            UnionMonitor.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 前端芯片监控
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdminView_Click(object sender, RoutedEventArgs e)
        {
            SettingView.Visibility = Visibility.Collapsed;
            SystemSettingView.Visibility = Visibility.Collapsed;
            HistoryLogView.Visibility = Visibility.Collapsed;
            RealTimeMonitor.Visibility = Visibility.Collapsed;
            AdminView.Visibility = Visibility.Visible;
            UnionMonitor.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 前端芯片监控
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RealTimeMonitor_Click(object sender, RoutedEventArgs e)
        {
            SettingView.Visibility = Visibility.Collapsed;
            SystemSettingView.Visibility = Visibility.Collapsed;
            HistoryLogView.Visibility = Visibility.Collapsed;
            AdminView.Visibility = Visibility.Collapsed;
            RealTimeMonitor.Visibility = Visibility.Visible;
            UnionMonitor.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 并联监控
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnionMonitor_Click(object sender, RoutedEventArgs e)
        {
            SettingView.Visibility = Visibility.Collapsed;
            SystemSettingView.Visibility = Visibility.Collapsed;
            HistoryLogView.Visibility = Visibility.Collapsed;
            AdminView.Visibility = Visibility.Collapsed;
            RealTimeMonitor.Visibility = Visibility.Collapsed;
            UnionMonitor.Visibility = Visibility.Visible;
        }




    }
}
