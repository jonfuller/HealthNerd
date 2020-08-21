using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthKitData.Core;
using HealthNerd.Services;
using HealthNerd.Utility;
using HealthNerd.Utility.Mvvm;
using NodaTime;
using NodaTime.Extensions;
using Resources;
using Serilog;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace HealthNerd.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly ISettingsStore _settings;

        private bool _isQueryingHealth;
        private string _operationStatus;

        public MainPageViewModel(AuthorizeHealthCommand authorizer, IClock clock, ISettingsStore settings, INavigationService nav, ILogger logger, IFirebaseAnalyticsService analytics, IHealthStore healthStore)
        {
            _settings = settings;

            AuthorizeHealthCommand = authorizer.GetCommand(() =>
            {
                OnPropertyChanged(nameof(NeedsHealthAuthorization));
                QueryHealthCommand.ChangeCanExecute();
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
                        analytics.LogEvent(AnalyticsEvents.FileExport.Success, new Dictionary<string, string>
                        {
                            {AnalyticsEvents.FileExport.Success_WorkoutCount, workouts.Length.ToString()},
                            {AnalyticsEvents.FileExport.Success_RecordCount, records.Length.ToString()},
                        });
                    }
                    catch (Exception ex)
                    {
                        OperationStatus = AppRes.MainPage_Status_Error;
                        analytics.LogEvent(AnalyticsEvents.FileExport.Failure, "exception", ex.ToString());
                        logOperation.Error(ex, "An error occurred");
                    }
                    finally
                    {
                        IsQueryingHealth = false;
                    }

                    async Task<(Workout[] workouts, Record[] records)> QueryHealth(DateInterval dateRange)
                    {
                        return (
                            (await healthStore.GetWorkoutsAsync(dateRange)).ToArray(),
                            (await healthStore.GetHealthRecordsAsync(dateRange)).ToArray());
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

        public Command GoToSettings { get; }
        public Command AuthorizeHealthCommand { get; }
        public Command QueryHealthCommand { get; }
    }
}