using AutoMapper;
using KhoaCNTT.Application.Common.Exceptions;
using KhoaCNTT.Application.DTOs.Lecturer;
using KhoaCNTT.Application.Interfaces.Repositories;
using KhoaCNTT.Application.Interfaces.Services;
using KhoaCNTT.Domain.Entities;
using KhoaCNTT.Domain.Enums;

namespace KhoaCNTT.Application.Services
{
    public class LecturerService : ILecturerService
    {
        private readonly ILecturerRepository _lecturerRepo;
        private readonly ISubjectRepository _subjectRepo;
        private readonly IMapper _mapper;

        public LecturerService(ILecturerRepository lecturerRepo, ISubjectRepository subjectRepo, IMapper mapper)
        {
            _lecturerRepo = lecturerRepo;
            _subjectRepo = subjectRepo;
            _mapper = mapper;
        }

        public async Task<PagedLecturerResult> GetListAsync(LecturerSearchParams searchParams)
        {
            string? name = string.IsNullOrWhiteSpace(searchParams.Name) ? null : searchParams.Name.Trim();
            string? subjectCodeOrName = string.IsNullOrWhiteSpace(searchParams.SubjectCodeOrName) ? null : searchParams.SubjectCodeOrName.Trim();
            string? position = string.IsNullOrWhiteSpace(searchParams.Position) ? null : searchParams.Position.Trim();

            int page = searchParams.Page < 1 ? 1 : searchParams.Page;
            int pageSize = searchParams.PageSize < 1 ? 10 : Math.Min(searchParams.PageSize, 100);

            var (list, totalCount) = await _lecturerRepo.GetAllAsync(name, searchParams.Degree, position, subjectCodeOrName, page, pageSize);
            var items = _mapper.Map<List<LecturerResponse>>(list);

            return new PagedLecturerResult
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<LecturerResponse?> GetByIdAsync(int id)
        {
            var lecturer = await _lecturerRepo.GetByIdAsync(id);
            return lecturer == null ? null : _mapper.Map<LecturerResponse>(lecturer);
        }

        public async Task CreateLecturerAsync(CreateLecturerRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new BusinessRuleException("Email không được để trống.");
            if (string.IsNullOrWhiteSpace(request.FullName))
                throw new BusinessRuleException("Họ tên không được để trống.");

            var lecturer = new Lecturer
            {
                FullName = request.FullName.Trim(),
                ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? "" : request.ImageUrl.Trim(),
                Degree = request.Degree,
                Position = (request.Position ?? "").Trim(),
                Birthdate = request.Birthdate,
                Email = request.Email.Trim(),
                PhoneNumber = (request.PhoneNumber ?? "").Trim()
            };

            await SetLecturerSubjectsAsync(lecturer, request.SubjectCodes ?? new List<string>());
            await _lecturerRepo.AddAsync(lecturer);
        }

        public async Task UpdateLecturerAsync(int id, UpdateLecturerRequest request)
        {
            var lecturer = await _lecturerRepo.GetByIdAsync(id);
            if (lecturer == null) throw new NotFoundException("Giảng viên", id);
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new BusinessRuleException("Email không được để trống.");
            if (string.IsNullOrWhiteSpace(request.FullName))
                throw new BusinessRuleException("Họ tên không được để trống.");

            lecturer.FullName = request.FullName.Trim();
            lecturer.ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? "" : request.ImageUrl.Trim();
            lecturer.Degree = request.Degree;
            lecturer.Position = (request.Position ?? "").Trim();
            lecturer.Birthdate = request.Birthdate;
            lecturer.Email = request.Email.Trim();
            lecturer.PhoneNumber = (request.PhoneNumber ?? "").Trim();
            lecturer.UpdatedAt = DateTime.UtcNow;

            await SetLecturerSubjectsAsync(lecturer, request.SubjectCodes ?? new List<string>());
            await _lecturerRepo.UpdateAsync(lecturer);
        }

        public async Task DeleteLecturerAsync(int id)
        {
            var lecturer = await _lecturerRepo.GetByIdAsync(id);
            if (lecturer == null) throw new NotFoundException("Giảng viên", id);
            await _lecturerRepo.DeleteAsync(lecturer);
        }

        private async Task SetLecturerSubjectsAsync(Lecturer lecturer, List<string> subjectCodes)
        {
            lecturer.LecturerSubjects.Clear();
            if (subjectCodes == null || subjectCodes.Count == 0) return;

            foreach (var code in subjectCodes.Distinct())
            {
                if (string.IsNullOrWhiteSpace(code)) continue;
                var subject = await _subjectRepo.GetByCodeAsync(code.Trim());
                if (subject == null) continue;

                lecturer.LecturerSubjects.Add(new LecturerSubject
                {
                    LecturerId = lecturer.Id,
                    SubjectCode = subject.SubjectCode,
                    Subject = subject
                });
            }
        }
    }
}
