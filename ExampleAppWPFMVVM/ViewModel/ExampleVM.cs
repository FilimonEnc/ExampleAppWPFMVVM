using ExampleAppWPFMVVM.Interface;
using ExampleAppWPFMVVM.Utils;
using ExampleAppWPFMVVM.ViewModel.Base;

using System.Windows.Input;

namespace ExampleAppWPFMVVM.ViewModel
{
    public class ExampleVM : BaseVM, IExampleVM
    {
        public object ExampleProperty
        {
            get => _exampleProperty;
            set
            {
                _exampleProperty = value;
                OnPropertyChanged(nameof(ExampleProperty));
            }
        }
        private object _exampleProperty = null!;

        public ICommand ExampleCommand => new Command(_ =>
        {

        });

        public ICommand ExampleAsyncCommand => new AsyncCommand(async _ =>
        {

        });
    }
}
