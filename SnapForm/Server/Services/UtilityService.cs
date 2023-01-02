using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using SnapForm.Shared;

namespace SnapForm.Server.Services
{
    public class UtilityService : IUtilityService
    {
        private readonly IHttpContextAccessor _httpCtxAccessor;
        private readonly IMongoCollection<SnapFormUser> _appUser;
        private readonly IMongoCollection<EnterpriseUser> _enterpriseUser;
        private readonly IServerOptions _config;

        public UtilityService(IHttpContextAccessor httpCtxAccessor, IServerOptions config)
        {
            _httpCtxAccessor = httpCtxAccessor;

            _config = config;

            var client = new MongoClient(_config.MongoConnectionString);
            var database = client.GetDatabase(_config.DatabaseName);
            _appUser = database.GetCollection<SnapFormUser>(CollectionNames.AppUser);
            _enterpriseUser = database.GetCollection<EnterpriseUser>(CollectionNames.EnterpriseUser);
        }

        public async Task<SnapFormUser> GetAppUser()
        {
            // Get user ID function
            var uId = _httpCtxAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Get user function
            var user = (await _appUser.FindAsync(u => u.Id == uId)).FirstOrDefault();
            return user;
        }

        public async Task<EnterpriseUser> GetEnterpriseUser()
        {
            // Get user ID function
            var uId = _httpCtxAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Get user function
            var user = (await _enterpriseUser.FindAsync(u => u.Id == uId)).FirstOrDefault();
            return user;
        }
    }
}