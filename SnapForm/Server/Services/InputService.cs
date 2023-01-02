using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using SnapForm.Shared;

namespace SnapForm.Server.Services
{
    public class InputService
    {
        private readonly IServerOptions _config;
        private readonly IMongoCollection<SnapFormField> _fields;
        private readonly IMongoCollection<SnapFormUserEntry> _userFields;
        private readonly IMongoCollection<EnterpriseForm> _form;

        public InputService(IServerOptions config)
        {
            _config = config;
            var client = new MongoClient(_config.MongoConnectionString);
            var database = client.GetDatabase(_config.DatabaseName);
            _fields = database.GetCollection<SnapFormField>(CollectionNames.Fields);
            _userFields = database.GetCollection<SnapFormUserEntry>(CollectionNames.UserFields);
            _form = database.GetCollection<EnterpriseForm>(CollectionNames.Forms);
        }

        public async Task<ServiceResponse<string>> SaveAppUserInputs(List<SnapFormUserEntry> entries)
        {
            List<UpdateOneModel<SnapFormUserEntry>> requests = new List<UpdateOneModel<SnapFormUserEntry>>(entries.Count());
            foreach (var entity in entries)
            {
                var filter = new FilterDefinitionBuilder<SnapFormUserEntry>()
                    .Where(m => m.InputId == entity.InputId && m.UserId == entity.UserId);
                var update = new UpdateDefinitionBuilder<SnapFormUserEntry>()
                    .Set(m => m.Input, entity.Input)
                    .Set(m => m.InputType, entity.InputType)
                    .Set(m => m.AlternativeEntries, entity.AlternativeEntries);
                var request = new UpdateOneModel<SnapFormUserEntry>(filter, update) { IsUpsert = true };
                requests.Add(request);
            }

            var res = await _userFields.BulkWriteAsync(requests);

            return new ServiceResponse<string>()
            {
                Success = true,
                Message = $"{res.ModifiedCount} Modified, {res.Upserts.Count} Inserted",
                Data = "Success!"
            };
        }

        public async Task<ServiceResponse<List<SnapFormUserEntry>>> GetAppUserInputs(string userId)
        {
            var res = await (await _userFields.FindAsync(x => x.UserId == userId)).ToListAsync();
            return new ServiceResponse<List<SnapFormUserEntry>>()
            {
                Data = res,
                Success = true,
                Message = $"Found {res.Count} Inputs",
            };
        }

        public async Task<IEnumerable<SnapFormField>> GetMissingRequiredInputs(string formId, SnapFormUser user)
        {
            // Get the form to be filled out
            EnterpriseForm form = (await _form.FindAsync(x => x.ID == formId)).FirstOrDefault();
            // Get the Fields in the form
            var formFieldsFilter = Builders<SnapFormField>.Filter.In(f => f.Id, form.FormInputIds);
            List<SnapFormField> fields = await (await _fields.FindAsync(formFieldsFilter)).ToListAsync();
            // Get the user inputs for such fields
            var userInputFilter = Builders<SnapFormUserEntry>.Filter.And(
                Builders<SnapFormUserEntry>.Filter.In(f => f.InputId, form.FormInputIds),
                Builders<SnapFormUserEntry>.Filter.Eq(f => f.UserId, user.Id)
            );
            List<SnapFormUserEntry> entries = await (await _userFields.FindAsync(userInputFilter)).ToListAsync();

            // Check if the user has the required fields
            var requiredFields = fields.Where(f => f.IsRequired).Select(x => x.Id);
            var requiredMissing = requiredFields.Except(entries.Select(x => x.InputId)).ToList();

            return fields.Where(x => requiredMissing.Contains(x.Id));
        }

        public async Task<List<SnapFormUserEntry>> FindAlternativeInputs(SnapFormUserPresets preset)
        {
            var filter = Builders<SnapFormUserEntry>.Filter.And(
                Builders<SnapFormUserEntry>.Filter.Eq(x => x.UserId, preset.UserId),
                Builders<SnapFormUserEntry>.Filter
                    .ElemMatch(x => x.AlternativeEntries, x => x.PresetId == preset.Id));

            var res = await _userFields.FindAsync(filter);

            return await res.ToListAsync();
        }
    }
}
