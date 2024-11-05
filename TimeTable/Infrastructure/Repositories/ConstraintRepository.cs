using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class ConstraintRepository : IConstraintRepository
    {
        private readonly ApplicationDbContext context;

        public ConstraintRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Result<Guid>> AddAsync(Constraint constraint)
        {
            try
            {
                await context.Constraints.AddAsync(constraint);
                await context.SaveChangesAsync();
                return Result<Guid>.Success(constraint.Id);
            }
            catch (Exception e)
            {
                return Result<Guid>.Failure(e.Message);
            }
        }

        public async Task<Result<IEnumerable<Constraint>>> GetAllAsync()
        {
            try
            {
                var constraints = await context.Constraints.ToListAsync();
                return Result<IEnumerable<Constraint>>.Success(constraints);
            }
            catch (Exception e)
            {
                return Result<IEnumerable<Constraint>>.Failure(e.Message);
            }
        }

        public async Task<Result<Constraint>> GetByIdAsync(Guid id)
        {
            try
            {
                var constraint = await context.Constraints.FindAsync(id);
                return constraint == null ? Result<Constraint>.Failure("Constraint not found.") : Result<Constraint>.Success(constraint);
            }
            catch (Exception e)
            {
                return Result<Constraint>.Failure(e.Message);
            }
        }

        public async Task<Result<Guid>> UpdateAsync(Constraint constraint)
        {
            try
            {
                context.Entry(constraint).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Result<Guid>.Success(constraint.Id);
            }
            catch (Exception e)
            {
                return Result<Guid>.Failure(e.Message);
            }
        }
    }
}