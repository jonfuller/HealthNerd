using System.Linq;
using HealthNerd.iOS.Services;
using HealthNerd.iOS.Utility.Mvvm;
using HealthNerd.iOS.ViewModels;
using NodaTime;
using TinyIoC;
using Xamarin.Forms;

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

            container.Register<INavigationService>(navigator);
            RegisterInNamespaceOfType<MainPageViewModel>(container);

            navigator.PresentAsNavigatableMainPage<MainPageViewModel>();
        }

        private void RegisterInNamespaceOfType<T>(TinyIoCContainer container)
        {
            typeof(T).Assembly
               .GetTypes()
               .Where(t => t.Namespace == typeof(T).Namespace)
               .ToList()
               .ForEach(t => container.Register(t));

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
