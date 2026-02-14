
using KhoaCNTT.Domain.Entities;
using KhoaCNTT.Infrastructure.Persistence;
using KhoaCNTT.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;


public class SubjectRepository : ISubjectRepository
{
    private readonly AppDbContext _context;
    public SubjectRepository(AppDbContext context) { _context = context; }

    public async Task<List<Subject>> GetAllAsync()
    {
        return await _context.Subjects.AsNoTracking().ToListAsync();
    }

    public async Task<Subject?> GetByCodeAsync(string code)
    {
        return await _context.Subjects.AsNoTracking().FirstOrDefaultAsync(s => s.SubjectCode == code);
    }

}