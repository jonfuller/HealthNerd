using System.IO;

namespace HealthNerd.Services
{
    public interface IFileCreator
    {
        FileInfo GetFileName();
    }
}