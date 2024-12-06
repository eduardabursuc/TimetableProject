using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface IRoomRepository
    {
        Task<Result<IEnumerable<Room>>> GetAllAsync();
        Task<Result<Room>> GetByNameAsync(string name);
    }
}