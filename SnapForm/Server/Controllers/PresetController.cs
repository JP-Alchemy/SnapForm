using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnapForm.Server.Services;
using SnapForm.Shared;

namespace SnapForm.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PresetController : ControllerBase
    {
        private readonly PresetsService _presetsService;
        private readonly IUtilityService _utilService;

        public PresetController(PresetsService presetsService, IUtilityService utilService)
        {
            _presetsService = presetsService;
            _utilService = utilService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var user = await _utilService.GetAppUser();
            var res = await _presetsService.GetAllByUserId(user.Id);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Post(SnapFormUserPresets entry)
        {
            var user = await _utilService.GetAppUser();
            entry.UserId = user.Id;
            var res = await _presetsService.Save(entry);
            return Ok(res);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete(SnapFormUserPresets entry)
        {
            var user = await _utilService.GetAppUser();
            entry.UserId = user.Id;
            var res = await _presetsService.Remove(entry);
            return Ok(res);
        }
    }
}