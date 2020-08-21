using HealthNerd.Services;
using HealthNerd.Utility.Mvvm;
using Xamarin.Forms;

namespace HealthNerd.ViewModels.OnboardingPages
{
    public class OnboardingAuthViewModel : OnboardingViewModelBase
    {
        public OnboardingAuthViewModel(INavigationService nav, AuthorizeHealthCommand authorizer) : base(nav)
        {
            AuthorizeHealthCommand = authorizer.GetCommand();
        }

        public Command AuthorizeHealthCommand { get; }
    }
}