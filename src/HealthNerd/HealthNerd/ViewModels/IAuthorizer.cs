using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.Common;

namespace HealthNerd.ViewModels
{
    public interface IAuthorizer
    {
        Task<Option<Error>> RequestAuthorizeAppleHealth();
    }
}