using System.Collections.Generic;
using HealthNerd.iOS.Utility.Mvvm;
using HealthNerd.iOS.ViewModels.OnboardingPages;
using Xamarin.Forms;

namespace HealthNerd.iOS.ViewModels
{
    public class OnboardingPageViewModel : ViewModelBase
    {
        public IList<OnboardingViewModelBase> ViewModels { get; set; }

        public OnboardingWelcomeViewModel Welcome { get; }
        public OnboardingAuthViewModel Auth { get; }
        public OnboardingSettingsViewModel Settings { get; }
        public OnboardingFinishViewModel Finish { get; }

        public Command Close { get; }
        public Command Previous { get; }
        public Command Next { get; }

        private int _carouselPosition;

        public int CarouselPosition
        {
            get => _carouselPosition;
            set
            {
                _carouselPosition = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsLast));
                OnPropertyChanged(nameof(IsNotFirst));
                OnPropertyChanged(nameof(IsNotLast));
            }
        }
        public bool IsLast => CarouselPosition + 1 == ViewModels.Count;
        public bool IsNotLast => !IsLast;
        public bool IsNotFirst => CarouselPosition != 0;

        public OnboardingPageViewModel(INavigationService nav,
            OnboardingWelcomeViewModel welcomeVm,
            OnboardingAuthViewModel authVm,
            OnboardingSettingsViewModel settingsVm,
            OnboardingFinishViewModel finish)
        {
            ViewModels = new List<OnboardingViewModelBase>
            {
                (Welcome = welcomeVm),
                (Auth = authVm),
                (Settings = settingsVm),
                (Finish = finish),
            };

            Close = new Command(() => nav.PresentAsNavigatableMainPage<MainPageViewModel>());
            Next = new Command(() => CarouselPosition++);
            Previous = new Command(() => CarouselPosition--);
        }
    }
}