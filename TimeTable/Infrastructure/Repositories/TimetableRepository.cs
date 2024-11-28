using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
namespace Infrastructure.Repositories;

public class TimetableRepository(ApplicationDbContext context) : ITimetableRepository
{
    public async Task<Result<Timetable>> AddAsync(Timetable timetable)
    {
        try
        {
            await context.Timetables.AddAsync(timetable);
            await context.SaveChangesAsync();
            return Result<Timetable>.Success(timetable);
        }
        catch (Exception e)
        {
            return Result<Timetable>.Failure(e.Message);
        }
    }
}