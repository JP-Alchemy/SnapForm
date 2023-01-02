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
    public class FormController : ControllerBase
    {
        private readonly FormService _formService;
        private readonly InputService _inputService;
        private readonly FieldsService _fieldsService;
        private readonly IUtilityService _utilService;

        public FormController(FormService formService, InputService inputService,
            FieldsService fieldsService, IUtilityService utilService)
        {
            _formService = formService;
            _inputService = inputService;
            _fieldsService = fieldsService;
            _utilService = utilService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var user = await _utilService.GetEnterpriseUser();
            var fields = await _formService.GetForms(user.EnterpriseId);
            return Ok(fields);
        }

        [HttpGet("{formId}")]
        public async Task<IActionResult> Get(string formId)
        {
            var form = await _formService.GetEnterpriseForm(formId);
            return Ok(form);
        }

        [HttpGet("with_fields/{formId}")]
        public async Task<IActionResult> GetFormWithFields(string formId)
        {
            var form = await _formService.GetEnterpriseForm(formId);
            if (!form.Success) return Ok(form);
            var fields = await _fieldsService.GetFields(form.Data.FormInputIds);
            form.Data.Fields = fields.Data;
            return Ok(form);
        }

        [HttpGet("scan/{formId}")]
        public async Task<IActionResult> Scan(string formId)
        {
            var user = await _utilService.GetAppUser();
            var form = await _formService.ScanForm(formId, user);
            return Ok(form);
        }

        [HttpGet("missing/{formId}")]
        public async Task<IActionResult> GetMissing(string formId)
        {
            var user = await _utilService.GetAppUser();
            var form = await _inputService.GetMissingRequiredInputs(formId, user);
            return Ok(new ServiceResponse<IEnumerable<SnapFormField>>
            {
                Data = form,
                Message = "Found Missing Form Inputs",
                Success = true
            });
        }

        [HttpGet("submissions/count/{formId}")]
        public async Task<IActionResult> CountFormSubmission(string formId)
        {
            var form = await _formService.FormSubmissionCount(formId);
            return Ok(new ServiceResponse<long>
            {
                Data = form,
                Message = $"Found {form} submissions",
                Success = true
            });
        }

        [HttpGet("submissions/{formId}")]
        public async Task<IActionResult> GetFormSubmission(string formId)
        {
            var form = await _formService.GetFormSubmissions(formId);
            return Ok(new ServiceResponse<List<FormSubmission>>
            {
                Data = form,
                Message = $"Found {form.Count} submissions",
                Success = true
            });
        }

        [HttpGet("user_submissions")]
        public async Task<IActionResult> GetUserFormSubmission()
        {
            var user = await _utilService.GetAppUser();
            var submissions = await _formService.GetUserFormSubmissions(user.Id);
            return Ok(new ServiceResponse<List<FormSubmission>>
            {
                Data = submissions,
                Message = $"Found {submissions.Count} submissions",
                Success = true
            });
        }

        [HttpGet("user_submissions/{submissionId}")]
        public async Task<IActionResult> GetUserFormSubmission(string submissionId)
        {
            var user = await _utilService.GetAppUser();
            var submissions = await _formService.GetUserFormSubmissions(user.Id, submissionId);
            return Ok(new ServiceResponse<FormSubmission>
            {
                Data = submissions,
                Message = "Found submissions",
                Success = true
            });
        }

        [HttpPost]
        public async Task<ServiceResponse<string>> Post(EnterpriseFormCreation entry)
        {
            var user = await _utilService.GetEnterpriseUser();
            var res = await _formService.SaveEnterpriseForm(new EnterpriseForm()
            {
                ID = entry.ID,
                Created_by = user.Id,
                EnterpriseId = user.EnterpriseId,
                Description = entry.Description,
                FormInputIds = entry.FormInputIds,
                Name = entry.Name
            });
            return res;
        }

        [HttpPost("qr")]
        public async Task<IActionResult> Post([FromBody] string values)
        {
            return Ok(await _formService.GenerateQrCode(values));
        }
    }
}