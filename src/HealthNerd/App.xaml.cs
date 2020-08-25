using System;
using HealthNerd.Utility.Mvvm;
using HealthNerd.ViewModels;
using TinyIoC;
using Xamarin.Essentials;

namespace HealthNerd
{
    public partial class App : IHaveMainPage
    {
        public App(TinyIoCContainer container, Action<TinyIoCContainer, App> configurationContainer)
        {
            InitializeComponent();

            configurationContainer(container, this);

            var navService = container.Resolve<INavigationService>();

            if (VersionTracking.IsFirstLaunchEver)
            {
                navService.PresentAsMainPage<OnboardingPageViewModel>();
            }
            else
            {
                navService.PresentAsMainPage<MainPageViewModel>();
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
