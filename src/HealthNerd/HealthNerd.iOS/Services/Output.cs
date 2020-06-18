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
using Xamarin.Essentials;
using static LanguageExt.Prelude;

namespace HealthNerd.iOS.Services
{
    public static class Output
    {
        public static Option<(FileInfo filename, ContentType contentType)> Create(Option<IEnumerable<Intervaled<int>>> stepData)
        {
            return stepData.Match(
                Some: WriteStepsToCsv,
                None: None);

            Option<(FileInfo filename, ContentType ContentType)> WriteStepsToCsv(IEnumerable<Intervaled<int>> steps)
            {
                var file = Path.Combine(FileSystem.CacheDirectory, $"HealthNerd.csv");

                using (var csvFile = new CsvWriter(File.CreateText(file), leaveOpen: false, cultureInfo: CultureInfo.CurrentUICulture))
                {
                    csvFile.Configuration.TypeConverterCache.AddConverter(typeof(Instant), new InstantConverter());
                    csvFile.WriteRecords(steps.Select(s => new { s.Interval.Start, s.Value }));
                }
                return Prelude.Some((new FileInfo(file), new ContentType("text/csv")));
            }
        }
    }
}