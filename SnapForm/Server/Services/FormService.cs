using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using QRCoder;
using SnapForm.Shared;

namespace SnapForm.Server.Services
{
    public class FormService
    {
        private readonly IServerOptions _config;
        private readonly IMongoCollection<SnapFormField> _fields;
        private readonly IMongoCollection<SnapFormUserEntry> _userFields;
        private readonly IMongoCollection<EnterpriseForm> _form;
        private readonly IMongoCollection<FormSubmission> _formSubmission;

        public FormService(IServerOptions config)
        {
            _config = config;
            var client = new MongoClient(_config.MongoConnectionString);
            var database = client.GetDatabase(_config.DatabaseName);
            _fields = database.GetCollection<SnapFormField>(CollectionNames.Fields);
            _userFields = database.GetCollection<SnapFormUserEntry>(CollectionNames.UserFields);
            _form = database.GetCollection<EnterpriseForm>(CollectionNames.Forms);
            _formSubmission = database.GetCollection<FormSubmission>(CollectionNames.FormSubmissions);
        }

        public async Task<ServiceResponse<string>> SaveEnterpriseForm(EnterpriseForm entry)
        {
            if (entry.ID == null)
            {
                await _form.InsertOneAsync(entry);

                return new ServiceResponse<string>()
                {
                    Success = true,
                    Message = "Inserted Successfully"
                };
            }

            var filter = Builders<EnterpriseForm>.Filter.Eq(u => u.ID, entry.ID);
            var update = Builders<EnterpriseForm>.Update
                .Set(f => f.EnterpriseId, entry.EnterpriseId)
                .Set(f => f.Name, entry.Name)
                .Set(f => f.Description, entry.Description)
                .Set(f => f.FormInputIds, entry.FormInputIds);

            var res = await _form.UpdateOneAsync(filter, update, new UpdateOptions() { IsUpsert = true });

            return new ServiceResponse<string>()
            {
                Success = true,
                Message = $"{res.ModifiedCount} Modified"
            };
        }

        public async Task<UpdateResult> IncreaseFormSubmission(EnterpriseForm entry)
        {
            entry.SubmissionCount++;
            var filter = Builders<EnterpriseForm>.Filter.Eq(u => u.ID, entry.ID);
            var update = Builders<EnterpriseForm>.Update
                .Set(f => f.SubmissionCount, entry.SubmissionCount);

            var res = await _form.UpdateOneAsync(filter, update, new UpdateOptions() { IsUpsert = true });

            return res;
        }

        public async Task<ServiceResponse<EnterpriseForm>> GetEnterpriseForm(string formId)
        {
            var res = (await _form.FindAsync(x => x.ID == formId)).FirstOrDefault();
            return new ServiceResponse<EnterpriseForm>()
            {
                Data = res,
                Success = true,
                Message = "Found Form",
            };
        }

        public async Task<ServiceResponse<List<EnterpriseForm>>> GetForms(string enterpriseId)
        {
            var res = await (await _form.FindAsync(x => x.EnterpriseId == enterpriseId)).ToListAsync();
            return new ServiceResponse<List<EnterpriseForm>>()
            {
                Data = res,
                Success = true,
                Message = $"Found {res.Count} Forms",
            };
        }

        public async Task<ServiceResponse<ServiceResponseCode>> ScanForm(string formId, SnapFormUser user)
        {
            // Get the form to be filled out
            EnterpriseForm form = (await _form.FindAsync(x => x.ID == formId)).FirstOrDefault();

            // Check if the form exists
            if (form == null)
            {
                return new ServiceResponse<ServiceResponseCode>()
                {
                    Data = ServiceResponseCode.Exists,
                    Success = false,
                    Message = $"Form does not exist",
                };
            }

            // Check if it already exists
            if (await FormSubmissionExists(user, form))
            {
                return new ServiceResponse<ServiceResponseCode>()
                {
                    Data = ServiceResponseCode.Exists,
                    Success = false,
                    Message = $"Form already submitted before",
                };
            }

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

            if (requiredMissing.Count != 0)
            {
                return new ServiceResponse<ServiceResponseCode>()
                {
                    Data = ServiceResponseCode.MissingInputs,
                    Success = false,
                    Message = $"Please fill out the required missing inputs",
                };
            }

            // Start saving the form submission
            await SubmitForm(user, form, fields, entries);

            await IncreaseFormSubmission(form);

            return new ServiceResponse<ServiceResponseCode>()
            {
                Success = true,
                Message = $"Form Submission successful",
            };
        }

        public async Task SubmitForm(SnapFormUser user, EnterpriseForm form, List<SnapFormField> fields, List<SnapFormUserEntry> entries)
        {
            FormSubmission submission = new FormSubmission
            {
                User = new UserBase
                {
                    DateCreated = user.DateCreated,
                    Id = user.Id,
                    Email = user.Email
                },
                Form = form,
                Fields = fields,
                Entries = entries
            };

            await _formSubmission.InsertOneAsync(submission);
        }

        public async Task<bool> FormSubmissionExists(SnapFormUser user, EnterpriseForm form)
        {
            var res = await _formSubmission.CountDocumentsAsync(u => u.User.Id == user.Id && u.Form.ID == form.ID);
            return res > 0;
        }

        public async Task<long> FormSubmissionCount(string formId)
        {
            var res = await _formSubmission.CountDocumentsAsync(f => f.Form.ID == formId);
            return res;
        }

        public async Task<List<FormSubmission>> GetFormSubmissions(string formId)
        {
            var res = await (await _formSubmission.FindAsync(f => f.Form.ID == formId)).ToListAsync();
            return res;
        }

        public async Task<List<FormSubmission>> GetUserFormSubmissions(string userId)
        {
            var res = await (await _formSubmission.FindAsync(f => f.User.Id == userId)).ToListAsync();
            return res;
        }

        public async Task<FormSubmission> GetUserFormSubmissions(string userId, string submissionId)
        {
            var res = (await _formSubmission
                .FindAsync(f => f.User.Id == userId && f.Id == submissionId)).FirstOrDefault();
            return res;
        }

        public async Task<ServiceResponse<string>> GenerateQrCode(string input)
        {
            await using MemoryStream ms = new MemoryStream();
            QRCodeGenerator qrGen = new QRCodeGenerator();
            QRCodeData qrData = qrGen.CreateQrCode(input, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrData);

            using Bitmap bitmap = qrCode.GetGraphic(20);
            bitmap.Save(ms, ImageFormat.Png);
            return new ServiceResponse<string>
            {
                Data = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray()),
                Message = "QR Generated",
                Success = true
            };
        }
    }
}