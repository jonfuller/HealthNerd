using System.Threading.Tasks;
using HealthKitData.Core.Excel.Settings;

namespace HealthNerd.Cli
{
    public static class SettingsActions
    {
        public static Task<ExitCode> CreateDefaultSettingsFile(CreateSettingsOptions opts)
        {
            SettingsFileHelpers.ToFile(opts.OutputFilename, Settings.Default);
            // TODO provide help for alternative values
            return Task.FromResult(ExitCode.Success);
        }
    }
}