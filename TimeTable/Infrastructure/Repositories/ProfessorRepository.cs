using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class ProfessorRepository : IProfessorRepository
    {
        private readonly ApplicationDbContext context;

        public ProfessorRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

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

        public async Task<Result<IEnumerable<Professor>>> GetAllAsync()
        {
            try
            {
                var professors = await context.Professors.ToListAsync();
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

        public async Task<Result<Guid>> DeleteAsync(Guid id)
        {
            try
            {
                var professor = await context.Professors.FindAsync(id);
                if (professor == null) return Result<Guid>.Failure("Professor not found.");

                context.Professors.Remove(professor);
                await context.SaveChangesAsync();
                return Result<Guid>.Success(professor.Id);
            }
            catch (Exception e)
            {
                return Result<Guid>.Failure(e.Message);
            }
        }
    }
}