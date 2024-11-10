using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface IConstraintRepository
    {
        Task<Result<IEnumerable<Constraint>>> GetAllAsync();
        Task<Result<Constraint>> GetByIdAsync(Guid id);
        Task<Result<Guid>> AddAsync(Constraint constraint);
        Task<Result<Guid>> UpdateAsync(Constraint constraint);
        Task<Result<Guid>> DeleteAsync(Guid id);
    }
}