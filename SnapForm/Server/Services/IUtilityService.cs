using System.Threading.Tasks;
using SnapForm.Shared;

namespace SnapForm.Server.Services
{
    public interface IUtilityService
    {
        Task<SnapFormUser> GetAppUser();
        Task<EnterpriseUser> GetEnterpriseUser();
    }
}