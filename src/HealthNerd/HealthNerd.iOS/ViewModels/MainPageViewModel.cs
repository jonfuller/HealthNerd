using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HealthKit;
using HealthKitData.Core;
using HealthKitData.iOS;
using HealthNerd.iOS.Services;
using HealthNerd.iOS.Utility.Mvvm;
using NodaTime;
using NodaTime.Extensions;
using Resources;
using Serilog;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace HealthNerd.iOS.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly ISettingsStore _settings;
        private bool _isQueryingHealth;
        private string _operationStatus;

        public MainPageViewModel(IAuthorizer authorizer, IAlertPresenter alertPresenter, IClock clock, ISettingsStore settings, INavigationService nav, ILogger logger)
        {
            _settings = settings;

            AuthorizeHealthCommand = new Command(async () =>
            {
                (await authorizer.RequestAuthorizeAppleHealth()).Match(
                    error =>
                    {
                        alertPresenter.DisplayAlert(
                            AppRes.MainPage_HealtKitAuthorization_Error_Title,
                            AppRes.MainPage_HealtKitAuthorization_Error_Message,
                            AppRes.MainPage_HealtKitAuthorization_Error_Button);
                        // TODO: log to analytics
                        logger.Error("Error authorizing with Health: {@Error}", error);
                    },
                    () =>
                    {
                        _settings.SetHealthKitAuthorized(clock.GetCurrentInstant());
                        OnPropertyChanged(nameof(NeedsHealthAuthorization));
                        QueryHealthCommand.ChangeCanExecute();

                        alertPresenter.DisplayAlert(
                            AppRes.MainPage_HealtKitAuthorization_Success_Title,
                            AppRes.MainPage_HealtKitAuthorization_Success_Message,
                            AppRes.MainPage_HealtKitAuthorization_Success_Button);
                        logger.Information("Authorized with Health.");
                    });
            });

            GoToSettings = new Command(() => nav.NavigateTo<SettingsViewModel>());

            QueryHealthCommand = new Command(async () =>
                {
                    var logOperation = logger.ForContext("NerdOperation", Guid.NewGuid());
                    try
                    {
                        var queryRange = new DateInterval(
                            start: settings.SinceDate.Match(
                                Some: s => s,
                                None: LocalDate.FromDateTime(new DateTime(2020, 01, 01))),
                            end: clock.InTzdbSystemDefaultZone().GetCurrentDate());

                        logOperation.Verbose("Starting nerd operation for {QueryRange}", queryRange);
                        IsQueryingHealth = true;

                        OperationStatus = AppRes.MainPage_Status_Gathering;
                        var (workouts, records) = await QueryHealth(queryRange);

                        OperationStatus = AppRes.MainPage_Status_SavingFile;
                        logOperation.Verbose("Creating output report");
                        var excelReport = Output.CreateExcelReport(records, workouts, _settings, clock);

                        OperationStatus = AppRes.MainPage_Status_SharingFile;
                        logOperation.Verbose("Sharing file {File}", excelReport);
                        await Share.RequestAsync(new ShareFileRequest
                        {
                            File = new ShareFile(excelReport.filename.FullName, excelReport.contentType.Name)
                        });
                        OperationStatus = string.Format(AppRes.MainPage_Status_Complete, excelReport.filename.Name);
                    }
                    catch (Exception ex)
                    {
                        OperationStatus = AppRes.MainPage_Status_Error;
                        logOperation.Error(ex, "An error occurred");
                    }
                    finally
                    {
                        IsQueryingHealth = false;
                    }

                    static async Task<(IEnumerable<Workout> workouts, IEnumerable<Record> records)> QueryHealth(DateInterval dateRange)
                    {
                        var store = new HKHealthStore();

                        return (
                            await HealthKitQueries.GetWorkouts(store, dateRange),
                            await HealthKitQueries.GetHealthRecords(store, dateRange));
                    }
                },
            canExecute: CanExecuteHealthQuery);
        }

        private Func<bool> CanExecuteHealthQuery => () => _settings.IsHealthKitAuthorized && !IsQueryingHealth;

        public string OperationStatus
        {
            get => _operationStatus;
            set
            {
                _operationStatus = value;
                OnPropertyChanged();
            }
        }
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