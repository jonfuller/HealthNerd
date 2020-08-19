using HealthNerd.iOS.Utility.Mvvm;
using Xamarin.Forms;

namespace HealthNerd.iOS.ViewModels.OnboardingPages
{
    public class OnboardingViewModelBase : ViewModelBase
    {
        public OnboardingViewModelBase(INavigationService nav)
        {
            Complete = new Command(() => nav.PresentAsNavigatableMainPage<MainPageViewModel>());
        }

        public Command Complete { get; }
    }
}