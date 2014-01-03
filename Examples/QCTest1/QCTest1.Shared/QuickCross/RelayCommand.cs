using System;
using System.Windows.Input;

namespace QuickCross
{
    public class RelayCommand : ICommand
    {
        private readonly Action _handler;
        private readonly Action<object> _handlerWithParameter;
        private bool _isEnabled;

        public RelayCommand(Action handler, bool isEnabled = true)
        {
            _handler = handler;
            _isEnabled = isEnabled;
        }

        public RelayCommand(Action<object> handler, bool isEnabled = true)
        {
            _handlerWithParameter = handler;
            _isEnabled = isEnabled;
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (value != _isEnabled)
                {
                    _isEnabled = value;
                    if (CanExecuteChanged != null)
                    {
                        CanExecuteChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            return IsEnabled;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (_handlerWithParameter != null) _handlerWithParameter(parameter); else _handler();
        }
    }
}
