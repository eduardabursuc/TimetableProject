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
        Task<Result<Timetable>> GetByGroupAsync(Guid id, Guid groupId);
        Task<Result<Timetable>> GetByProfessorAsync(Guid id, Guid professorId);
        Task<Result<Timetable>> GetByRoomAsync(Guid id, Guid roomId);
        Task<Result<Unit>> UpdateAsync(Timetable? timetable);
        Task<Result<Unit>> DeleteAsync(Guid id);
    }
}