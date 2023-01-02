using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SnapForm.Shared;

namespace SnapForm.Client.Services
{
    public class FieldService
    {
        private readonly HttpClient _http;
        public IList<SnapFormField> Fields { get; set; } = new List<SnapFormField>();

        public FieldService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ServiceResponse<List<SnapFormField>>> GetAll()
        {
            var res = await _http.GetFromJsonAsync<ServiceResponse<List<SnapFormField>>>("api/fields");
            Fields = res.Data;
            return res;
        }

        public async Task<ServiceResponse<SnapFormField>> Get(string id)
        {
            var res = await _http.GetFromJsonAsync<ServiceResponse<SnapFormField>>($"api/fields/{id}");
            return res;
        }

        public async Task<ServiceResponse<string>> Save(SnapFormField field)
        {
            var res = await _http.PostAsJsonAsync("api/fields/save", field);
            return await res.Content.ReadFromJsonAsync<ServiceResponse<string>>();
        }
    }
}
