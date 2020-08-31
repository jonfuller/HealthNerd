using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
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
using static LanguageExt.Prelude;

namespace HealthNerd.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly ISettingsStore _settings;

        private bool _isQueryingHealth;
        private string _operationStatus;

        public MainPageViewModel(AuthorizeHealthCommand authorizer, IClock clock, ISettingsStore settings, INavigationService nav, ILogger logger, IFirebaseAnalyticsService analytics, IHealthStore healthStore, IFileManager fileManager, IActionPresenter actionPresenter, IShare share)
        {
            var latestEligibleTime = clock.InTzdbSystemDefaultZone().GetCurrentLocalDateTime().Minus(Period.FromMinutes(5));

            _settings = settings;

            AuthorizeHealthCommand = authorizer.GetCommand(() =>
            {
                OnPropertyChanged(nameof(NeedsHealthAuthorization));
                QueryHealthCommand.ChangeCanExecute();
            });

            GoToSettings = new Command(async () => await nav.Modal<SettingsViewModel>());

            QueryHealthCommand = new Command(async () =>
                {
                    if (HasRecentUsableExport(fileManager, latestEligibleTime, out var latestFile))
                    {
                        var latestFileTimeString = latestFile.exportTime.ToString("HH:mm", CultureInfo.CurrentUICulture);
                        actionPresenter.ActionSheet()
                           .WithCancel("Cancel", () => { })
                           .With($"Continue with Previous (Today at {latestFileTimeString})", async () => await ShareFile(latestFile.file, latestFile.contentType))
                           .With("Create New", async () => await ExportNewFile())
                           .Show("Found a recent export.");
                    }
                    else
                    {
                        await ExportNewFile();
                    }
                    CleanupOldExports(fileManager, latestEligibleTime);

                    async Task ExportNewFile()
                    {
                        var logOperation = logger.ForContext("NerdOperation", Guid.NewGuid());
                        IsQueryingHealth = true;
                        await TryAsync(async () => await ExportHealthToExcel(logOperation, settings, clock, fileManager, analytics, healthStore))
                            .Match(
                                Succ: async x => await ShareFile(x.file, x.contentType),
                                Fail: ex =>
                                {
                                    OperationStatus = AppRes.MainPage_Status_Error;
                                    analytics.LogEvent(AnalyticsEvents.FileExport.Failure, "exception", ex.ToString());
                                    logOperation.Error(ex, "An error occurred");
                                });
                        IsQueryingHealth = false;
                    }

                    async Task ShareFile(FileInfo file, ContentType contentType)
                    {
                        logger.Verbose("Sharing file {File}", file);
                        OperationStatus = AppRes.MainPage_Status_SharingFile;
                        await share.RequestAsync(new ShareFileRequest
                        {
                            File = new ShareFile(file.FullName, contentType.Name)
                        });
                        OperationStatus = string.Format(AppRes.MainPage_Status_Complete, file.Name);
                    }
                },
            canExecute: CanExecuteHealthQuery);
        }

        private static void CleanupOldExports(IFileManager fileManager, LocalDateTime recencyThreshold)
        {
            fileManager.DeleteExportsBefore(recencyThreshold);
        }

        private static bool HasRecentUsableExport(IFileManager fileManager, LocalDateTime recencyThreshold, out (FileInfo file, LocalDateTime exportTime, ContentType contentType) latestFile)
        {
            var latestExport = fileManager.GetLatestExportFile();
            latestFile = latestExport.Match(_ => _, Empty);

            return latestExport
               .Map(x => x.exportTime > recencyThreshold)
               .Match(
                    Some: recentEnough => recentEnough,
                    None: () => false);

            static (FileInfo File, LocalDateTime exportTime, ContentType contentType) Empty() =>
                (null, LocalDateTime.FromDateTime(DateTime.MinValue), null);
        }

        private async Task<(FileInfo file, ContentType contentType)> ExportHealthToExcel(ILogger logger, ISettingsStore settings, IClock clock, IFileManager fileManager, IFirebaseAnalyticsService analytics, IHealthStore healthStore)
        {
            var queryRange = new DateInterval(
                start: settings.SinceDate.Match(
                    Some: s => s,
                    None: LocalDate.FromDateTime(new DateTime(2020, 01, 01))),
                end: clock.InTzdbSystemDefaultZone().GetCurrentDate());

            logger.Verbose("Starting nerd operation for {QueryRange}", queryRange);

            OperationStatus = AppRes.MainPage_Status_Gathering;
            var (workouts, records) = await QueryHealth(queryRange);

            analytics.LogEvent(AnalyticsEvents.FileExport.Success, new Dictionary<string, string>
            {
                {AnalyticsEvents.FileExport.Success_WorkoutCount, workouts.Length.ToString()},
                {AnalyticsEvents.FileExport.Success_RecordCount, records.Length.ToString()},
            });

            OperationStatus = AppRes.MainPage_Status_SavingFile;
            logger.Verbose("Creating output report");
            return Output.CreateExcelReport(records, workouts, _settings, fileManager);

            async Task<(Workout[] workouts, Record[] records)> QueryHealth(DateInterval dateRange)
            {
                return (
                    (await healthStore.GetWorkoutsAsync(dateRange)).ToArray(),
                    (await healthStore.GetHealthRecordsAsync(dateRange)).ToArray());
            }

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