using System;
using HealthNerd.Utility;
using NodaTime;
using Resources;
using Serilog;
using Xamarin.Forms;

namespace HealthNerd.Services
{
    public class AuthorizeHealthCommand
    {
        private readonly IAuthorizer _authorizer;
        private readonly IClock _clock;
        private readonly IAlertPresenter _alertPresenter;
        private readonly IFirebaseAnalyticsService _analytics;
        private readonly ILogger _logger;
        private readonly ISettingsStore _settings;

        public AuthorizeHealthCommand(IAuthorizer authorizer, IClock clock, IAlertPresenter alertPresenter, IFirebaseAnalyticsService analytics, ILogger logger, ISettingsStore settings)
        {
            _authorizer = authorizer;
            _clock = clock;
            _alertPresenter = alertPresenter;
            _analytics = analytics;
            _logger = logger;
            _settings = settings;
        }

        public Command GetCommand(Action onSuccess = null)
        {
            return new Command(async () =>
            {
                (await _authorizer.RequestAuthorizeAppleHealth()).Match(
                    error =>
                    {
                        _alertPresenter.DisplayAlert(
                            AppRes.HealtKitAuthorization_Error_Title,
                            AppRes.HealtKitAuthorization_Error_Message,
                            AppRes.HealtKitAuthorization_Error_Button);
                        _analytics.LogEvent(AnalyticsEvents.AuthorizeHealth.Failure, nameof(error), $"{error.Message} - {error.Code}");
                        _logger.Error("Error authorizing with Health: {@Error}", error);
                    },
                    () =>
                    {
                        _settings.SetHealthKitAuthorized(_clock.GetCurrentInstant());
                        onSuccess?.Invoke();
                        _alertPresenter.DisplayAlert(
                            AppRes.HealtKitAuthorization_Success_Title,
                            AppRes.HealtKitAuthorization_Success_Message,
                            AppRes.HealtKitAuthorization_Success_Button);
                        _analytics.LogEvent(AnalyticsEvents.AuthorizeHealth.Success);
                        _logger.Information("Authorized with Health.");
                    });
            });
        }
    }
}