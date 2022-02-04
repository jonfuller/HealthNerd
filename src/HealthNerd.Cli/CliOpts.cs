using System;
using CommandLine;
using Microsoft.Extensions.Logging;

namespace HealthNerd.Cli
{
    interface IHasLogLevelOption
    {
        [Option('v', "log-level", Required = false, Default = Microsoft.Extensions.Logging.LogLevel.Information, HelpText = "The Doc ID to find the table in.")]
        LogLevel LogLevel { get; set; }
    }

    [Verb("excel-export", HelpText = "Creates Excel (XLSX) export of an Apple Health export file.")]
    public record ExcelReportOptions : IHasLogLevelOption
    {
        [Option(longName: "export-file", HelpText = "Path to Apple Health export file.", Required = true)]
        public string PathToHealthExportFile { get; set; }

        [Option(longName: "output", HelpText = "Path to output file.", Required = false, Default = "output.xlsx")]
        public string OutputFilename { get; set; }

        [Option(longName: "custom-sheets", HelpText = "Path to XLSX custom sheets file.", Required = false)]
        public string PathToCustomSheetsExcelFile { get; set; }
        [Option(longName: "settings-file", HelpText = "Path to settings file.", Required = false)]
        public string PathToSettingsFile { get; set; }
        public LogLevel LogLevel { get; set; }
    }

    [Verb("settings-generate", HelpText = "Generates a default settings file for use during Excel report creation.")]
    public record CreateSettingsOptions
    {
        [Option(longName: "output", HelpText = "Path to output file.", Required = false, Default = "settings.json")]
        public string OutputFilename { get; set; }
    }

    [Verb("settings-help", HelpText = "Shows possible values for all settings.")]
    public record HelpSettingsOptions
    {
    }

    public record ExitCode
    {
        public static ExitCode Success = new (0, "Success");
        public static ExitCode ParseError = new (5, "Error parsing program arguments.");
        public static ExitCode GeneralException(Exception e) => new (-1, e.ToString());
        public static ExitCode ExportFileNotFound(string filename) => new (1, $"Export file not found: {filename}");
        public static ExitCode ExportFileExists(string filename) => new(2, $"File exists: {filename}");

        private ExitCode(int value, string message)
        {
            Value = value;
            Message = message;
        }

        public int Value { get; }
        public string Message { get; }
    }
}