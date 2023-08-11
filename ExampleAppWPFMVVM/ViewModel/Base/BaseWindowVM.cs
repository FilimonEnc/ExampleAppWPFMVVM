using ExampleAppWPFMVVM.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace ExampleAppWPFMVVM.ViewModel.Base
{
    public abstract class BaseWindowVM : BaseVM
    {
        /// <summary>
        /// Родительский BaseWindowVM
        /// </summary>
        public BaseWindowVM? Parent { get; set; }

        /// <summary>
        /// Дочерние страницы текущего ViewModel
        /// </summary>
        public LinkedList<BasePageVM> Pages { get; set; } = new();

        /// <summary>
        /// UI окна загружено
        /// </summary>
        public event EventHandler Loaded = delegate { };

        /// <summary>
        /// UI окна закрыто
        /// </summary>
        public event EventHandler Closed = delegate { };

        /// <summary>
        /// Текущая страница
        /// </summary>
        public Page CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }
        private Page _currentPage = null!;

        public BaseWindowVM() { }

        /// <summary>
        /// Запуск окна с контекстом в модальном режиме
        /// </summary>
        /// <param name="vm"></param>
        public async Task ShowModalWindow(BaseWindowVM vm)
        {
            try
            {
                vm.Parent = this;
                await App.DisplayRegistry.ShowModalWindow(vm);
            }
            catch (Exception ex)
            {
                MessageBoxShowError(ex.Message);
            }
        }

        /// <summary>
        /// Запуск окна с контекстом в немодальном режиме
        /// </summary>
        /// <param name="vm"></param>
        public void ShowWindow(BaseWindowVM vm)
        {
            try
            {
                vm.Parent = this;
                App.DisplayRegistry.ShowWindow(vm);
            }
            catch (Exception ex)
            {
                MessageBoxShowError(ex.Message);
            }
        }

        /// <summary>
        /// Закрытие окна
        /// </summary>
        public ICommand Close => new Command((obj) => CloseWindow());

        /// <summary>
        /// Закрытие окна
        /// </summary>
        public void CloseWindow()
        {
            try
            {
                App.DisplayRegistry.CloseWindow(this);
            }
            catch (Exception ex)
            {
                MessageBoxShowError(ex.Message);
            }
        }

        /// <summary>
        /// Закрытие окна
        /// </summary>
        /// <param name="vm"></param>
        public static void CloseWindow(BaseWindowVM vm)
        {
            try
            {
                App.DisplayRegistry.CloseWindow(vm);
            }
            catch (Exception ex)
            {
                MessageBoxShowError(ex.Message);
            }
        }

        /// <summary>
        /// Открывает страницу
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        public void OpenPage(BasePageVM vm)
        {
            try
            {
                BasePageVM? page_vm = Pages.Where(x => x == vm).FirstOrDefault();
                if (page_vm == null) Pages.AddLast(vm);

                CurrentPage = App.DisplayRegistry.OpenPage(vm);
            }
            catch (Exception ex)
            {
                MessageBoxShowError(ex.Message);
                return;
            }
        }

        public void ClosePage()
        {
            if (Pages.Count > 0) Pages.RemoveLast();
            if (Pages.Last != null) CurrentPage = App.DisplayRegistry.OpenPage(Pages.Last.Value);

        }

        /// <summary>
        /// Открытие UI окна
        /// </summary>
        public void UI_Loaded()
        {
            Loaded.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Закрытие UI окна
        /// </summary>
        public void UI_Closed()
        {
            Closed.Invoke(this, EventArgs.Empty);
        }

    }
}
