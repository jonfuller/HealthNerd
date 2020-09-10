using HealthNerd.Services;
using HealthNerd.Utility.Mvvm;

namespace HealthNerd.ViewModels.OnboardingPages
{
    public class OnboardingFinishViewModel : OnboardingViewModelBase
    {
        public OnboardingFinishViewModel(INavigationService nav, IAnalytics analytics) : base(nav, analytics)
        {
        }
    }
}