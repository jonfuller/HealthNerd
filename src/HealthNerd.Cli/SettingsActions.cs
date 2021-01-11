using System.IO;
using System.Threading.Tasks;
using HealthKitData.Core.Excel.Settings;

namespace HealthNerd.Cli
{
    public static class SettingsActions
    {
        public static Task<ExitCode> CreateDefaultSettingsFile(CreateSettingsOptions opts, TextWriter stdErr)
        {
            if (File.Exists(opts.OutputFilename))
            {
                stdErr.WriteLine("Output file '' already exists. Aborting.");
                return Task.FromResult(ExitCode.ExportFileExists(opts.OutputFilename));
            }
            SettingsFileHelpers.ToFile(opts.OutputFilename, Settings.Default);
            // TODO provide help for alternative values
            return Task.FromResult(ExitCode.Success);
        }
    }
}