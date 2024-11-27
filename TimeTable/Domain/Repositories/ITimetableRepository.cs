using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface ITimetableRepository
    {
        Task<Result<IEnumerable<Timetable>>> GetAllAsync();
    }
}