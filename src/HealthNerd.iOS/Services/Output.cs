using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using HealthKitData.Core;
using HealthKitData.Core.Excel;
using HealthKitData.Core.Excel.Settings;
using HealthNerd.iOS.Utility;
using NodaTime;
using NodaTime.Extensions;
using OfficeOpenXml;
using Xamarin.Essentials;

namespace HealthNerd.iOS.Services
{
    public static class Output
    {
        private static readonly ContentType XlsxContentType = new ContentType("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

        public static (FileInfo filename, ContentType contentType) CreateExcelReport(IEnumerable<Record> records, IEnumerable<Workout> workouts, ISettingsStore settings, IClock clock)
        {
            var file = GetFileName(clock);

            WriteExcelReport(file, records, workouts, GetSettings(settings));

            return (file, XlsxContentType);

            static void WriteExcelReport(FileInfo file, IEnumerable<Record> records, IEnumerable<Workout> workouts, Settings settings)
            {
                using var excelFile = new ExcelPackage();

                ExcelReport.BuildReport(records.ToList(), workouts.ToList(), excelFile.Workbook, settings, Enumerable.Empty<ExcelWorksheet>());

                excelFile.SaveAs(file);
            }

            static FileInfo GetFileName(IClock clock) =>
                new FileInfo(Path.Combine(
                    FileSystem.CacheDirectory,
                    $"HealthNerd-{clock.InTzdbSystemDefaultZone().GetCurrentLocalDateTime().ToString("yyyy-MM-dd-HH-mm", CultureInfo.InvariantCulture)}.xlsx"));

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