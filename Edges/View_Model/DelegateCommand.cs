using System;
using System.Windows.Input;

namespace Edges.View_Model
{
    internal class DelegateCommand<T> : ICommand where T : class
    {
        #region Private Members

        private readonly Predicate<T> _canExecute;
        private readonly Action<T> _execute;

        #endregion

        #region Construction

        public DelegateCommand(Predicate<T> canExecute, Action<T> execute)
        {
            _canExecute = canExecute;
            _execute = execute;
        }

        public DelegateCommand(Action<T> execute)
            : this(null, execute)
        {

        }

        #endregion

        #region ICommand

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter as T);
        }

        public void Execute(object parameter)
        {
            if (_execute == null)
                return;

            _execute(parameter as T);
        }

        #endregion
    }

    /// <summary>
    /// Convenience class equivalent to the generic DelegateCommand of type object.
    /// <para>Encapsulates a Command with an execute Action and an optional canExecute Predicate.</para>
    /// </summary>
    internal class DelegateCommand : DelegateCommand<object>
    {
        /// <summary>Create a DelegateCommand which takes an object input parameter during execution</summary>
        public DelegateCommand(Action<object> execute)
            : base(execute) { }

        /// <summary>Create a DelegateCommand which takes an object input parameter during test and execution</summary>
        public DelegateCommand(Predicate<object> canExecute, Action<object> execute)
            : base(canExecute, execute) { }

        /// <summary>Create a DelegateCommand which executes without input parameters</summary>
        public DelegateCommand(Action execute)
            : base(o => execute()) { }

        /// <summary>Create a DelegateCommand which executes without input parameters during test or execution</summary>
        public DelegateCommand(Func<bool> canExecute, Action execute)
            : base(o => canExecute(), o => execute()) { }
    }
}
