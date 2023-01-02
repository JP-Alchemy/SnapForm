using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using SnapForm.Shared;

namespace SnapForm.Server.Services
{
    public class FieldsService
    {
        private readonly IServerOptions _config;
        private readonly IMongoCollection<SnapFormField> _fields;
        private readonly IMongoCollection<Occupation> _OccupationData;

        public FieldsService(IServerOptions config)
        {
            _config = config;
            var client = new MongoClient(_config.MongoConnectionString);
            var database = client.GetDatabase(_config.DatabaseName);
            _fields = database.GetCollection<SnapFormField>(CollectionNames.Fields);
            _OccupationData = database.GetCollection<Occupation>(CollectionNames.Occupation);
        }
        
        public async Task<ServiceResponse<List<Occupation>>> GetAllOccupations()
        {
            var res = await (await _OccupationData.FindAsync(_ => true)).ToListAsync();
            return new ServiceResponse<List<Occupation>>()
            {
                Data = res,
                Success = true,
                Message = $"Found {res.Count} Forms",
            };
        }

        public async Task SaveFields(IEnumerable<SnapFormField> entries)
        {
            await _fields.InsertManyAsync(entries);
        }

        public async Task<ServiceResponse<string>> SaveField(SnapFormField entry)
        {
            if (entry.Id == null)
            {
                await _fields.InsertOneAsync(entry);

                return new ServiceResponse<string>()
                {
                    Success = true,
                    Message = "Inserted Successfully"
                };
            }

            var filter = Builders<SnapFormField>.Filter.Eq(u => u.Id, entry.Id);
            var update = Builders<SnapFormField>.Update
                .Set(f => f.Title, entry.Title)
                .Set(f => f.Description, entry.Description)
                .Set(f => f.InputType, entry.InputType)
                .Set(f => f.IsRequired, entry.IsRequired)
                .Set(f => f.SelectMultiple, entry.SelectMultiple)
                .Set(f => f.UseDiagram, entry.UseDiagram)
                .Set(f => f.UseDateRange, entry.UseDateRange)
                .Set(f => f.DiagramImage, entry.DiagramImage)
                .Set(f => f.Options, entry.Options);

            var res = await _fields.UpdateOneAsync(filter, update, new UpdateOptions() { IsUpsert = true });

            return new ServiceResponse<string>()
            {
                Success = true,
                Message = $"{res.ModifiedCount} Modified"
            };
        }

        public async Task<ServiceResponse<List<SnapFormField>>> GetFields()
        {
            var resA = await _fields.FindAsync(_ => true);
            var resB = await resA.ToListAsync();

            return new ServiceResponse<List<SnapFormField>>()
            {
                Data = resB,
                Success = true
            };
        }

        public async Task<ServiceResponse<SnapFormField>> GetField(string ids)
        {
            var formFieldsFilter = Builders<SnapFormField>.Filter.Eq(f => f.Id, ids);
            SnapFormField fields = (await _fields.FindAsync(formFieldsFilter)).FirstOrDefault();
            return new ServiceResponse<SnapFormField>()
            {
                Data = fields,
                Success = true
            };
        }

        public async Task<ServiceResponse<List<SnapFormField>>> GetFields(List<string> ids)
        {
            var formFieldsFilter = Builders<SnapFormField>.Filter.In(f => f.Id, ids);
            List<SnapFormField> fields = await (await _fields.FindAsync(formFieldsFilter)).ToListAsync();
            return new ServiceResponse<List<SnapFormField>>()
            {
                Data = fields,
                Success = true
            };
        }
    }
}
