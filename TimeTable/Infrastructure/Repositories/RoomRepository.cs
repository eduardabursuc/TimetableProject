using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class RoomRepository(ApplicationDbContext context) : IRoomRepository
    {

        public async Task<Result<IEnumerable<Room>>> GetAllAsync()
        {
            try
            {
                var rooms = await context.Rooms.ToListAsync();
                return Result<IEnumerable<Room>>.Success(rooms);
            }
            catch (Exception e)
            {
                return Result<IEnumerable<Room>>.Failure(e.Message);
            }
        }


    }
}