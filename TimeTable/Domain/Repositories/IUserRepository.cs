using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface IUserRepository
    {
        Task<Result<string>> Register(User user, CancellationToken cancellationToken);
        Task<Result<string>> Login(User user);
        Task<Result<User>> GetByEmailAsync(string email);
        Task<Result<string>> GetToken(string email);
    }
}