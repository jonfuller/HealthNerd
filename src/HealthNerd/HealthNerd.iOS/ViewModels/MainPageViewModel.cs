using System;
using System.Threading.Tasks;
using HealthKit;
using HealthKitData.iOS;
using HealthNerd.iOS.Services;
using HealthNerd.iOS.Utility.Mvvm;
using NodaTime;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace HealthNerd.iOS.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly ISettingsStore _settings;

        public MainPageViewModel(IAuthorizer authorizer, IAlertPresenter alertPresenter, IClock clock, ISettingsStore settings, INavigationService nav)
        {
            _settings = settings;

            AuthorizeHealthCommand = new Command(async () =>
            {
                (await authorizer.RequestAuthorizeAppleHealth()).Match(
                    error =>
                    {
                        alertPresenter.DisplayAlert(
                            Resources.AppRes.MainPage_HealtKitAuthorization_Error_Title,
                            Resources.AppRes.MainPage_HealtKitAuthorization_Error_Message,
                            Resources.AppRes.MainPage_HealtKitAuthorization_Error_Button);
                        // TODO: log to analytics
                        Console.WriteLine(error.Message);
                    },
                    () =>
                    {
                        _settings.SetHealthKitAuthorized(clock.GetCurrentInstant());
                        OnPropertyChanged(nameof(NeedsHealthAuthorization));
                        QueryHealthCommand.ChangeCanExecute();

                        alertPresenter.DisplayAlert(
                            Resources.AppRes.MainPage_HealtKitAuthorization_Success_Title,
                            Resources.AppRes.MainPage_HealtKitAuthorization_Success_Message,
                            Resources.AppRes.MainPage_HealtKitAuthorization_Success_Button);
                    });
            });

            GoToSettings = new Command(() => nav.NavigateTo<SettingsViewModel>());

            QueryHealthCommand = new Command(async () =>
            {
                var store = new HKHealthStore();

                var fetchDate = _settings.SinceDate.Match(
                    Some: s => s,
                    None: LocalDate.FromDateTime(new DateTime(2020, 01, 01)));

                var dateRange = new DateInterval(
                    start: fetchDate,
                    end: LocalDate.FromDateTime(DateTime.Today));

                var workouts = await HealthKitQueries.GetWorkouts(store, dateRange);
                var records = await HealthKitQueries.GetHealthRecords(store, dateRange);

                Output.CreateExcelReport(records, workouts, _settings).IfSome(async f =>
                {
                    await Share.RequestAsync(new ShareFileRequest
                    {
                        File = new ShareFile(f.filename.FullName, f.contentType.Name)
                    });
                });
            },
            canExecute: () => _settings.IsHealthKitAuthorized);
        }

        public override Task BeforeAppearing()
        {
            RaiseAllPropertiesChanged();
            QueryHealthCommand.ChangeCanExecute();

            return base.BeforeAppearing();
        }

        public bool NeedsHealthAuthorization => !_settings.IsHealthKitAuthorized;
        public bool NeedsConfiguration => _settings.SinceDate.IsNone;

        public Command GoToSettings { get; }
        public Command AuthorizeHealthCommand { get; }
        public Command QueryHealthCommand { get; }
        public Command SettingsCommand { get; }
    }
}