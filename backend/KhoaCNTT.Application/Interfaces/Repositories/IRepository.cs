using System.Linq.Expressions;

namespace KhoaCNTT.Application.Interfaces.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id); // file, request, reousrce
        Task<List<T>> GetAllAsync(); // 

        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate); // request, 

        Task AddAsync(T entity, CancellationToken ct = default); // file, request, resource, approval
        Task UpdateAsync(T entity, CancellationToken ct = default); // file, request, 
        Task DeleteAsync(T entity, CancellationToken ct = default); // file, 
        // file: search, 
        // request: GetPendingRequestsWithDetailsAsync
        // 
    }
}