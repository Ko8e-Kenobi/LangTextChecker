using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace LangTextChecker
{
    public class Commands
    {
        public delegate void SomeVoidMethod();
        public class CompicatedCommand : ICommand
        {
            private readonly Predicate<object> _canExecute;
            private readonly Action<object> _execute;

            public CompicatedCommand(Predicate<object> canExecute, Action<object> execute)
            {
                _canExecute = canExecute;
                _execute = execute;
            }

            public event EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }

            public bool CanExecute(object parameter)
            {
                return _canExecute(parameter);
            }

            public void Execute(object parameter)
            {
                _execute(parameter);
            }
        }

        public class VoidCommand : ICommand
        {
            SomeVoidMethod ExecuteMethod;
            bool canExecute;
            public VoidCommand(SomeVoidMethod ToExecute, bool canExecute)
            {
                ExecuteMethod = ToExecute;
                this.canExecute = canExecute;
            }
            #region ICommand Members  

            public bool CanExecute(object parameter)
            {
                return canExecute;
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public void Execute(object parameter)
            {
                ExecuteMethod();
            }

            #endregion
        }

        public class BrowseMessageFileCmd : ICommand
        {

            public BrowseMessageFileCmd()
            {
                //ExecuteMethod = voidMethod;
            }
            #region ICommand Members  

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public void Execute(object parameter)
            {
                MessageBox.Show($"SIMPLE: ");
            }
            #endregion
        }
    }
}
