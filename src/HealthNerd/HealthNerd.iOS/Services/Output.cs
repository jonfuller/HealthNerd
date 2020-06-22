using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using HealthKitData.Core;
using HealthKitData.Core.Excel;
using HealthKitData.Core.Excel.Settings;
using LanguageExt;
using OfficeOpenXml;
using Xamarin.Essentials;
using static LanguageExt.Prelude;

namespace HealthNerd.iOS.Services
{
    public static class Output
    {
        public static Option<(FileInfo filename, ContentType contentType)> CreateExcelReport(IEnumerable<Record> records, IEnumerable<Workout> workouts)
        {
            return Some(WriteExcelReport(records, workouts));

            static (FileInfo file, ContentType) WriteExcelReport(IEnumerable<Record> records, IEnumerable<Workout> workouts)
            {
                var file = new FileInfo(Path.Combine(FileSystem.CacheDirectory, $"HealthNerd.xlsx"));

                using (var excelFile = new ExcelPackage())
                {
                    ExcelReport.BuildReport(records.ToList(), workouts.ToList(), excelFile.Workbook, Settings.Default, Enumerable.Empty<ExcelWorksheet>());

                    excelFile.SaveAs(file);
                }
                return (file, new ContentType("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));

            }
        }
    }
}