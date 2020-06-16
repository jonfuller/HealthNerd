using HealthNerd.ViewModels;
using Xamarin.Forms;

namespace HealthNerd
{
    public partial class App : Application
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
