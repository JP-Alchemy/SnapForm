using System;
using System.Collections.Generic;
using System.Linq;
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
    public class FieldsController : ControllerBase
    {
        private readonly FormService _formService;
        private readonly FieldsService _fieldsService;
        private readonly InputService _inputService;
        private readonly IUtilityService _utilService;

        public FieldsController(FormService formService, FieldsService fieldsService, InputService inputService, IUtilityService utilService)
        {
            _formService = formService;
            _fieldsService = fieldsService;
            _inputService = inputService;
            _utilService = utilService;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var fields = await _fieldsService.GetFields();
            return Ok(fields);
        }

        [HttpGet("{fieldId}")]
        public async Task<IActionResult> Get(string fieldId)
        {
            var res = await _fieldsService.GetField(fieldId);
            return Ok(res);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("save")]
        public async Task<IActionResult> Save(SnapFormField field)
        {
            var res = await _fieldsService.SaveField(field);
            return Ok(res);
        }

        [HttpPost]
        public async Task<ServiceResponse<string>> Post(List<SnapFormUserEntry> entries)
        {
            var user = await _utilService.GetAppUser();

            foreach (var e in entries)
            {
                e.UserId = user.Id;
            }

            var res = await _inputService.SaveAppUserInputs(entries);

            return res;
        }

        [HttpGet("inputs")]
        public async Task<IActionResult> GetUserInputs()
        {
            var user = await _utilService.GetAppUser();
            var res = await _inputService.GetAppUserInputs(user.Id);
            return Ok(res);
        }
        
        [HttpGet("occupations")]
        public async Task<IActionResult> GetOccupations()
        {
            var occupations = await _fieldsService.GetAllOccupations();
            return Ok(occupations);
        }
    }
}