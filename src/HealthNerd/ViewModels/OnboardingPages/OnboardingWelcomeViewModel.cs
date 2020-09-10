using HealthNerd.Services;
using HealthNerd.Utility.Mvvm;

namespace HealthNerd.ViewModels.OnboardingPages
{
    public class OnboardingWelcomeViewModel : OnboardingViewModelBase
    {
        public OnboardingWelcomeViewModel(INavigationService nav, IAnalytics analytics) : base(nav, analytics)
        {
        }
    }
}