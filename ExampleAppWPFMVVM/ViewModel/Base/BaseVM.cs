using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;


namespace ExampleAppWPFMVVM.ViewModel.Base
{
    public abstract class BaseVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public BaseVM() { }

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Отображение сообщения
        /// </summary>
        /// <param name="text">Текст сообщения</param>
        /// <param name="caption">Заголовок</param>
        public static void MessageBoxShow(string text, string caption, MessageBoxButton messageBoxButton, MessageBoxImage messageBoxImage)
        {
            MessageBox.Show(text, caption, messageBoxButton, messageBoxImage);
        }

        /// <summary>
        /// Отображение сообщения об ошибке
        /// </summary>
        /// <param name="text">Текст сообщения</param>
        /// <param name="caption">Заголовок</param>
        public static void MessageBoxShowError(string text, string caption = "Ошибка")
        {
            MessageBoxShow(text, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
