using System.Threading.Tasks;
using HealthNerd.Services;
using HealthNerd.Utility.Mvvm;
using Xamarin.Forms;

namespace HealthNerd.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly ISettingsStore _settings;

        public MainPageViewModel(AuthorizeHealthCommand authorizer, ExportSpreadsheetCommand exporter, ISettingsStore settings, INavigationService nav, IAnalytics analytics) : base(analytics)
        {
            _settings = settings;

            Exporter = exporter;
            AuthorizeHealthCommand = authorizer.GetCommand(() =>
            {
                OnPropertyChanged(nameof(NeedsHealthAuthorization));
                Exporter.Command.ChangeCanExecute();
            });

            GoToSettings = new Command(async () => await nav.Modal<SettingsViewModel>());
        }

        public override Task BeforeAppearing()
        {
            RaiseAllPropertiesChanged();
            Exporter.Command.ChangeCanExecute();

            return base.BeforeAppearing();
        }

        public bool NeedsHealthAuthorization => !_settings.IsHealthKitAuthorized;

        public ExportSpreadsheetCommand Exporter { get; }
        public Command GoToSettings { get; }
        public Command AuthorizeHealthCommand { get; }
    }
}