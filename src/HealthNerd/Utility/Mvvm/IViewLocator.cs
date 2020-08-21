using Xamarin.Forms;

namespace HealthNerd.Utility.Mvvm
{
    public interface IViewLocator
    {
        Page CreateAndBindPageFor<TViewModel>(TViewModel viewModel) where TViewModel : ViewModelBase;
    }
}