using System;
using System.Threading.Tasks;
using Firebase.Crashlytics;
using Foundation;
using HealthKit;
using HealthNerd.iOS.Services;
using HealthNerd.Services;
using HealthNerd.Utility.Mvvm;
using HealthNerd.ViewModels;
using HealthNerd.ViewModels.OnboardingPages;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
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
            Firebase.Core.App.Configure();
            Crashlytics.Configure();

            LoadApplication(new App(TinyIoCContainer.Current, ConfigureContainer));

            return base.FinishedLaunching(app, options);
        }

        private static void ConfigureContainer(TinyIoCContainer container, IHaveMainPage mainApp)
        {
            var appInsights = new TelemetryClient(new TelemetryConfiguration("97e6fd64-c506-4830-9530-fbe9a7274326"));

            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.NSLog()
               .WriteTo.ApplicationInsights(
                    appInsights,
                    TelemetryConverter.Traces,
                    restrictedToMinimumLevel: LogEventLevel.Information)
               .WriteTo.Debug(restrictedToMinimumLevel: LogEventLevel.Information)
               .CreateLogger();

            AppDomain.CurrentDomain.UnhandledException += (s, e) => LogUnhandled(e.ExceptionObject as Exception);
            TaskScheduler.UnobservedTaskException += (s, e) => LogUnhandled(e.Exception);

            container.Register(Log.Logger);
            container.Register<IAuthorizer, Authorizer>();
            container.Register<IActionPresenter, ActionPresenter>();
            container.Register<IShare, HealthNerd.Services.Share>();
            container.Register<IClock>(SystemClock.Instance);
            container.Register<IFileManager, FileManager>(
                new FileManager(container.Resolve<IClock>(), FileSystem.CacheDirectory));
            container.Register<ISettingsStore, SettingsStore>();
            container.Register<IAnalytics>(new MultiAnalytics(
                new FirebaseAnalytics(),
                new AppInsightsAnalytics(appInsights)));
            container.Register<AuthorizeHealthCommand>();
            container.Register<IHealthStore>(new HealthStore(new HKHealthStore()));

            var navigator = new NavigationService(mainApp, new ViewLocator(), container);

            container.Register<MainPageViewModel>();
            container.Register<SettingsViewModel>();
            container.Register<ExportSettingsViewModel>();
            container.Register<OnboardingPageViewModel>();
            container.Register<OnboardingWelcomeViewModel>();
            container.Register<OnboardingWhatViewModel>();
            container.Register<OnboardingAuthViewModel>();
            container.Register<OnboardingSettingsViewModel>();
            container.Register<OnboardingFinishViewModel>();

            container.Register<INavigationService>(navigator);

            void LogUnhandled(Exception ex)
            {
                Log.Logger.Error(ex, "Unhandled exception");
                appInsights.TrackException(ex);
                appInsights.Flush();
            }
        }
    }
}
