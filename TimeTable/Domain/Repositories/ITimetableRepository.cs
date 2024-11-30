using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface ITimetableRepository
    {
        Task<Result<Guid>> AddAsync(Timetable timetable);
        Task<Result<IEnumerable<Timetable>>> GetAllAsync();
        Task<Result<Timetable>> GetByIdAsync(Guid id);
        Task<Result<Guid>> UpdateAsync(Timetable timetable);
        Task<Result<Guid>> DeleteAsync(Guid id);
    }
}