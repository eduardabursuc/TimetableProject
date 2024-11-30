using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Domain.Repositories
{
    public interface ITimetableRepository
    {
        Task<Result<Guid>> AddAsync(Timetable timetable);
        Task<Result<IEnumerable<Timetable>>> GetAllAsync();
        Task<Result<Timetable>> GetByIdAsync(Guid id);
        Task<Result<Timetable>> GetByGroupAsync(Guid id, string groupName);
        Task<Result<Timetable>> GetByProfessorAsync(Guid id, Guid professorId);
        Task<Result<Timetable>> GetByRoomAsync(Guid id, string roomName);
        Task<Result<Guid>> UpdateAsync(Timetable timetable);
        Task<Result<Unit>> DeleteAsync(Guid id);
        
    }
}