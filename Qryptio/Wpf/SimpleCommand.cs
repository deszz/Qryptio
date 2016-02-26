using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Qryptio.Wpf
{
    public class SimpleCommand : ICommand
    {
        public bool CommandSucceeded
        { get; set; }

        public Predicate<object> CanExecuteDelegate
        { get; set; }

        public Action SimpleExecuteDelegate
        { get; set; }

        public Action<object> ExecuteDelegate
        { get; set; }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return CanExecuteDelegate == null || CanExecuteDelegate(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        [DebuggerStepThrough]
        public void Execute(object parameter)
        {
            if (SimpleExecuteDelegate != null)
            {
                SimpleExecuteDelegate();
                CommandSucceeded = true;
            }
            else if (ExecuteDelegate != null)
            {
                ExecuteDelegate(parameter);
                CommandSucceeded = true;
            }
        }
    }
}
