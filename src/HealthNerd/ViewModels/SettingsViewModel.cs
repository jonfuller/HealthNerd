using System;
using System.Globalization;
using HealthNerd.Services;
using HealthNerd.Utility;
using HealthNerd.Utility.Mvvm;
using NodaTime;
using Resources;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace HealthNerd.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly ISettingsStore _settings;
        private readonly IAnalytics _analytics;

        public SettingsViewModel(AuthorizeHealthCommand commander, ISettingsStore settings, INavigationService nav, IAnalytics analytics)
        {
            _settings = settings;
            _analytics = analytics;

            AuthorizeHealthCommand = commander.GetCommand(() =>
                OnPropertyChanged(nameof(HealthAuthorizationStatusText)));

            Dismiss = new Command(() => nav.DismissModal());
            GoToExportSettings = new Command(() => nav.Modal<ExportSettingsViewModel>());
            GoToOnboarding = new Command(() => nav.PresentAsMainPage<OnboardingPageViewModel>());
            GoToSEP = new Command(async () => await Browser.OpenAsync("https://www.sep.com", BrowserLaunchMode.SystemPreferred));
            GoToJmDesignz = new Command(async () => await Browser.OpenAsync("https://twitter.com/jm_designz", BrowserLaunchMode.SystemPreferred));
        }

        public string HealthAuthorizationStatusText => _settings.IsHealthKitAuthorized
            ? AppRes.Settings_IsAuthorizedButton_True
            : AppRes.Settings_IsAuthorizedButton_False;

        public Command Dismiss { get; }
        public Command AuthorizeHealthCommand { get; }
        public Command GoToExportSettings { get; }
        public Command GoToOnboarding { get; }
        public Command GoToSEP { get; }
        public Command GoToJmDesignz { get; }

        public DateTime EarliestFetchDate
        {
            get
            {
                return _settings.SinceDate.Match(
                    Some: d => d.ToDateTimeUnspecified(),
                    None: () => SettingsDefaults.EarliestFetchDate.ToDateTimeUnspecified());
            }
            set
            {
                _settings.SetSinceDate(LocalDate.FromDateTime(value));
                _analytics.LogEvent(AnalyticsEvents.Settings.For(nameof(EarliestFetchDate)), AnalyticsEvents.Settings.ParamValue, value.ToString(CultureInfo.InvariantCulture));
                OnPropertyChanged();
            }
        }

    }
}