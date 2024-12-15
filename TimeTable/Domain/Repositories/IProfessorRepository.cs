using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Domain.Repositories
{
    public interface IProfessorRepository
    {
        Task<Result<IEnumerable<Professor>>> GetAllAsync(string userEmail);
        Task<Result<Professor>> GetByIdAsync(Guid id);
        Task<Result<Guid>> AddAsync(Professor professor);
        Task<Result<Guid>> UpdateAsync(Professor professor);
        Task<Result<Unit>> DeleteAsync(Guid id);
        Task<Result<Unit>> AddTimetableAsync(Guid id, Guid timetableId);
    }
}