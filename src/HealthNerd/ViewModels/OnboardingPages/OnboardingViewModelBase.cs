using HealthNerd.Services;
using HealthNerd.Utility.Mvvm;
using Xamarin.Forms;

namespace HealthNerd.ViewModels.OnboardingPages
{
    public class OnboardingViewModelBase : ViewModelBase
    {
        public OnboardingViewModelBase(INavigationService nav, IAnalytics analytics) : base(analytics)
        {
            Close = new Command(() => nav.PresentAsMainPage<MainPageViewModel>());
        }

        public Command Close { get; }
    }
}