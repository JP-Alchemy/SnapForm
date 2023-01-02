using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SnapForm.Server.Data;
using SnapForm.Shared;

namespace SnapForm.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authRepo;

        public AuthController(AuthService authRepo)
        {
            _authRepo = authRepo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(SnapFormUserRegister req)
        {
            if (req.Password != req.ConfirmPassword)
                return Ok(new ServiceResponse<string>
                {
                    Message = "Passwords do not match"
                });

            var res = await _authRepo.AppRegister(
                new SnapFormUser()
                {
                    Email = req.Email,
                    DateCreated = req.DateCreated,
                    IsConfirmed = req.IsConfirmed
                }, req.Password);
            return Ok(res);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLogin req)
        {
            var res = await _authRepo.AppLogin(req.Email, req.Password);
            return Ok(res);
        }

        [HttpPost("enterprise/register")]
        public async Task<IActionResult> Enterprise_Register(EnterpriseRegister req)
        {
            if (req.Password != req.ConfirmPassword)
                return Ok(new ServiceResponse<string>
                {
                    Message = "Passwords do not match"
                });

            var res = await _authRepo.EnterpriseRegister(
                new EnterpriseUser()
                {
                    Email = req.Email,
                    DateCreated = req.DateCreated,
                    IsConfirmed = req.IsConfirmed,
                    FirstName = req.FirstName,
                    LastName = req.LastName
                },
                new Enterprise()
                {
                    Name = req.EnterpriseName,
                    Industry = req.EnterpriseIndustry
                }
                , req.Password);
            return Ok(res);
        }

        [HttpPost("enterprise/login")]
        public async Task<IActionResult> Enterprise_Login(UserLogin req)
        {
            var res = await _authRepo.EnterpriseLogin(req.Email, req.Password);
            return Ok(res);
        }
    }
}