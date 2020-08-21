using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.Common;

namespace HealthNerd.Services
{
    public interface IAuthorizer
    {
        Task<Option<Error>> RequestAuthorizeAppleHealth();
    }
}