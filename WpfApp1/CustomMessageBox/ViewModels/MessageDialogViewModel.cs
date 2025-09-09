using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using WpfApp1.Command;
using WpfApp1.CustomMessageBox.Models;
using WpfApp1.ViewModels;


namespace WpfApp1.CustomMessageBox.ViewModels
{
    public class MessageDialogViewModel : BaseViewModel
    {
        private readonly MessageDialogModel _model;
        private MessageResult _result = MessageResult.None;
        private Window _window;

        public MessageDialogViewModel(MessageDialogModel model, Window window)
        {
            _model = model;
            _window = window;

            OkCommand = new RelayCommand(OnOk);
            CancelCommand = new RelayCommand(OnCancel);
            CloseCommand = new RelayCommand(OnClose);

            SetIconProperties();
        }

        public string Title => _model.Title;
        public string Message => _model.Message;
        public double FontSize => _model.FontSize;

        public Geometry IconPathData { get; private set; }
        public Brush IconBrush { get; private set; }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand CloseCommand { get; }

        public MessageResult Result
        {
            get => _result;
            set
            {
                _result = value;
                OnPropertyChanged();
            }
        }

        private void SetIconProperties()
        {
            switch (_model.Icon)
            {
                case MessageIcon.Information:
                    IconPathData = Geometry.Parse("M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 15h-2v-6h2v6zm0-8h-2V7h2v2z");
                    IconBrush = new SolidColorBrush(Color.FromRgb(0, 120, 215)); // 蓝色
                    break;
                case MessageIcon.Warning:
                    IconPathData = Geometry.Parse("M12 5.99L19.53 19H4.47L12 5.99M12 2L1 21h22L12 2zm1 14h-2v2h2v-2zm0-6h-2v4h2v-4z");
                    IconBrush = new SolidColorBrush(Color.FromRgb(255, 185, 0)); // 黄色
                    break;
                case MessageIcon.Error:
                    IconPathData = Geometry.Parse("M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 15h-2v-2h2v2zm0-4h-2V7h2v6z");
                    IconBrush = new SolidColorBrush(Color.FromRgb(232, 17, 35)); // 红色
                    break;
                case MessageIcon.Question:
                    IconPathData = Geometry.Parse("M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 17h-2v-2h2v2zm2.07-7.75l-.9.92C13.45 12.9 13 13.5 13 15h-2v-.5c0-1.1.45-2.1 1.17-2.83l1.24-1.26c.37-.36.59-.86.59-1.41 0-1.1-.9-2-2-2s-2 .9-2 2H8c0-2.21 1.79-4 4-4s4 1.79 4 4c0 .88-.36 1.68-.93 2.25z");
                    IconBrush = new SolidColorBrush(Color.FromRgb(0, 120, 215)); // 蓝色
                    break;
                case MessageIcon.Pass: // 新增 PASS 图标  带圆环的勾选图标
                    IconPathData = Geometry.Parse("M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-2 15l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z");
                    IconBrush = new SolidColorBrush(Color.FromRgb(0, 180, 0));
                    break;
                
            }
        }

        private void OnOk()
        {
            Result = MessageResult.OK;
            _window.DialogResult = true;
            _window.Close();
        }

        private void OnCancel()
        {
            Result = MessageResult.Cancel;
            _window.DialogResult = false;
            _window.Close();
        }

        private void OnClose()
        {
            Result = MessageResult.Cancel;
            _window.DialogResult = false;
            _window.Close();
        }
    }
}
