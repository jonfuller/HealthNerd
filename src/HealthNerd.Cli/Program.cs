using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using OfficeOpenXml;

namespace HealthNerd.Cli
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var stdOut = Console.Out;
            var stdErr = Console.Error;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var result = Parser.Default.ParseArguments<ExcelReportOptions, CreateSettingsOptions>(args)
               .MapResult(
                    (ExcelReportOptions opts) => ReportActions.CreateReport(opts, stdOut, stdErr),
                    (CreateSettingsOptions opts) => SettingsActions.CreateDefaultSettingsFile(opts),
                    Err);

            var exitCode = await result;
            stdErr.WriteLine($"{exitCode.Value} - {exitCode.Message}");
            return exitCode.Value;
        }

        static Task<ExitCode> Err(IEnumerable<Error> parseErrors)
        {
            return Task.FromResult(ExitCode.ParseError);
        }
    }
}
