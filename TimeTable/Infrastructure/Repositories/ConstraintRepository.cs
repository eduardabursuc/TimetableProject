using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ConstraintRepository(ApplicationDbContext context) : IConstraintRepository
    {
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

        public async Task<Result<IEnumerable<Constraint>>> GetAllAsync(Guid timetable)
        {
            try
            {
                // Query constraints where UserEmail matches the provided email
                var constraints = await context.Constraints
                    .Where(c => c.TimetableId == timetable)
                    .ToListAsync();

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

        public async Task<Result<Unit>> UpdateAsync(Constraint constraint)
        {
            try
            {
                context.Entry(constraint).State = EntityState.Modified;
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
                var constraint = await context.Constraints.FindAsync(id);
                if (constraint == null) return Result<Unit>.Failure("Constraint not found.");

                context.Constraints.Remove(constraint);
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