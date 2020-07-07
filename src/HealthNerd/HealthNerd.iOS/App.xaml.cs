using HealthNerd.iOS.Pages;
using HealthNerd.iOS.Services;
using HealthNerd.iOS.ViewModels;
using NodaTime;
using Xamarin.Forms;

namespace HealthNerd.iOS
{
    public partial class App
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage
            {
                BindingContext = new MainPageViewModel(
                    DependencyService.Resolve<IAuthorizer>(),
                    DependencyService.Resolve<IAlertPresenter>(),
                    SystemClock.Instance,
                    DependencyService.Resolve<ISettingsStore>())
            });
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
