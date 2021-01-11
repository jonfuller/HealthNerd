using System;
using CommandLine;

namespace HealthNerd.Cli
{
    [Verb("excel-export", HelpText = "Creates Excel (XLSX) export of an Apple Health export file.")]
    public class ExcelReportOptions
    {
        [Option(longName: "export-file", HelpText = "Path to Apple Health export file.", Required = true)]
        public string PathToHealthExportFile { get; set; }

        [Option(longName: "output", HelpText = "Path to output file.", Required = false, Default = "output.xlsx")]
        public string OutputFilename { get; set; }

        [Option(longName: "custom-sheets", HelpText = "Path to XLSX custom sheets file.", Required = false)]
        public string PathToCustomSheetsExcelFile { get; set; }
        [Option(longName: "settings-file", HelpText = "Path to settings file.", Required = false)]
        public string PathToSettingsFile { get; set; }
    }

    [Verb("settings-generate", HelpText = "Generates a default settings file for use during Excel report creation.")]
    public class CreateSettingsOptions
    {
        [Option(longName: "output", HelpText = "Path to output file.", Required = false, Default = "settings.json")]
        public string OutputFilename { get; set; }
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