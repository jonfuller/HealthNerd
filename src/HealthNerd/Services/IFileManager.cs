using System.IO;
using System.Net.Mime;
using LanguageExt;
using NodaTime;

namespace HealthNerd.Services
{
    public interface IFileManager
    {
        FileInfo GetNewFileName();
        Option<(FileInfo file, LocalDateTime exportTime, ContentType contentType)> GetLatestExportFile();
        void DeleteExportsBefore(LocalDateTime reference);
    }
}