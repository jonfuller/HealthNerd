using System.Globalization;
using System.IO;
using NodaTime;
using NodaTime.Extensions;

namespace HealthNerd.Services
{
    public class ClockBasedFileCreator : IFileCreator
    {
        private readonly IClock _clock;
        private readonly string _directory;

        public ClockBasedFileCreator(IClock clock, string directory)
        {
            _clock = clock;
            _directory = directory;
        }

        public FileInfo GetFileName() => new FileInfo(Path.Combine(
            _directory,
            $"HealthNerd-{_clock.InTzdbSystemDefaultZone().GetCurrentLocalDateTime().ToString("yyyy-MM-dd-HH-mm", CultureInfo.InvariantCulture)}.xlsx"));
    }
}