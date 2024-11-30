using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Infrastructure.Repositories
{
    public class TimetableRepository(ApplicationDbContext context) : ITimetableRepository
     {
         public async Task<Result<Guid>> AddAsync(Timetable timetable)
         {
             try
             {
                 await context.Timetables.AddAsync(timetable);
                 await context.SaveChangesAsync();
                 return Result<Guid>.Success(timetable.Id);
             }
             catch (Exception e)
             {
                 return Result<Guid>.Failure(e.Message);
             }
         }
         
         public async Task<Result<IEnumerable<Timetable>>> GetAllAsync()
         {
             try
             {
                 var timetables = await context.Timetables.ToListAsync();
                 return Result<IEnumerable<Timetable>>.Success(timetables);
             }
             catch (Exception e)
             {
                 return Result<IEnumerable<Timetable>>.Failure(e.Message);
             }
         }
 
         public async Task<Result<Timetable>> GetByIdAsync(Guid id)
         {
             try
             {
                 var timetable = await context.Timetables.FindAsync(id);
                 return timetable == null ? Result<Timetable>.Failure("Timetable not found.") : Result<Timetable>.Success(timetable);
             }
             catch (Exception e)
             {
                 return Result<Timetable>.Failure(e.Message);
             }
         }
         
         public async Task<Result<Timetable>> GetByGroupAsync(Guid id, string groupName)
         {
            try
            {
                var timetable = await context.Timetables.FirstOrDefaultAsync(t => t.Id == id);
                if (timetable == null) return Result<Timetable>.Failure("Timetable not found.");
                
                timetable.Timeslots = timetable.Timeslots.Where(t => t.Event.Group == groupName).ToList();
                return timetable.Timeslots.Count == 0 ? Result<Timetable>.Failure("No records.") : Result<Timetable>.Success(timetable);
            }
            catch (Exception e)
            {
                return Result<Timetable>.Failure(e.Message);
            }
         }
         
        public async Task<Result<Timetable>> GetByProfessorAsync(Guid id, Guid professorId)
        {
            try
            {
                var timetable = await context.Timetables.FirstOrDefaultAsync(t => t.Id == id);
                if (timetable == null) return Result<Timetable>.Failure("Timetable not found.");
                
                timetable.Timeslots = timetable.Timeslots.Where(t => t.Event.ProfessorId == professorId).ToList();
                return timetable.Timeslots.Count == 0 ? Result<Timetable>.Failure("No records.") : Result<Timetable>.Success(timetable);
            }
            catch (Exception e)
            {
                return Result<Timetable>.Failure(e.Message);
            }
        }

        public async Task<Result<Timetable>> GetByRoomAsync(Guid id, string roomName)
        {
            try
            {
                var timetable = await context.Timetables.FirstOrDefaultAsync(t => t.Id == id);
                if (timetable == null) return Result<Timetable>.Failure("Timetable not found.");
                timetable.Timeslots = timetable.Timeslots.Where(t => t.RoomName == roomName).ToList();
                return timetable.Timeslots.Count == 0 ? Result<Timetable>.Failure("No records.") : Result<Timetable>.Success(timetable);
            }
            catch (Exception e)
            {
                return Result<Timetable>.Failure(e.Message);
            }
        }
 
         public async Task<Result<Guid>> UpdateAsync(Timetable timetable)
         {
             try
             {
                 context.Entry(timetable).State = EntityState.Modified;
                 await context.SaveChangesAsync();
                 return Result<Guid>.Success(timetable.Id);
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
                 var timetable = await context.Timetables.FindAsync(id);
                 if (timetable == null) return Result<Guid>.Failure("Timetable not found.");
 
                 context.Timetables.Remove(timetable);
                 await context.SaveChangesAsync();
                 return Result<Guid>.Success(timetable.Id);
             }
             catch (Exception e)
             {
                 return Result<Guid>.Failure(e.Message);
             }
         }
     }
    
}

