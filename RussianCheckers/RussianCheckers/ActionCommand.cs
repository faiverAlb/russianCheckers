using System;
using System.Windows.Input;

namespace RussianCheckers
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
            if (_canExecuteFunc != null)
                return _canExecuteFunc();
            return true;
        }
        public void Execute(object parameter)
        {
            _action(parameter);
        }
    }
}