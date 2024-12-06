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


    }       
}