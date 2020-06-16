using HealthNerd.iOS.Pages;
using HealthNerd.iOS.Services;
using HealthNerd.iOS.ViewModels;
using Xamarin.Forms;

namespace HealthNerd.iOS
{
    public partial class App
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage
            {
                BindingContext = new MainPageViewModel(DependencyService.Resolve<IAuthorizer>())
            };
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
