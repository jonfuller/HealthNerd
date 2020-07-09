using System;
using HealthNerd.iOS.Services;
using HealthNerd.iOS.Utility.Mvvm;
using HealthNerd.iOS.ViewModels;
using NodaTime;
using TinyIoC;

namespace HealthNerd.iOS
{
    public partial class App : IHaveMainPage
    {
        public App()
        {
            InitializeComponent();
            var container = TinyIoCContainer.Current;
            container.Register<IAuthorizer, Authorizer>();
            container.Register<IAlertPresenter, AlertPresenter>();
            container.Register<IClock>(SystemClock.Instance);
            container.Register<ISettingsStore, SettingsStore>();

            var navigator = new NavigationService(this, new ViewLocator(), container);

            container.AutoRegister(type => type.Name.EndsWith("ViewModel", StringComparison.CurrentCultureIgnoreCase));
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
