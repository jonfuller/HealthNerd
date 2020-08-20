using HealthNerd.iOS.Utility.Mvvm;
using Xamarin.Forms;

namespace HealthNerd.iOS.ViewModels.OnboardingPages
{
    public class OnboardingViewModelBase : ViewModelBase
    {
        public OnboardingViewModelBase(INavigationService nav)
        {
            Close = new Command(() => nav.PresentAsNavigatableMainPage<MainPageViewModel>());
        }

        public Command Close { get; }
    }
}