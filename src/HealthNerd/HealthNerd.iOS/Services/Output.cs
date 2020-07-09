using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using HealthKitData.Core;
using HealthKitData.Core.Excel;
using HealthKitData.Core.Excel.Settings;
using HealthNerd.iOS.Utility;
using LanguageExt;
using OfficeOpenXml;
using Xamarin.Essentials;
using static LanguageExt.Prelude;

namespace HealthNerd.iOS.Services
{
    public static class Output
    {
        public static Option<(FileInfo filename, ContentType contentType)> CreateExcelReport(IEnumerable<Record> records, IEnumerable<Workout> workouts, ISettingsStore settings)
        {
            return Some(WriteExcelReport(records, workouts, GetSettings(settings)));

            static (FileInfo file, ContentType) WriteExcelReport(IEnumerable<Record> records, IEnumerable<Workout> workouts, Settings settings)
            {
                var file = new FileInfo(Path.Combine(FileSystem.CacheDirectory, $"HealthNerd.xlsx"));

                using (var excelFile = new ExcelPackage())
                {
                    ExcelReport.BuildReport(records.ToList(), workouts.ToList(), excelFile.Workbook, settings, Enumerable.Empty<ExcelWorksheet>());

                    excelFile.SaveAs(file);
                }
                return (file, new ContentType("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
            }

            static Settings GetSettings(ISettingsStore settings)
            {
                return new Settings
                {
                    DistanceUnit = settings.DistanceUnit.Match(
                        Some: s => s,
                        None: SettingsDefaults.DistanceUnit),
                    WeightUnit = settings.MassUnit.Match(
                        Some: s => s,
                        None: SettingsDefaults.MassUnit),
                    EnergyUnit = settings.EnergyUnit.Match(
                        Some: s => s,
                        None: SettingsDefaults.EnergyUnit),
                    DurationUnit = settings.DurationUnit.Match(
                        Some: s => s,
                        None: SettingsDefaults.DurationUnit),

                    // Excel Settings, TODO
                    CustomSheetsPlacement = CustomSheetsPlacement.AfterSummary,
                    NumberOfMonthlySummaries = 3,
                    OmitEmptyColumnsOnMonthlySummary = true,
                    OmitEmptyColumnsOnOverallSummary = true,
                    OmitEmptySheets = true,
                    UseConstantNameForMostRecentMonthlySummarySheet = true,
                    UseConstantNameForPreviousMonthlySummarySheet = true
                };
            }
        }
    }
}