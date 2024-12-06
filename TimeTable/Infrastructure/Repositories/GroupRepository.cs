using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class GroupRepository(ApplicationDbContext context) : IGroupRepository
    {

        public async Task<Result<IEnumerable<Group>>> GetAllAsync()
        {
            try
            {
                var groups = await context.Groups.ToListAsync();
                return Result<IEnumerable<Group>>.Success(groups);
            }
            catch (Exception e)
            {
                return Result<IEnumerable<Group>>.Failure(e.Message);
            }
        }
        
        public async Task<Result<Group>> GetByNameAsync(string name)
        {
            try
            {
                var group = await context.Groups.FirstOrDefaultAsync(r => r.Name == name);
                return group == null
                    ? Result<Group>.Failure($"Group with name {name} not found.")
                    : Result<Group>.Success(group);
            }
            catch (Exception e)
            {
                return Result<Group>.Failure(e.Message);
            }
        }


    }       
}