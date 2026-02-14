
using KhoaCNTT.Application.Interfaces.Repositories;
using KhoaCNTT.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using KhoaCNTT.Application.Common.Utils;

namespace KhoaCNTT.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly ISchoolApiService _schoolApi;
        private readonly IJwtTokenGenerator _jwtGenerator;
        private readonly IConfiguration _config;
        private readonly IAdminRepository _adminRepo;
        private readonly PasswordHasher _hasher;

        public AuthService(ISchoolApiService schoolApi, IJwtTokenGenerator jwtGenerator, IConfiguration config, IAdminRepository adminRepo, PasswordHasher hasher)
        {
            _schoolApi = schoolApi;
            _jwtGenerator = jwtGenerator;
            _config = config;
            _adminRepo = adminRepo;
            _hasher = hasher;
        }

        public async Task<string> LoginAdminAsync(string username, string password)
        {

            // ƯU TIÊN: Check trong AppSettings (Test Admin)
            var testAdminUser = _config["TestAdmin:Username"];
            var testAdminPass = _config["TestAdmin:Password"];

            if (!string.IsNullOrEmpty(testAdminUser) && username == testAdminUser && password == testAdminPass)
            {
                // quyền admin level 1
                return _jwtGenerator.GenerateAdminToken(username, 1);
            }

            // Check trong database
            var adminInDb = await _adminRepo.GetByUsernameAsync(username);

            if (adminInDb != null)
            {
                if (!adminInDb.IsActive)
                {
                    throw new Exception("Tài khoản Admin này đã bị vô hiệu hóa.");
                }
                if (!_hasher.Verify(adminInDb.PasswordHash, password))
                {
                    throw new Exception("Mật khẩu Admin không chính xác.");
                }
                // return token with username and level
                return _jwtGenerator.GenerateAdminToken(adminInDb.Username, adminInDb.Level);
            }

            throw new Exception("Thông tin đăng nhập Admin không chính xác.");
        }

        public async Task<string> LoginStudentAsync(string username, string password)
        {
            var token = await _schoolApi.LoginStudentAsync(username, password);
            
            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("Mã sinh viên hoặc mật khẩu không đúng (hoặc hệ thống trường lỗi).");
            }

            return token;
        }
    }
}