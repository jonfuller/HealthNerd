using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using HealthKitData.Core;
using HealthKitData.Core.Excel;
using HealthKitData.Core.Excel.Settings;
using HealthNerd.Utility;
using LanguageExt;
using NodaTime;
using OfficeOpenXml;

namespace HealthNerd.Services
{
    public static class Output
    {
        public static readonly ContentType XlsxContentType = new ContentType("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

        public static (FileInfo filename, ContentType contentType) CreateExcelReport(IEnumerable<Record> records, IEnumerable<Workout> workouts, ISettingsStore settings, IFileManager fileManager, DateTimeZone zone)
        {
            var file = fileManager.GetNewFileName();

            WriteExcelReport(file, records, workouts, GetSettings(settings), settings, zone);

            return (file, XlsxContentType);

            static void WriteExcelReport(FileInfo file, IEnumerable<Record> records, IEnumerable<Workout> workouts, Settings settings, ISettingsStore settingsStore, DateTimeZone zone)
            {
                using var excelFile = new ExcelPackage();

                var customSheets = LoadCustom(settingsStore).Match(
                    Some: c => c,
                    None: Enumerable.Empty<ExcelWorksheet>());

                ExcelReport.BuildReport(records.ToList(), workouts.ToList(), excelFile.Workbook, settings, zone, customSheets);

                excelFile.SaveAs(file);
            }

            static Option<IEnumerable<ExcelWorksheet>> LoadCustom(ISettingsStore settingsStore)
            {
                return settingsStore.CustomSheetsLocation
                   .Map(s => new ExcelPackage(new FileInfo(s)).Workbook.Worksheets.AsEnumerable());
            }

            static Settings GetSettings(ISettingsStore settings)
            {
                return new Settings
                {
                    DistanceUnit = settings.DistanceUnit.Match(
                        Some: s => s,
                        None: () => SettingsDefaults.DistanceUnit),
                    WeightUnit = settings.MassUnit.Match(
                        Some: s => s,
                        None: () => SettingsDefaults.MassUnit),
                    EnergyUnit = settings.EnergyUnit.Match(
                        Some: s => s,
                        None: () => SettingsDefaults.EnergyUnit),
                    DurationUnit = settings.DurationUnit.Match(
                        Some: s => s,
                        None: () => SettingsDefaults.DurationUnit),

                    NumberOfMonthlySummaries = settings.NumberOfMonthlySummaries.Match(
                        Some: s => s,
                        None: () => SettingsDefaults.NumberOfMonthlySummaries),

                    OmitEmptyColumnsOnMonthlySummary = settings.OmitEmptyColumnsOnMonthlySummary.Match(
                        Some: s => s,
                        None: () => SettingsDefaults.OmitEmptyColumnsOnMonthlySummary),
                    OmitEmptyColumnsOnOverallSummary = settings.OmitEmptyColumnsOnOverallSummary.Match(
                        Some: s => s,
                        None: () => SettingsDefaults.OmitEmptyColumnsOnOverallSummary),
                    OmitEmptySheets = settings.OmitEmptySheets.Match(
                        Some: s => s,
                        None: () => SettingsDefaults.OmitEmptySheets),

                    // Other Excel Settings, not hooked up to settings store
                    CustomSheetsPlacement = SettingsDefaults.CustomSheetsPlacement,
                    UseConstantNameForMostRecentMonthlySummarySheet = SettingsDefaults.UseConstantNameForMostRecentMonthlySummarySheet,
                    UseConstantNameForPreviousMonthlySummarySheet = SettingsDefaults.UseConstantNameForPreviousMonthlySummarySheet
                };
            }
        }
    }
}