using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class GroupRepository(ApplicationDbContext context) : IGroupRepository
    {

        public async Task<Result<IEnumerable<Group>>> GetAllAsync(string userEmail)
        {
            try
            {
                // Query groups where UserEmail matches the provided email
                var groups = await context.Groups
                    .Where(g => g.UserEmail == userEmail)
                    .ToListAsync();

                return Result<IEnumerable<Group>>.Success(groups);
            }
            catch (Exception e)
            {
                return Result<IEnumerable<Group>>.Failure(e.Message);
            }
        }

        
        public async Task<Result<Group>> GetByIdAsync(Guid id)
        {
            try
            {
                var group = await context.Groups.FindAsync(id);
                return group == null
                    ? Result<Group>.Failure($"Group not found.")
                    : Result<Group>.Success(group);
            }
            catch (Exception e)
            {
                return Result<Group>.Failure(e.Message);
            }
        }
        
        public async Task<Result<Guid>> AddAsync(Group group)
        {
            try
            {
                await context.Groups.AddAsync(group);
                await context.SaveChangesAsync();
                return Result<Guid>.Success(group.Id);
            }
            catch (Exception e)
            {
                return Result<Guid>.Failure(e.Message);
            }
        }
        
        public async Task<Result<Unit>> UpdateAsync(Group group)
        {
            try
            {
                context.Entry(group).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception e)
            {
                return Result<Unit>.Failure(e.Message);
            }
        }
        
        public async Task<Result<Unit>> DeleteAsync(Guid id)
        {
            try
            {
                var group = await context.Groups.FindAsync(id);
                context.Groups.Remove(group);
                await context.SaveChangesAsync();
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception e)
            {
                return Result<Unit>.Failure(e.Message);
            }
        }


    }       
}