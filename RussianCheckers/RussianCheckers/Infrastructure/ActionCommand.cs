using System;
using System.Windows.Input;

namespace RussianCheckers.Infrastructure
{

    public class ActionCommand : ICommand
    {
        private readonly Action<object> _action;
        private readonly Func<bool> _canExecuteFunc;
        public event EventHandler CanExecuteChanged;

        public ActionCommand(Action<object> action, Func<bool> canExecuteFunc  = null)
        {
            _action = action;
            _canExecuteFunc = canExecuteFunc;
        }
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
        
        public bool CanExecute(object parameter)
        {
            return _canExecuteFunc == null || _canExecuteFunc();
        }
        public void Execute(object parameter)
        {
            _action(parameter);
        }
    }
}