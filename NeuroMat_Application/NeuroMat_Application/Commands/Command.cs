using System;
using System.Windows.Input;

namespace NeuroMat_Application.Commands
{
    public class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action<object> _executeAction;
        private Func<bool> _updateStatus;

        public Command(Action<object> executeAction, Func<bool> updateStatus)
        {
            _executeAction = executeAction;
            _updateStatus = updateStatus;
        }

        public bool CanExecute(object parameter)
        {
            return _updateStatus();
        }

        public void Execute(object parameter)
        {
            _executeAction.Invoke(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
