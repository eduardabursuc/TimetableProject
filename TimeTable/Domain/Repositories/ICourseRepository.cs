using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface ICourseRepository
    {
        Task<Result<IEnumerable<Course>>> GetAllAsync();
        Task<Result<Course>> GetByNameAsync(string courseName);
        Task<Result<string>> AddAsync(Course course);
        Task<Result<string>> UpdateAsync(Course course);
        Task<Result<string>> DeleteAsync(string courseName);
    }
}