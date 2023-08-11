using ExampleAppWPFMVVM.ViewModel.Base;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ExampleAppWPFMVVM
{
    public class DisplayRegistry
    {
        private readonly Dictionary<Type, Type> pagesMapping = new();
        private readonly Dictionary<Type, Type> windowsMapping = new();
        private readonly Dictionary<BaseWindowVM, Window> openWindows = new();
        private readonly Dictionary<BasePageVM, Page> openPages = new();

        /// <summary>
        /// Регистрация окна связанного c ViewModel
        /// </summary>
        /// <typeparam name="VM">Тип ViewModel</typeparam>
        /// <typeparam name="Win">Тип Window</typeparam>
        /// <exception cref="InvalidOperationException">Если окно уже зарегистрировано</exception>
        public void RegisterWindow<VM, Win>() where Win : Window, new() where VM : BaseWindowVM
        {
            var vmType = typeof(VM);
            if (windowsMapping.ContainsKey(vmType))
                throw new InvalidOperationException($"Окно {vmType.FullName} уже зарегистрировано");
            windowsMapping[vmType] = typeof(Win);
        }

        /// <summary>
        /// Регистрация страницы связанного ViewModel
        /// </summary>
        /// <typeparam name="VM">Тип ViewModel</typeparam>
        /// <typeparam name="P">Тип Page</typeparam>
        /// <exception cref="InvalidOperationException">Если страница уже зарегистрирована</exception>
        public void RegisterPage<VM, P>() where P : Page, new() where VM : BasePageVM
        {
            var vmType = typeof(VM);
            if (pagesMapping.ContainsKey(vmType))
                throw new InvalidOperationException($"Страница {vmType.FullName} уже зарегистрирована");
            pagesMapping[vmType] = typeof(P);
        }

        /// <summary>
        /// Исключение из регистрации ViewModel
        /// </summary>
        /// <typeparam name="VM"></typeparam>
        /// <exception cref="InvalidOperationException">Если ViewModel еще не зарегистрирован</exception>
        public void UnregisterUI<VM>()
        {
            var vmType = typeof(VM);
            if (windowsMapping.ContainsKey(vmType))
            {
                windowsMapping.Remove(vmType);
                return;
            }
            else if (pagesMapping.ContainsKey(vmType))
            {
                pagesMapping.Remove(vmType);
                return;
            }
            throw new InvalidOperationException($"Тип {vmType.FullName} не зарегистрирован");
        }

        /// <summary>
        /// Отображение окна в немодальном режиме
        /// </summary>
        /// <param name="vm">ViewModel</param>
        /// <exception cref="InvalidOperationException">Если окно уже отображается</exception>
        public void ShowWindow(BaseWindowVM vm)
        {
            if (openWindows.ContainsKey(vm))
                throw new InvalidOperationException("UI для этого VM уже отображается");

            var window = CreateWindowInstanceWithVM(vm);
            window.Show();
        }

        /// <summary>
        /// Отображение окна в модальном режиме
        /// </summary>
        /// <param name="vm">ViewModel</param>
        /// <exception cref="InvalidOperationException">Если окно уже отображается</exception>
        public async Task ShowModalWindow(BaseWindowVM vm)
        {
            if (openWindows.ContainsKey(vm))
                throw new InvalidOperationException("UI для этого VM уже отображается");

            var window = CreateWindowInstanceWithVM(vm);

            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            await window.Dispatcher.InvokeAsync(() => window.ShowDialog());
        }

        /// <summary>
        /// Закрытие окна
        /// </summary>
        /// <param name="vm">ViewModel</param>
        /// <exception cref="InvalidOperationException">Если окно еще не отображается</exception>
        public void CloseWindow(BaseWindowVM vm)
        {
            if (!openWindows.TryGetValue(vm, out Window? window))
                throw new InvalidOperationException("UI для этого VM еще не отображается");
            window.Close();
        }

        public Page OpenPage(BasePageVM vm)
        {
            if (openPages.ContainsKey(vm)) return openPages[vm];
            return CreatePageInstanceWithVM(vm);
        }

        /// <summary>
        /// Создание страницы с соответствующим ViewModel
        /// </summary>
        /// <param name="vm">ViewModel</param>
        /// <returns>Созданная страница</returns>
        /// <exception cref="ArgumentException">Если страница не зарегистрирована для этого ViewModel</exception>
        private Page CreatePageInstanceWithVM(BasePageVM vm)
        {
            Type? pageType = null;

            var vmType = vm.GetType();
            while (vmType != null && !pagesMapping.TryGetValue(vmType, out pageType))
                vmType = vmType.BaseType;

            if (pageType == null)
                throw new ArgumentException($"Не зарегистрирована страница для типа {vm.GetType().FullName}");

            var page = (Page)Activator.CreateInstance(pageType)!;
            page.DataContext = vm;
            page.Loaded += Page_Loaded;
            page.Unloaded += Page_Unloaded;
            return page;
        }

        private Window CreateWindowInstanceWithVM(BaseWindowVM vm)
        {
            Type? windowType = null;

            var vmType = vm.GetType();
            while (vmType != null && !windowsMapping.TryGetValue(vmType, out windowType))
                vmType = vmType.BaseType;

            if (windowType == null)
                throw new ArgumentException($"Не зарегистрировано окно для типа {vm.GetType().FullName}");

            var window = (Window)Activator.CreateInstance(windowType)!;
            window.DataContext = vm;

            var ivm = vm.Parent;
            while (ivm != null)
            {
                if (openWindows.ContainsKey(ivm))
                {
                    window.Owner = openWindows[ivm];
                    break;
                }
                ivm = ivm.Parent;
            }
            if (window.Owner == null && window != Application.Current.MainWindow) window.Owner = Application.Current.MainWindow;
            openWindows[vm] = window;
            window.Loaded += Window_Loaded;
            window.Closed += Window_Closed;
            return window;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is not Window window) return;
            if (window.DataContext is not BaseWindowVM vm) return;
            vm.UI_Loaded();
        }

        private void Window_Closed(object? sender, EventArgs e)
        {
            if (sender is not Window window) return;
            if (window.DataContext is not BaseWindowVM vm) return;
            vm.UI_Closed();
            openWindows.Remove(vm);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is not Page window) return;
            if (window.DataContext is not BasePageVM vm) return;
            vm.UI_Loaded();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (sender is not Page window) return;
            if (window.DataContext is not BasePageVM vm) return;
            vm.UI_Unloaded();
        }
    }
}
