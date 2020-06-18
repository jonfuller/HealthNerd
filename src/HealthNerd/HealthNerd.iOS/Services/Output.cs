using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using CsvHelper;
using HealthNerd.iOS.Services.Csv;
using HealthNerd.iOS.Utility;
using LanguageExt;
using NodaTime;
using OfficeOpenXml;
using Xamarin.Essentials;
using static LanguageExt.Prelude;

namespace HealthNerd.iOS.Services
{
    public static class Output
    {
        public static Option<(FileInfo filename, ContentType contentType)> Create(Option<IEnumerable<Intervaled<int>>> stepData)
        {
            return stepData.Match(
                Some: _ => Some(WriteStepsToCsv(_)),
                None: None);

        }

        static (FileInfo, ContentType) WriteStepsToCsv(IEnumerable<Intervaled<int>> steps)
        {
            var file = new FileInfo(Path.Combine(FileSystem.CacheDirectory, $"HealthNerd.csv"));

            using (var csv = new CsvWriter(file.CreateText(), leaveOpen: false, cultureInfo: CultureInfo.CurrentUICulture))
            {
                csv.Configuration.TypeConverterCache.AddConverter(typeof(Instant), new InstantConverter());
                csv.WriteRecords(steps.Select(s => new { s.Interval.Start, s.Value }));
            }

            return (file, new ContentType("text/csv"));
        }

        static (FileInfo, ContentType) WriteStepsToExcel(IEnumerable<Intervaled<int>> steps)
        {
            var file = new FileInfo(Path.Combine(FileSystem.CacheDirectory, $"HealthNerd.xlsx"));

            using (var excelFile = new ExcelPackage())
            {
                var sheet = excelFile.Workbook.Worksheets.Add("hello");

                excelFile.SaveAs(file);
            }
            return (file, new ContentType("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
        }
    }
}