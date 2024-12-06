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
        
        public async Task<Result<Room>> GetByNameAsync(string name)
        {
            try
            {
                var room = await context.Rooms.FirstOrDefaultAsync(r => r.Name == name);
                return room == null
                    ? Result<Room>.Failure($"Room with name {name} not found.")
                    : Result<Room>.Success(room);
            }
            catch (Exception e)
            {
                return Result<Room>.Failure(e.Message);
            }
        }


    }
}