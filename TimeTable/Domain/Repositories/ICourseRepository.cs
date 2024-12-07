using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Domain.Repositories
{
    public interface ICourseRepository
    {
        Task<Result<IEnumerable<Course>>> GetAllAsync(string userEmail);
        Task<Result<Course>> GetByIdAsync(Guid id);
        Task<Result<Guid>> AddAsync(Course course);
        Task<Result<Guid>> UpdateAsync(Course course);
        Task<Result<Unit>> DeleteAsync(Guid id);
    }
}