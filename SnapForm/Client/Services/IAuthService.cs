using System.Threading.Tasks;
using SnapForm.Shared;

namespace SnapForm.Client.Services
{
    public interface IAuthService
    {
        Task<ServiceResponse<string>> Register(EnterpriseRegister request);
        Task<ServiceResponse<string>> Login(UserLogin request);
    }
}