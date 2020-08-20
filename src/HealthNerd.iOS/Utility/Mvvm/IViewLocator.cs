using Xamarin.Forms;

namespace HealthNerd.iOS.Utility.Mvvm
{
    public interface IViewLocator
    {
        Page CreateAndBindPageFor<TViewModel>(TViewModel viewModel) where TViewModel : ViewModelBase;
    }
}