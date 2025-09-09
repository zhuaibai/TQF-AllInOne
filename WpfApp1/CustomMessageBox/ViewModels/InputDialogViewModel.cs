using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using WpfApp1.Command;
using WpfApp1.CustomMessageBox.Models;
using WpfApp1.ViewModels;

namespace WpfApp1.CustomMessageBox.ViewModels
{
    public class InputDialogViewModel : BaseViewModel
    {
        private readonly InputDialogModel _model;
        private Window _window;
        private string _userInput;
        private bool _isInputValid = true;
        private string _validationError;

        public InputDialogViewModel(InputDialogModel model, Window window)
        {
            _model = model;
            _window = window;
            _userInput = model.DefaultInput;

            OkCommand = new RelayCommand(OnOk, CanExecuteOk);
            CancelCommand = new RelayCommand(OnCancel);
            //CloseCommand = new RelayCommand(OnClose);
        }

        public string Title => _model.Title;
        public string Message => _model.Message;
        public double FontSize => _model.FontSize;
        public InputType InputType => _model.InputType;
        public string InputLabel => _model.InputLabel;
        public int MaxLength => _model.MaxLength;
        public int MultilineHeight => _model.MultilineHeight;

        public string UserInput
        {
            get => _userInput;
            set
            {
                _userInput = value;
                OnPropertyChanged();
                ValidateInput();
                CommandManager.InvalidateRequerySuggested(); // 刷新命令状态
            }
        }

        public bool IsInputValid
        {
            get => _isInputValid;
            set {
                _isInputValid = value;
                OnPropertyChanged();
            }
        }

        public string ValidationError
        {
            get => _validationError;
            set {
                _validationError = value;
                OnPropertyChanged();
            }
        }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand CloseCommand { get; }

        public string ResultInput { get; private set; }
        public event Action OnClose;

        private bool CanExecuteOk()
        {
            return IsInputValid && !string.IsNullOrWhiteSpace(UserInput);
        }

        private void ValidateInput()
        {
            if (_model.Validator == null)
            {
                IsInputValid = true;
                ValidationError = string.Empty;
                return;
            }

            var isValid = _model.Validator(UserInput);
            IsInputValid = isValid;
            ValidationError = isValid ? string.Empty : _model.ValidationMessage;
        }

        private void OnOk()
        {
            if (!CanExecuteOk()) return;
            ResultInput = UserInput;
            _window.DialogResult = true;
            _window.Close();
        }

        private void OnCancel()
        {
            ResultInput = null;
            _window.DialogResult = false;
            _window.Close();
        }
       

    }
}
