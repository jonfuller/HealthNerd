using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using HealthNerd.Utility;
using LanguageExt;
using NodaTime;
using NodaTime.Extensions;
using NodaTime.Text;
using static LanguageExt.Prelude;

namespace HealthNerd.Services
{
    public class FileManager : IFileManager
    {
        private const string ExportFilePrefix = "HealthNerd";
        private readonly LocalDateTimePattern _exportFilePattern = LocalDateTimePattern.CreateWithInvariantCulture($"'{ExportFilePrefix}-'yyyy-MM-dd-HH-mm'.xlsx'");
        private readonly IClock _clock;
        private readonly string _directory;

        public FileManager(IClock clock, string directory)
        {
            _clock = clock;
            _directory = directory;
        }

        public Option<(FileInfo file, LocalDateTime exportTime, ContentType contentType)> GetLatestExportFile()
        {
            return GetExportFiles()
               .OrderBy(s => s.Name)
               .Select(x => (file: x, parsed: _exportFilePattern.Parse(x.Name)))
               .Where(x => x.parsed.Success)
               .Select(x => (x.file, exportTime: x.parsed.Value, contentType: Output.XlsxContentType))
               .LastOrDefault()
               .Then(Optional);
        }

        private IEnumerable<FileInfo> GetExportFiles()
        {
            return new DirectoryInfo(_directory).EnumerateFiles()
               .Where(f => f.Name.StartsWith(ExportFilePrefix));
        }

        public void DeleteExportsBefore(LocalDateTime reference)
        {
            GetExportFiles()
               .Select(x => (file: x, parsed: _exportFilePattern.Parse(x.Name)))
               .Where(x => x.parsed.Success)
               .Where(x => x.parsed.Value < reference)
               .Select(x => x.file)
               .Then(Delete);

            static void Delete(IEnumerable<FileInfo> filesToDelete)
            {
                foreach (var file in filesToDelete)
                {
                    file.Delete();
                }
            }
        }

        public FileInfo GetNewFileName() => new FileInfo(Path.Combine(_directory, _exportFilePattern.Format(_clock.InTzdbSystemDefaultZone().GetCurrentLocalDateTime())));
    }
}