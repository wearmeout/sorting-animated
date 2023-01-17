using System;
using System.Windows.Input;

namespace FractionalSorting.Visualization.Common
{
    public class DelegateCommand : ICommand
    {
        Action _execute;
        Func<bool> _canExecute;

        public DelegateCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter = null)
        {
            if (_canExecute == null)
            {
                return true;
            }
            return _canExecute();
        }

        public void Execute(object parameter = null)
        {
            if (!_canExecute?.Invoke())
                return;

            _execute?.Invoke();
        }

        public virtual void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
