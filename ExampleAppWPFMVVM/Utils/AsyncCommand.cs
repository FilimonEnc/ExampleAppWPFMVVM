using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ExampleAppWPFMVVM.Utils
{
    public class AsyncCommand : ICommand
    {

        readonly Func<object?, Task> _executeTask;
        readonly Func<object?, bool> _canExecuteTask;

        public bool IsExecuting
        {
            get => _isExecuting;
            set
            {
                _isExecuting = value;
            }
        }
        private bool _isExecuting;

        public AsyncCommand(Func<object?, Task> execute, Func<object?, bool> canExecute = null!)
        {
            _executeTask = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecuteTask = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            if (IsExecuting) return false;
            return _canExecuteTask == null || _canExecuteTask(parameter);
        }

        public async void Execute(object? parameter)
        {
            IsExecuting = true;
            CommandManager.InvalidateRequerySuggested();
            try
            {
                await _executeTask(parameter);
            }
            catch (TaskCanceledException)
            {
                // Игнорирование отмененных задач
            }
            //catch (ApiException ex)
            //{
            //    MessageBox.Show(ex.Message, "Ошибка от сервера", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Непредвиденная ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            IsExecuting = false;
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
