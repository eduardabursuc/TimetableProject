using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface IGroupRepository
    {
        Task<Result<IEnumerable<Group>>> GetAllAsync();
    }
}