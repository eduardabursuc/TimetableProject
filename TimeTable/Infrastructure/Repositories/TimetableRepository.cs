using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories
{
    public class TimetableRepository(ApplicationDbContext context) : ITimetableRepository
    {
        public async Task<Result<IEnumerable<Timetable>>> GetAllAsync()
        {
            try
            {
                var timetables = await context.timetables.ToListAsync();
                return Result<IEnumerable<Timetable>>.Success(timetables);
            }
            catch (Exception e)
            {
                return Result<IEnumerable<Timetable>>.Failure(e.Message);
            }
        }
    }
}