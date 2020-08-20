using HealthNerd.iOS.Services;
using HealthNerd.iOS.Utility.Mvvm;
using HealthNerd.iOS.ViewModels;
using HealthNerd.iOS.ViewModels.OnboardingPages;
using NodaTime;
using Serilog;
using Serilog.Events;
using TinyIoC;
using Xamarin.Essentials;

namespace HealthNerd.iOS
{
    public partial class App : IHaveMainPage
    {
        public App()
        {
            InitializeComponent();

            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.NSLog()
               .WriteTo.ApplicationInsights(
                    instrumentationKey: "97e6fd64-c506-4830-9530-fbe9a7274326",
                    TelemetryConverter.Traces,
                    restrictedToMinimumLevel: LogEventLevel.Information)
               .WriteTo.Debug(restrictedToMinimumLevel: LogEventLevel.Information)
               .CreateLogger();

            var container = TinyIoCContainer.Current;
            container.Register(Log.Logger);
            container.Register<IAuthorizer, Authorizer>();
            container.Register<IAlertPresenter, AlertPresenter>();
            container.Register<IClock>(SystemClock.Instance);
            container.Register<ISettingsStore, SettingsStore>();
            container.Register<IFirebaseAnalyticsService, FirebaseAnalyticsService>();
            container.Register<AuthorizeHealthCommand>();

            var navigator = new NavigationService(this, new ViewLocator(), container);

            container.Register<MainPageViewModel>();
            container.Register<SettingsViewModel>();
            container.Register<ExportSettingsViewModel>();
            container.Register<OnboardingPageViewModel>();
            container.Register<OnboardingFinishViewModel>();
            container.Register<OnboardingSettingsViewModel>();
            container.Register<OnboardingWelcomeViewModel>();

            container.Register<INavigationService>(navigator);

            if (VersionTracking.IsFirstLaunchEver)
            {
                navigator.PresentAsMainPage<OnboardingPageViewModel>();
            }
            else
            {
                navigator.PresentAsNavigatableMainPage<MainPageViewModel>();
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
