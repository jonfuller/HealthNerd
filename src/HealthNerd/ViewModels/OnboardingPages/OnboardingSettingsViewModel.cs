using HealthNerd.Services;
using HealthNerd.Utility.Mvvm;

namespace HealthNerd.ViewModels.OnboardingPages
{
    public class OnboardingSettingsViewModel : OnboardingViewModelBase
    {
        public OnboardingSettingsViewModel(INavigationService nav, IAnalytics analytics) : base(nav, analytics)
        {
        }
    }
}