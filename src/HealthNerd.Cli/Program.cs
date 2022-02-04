using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace HealthNerd.Cli
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var stdOut = Console.Out;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var parsed = Parser.Default.ParseArguments(args, GetVerbs());

            var loggerFactory = GetLoggerFactory(GetLogLevel(parsed));
            var mainLogger = loggerFactory.CreateLogger("Main");
            var result = Task.FromResult(ExitCode.ParseError);

            parsed.WithParsed(parsed => result = parsed switch
            {
                ExcelReportOptions opts => ReportActions.CreateReport(opts, loggerFactory),
                CreateSettingsOptions opts => SettingsActions.CreateDefaultSettingsFile(opts, loggerFactory),
                HelpSettingsOptions opts => SettingsActions.ShowSettingsOptions(stdOut),
                _ => Task.FromResult(ExitCode.ParseError)
            })
            .WithNotParsed(errors => {
                foreach (var error in errors)
                {
                    mainLogger.LogError(error.ToString());
                }

                result = Task.FromResult(ExitCode.ParseError);
            });

            var exitCode = await result;

            if (exitCode != ExitCode.Success)
            {
                Console.Error.WriteLine($"{exitCode.Message} ({exitCode.Value})");
            }
            return exitCode.Value;
        }

        static Type[] GetVerbs() => typeof(ExcelReportOptions).Assembly.GetTypes()
            .Where(x => x.CustomAttributes.Where(ca => ca.AttributeType == typeof(VerbAttribute)).Any())
            .ToArray();
        static LogLevel GetLogLevel(ParserResult<object> parseResult) => parseResult.MapResult(
            (IHasLogLevelOption x) => x.LogLevel,
            _ => LogLevel.Information);

        static ILoggerFactory GetLoggerFactory(LogLevel level) => LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(level);
            builder
                .AddFilter("Microsoft", LogLevel.Warning)
                .AddFilter("System", LogLevel.Warning)
                .AddSimpleConsole(opts =>
                {
                    opts.IncludeScopes = true;
                    opts.TimestampFormat = "hh:mm:ss ";
                })
                .AddConsole(opts =>
                {
                    opts.LogToStandardErrorThreshold = LogLevel.Trace; // put all logging to STDERR
            });
        });
    }
}
