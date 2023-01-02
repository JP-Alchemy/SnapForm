using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SnapForm.Shared;

namespace SnapForm.Client.Services
{
    public interface IFormService
    {
        IList<EnterpriseForm> Forms { get; set; }
        Task<ServiceResponse<List<EnterpriseForm>>> GetAll();
        Task<ServiceResponse<EnterpriseForm>> Get(string formId);
        Task<ServiceResponse<EnterpriseForm>> GetFormWithFields(string formId);
        Task<ServiceResponse<string>> Save(EnterpriseFormCreation form);
        Task<ServiceResponse<string>> GetQR(string formId);
        Task<ServiceResponse<List<FormSubmission>>> GetSubmissionsTask(string formId);
    }
}
