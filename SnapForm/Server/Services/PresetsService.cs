using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using SnapForm.Shared;

namespace SnapForm.Server.Services
{
    public class PresetsService
    {
        private readonly IServerOptions _config;
        private readonly InputService _inputService;
        private readonly IMongoCollection<SnapFormUserPresets> _presets;

        public PresetsService(IServerOptions config, InputService inputService)
        {
            _config = config;
            _inputService = inputService;
            var client = new MongoClient(_config.MongoConnectionString);
            var database = client.GetDatabase(_config.DatabaseName);
            _presets = database.GetCollection<SnapFormUserPresets>(CollectionNames.UserPresets);
        }

        public async Task<ServiceResponse<string>> Save(SnapFormUserPresets entry)
        {
            if (entry.Id == null)
            {
                var exists = await PresetExists(entry.Name);

                if (exists) return new ServiceResponse<string>()
                {
                    Success = false,
                    Message = $"Preset: {entry.Name} already exists"
                };

                await _presets.InsertOneAsync(entry);

                return new ServiceResponse<string>()
                {
                    Data = entry.Id,
                    Success = true,
                    Message = "Saved"
                };
            }

            var filter = Builders<SnapFormUserPresets>.Filter.Eq(u => u.Id, entry.Id);
            var update = Builders<SnapFormUserPresets>.Update
                .Set(f => f.Name, entry.Name)
                .Set(f => f.Description, entry.Description);
            var res = await _presets.UpdateOneAsync(filter, update, new UpdateOptions() { IsUpsert = true });

            return new ServiceResponse<string>()
            {
                Data = entry.Id,
                Success = true,
                Message = $"{res.ModifiedCount} Modified"
            };
        }

        public async Task<ServiceResponse<string>> Remove(SnapFormUserPresets entry)
        {
            var res = await _presets.DeleteOneAsync(x => x.Id == entry.Id);
            if (res != null && res.DeletedCount > 0)
            {
                var entryList = await _inputService.FindAlternativeInputs(entry);
                foreach (var e in entryList)
                {
                    e.AlternativeEntries.RemoveAll(x => x.PresetId == entry.Id);
                }

                var savedEntriesResponse = await _inputService.SaveAppUserInputs(entryList);

                return savedEntriesResponse;
            }

            return new ServiceResponse<string>()
            {
                Success = false,
                Message = "Could not delete preset",
                Data = "Nope!"
            };
        }

        public async Task<ServiceResponse<List<SnapFormUserPresets>>> GetAllByUserId(string uid)
        {
            var resA = await _presets.FindAsync(x => x.UserId == uid);
            var resB = await resA.ToListAsync();
            return new ServiceResponse<List<SnapFormUserPresets>>()
            {
                Data = resB,
                Success = true
            };
        }

        public async Task<bool> PresetExists(string name)
        {
            var resA = await _presets.CountDocumentsAsync(x => x.Name == name);
            return resA != 0;
        }
    }
}
