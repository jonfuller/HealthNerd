using HealthNerd.iOS.Services;
using HealthNerd.iOS.Utility.Mvvm;
using HealthNerd.iOS.ViewModels;
using NodaTime;
using Xamarin.Forms;

namespace HealthNerd.iOS
{
    public partial class App : IHaveMainPage
    {
        public App()
        {
            InitializeComponent();

            var navigator = new NavigationService(this, new ViewLocator());

            navigator.PresentAsNavigatableMainPage(new MainPageViewModel(
                DependencyService.Resolve<IAuthorizer>(),
                DependencyService.Resolve<IAlertPresenter>(),
                SystemClock.Instance,
                DependencyService.Resolve<ISettingsStore>()));
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
