using System.Windows.Input;

namespace ExampleAppWPFMVVM.Interface
{
    public interface IExampleVM
    {
        object ExampleProperty { get; set; }
        ICommand ExampleCommand { get; }
        ICommand ExampleAsyncCommand { get; }
    }
}
