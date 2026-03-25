using KhoaCNTT.Domain.Entities;
using KhoaCNTT.Domain.Enums;
using KhoaCNTT.Application.Interfaces.Repositories;
using KhoaCNTT.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KhoaCNTT.Infrastructure.Repositories
{
    public class LecturerRepository : ILecturerRepository
    {
        private readonly AppDbContext _context;

        public LecturerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Lecturer?> GetByIdAsync(int id)
        {
            return await _context.Lecturers
                .Include(l => l.LecturerSubjects)
                .ThenInclude(ls => ls.Subject)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<(List<Lecturer> Items, int TotalCount)> GetAllAsync(string? name, DegreeType? degree, string? position, string? subjectCodeOrName, int page, int pageSize)
        {
            var query = _context.Lecturers
                .Include(l => l.LecturerSubjects)
                .ThenInclude(ls => ls.Subject)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(name))
            {
                var term = name.Trim();
                query = query.Where(l => l.FullName.Contains(term));
            }
            if (degree.HasValue)
                query = query.Where(l => l.Degree == degree.Value);
            if (!string.IsNullOrWhiteSpace(position))
            {
                var term = position.Trim();
                query = query.Where(l => l.Position.Contains(term));
            }
            if (!string.IsNullOrWhiteSpace(subjectCodeOrName))
            {
                var term = subjectCodeOrName.Trim();
                query = query.Where(l => l.LecturerSubjects.Any(ls =>
                    ls.Subject.SubjectCode.Contains(term) || ls.Subject.SubjectName.Contains(term)));
            }

            int totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(l => l.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (items, totalCount);
        }

        public async Task AddAsync(Lecturer lecturer)
        {
            await _context.Lecturers.AddAsync(lecturer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Lecturer lecturer)
        {
            var oldSubjects = await _context.LecturerSubjects.Where(ls => ls.LecturerId == lecturer.Id).ToListAsync();
            _context.LecturerSubjects.RemoveRange(oldSubjects);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Lecturer lecturer)
        {
            _context.Lecturers.Remove(lecturer);
            await _context.SaveChangesAsync();
        }
    }
}
