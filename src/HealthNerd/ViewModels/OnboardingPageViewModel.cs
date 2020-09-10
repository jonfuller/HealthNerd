using System.Collections.Generic;
using HealthNerd.Services;
using HealthNerd.Utility.Mvvm;
using HealthNerd.ViewModels.OnboardingPages;
using Xamarin.Forms;

namespace HealthNerd.ViewModels
{
    public class OnboardingPageViewModel : ViewModelBase
    {
        public IList<OnboardingViewModelBase> ViewModels { get; set; }

        public OnboardingWelcomeViewModel Welcome { get; }
        public OnboardingAuthViewModel Auth { get; }
        public OnboardingWhatViewModel WhatIsIt { get; }
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
            OnboardingWhatViewModel whatVm,
            OnboardingSettingsViewModel settingsVm,
            OnboardingFinishViewModel finish,
            IAnalytics analytics) : base(analytics)
        {
            ViewModels = new List<OnboardingViewModelBase>
            {
                (Welcome = welcomeVm),
                (WhatIsIt = whatVm),
                (Auth = authVm),
                //(Settings = settingsVm),
                (Finish = finish),
            };

            Close = new Command(() => nav.PresentAsMainPage<MainPageViewModel>());
            Next = new Command(() => CarouselPosition++);
            Previous = new Command(() => CarouselPosition--);
        }
    }
}