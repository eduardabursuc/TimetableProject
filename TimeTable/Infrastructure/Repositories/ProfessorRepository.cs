using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProfessorRepository(ApplicationDbContext context) : IProfessorRepository
    {
        public async Task<Result<Guid>> AddAsync(Professor professor)
        {
            try
            {
                await context.Professors.AddAsync(professor);
                await context.SaveChangesAsync();
                return Result<Guid>.Success(professor.Id);
            }
            catch (Exception e)
            {
                return Result<Guid>.Failure(e.Message);
            }
        }

        public async Task<Result<IEnumerable<Professor>>> GetAllAsync(string userEmail)
        {
            try
            {
                // Query professors where UserEmail matches the provided email
                var professors = await context.Professors
                    .Where(p => p.UserEmail == userEmail)
                    .ToListAsync();

                return Result<IEnumerable<Professor>>.Success(professors);
            }
            catch (Exception e)
            {
                return Result<IEnumerable<Professor>>.Failure(e.Message);
            }
        }


        public async Task<Result<Professor>> GetByIdAsync(Guid id)
        {
            try
            {
                var professor = await context.Professors.FindAsync(id);
                return professor == null ? Result<Professor>.Failure("Professor not found.") : Result<Professor>.Success(professor);
            }
            catch (Exception e)
            {
                return Result<Professor>.Failure(e.Message);
            }
        }

        public async Task<Result<Guid>> UpdateAsync(Professor professor)
        {
            try
            {
                context.Entry(professor).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Result<Guid>.Success(professor.Id);
            }
            catch (Exception e)
            {
                return Result<Guid>.Failure(e.Message);
            }
        }

        public async Task<Result<Unit>> DeleteAsync(Guid id)
        {
            try
            {
                var professor = await context.Professors.FindAsync(id);
                if (professor == null) return Result<Unit>.Failure("Professor not found.");

                context.Professors.Remove(professor);
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