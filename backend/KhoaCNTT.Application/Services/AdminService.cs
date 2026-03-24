
using AutoMapper;
using KhoaCNTT.Application.Common.Exceptions;
using KhoaCNTT.Application.Common.Utils; // PasswordHasher
using KhoaCNTT.Application.DTOs.Admin;
using KhoaCNTT.Application.Interfaces.Repositories;
using KhoaCNTT.Application.Interfaces.Services;
using KhoaCNTT.Domain.Entities;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace KhoaCNTT.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _repo;
        private readonly IMapper _mapper;
        private readonly PasswordHasher _hasher;

        public AdminService(IAdminRepository repo, IMapper mapper, PasswordHasher hasher)
        {
            _repo = repo;
            _mapper = mapper;
            _hasher = hasher;
        }

        public async Task<List<AdminResponse>> GetAllAdminsAsync()
        {
            var admins = await _repo.GetAllAsync();
            return _mapper.Map<List<AdminResponse>>(admins);
        }

        public async Task CreateAdminAsync(CreateAdminRequest request)
        {
            // 1. Check fields
            // check username
            if (string.IsNullOrWhiteSpace(request.Username) ||
                !Regex.IsMatch(request.Username, @"^[a-zA-Z0-9._]{3,20}$"))
            {
                throw new BusinessRuleException(
                    "Tên đăng nhập phải từ 3–20 ký tự và chỉ chứa chữ, số, dấu chấm hoặc dấu gạch dưới."
                );
            }
            // Check các trường khác
            CheckFields(request.Password, request.FullName, request.Email, request.Level, true);

            // Check trùng Username
            var existUsername = await _repo.GetByUsernameAsync(request.Username);
            if (existUsername != null)
                throw new BusinessRuleException("Tên đăng nhập này đã tồn tại.");

            var existEmail = await _repo.GetByEmailAsync(request.Email);
            if (existEmail != null)
                throw new BusinessRuleException("Email này đã tồn tại.");

            // 2. Hash Password
            var passwordHash = _hasher.Hash(request.Password);

            // 3. Tạo Entity
            var newAdmin = new Admin
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                FullName = request.FullName,
                Email = request.Email,
                Level = request.Level,
                IsActive = request.IsActive
            };

            await _repo.AddAsync(newAdmin);
        }

        public async Task UpdateAdminAsync(int id, UpdateAdminRequest request)
        {
            var admin = await _repo.GetByIdAsync(id) ?? throw new NotFoundException("Admin", id);
            CheckFields(request.Password, request.FullName, request.Email, request.Level, false);

            var existEmail = await _repo.GetByEmailAsync(request.Email);
            if (existEmail != null && existEmail.Id != id)
                throw new BusinessRuleException("Email này đã tồn tại.");


            if (admin.Level == 1)
            {
                throw new BusinessRuleException("Không thể chỉnh sửa thông tin của Super Admin.");
            }

            // Cập nhật thông tin (Không cho đổi Username)
            admin.FullName = request.FullName ?? admin.FullName;
            admin.Email = request.Email ?? admin.Email;
            admin.PasswordHash = request.Password != null ? _hasher.Hash(request.Password) : admin.PasswordHash;
            admin.Level = request.Level ?? admin.Level;
            admin.IsActive = request.IsActive ?? admin.IsActive;

            await _repo.UpdateAsync(admin);
        }

        public async Task DeleteAdminAsync(string currentUsername, int id)
        {
            var admin = await _repo.GetByIdAsync(id) ?? throw new NotFoundException("Admin", id);

            // Không cho tự xóa chính mình
            if (currentUsername == admin.Username)
            {
                throw new BusinessRuleException("Không thể xóa chính mình.");
            }

            await _repo.DeleteAsync(admin);
        }

        private static bool CheckFields(string? password, string? fullName, string? email, int? level, bool isCreate)
        {
            if (level != null && (level < 1 || level > 3))
            {
                throw new BusinessRuleException("Cấp độ quản trị viên không hợp lệ. Phải là 1, 2 hoặc 3.");
            }
            if (string.IsNullOrWhiteSpace(fullName) && string.IsNullOrWhiteSpace(fullName))
            {
                throw new BusinessRuleException("Tên người dùng không được để trống.");
            }
            if (!string.IsNullOrWhiteSpace(email))
            {
                try
                {
                    _ = new MailAddress(email);
                }
                catch
                {
                    throw new BusinessRuleException("Email không hợp lệ.");
                }
            } else
            {
                if (isCreate)
                {
                    throw new BusinessRuleException("Email không được để trống.");
                }
            }

            if ((string.IsNullOrWhiteSpace(password) || password.Length < 6) && isCreate)
            {
                throw new BusinessRuleException("Mật khẩu phải có ít nhất 6 ký tự.");
            }
            return true;
        }

        //public async Task ChangePasswordAsync(int id, string newPassword)
        //{
        //    var admin = await _repo.GetByIdAsync(id);
        //    if (admin == null) throw new NotFoundException("Admin", id);

        //    admin.PasswordHash = _hasher.Hash(newPassword);
        //    await _repo.UpdateAsync(admin);
        //}
    }
}