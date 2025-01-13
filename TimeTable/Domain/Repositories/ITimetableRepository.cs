using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Domain.Repositories
{
    public interface ITimetableRepository
    {
        Task<Result<Guid>> AddAsync(Timetable timetable);
        Task<Result<IEnumerable<Timetable>>> GetAllAsync(string userEmail);
        Task<Result<Timetable>> GetByIdAsync(Guid id);
        Task<Result<Unit>> UpdateAsync(Timetable? timetable);
        Task<Result<Unit>> DeleteAsync(Guid id);
        Task<Result<IEnumerable<Timetable>>> GetAllForProfessorAsync(string professorEmail);
    }
}