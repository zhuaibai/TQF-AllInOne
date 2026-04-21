using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfApp1.Command
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object?>? _executeAsync;
        private readonly Action? _executeSync;
        private readonly Func<bool>? _canExecute;
        private readonly Func<object?, bool>? _canExecuteWithParam;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _executeAsync = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecuteWithParam = canExecute;
        }
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _executeSync = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object? parameter)
        {
            // 优先执行异步委托，否则执行同步委托
            if (_executeAsync != null)
            {
                 _executeAsync(parameter);
            }
            else if (_executeSync != null)
            {
                _executeSync();
            }

            // 命令执行后刷新状态（例如按钮可用性）
            RaiseCanExecuteChanged();
        }

        public event EventHandler? CanExecuteChanged;

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
