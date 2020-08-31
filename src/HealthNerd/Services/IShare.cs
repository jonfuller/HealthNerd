using System.Threading.Tasks;
using Xamarin.Essentials;

namespace HealthNerd.Services
{
    public interface IShare
    {
        Task RequestAsync(ShareFileRequest request);
    }
}