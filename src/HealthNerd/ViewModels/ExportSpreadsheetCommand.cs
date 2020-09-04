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
using LanguageExt;
using NodaTime;
using NodaTime.Extensions;
using Resources;
using Serilog;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace HealthNerd.ViewModels
{
    public class ExportSpreadsheetCommand : ViewModelBase
    {
        private readonly LocalDateTime _latestEligibleTime;
        private readonly IFileManager _fileManager;
        private readonly IActionPresenter _actionPresenter;
        private readonly ISettingsStore _settings;
        private readonly IClock _clock;
        private readonly IAnalytics _analytics;
        private readonly IHealthStore _healthStore;
        private readonly IShare _share;

        private bool _isQueryingHealth;
        private string _operationStatus;

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
                Command.ChangeCanExecute();
            }
        }

        public ExportSpreadsheetCommand(IFileManager fileManager, IActionPresenter actionPresenter, ISettingsStore settings, IClock clock, IAnalytics analytics, IHealthStore healthStore, IShare share, Configuration config, ILogger logger)
        {
            _latestEligibleTime = clock.InTzdbSystemDefaultZone().GetCurrentLocalDateTime().Minus(config.LatestEligibleExportPeriod);

            _fileManager = fileManager;
            _actionPresenter = actionPresenter;
            _settings = settings;
            _clock = clock;
            _analytics = analytics;
            _healthStore = healthStore;
            _share = share;
            Command = GetCommand(logger.ForContext("NerdOperation", Guid.NewGuid()));
        }

        public Command Command { get; }

        private Command GetCommand(ILogger logger)
        {
            return new Command(async () =>
                {
                    if (HasRecentUsableExport(_fileManager, _latestEligibleTime, out var latestFile))
                    {
                        var latestFileTimeString = latestFile.exportTime.ToString("HH:mm", CultureInfo.CurrentUICulture);
                        _actionPresenter.ActionSheet()
                           .WithCancel("Cancel", () => { })
                           .With($"Continue with Previous (Today at {latestFileTimeString})", async () => await ShareFile(latestFile.file, latestFile.contentType))
                           .With("Create New", async () => await ExportNewFile())
                           .Show("Found a recent export.");
                    }
                    else
                    {
                        await ExportNewFile();
                    }
                    CleanupOldExports(_fileManager, _latestEligibleTime);

                    async Task ExportNewFile()
                    {
                        var logOperation = logger.ForContext("NerdOperation", Guid.NewGuid());
                        IsQueryingHealth = true;
                        await Prelude.TryAsync(async () => await ExportHealthToExcel(logOperation, _settings, _clock, _fileManager, _analytics, _healthStore))
                           .Match(
                                Succ: async x => await ShareFile(x.file, x.contentType),
                                Fail: ex =>
                                {
                                    OperationStatus = AppRes.MainPage_Status_Error;
                                    _analytics.LogEvent(AnalyticsEvents.FileExport.Failure, "exception", ex.ToString());
                                    logOperation.Error(ex, "An error occurred");
                                });
                        IsQueryingHealth = false;
                    }

                    async Task ShareFile(FileInfo file, ContentType contentType)
                    {
                        logger.Verbose("Sharing file {File}", file);
                        OperationStatus = AppRes.MainPage_Status_SharingFile;
                        await _share.RequestAsync(new ShareFileRequest
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

        private async Task<(FileInfo file, ContentType contentType)> ExportHealthToExcel(ILogger logger, ISettingsStore settings, IClock clock, IFileManager fileManager, IAnalytics analytics, IHealthStore healthStore)
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
    }
}