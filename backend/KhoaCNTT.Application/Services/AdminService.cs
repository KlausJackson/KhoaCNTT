
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
            // check username không dược để trống và có độ dài từ 3-20 ký tự, chỉ chứa chữ, số, dấu châm và dấu gạch dưới
            if (string.IsNullOrWhiteSpace(request.Username) ||
                !Regex.IsMatch(request.Username, @"^[a-zA-Z0-9._]{3,20}$"))
            {
                throw new BusinessRuleException(
                    "Tên đăng nhập phải từ 3–20 ký tự và chỉ chứa chữ, số, dấu chấm hoặc dấu gạch dưới."
                );
            }

            // Check trùng Username
            var existAdmin = await _repo.GetByUsernameAsync(request.Username);
            if (existAdmin != null)
                throw new BusinessRuleException("Tên đăng nhập đã tồn tại.");

            // Check level
            if (request.Level != 2 && request.Level != 3)
                throw new BusinessRuleException("Cấp độ quản trị viên phải là 2 hoặc 3.");

            // Check email
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                try
                {
                    var email = new MailAddress(request.Email);
                }
                catch
                {
                    throw new BusinessRuleException("Email không hợp lệ.");
                }
            } else
            {
                throw new BusinessRuleException("Email không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            {
                throw new BusinessRuleException("Mật khẩu phải có ít nhất 6 ký tự.");
            }

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
                IsActive = true // Mặc định kích hoạt
            };

            await _repo.AddAsync(newAdmin);
        }

        public async Task UpdateAdminAsync(int id, UpdateAdminRequest request)
        {
            var admin = await _repo.GetByIdAsync(id);
            if (admin == null) throw new NotFoundException("Admin", id);

            if (admin.Level == 1)
            {
                throw new BusinessRuleException("Không thể chỉnh sửa thông tin của Super Admin.");
            }

            if (request.Level.HasValue && request.Level != 2 && request.Level != 3)
            {
                throw new BusinessRuleException("Cấp độ quản trị viên phải là 2 hoặc 3.");
            }

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                try
                {
                    var email = new MailAddress(request.Email);
                }
                catch
                {
                    throw new BusinessRuleException("Email không hợp lệ.");
                }
            }

            // Cập nhật thông tin (Không cho đổi Username)
            admin.FullName = request.FullName ?? admin.FullName;
            admin.Email = request.Email ?? admin.Email;
            admin.PasswordHash = request.PasswordHash != null ? _hasher.Hash(request.PasswordHash) : admin.PasswordHash;
            admin.Level = request.Level ?? admin.Level;
            admin.IsActive = request.IsActive ?? admin.IsActive;

            await _repo.UpdateAsync(admin);
        }

        public async Task DeleteAdminAsync(string currentUsername, int id)
        {
            var admin = await _repo.GetByIdAsync(id);
            if (admin == null) throw new NotFoundException("Admin", id);

            // Không cho tự xóa chính mình
            if (currentUsername == admin.Username)
            {
                throw new BusinessRuleException("Không thể xóa chính mình.");
            }

            await _repo.DeleteAsync(admin);
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