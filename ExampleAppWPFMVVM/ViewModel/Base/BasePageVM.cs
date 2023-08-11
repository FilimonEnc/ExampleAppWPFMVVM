using System;

namespace ExampleAppWPFMVVM.ViewModel.Base
{
    public abstract class BasePageVM : BaseVM
    {
        public BasePageVM(BaseWindowVM windowVM)
        {
            WindowVM = windowVM;
        }

        public BaseWindowVM WindowVM { get; private set; }

        /// <summary>
        /// UI окна загружено
        /// </summary>
        public event EventHandler Loaded = delegate { };

        /// <summary>
        /// UI окна выгружено
        /// </summary>
        public event EventHandler Unloaded = delegate { };

        /// <summary>
        /// Открытие UI страницы
        /// </summary>
        public void UI_Loaded()
        {
            Loaded.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Закрытие UI страницы
        /// </summary>
        public void UI_Unloaded()
        {
            Unloaded.Invoke(this, EventArgs.Empty);
        }
    }
}
