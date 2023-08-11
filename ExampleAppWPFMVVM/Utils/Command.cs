using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ExampleAppWPFMVVM.Utils
{
    public class Command : ICommand
    {
        readonly Action<object?> execute;
        readonly Func<object?, bool> canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Команда, которая будет выполнена
        /// </summary>
        /// <param name="execute">Действие, которое будет выполнено</param>
        /// <param name="canExecute">Условие при котором действие может начать выполняться</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Command(Action<object?> execute, Func<object?, bool> canExecute = null!)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            try
            {
                execute(parameter);
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
        }
    }
}
