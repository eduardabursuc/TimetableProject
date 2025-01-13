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

        public async Task<Result<IEnumerable<Constraint>>> GetAllForProfessorAsync(string professorEmail, Guid timetableId)
        {
            try
            {
                // Step 1: Find the professor's ID using the provided email
                var professor = await context.Professors
                    .FirstOrDefaultAsync(p => p.Email == professorEmail);

                if (professor == null)
                {
                    return Result<IEnumerable<Constraint>>.Failure("Professor not found.");
                }

                // Step 2: Retrieve constraints for the professor and the specified timetable ID
                var constraints = await context.Constraints
                    .Where(c => c.ProfessorId == professor.Id && c.TimetableId == timetableId)
                    .ToListAsync();
                
                return Result<IEnumerable<Constraint>>.Success(constraints);
            }
            catch (Exception ex)
            {
                // Log the exception and return failure result
                Console.WriteLine($"Error occurred: {ex.Message}");
                return Result<IEnumerable<Constraint>>.Failure(ex.Message);
            }
        }

    }
}