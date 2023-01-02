using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SnapForm.Shared;

namespace SnapForm.Client.Services
{
    public class FormService : IFormService
    {
        private readonly HttpClient _http;
        public IList<EnterpriseForm> Forms { get; set; } = new List<EnterpriseForm>();

        public FormService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ServiceResponse<List<EnterpriseForm>>> GetAll()
        {
            var res = await _http.GetFromJsonAsync<ServiceResponse<List<EnterpriseForm>>>("api/form/all");
            Forms = res.Data;
            return res;
        }

        public async Task<ServiceResponse<EnterpriseForm>> Get(string formId)
        {
            var res = await _http.GetFromJsonAsync<ServiceResponse<EnterpriseForm>>($"api/form/{formId}");
            return res;
        }

        public async Task<ServiceResponse<EnterpriseForm>> GetFormWithFields(string formId)
        {
            var res = await _http.GetFromJsonAsync<ServiceResponse<EnterpriseForm>>($"api/form/with_fields/{formId}");
            return res;
        }

        public async Task<ServiceResponse<string>> Save(EnterpriseFormCreation form)
        {
            var res = await _http.PostAsJsonAsync("api/form", form);
            return await res.Content.ReadFromJsonAsync<ServiceResponse<string>>();
        }

        public async Task<ServiceResponse<string>> GetQR(string formId)
        {
            var res = await _http.PostAsJsonAsync("api/form/qr", formId);
            return await res.Content.ReadFromJsonAsync<ServiceResponse<string>>();
        }

        public async Task<ServiceResponse<List<FormSubmission>>> GetSubmissionsTask(string formId)
        {
            var res = await _http.GetFromJsonAsync<ServiceResponse<List<FormSubmission>>>($"api/form/submissions/{formId}");
            return res;
        }
    }
}
