using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Domain.Repositories
{
    public interface IRoomRepository
    {
        Task<Result<IEnumerable<Room>>> GetAllAsync(string userEmail);
        Task<Result<Room>> GetByIdAsync(Guid id);
        Task<Result<Guid>> AddAsync(Room room);
        Task<Result<Unit>> UpdateAsync(Room room);
        Task<Result<Unit>> DeleteAsync(Guid id);
    }
}