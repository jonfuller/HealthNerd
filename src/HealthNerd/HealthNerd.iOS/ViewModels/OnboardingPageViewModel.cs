using System.Collections.Generic;
using HealthNerd.iOS.Utility.Mvvm;
using HealthNerd.iOS.ViewModels.OnboardingPages;

namespace HealthNerd.iOS.ViewModels
{
    public class OnboardingPageViewModel : ViewModelBase
    {
        public IEnumerable<OnboardingViewModelBase> ViewModels { get; set; }

        public OnboardingWelcomeViewModel Welcome { get; }
        public OnboardingFinishViewModel Finish { get; }

        public OnboardingPageViewModel(
            OnboardingWelcomeViewModel welcomeVm,
            OnboardingFinishViewModel finish)
        {
            ViewModels = new List<OnboardingViewModelBase>
            {
                (Welcome = welcomeVm),
                (Finish = finish),
            };
        }
    }
}