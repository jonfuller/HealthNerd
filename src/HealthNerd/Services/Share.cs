using System.Threading.Tasks;
using Xamarin.Essentials;

namespace HealthNerd.Services
{
    public class Share : IShare
    {
        public Task RequestAsync(ShareFileRequest request)
        {
            return Xamarin.Essentials.Share.RequestAsync(request);
        }
    }
}