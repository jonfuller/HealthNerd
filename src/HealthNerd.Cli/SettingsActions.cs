using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HealthKitData.Core.Excel.Settings;
using Microsoft.Extensions.Logging;

namespace HealthNerd.Cli
{
    public static class SettingsActions
    {
        public static Task<ExitCode> CreateDefaultSettingsFile(CreateSettingsOptions opts, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger(typeof(SettingsActions));
            logger.LogInformation(opts.ToString());

            if (File.Exists(opts.OutputFilename))
            {
                logger.LogError($"Output file '{opts.OutputFilename}' already exists. Aborting.");
                return Task.FromResult(ExitCode.ExportFileExists(opts.OutputFilename));
            }
            SettingsFileHelpers.ToFile(opts.OutputFilename, Settings.Default);
            return Task.FromResult(ExitCode.Success);
        }

        public static Task<ExitCode> ShowSettingsOptions(TextWriter stdOut)
        {
            foreach (var s in Settings.Default)
            {
                WriteSettingHelp(s, stdOut);
                stdOut.WriteLine($"------------------------------");
            }
            return Task.FromResult(ExitCode.Success);

            static void WriteSettingHelp(Setting s, TextWriter stdOut)
            {
                var settingType = typeof(Settings).GetProperty(s.Name).PropertyType;
                var validOpts = GetValidOpts(settingType);
                stdOut.WriteLine($"{s.Name} ({settingType.Name}) - {s.Description}");

                foreach (var opt in validOpts)
                {
                    stdOut.WriteLine($"  {opt}");
                }
            }

            static IEnumerable<string> GetValidOpts(Type type)
            {
                if (type.IsEnum)
                {
                    return Enum.GetNames(type);
                }
                if (type == typeof(bool))
                {
                    return new[] { true.ToString(), false.ToString() };
                }

                if (type == typeof(int))
                {
                    return new[] { "Any positive integer." };
                }

                return Enumerable.Empty<string>();
            }
        }
    }
}