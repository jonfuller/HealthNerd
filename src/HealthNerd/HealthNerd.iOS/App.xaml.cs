using HealthNerd.iOS.Services;
using HealthNerd.iOS.Utility.Mvvm;
using HealthNerd.iOS.ViewModels;
using NodaTime;
using Serilog;
using Serilog.Events;
using TinyIoC;

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
               .WriteTo.Debug(restrictedToMinimumLevel: LogEventLevel.Information)
               .CreateLogger();

            var container = TinyIoCContainer.Current;
            container.Register(Log.Logger);
            container.Register<IAuthorizer, Authorizer>();
            container.Register<IAlertPresenter, AlertPresenter>();
            container.Register<IClock>(SystemClock.Instance);
            container.Register<ISettingsStore, SettingsStore>();

            var navigator = new NavigationService(this, new ViewLocator(), container);

            container.Register<MainPageViewModel>();
            container.Register<SettingsViewModel>();

            container.Register<INavigationService>(navigator);

            navigator.PresentAsNavigatableMainPage<MainPageViewModel>();
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
