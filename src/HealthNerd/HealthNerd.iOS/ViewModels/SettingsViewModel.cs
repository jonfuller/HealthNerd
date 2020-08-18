using System;
using System.Globalization;
using HealthNerd.iOS.Services;
using HealthNerd.iOS.Utility;
using HealthNerd.iOS.Utility.Mvvm;
using NodaTime;
using Resources;
using Serilog;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace HealthNerd.iOS.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly ISettingsStore _settings;
        private readonly IFirebaseAnalyticsService _analytics;

        public SettingsViewModel(ISettingsStore settings, IAuthorizer authorizer, INavigationService nav, IClock clock, ILogger logger, IFirebaseAnalyticsService analytics)
        {
            _settings = settings;
            _analytics = analytics;


            AuthorizeHealthCommand = new Command(async () =>
            {
                (await authorizer.RequestAuthorizeAppleHealth()).Match(
                    error =>
                    {
                        analytics.LogEvent(AnalyticsEvents.AuthorizeHealth.Failure, nameof(error), $"{error.Message} - {error.Code}");
                        logger.Error("Error authorizing with Health: {@Error}", error);
                    },
                    () =>
                    {
                        _settings.SetHealthKitAuthorized(clock.GetCurrentInstant());
                        OnPropertyChanged(nameof(HealthAuthorizationStatusText));
                    });
            });

            GoToExportSettings = new Command(() => nav.NavigateTo<ExportSettingsViewModel>());
            GoToSEP = new Command(async () => await Browser.OpenAsync("https://www.sep.com", BrowserLaunchMode.SystemPreferred));
            GoToJmDesignz = new Command(async () => await Browser.OpenAsync("https://twitter.com/jm_designz", BrowserLaunchMode.SystemPreferred));
        }

        public string HealthAuthorizationStatusText => _settings.IsHealthKitAuthorized
            ? AppRes.Settings_IsAuthorizedButton_True
            : AppRes.Settings_IsAuthorizedButton_False;

        public Command AuthorizeHealthCommand { get; }
        public Command GoToExportSettings { get; }
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