using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Domain.Repositories
{
    public interface IGroupRepository
    {
        Task<Result<IEnumerable<Group>>> GetAllAsync(string userEmail);
        Task<Result<Group>> GetByIdAsync(Guid id);
        Task<Result<Guid>> AddAsync(Group group);
        Task<Result<Guid>> UpdateAsync(Group group);
        Task<Result<Unit>> DeleteAsync(Guid id);
    }
}