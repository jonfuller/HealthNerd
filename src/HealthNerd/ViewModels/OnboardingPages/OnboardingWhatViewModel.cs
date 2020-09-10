using HealthNerd.Services;
using HealthNerd.Utility.Mvvm;

namespace HealthNerd.ViewModels.OnboardingPages
{
    public class OnboardingWhatViewModel : OnboardingViewModelBase
    {
        public OnboardingWhatViewModel(INavigationService nav, IAnalytics analytics) : base(nav, analytics)
        {
        }
    }
}
