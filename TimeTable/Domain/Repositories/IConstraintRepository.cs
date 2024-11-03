using Domain.Entities;

namespace Domain.Repositories
{
    public interface IConstraintRepository
    {
        Task<IEnumerable<Constraint>> GetAllAsync();
        Task<Constraint> GetByIdAsync(Guid id);
        Task<Guid> AddAsync(Constraint constraint);
        Task UpdateAsync(Constraint constraint);
    }
}