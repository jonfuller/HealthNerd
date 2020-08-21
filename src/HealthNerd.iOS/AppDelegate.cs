using Firebase.Crashlytics;
using Foundation;
using HealthKit;
using HealthNerd.iOS.Services;
using HealthNerd.Services;
using HealthNerd.Utility.Mvvm;
using HealthNerd.ViewModels;
using HealthNerd.ViewModels.OnboardingPages;
using NodaTime;
using Serilog;
using Serilog.Events;
using TinyIoC;
using UIKit;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace HealthNerd.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Forms.SetFlags("CarouselView_Experimental", "IndicatorView_Experimental");
            global::Xamarin.Forms.Forms.Init();
            VersionTracking.Track();

            LoadApplication(new App(TinyIoCContainer.Current, ConfigureContainer));
            Firebase.Core.App.Configure();
            Crashlytics.Configure();

            return base.FinishedLaunching(app, options);
        }

        private static void ConfigureContainer(TinyIoCContainer container, IHaveMainPage mainApp)
        {
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.NSLog()
               .WriteTo.ApplicationInsights(
                    instrumentationKey: "97e6fd64-c506-4830-9530-fbe9a7274326",
                    TelemetryConverter.Traces,
                    restrictedToMinimumLevel: LogEventLevel.Information)
               .WriteTo.Debug(restrictedToMinimumLevel: LogEventLevel.Information)
               .CreateLogger();

            container.Register(Log.Logger);
            container.Register<IAuthorizer, Authorizer>();
            container.Register<IAlertPresenter, AlertPresenter>();
            container.Register<IClock>(SystemClock.Instance);
            container.Register<ISettingsStore, SettingsStore>();
            container.Register<IFirebaseAnalyticsService, FirebaseAnalyticsService>();
            container.Register<AuthorizeHealthCommand>();
            container.Register<IHealthStore>(new HealthStore(new HKHealthStore()));

            var navigator = new NavigationService(mainApp, new ViewLocator(), container);

            container.Register<MainPageViewModel>();
            container.Register<SettingsViewModel>();
            container.Register<ExportSettingsViewModel>();
            container.Register<OnboardingPageViewModel>();
            container.Register<OnboardingFinishViewModel>();
            container.Register<OnboardingSettingsViewModel>();
            container.Register<OnboardingWelcomeViewModel>();

            container.Register<INavigationService>(navigator);

        }
    }
}
