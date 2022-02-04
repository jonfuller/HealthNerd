using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Disposing;
using HealthKitData.Core.DataExport;
using HealthKitData.Core.Excel;
using HealthKitData.Core.Excel.Settings;
using Microsoft.Extensions.Logging;
using NodaTime;
using OfficeOpenXml;

namespace HealthNerd.Cli
{
    public static class ReportActions
    {
        public static Task<ExitCode> CreateReport(ExcelReportOptions opts, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger(typeof(ReportActions));
            logger.LogInformation(opts.ToString());
            
            if (!File.Exists(opts.PathToHealthExportFile))
            {
                logger.LogError($"export file {opts.PathToHealthExportFile} already exists.");
                return Task.FromResult(ExitCode.ExportFileNotFound(opts.PathToHealthExportFile));

            }
            if (File.Exists(opts.OutputFilename))
                return Task.FromResult(ExitCode.ExportFileExists(opts.OutputFilename));

            var loader = Usable.Using(new StreamReader(opts.PathToHealthExportFile), reader =>
                ZipUtilities.ReadArchive(
                        reader.BaseStream,
                        entry => entry.FullName == "apple_health_export/export.xml",
                        entry => new XmlReaderExportLoader(entry.Open()))
                   .FirstOrDefault());

            var settings = GetSettings(opts, logger);
            var (package, customSheets) = GetCustomSheets(opts, logger);

            using (var excelFile = new ExcelPackage())
            using (package)
            {
                HealthKitData.Core.Logging.Configure(loggerFactory);
                var timeZone = DateTimeZone.ForOffset(Offset.FromHours(-5));
                ExcelReport.BuildReport(loader.Records, loader.Workouts, excelFile.Workbook, settings, timeZone, customSheets);

                excelFile.SaveAs(new FileInfo(opts.OutputFilename));
            }

            return Task.FromResult(ExitCode.Success);
        }

        static (IDisposable package, IEnumerable<ExcelWorksheet> customSheets) GetCustomSheets(ExcelReportOptions opts, ILogger logger)
        {
            var disposableNop = Disposable.Create(() => { });

            if (opts.PathToCustomSheetsExcelFile == null)
            {
                logger.LogInformation($"Custom sheets file not specified. Not using custom sheets.");
                return (disposableNop, Enumerable.Empty<ExcelWorksheet>());
            }

            if (!File.Exists(opts.PathToCustomSheetsExcelFile))
            {
                logger.LogInformation($"Custom sheets file not found: {opts.PathToCustomSheetsExcelFile}. Not using custom sheets.");
                return (disposableNop, Enumerable.Empty<ExcelWorksheet>());
            }

            var customs = new ExcelPackage(new FileInfo(opts.PathToCustomSheetsExcelFile));
            var customSheets = customs.Workbook.Worksheets
               .Where(w => w.Name.StartsWith("custom", StringComparison.CurrentCultureIgnoreCase))
               .ToList();

            return (customs, customSheets);
        }

        static Settings GetSettings(ExcelReportOptions opts, ILogger logger)
        {
            if (opts.PathToSettingsFile == null)
            {
                logger.LogInformation($"Settings file not specified. Using default settings.");
                return Settings.Default;
            }

            if (!File.Exists(opts.PathToSettingsFile))
            {
                logger.LogInformation($"Could not find specified settings file: {opts.PathToSettingsFile}. Using default settings.");
                return Settings.Default;
            }

            return SettingsFileHelpers.FromFile(opts.PathToSettingsFile);
        }
    }
}