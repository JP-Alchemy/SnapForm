using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SnapForm.Shared;

namespace SnapForm.Client.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;

        public AuthService(HttpClient http)
        {
            _http = http;
        }
        
        public async Task<ServiceResponse<string>> Register(EnterpriseRegister request)
        {
            var res = await _http.PostAsJsonAsync("api/auth/enterprise/register", request);
            return await res.Content.ReadFromJsonAsync<ServiceResponse<string>>();
        }

        public async Task<ServiceResponse<string>> Login(UserLogin request)
        {
            var res = await _http.PostAsJsonAsync("api/auth/enterprise/login", request);
            return await res.Content.ReadFromJsonAsync<ServiceResponse<string>>();
        }
    }
}