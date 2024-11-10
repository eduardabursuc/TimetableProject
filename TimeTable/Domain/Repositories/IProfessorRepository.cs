using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface IProfessorRepository
    {
        Task<Result<IEnumerable<Professor>>> GetAllAsync();
        Task<Result<Professor>> GetByIdAsync(Guid id);
        Task<Result<Guid>> AddAsync(Professor professor);
        Task<Result<Guid>> UpdateAsync(Professor professor);
        Task<Result<Guid>> DeleteAsync(Guid id);
    }
}