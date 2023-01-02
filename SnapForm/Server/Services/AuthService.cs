using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using SnapForm.Shared;

namespace SnapForm.Server.Data
{
    public class AuthService
    {
        private readonly IServerOptions _config;
        private readonly IMongoCollection<UserSecure> _appUser;
        private readonly IMongoCollection<EnterpriseUser> _enterpriseUser;
        private readonly IMongoCollection<Enterprise> _enterprise;

        public AuthService(IServerOptions config)
        {
            _config = config;
            var client = new MongoClient(_config.MongoConnectionString);
            var database = client.GetDatabase(_config.DatabaseName);
            _appUser = database.GetCollection<UserSecure>(CollectionNames.AppUser);
            _enterpriseUser = database.GetCollection<EnterpriseUser>(CollectionNames.EnterpriseUser);
            _enterprise = database.GetCollection<Enterprise>(CollectionNames.Enterprise);
        }

        public async Task<ServiceResponse<string>> AppRegister(SnapFormUser user, string password)
        {
            if (await AppUserExists(user.Email))
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Message = "User already exists"
                };
            }

            CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _appUser.InsertOneAsync(user);

            return new ServiceResponse<string>
            {
                Data = user.Id,
                Message = "Registration Successful!"
            };
        }

        public async Task<ServiceResponse<string>> EnterpriseRegister(EnterpriseUser user, Enterprise enterprise, string password)
        {
            if (await AppUserExists(user.Email))
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Message = "User already exists"
                };
            }

            CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _enterpriseUser.InsertOneAsync(user);
            enterprise.Created_by = user.Id;
            await _enterprise.InsertOneAsync(enterprise);
            user.EnterpriseId = enterprise.ID;

            var filter = Builders<EnterpriseUser>.Filter.Eq(u => u.Id, user.Id);
            var update = Builders<EnterpriseUser>.Update.Set(f => f.EnterpriseId, user.EnterpriseId);
            await _enterpriseUser.UpdateOneAsync(filter, update);

            return new ServiceResponse<string>
            {
                Data = user.Id,
                Message = "Registration Successful!"
            };
        }

        public async Task<ServiceResponse<string>> AppLogin(string email, string password)
        {
            var res = new ServiceResponse<string>();
            var user = (await _appUser.FindAsync(u => u.Email.Equals(email.ToLower()))).FirstOrDefault();

            if (user == null)
            {
                res.Success = false;
                res.Message = "User not found.";
            }
            else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                res.Success = false;
                res.Message = "Incorrect username or password.";
            }
            else
            {
                res.Data = CreateToken(user);
                res.Success = true;
            }

            return res;
        }

        public async Task<ServiceResponse<string>> EnterpriseLogin(string email, string password)
        {
            var res = new ServiceResponse<string>();
            var user = (await _enterpriseUser.FindAsync(u => u.Email.Equals(email.ToLower()))).FirstOrDefault();

            if (user == null)
            {
                res.Success = false;
                res.Message = "User not found.";
            }
            else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                res.Success = false;
                res.Message = "Incorrect username or password.";
            }
            else
            {
                res.Data = CreateToken(user);
            }

            return res;
        }

        public async Task<bool> AppUserExists(string email)
        {
            var res = await _appUser.CountDocumentsAsync(u => u.Email.Equals(email.ToLower()));
            return res > 0;
        }

        private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using var hmac = new HMACSHA512();
            salt = hmac.Key;
            hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPasswordHash(string password, byte[] hash, byte[] salt)
        {
            using var hmac = new HMACSHA512(salt);
            var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            for (int i = 0; i < computeHash.Length; i++)
                if (computeHash[i] != hash[i])
                    return false;
            return true;
        }

        private string CreateToken(UserSecure user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("email", user.Email)
            };

            if (user is EnterpriseUser eUser)
            {
                claims.Add(new Claim(ClaimTypes.Name, eUser.FirstName));
                claims.Add(new Claim(ClaimTypes.Role, eUser.Role.ToString()));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Token));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddDays(15),
                signingCredentials: cred);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}