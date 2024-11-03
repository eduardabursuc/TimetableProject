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

        public async Task<Guid> AddAsync(Constraint constraint)
        {
            await context.Constraints.AddAsync(constraint);
            await context.SaveChangesAsync();
            return constraint.Id;
        }

        public async Task<IEnumerable<Constraint>> GetAllAsync()
        {
            return await context.Constraints.ToListAsync();
        }

        public async Task<Constraint> GetByIdAsync(Guid id)
        {
            return await context.Constraints.FindAsync(id);
        }

        public async Task UpdateAsync(Constraint constraint)
        {
            context.Entry(constraint).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
    }
}