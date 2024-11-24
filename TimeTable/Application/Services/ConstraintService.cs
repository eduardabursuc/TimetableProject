using Domain.Entities;
using Domain.Repositories;

namespace Application.Services
{
    public class ConstraintService(IConstraintRepository constraintRepository)
    {
        public async Task<HashSet<Constraint>> GetConstraintsAsync()
        {
            var result = await constraintRepository.GetAllAsync();
            return [..result.Data];
        }
    }
}

