using ExampleAppWPFMVVM.ViewModel;

using System.Threading;
using System.Windows;

namespace ExampleAppWPFMVVM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        /// <summary>
        /// Зарегистрированные элементы UI
        /// </summary>
        public static DisplayRegistry DisplayRegistry { get; set; } = new DisplayRegistry();

        /// <summary>
        /// Главное окно
        /// </summary>
        public static MainWindowVM? MainVM { get; private set; }

        /// <summary>
        /// Запуск одной копии приложения
        /// </summary>
        private Mutex mut = null!;

        public App()
        {
            DisplayMappingRegistry();
        }

        /// <summary>
        /// При запуске приложения 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            mut = new Mutex(true, "ExampleApp", out bool createdNew);
            if (!createdNew)
            {
                mut = null!;
                Current.Shutdown();
                return;
            }

            base.OnStartup(e);

            MainVM = new MainWindowVM();
            DisplayRegistry.ShowWindow(MainVM!);
        }

        /// <summary>
        /// Регистрация окон и страниц
        /// </summary>
        private static void DisplayMappingRegistry()
        {
            //DisplayRegistry.RegisterWindow<ExampleVM, ExampleWindow>();
            //DisplayRegistry.RegisterPage<ExamplePageVM, ExamplePage>();

            DisplayRegistry.RegisterWindow<MainWindowVM, MainWindow>();
        }

        /// <summary>
        /// При выходе из приложения
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            mut?.ReleaseMutex();
            base.OnExit(e);
        }


    }
}
