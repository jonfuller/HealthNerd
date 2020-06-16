using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.Common;

namespace HealthNerd.iOS.Services
{
    public interface IAuthorizer
    {
        Task<Option<Error>> RequestAuthorizeAppleHealth();
    }
}