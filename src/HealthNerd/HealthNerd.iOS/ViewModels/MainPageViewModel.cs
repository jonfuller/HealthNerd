using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HealthKit;
using HealthKitData.Core;
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
        private bool _isQueryingHealth;

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
                    IsQueryingHealth = true;
                    var (workouts, records) = await QueryHealth(_settings);
                    IsQueryingHealth = false;

                    Output.CreateExcelReport(records, workouts, _settings).IfSome(async f =>
                    {
                        await Share.RequestAsync(new ShareFileRequest
                        {
                            File = new ShareFile(f.filename.FullName, f.contentType.Name)
                        });
                    });

                    static async Task<(IEnumerable<Workout> workouts, IEnumerable<Record> records)> QueryHealth(ISettingsStore settings)
                    {
                        var store = new HKHealthStore();

                        var fetchDate = settings.SinceDate.Match(
                            Some: s => s,
                            None: LocalDate.FromDateTime(new DateTime(2020, 01, 01)));

                        var dateRange = new DateInterval(
                            start: fetchDate,
                            end: LocalDate.FromDateTime(DateTime.Today));

                        return (
                            await HealthKitQueries.GetWorkouts(store, dateRange),
                            await HealthKitQueries.GetHealthRecords(store, dateRange));
                    }
                },
            canExecute: CanExecuteHealthQuery);
        }

        private Func<bool> CanExecuteHealthQuery => () => _settings.IsHealthKitAuthorized && !IsQueryingHealth;

        public bool IsQueryingHealth
        {
            get => _isQueryingHealth;
            set
            {
                _isQueryingHealth = value;
                OnPropertyChanged();
                QueryHealthCommand.ChangeCanExecute();
            }

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
    }
}