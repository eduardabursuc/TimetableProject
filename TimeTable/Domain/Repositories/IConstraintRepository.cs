using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Domain.Repositories
{
    public interface IConstraintRepository
    {
        Task<Result<IEnumerable<Constraint>>> GetConstraintsByProfessorId(Guid professorId);
        Task<Result<IEnumerable<Constraint>>> GetAllAsync(Guid timetableId);
        Task<Result<Constraint>> GetByIdAsync(Guid id);
        Task<Result<Guid>> AddAsync(Constraint constraint);
        Task<Result<Unit>> DeleteAsync(Guid id);
        Task<Result<IEnumerable<Constraint>>> GetAllForProfessorAsync(string professorEmail, Guid timetableId);
    }
}