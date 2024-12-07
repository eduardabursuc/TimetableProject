using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class RoomRepository(ApplicationDbContext context) : IRoomRepository
    {

        public async Task<Result<IEnumerable<Room>>> GetAllAsync(string userEmail)
        {
            try
            {
                // Query rooms where UserEmail matches the provided email
                var rooms = await context.Rooms
                    .Where(r => r.UserEmail == userEmail)
                    .ToListAsync();

                return Result<IEnumerable<Room>>.Success(rooms);
            }
            catch (Exception e)
            {
                return Result<IEnumerable<Room>>.Failure(e.Message);
            }
        }

        
        public async Task<Result<Room>> GetByIdAsync(Guid id)
        {
            try
            {
                var room = await context.Rooms.FindAsync(id);
                return room == null
                    ? Result<Room>.Failure($"Room not found.")
                    : Result<Room>.Success(room);
            }
            catch (Exception e)
            {
                return Result<Room>.Failure(e.Message);
            }
        }
        
        public async Task<Result<Guid>> AddAsync(Room room)
        {
            try
            {
                await context.Rooms.AddAsync(room);
                await context.SaveChangesAsync();
                return Result<Guid>.Success(room.Id);
            }
            catch (Exception e)
            {
                return Result<Guid>.Failure(e.Message);
            }
        }
        
        public async Task<Result<Guid>> UpdateAsync(Room room)
        {
            try
            {
                context.Entry(room).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Result<Guid>.Success(room.Id);
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
                var room = await context.Rooms.FindAsync(id);
                if (room == null)
                {
                    return Result<Unit>.Failure("Room not found.");
                }
                context.Rooms.Remove(room);
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