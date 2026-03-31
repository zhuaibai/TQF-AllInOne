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
    /// CG_MonitorUC.xaml 的交互逻辑
    /// </summary>
    public partial class CG_MonitorUC : UserControl
    {
        public CG_MonitorUC()
        {
            InitializeComponent();
        }

        private void ElementView_Click(object sender, RoutedEventArgs e)
        {
            SettingView.Visibility = Visibility.Collapsed;
        }

        private void ElementSetting_Click(object sender, RoutedEventArgs e)
        {
            SettingView.Visibility = Visibility.Visible;
        }
    }
}
