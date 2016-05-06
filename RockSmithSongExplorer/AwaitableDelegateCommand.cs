using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RockSmithSongExplorer
{

    public class AwaitableDelegateCommand : AwaitableDelegateCommand<object>
    {
        public AwaitableDelegateCommand(Func<object, Task> executeMethod, Func<object, bool> canExecuteMethod, Action<Exception> errorCallback)
            :base(executeMethod,canExecuteMethod,errorCallback)
        {
        }
    }

    public class AwaitableDelegateCommand<Tparameter> : ICommand
    {
        private readonly Func<Tparameter, Task> _executeMethod;
        private readonly Func<Tparameter, bool> _canExecuteMethod;
        private readonly Action<Exception> _errorCallback;
        private bool _isExecuting;

        public AwaitableDelegateCommand(Func<Tparameter, Task> executeMethod, Func<Tparameter, bool> canExecuteMethod, Action<Exception> errorCallback)
        {
            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
            _errorCallback = errorCallback;
        }

        public async Task ExecuteAsync(object obj)
        {
            try
            {
                _isExecuting = true;
                RaiseCanExecuteChanged();
                await _executeMethod((Tparameter)obj);
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        public bool CanExecute(object parameter)
        {
            return !_isExecuting && (_canExecuteMethod == null || _canExecuteMethod((Tparameter)parameter));
        }

        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Should not invoked from clientcode. (Use ExecuteASync instead). This method is executed from WPF UI Elements (Butttons, etc.) bound to this ICommand.
        /// </summary>
        /// <param name="parameter"></param>
        public async void Execute(object parameter)
        {
            try
            {
                await ExecuteAsync(parameter);
            }
            catch (Exception ex)
            {
                if (_errorCallback != null)
                    _errorCallback(ex);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            var hnndlr = CanExecuteChanged;
            if (hnndlr != null)
                hnndlr(this, new EventArgs());
        }
    }
}
