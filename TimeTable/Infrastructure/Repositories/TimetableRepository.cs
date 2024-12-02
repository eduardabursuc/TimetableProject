using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using MediatR;
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
        
        public async Task<Result<Unit>> UpdateAsync(Timetable timetable)
        {
            try
            {
                // Load the existing timetable with its owned timeslots
                var existingTimetable = await context.Timetables
                    .Include(t => t.Timeslots)
                    .FirstOrDefaultAsync(t => t.Id == timetable.Id);

                if (existingTimetable == null)
                {
                    return Result<Unit>.Failure($"Timetable with ID '{timetable.Id}' not found.");
                }

                // Update the timetable properties
                context.Entry(existingTimetable).CurrentValues.SetValues(timetable);

                // Update owned Timeslots
                foreach (var incomingTimeslot in timetable.Timeslots)
                {
                    var existingTimeslot = existingTimetable.Timeslots
                        .FirstOrDefault(ts =>
                            ts.Time == incomingTimeslot.Time &&
                            ts.Day == incomingTimeslot.Day &&
                            ts.RoomName == incomingTimeslot.RoomName);

                    if (existingTimeslot == null)
                    {
                        // Add new timeslot
                        existingTimetable.Timeslots.Add(incomingTimeslot);
                    }
                    else
                    {
                        // Update existing timeslot
                        context.Entry(existingTimeslot).CurrentValues.SetValues(incomingTimeslot);
                    }
                }

                // Remove deleted timeslots
                foreach (var existingTimeslot in existingTimetable.Timeslots.ToList())
                {
                    if (!timetable.Timeslots.Any(ts =>
                            ts.Time == existingTimeslot.Time &&
                            ts.Day == existingTimeslot.Day &&
                            ts.RoomName == existingTimeslot.RoomName))
                    {
                        existingTimetable.Timeslots.Remove(existingTimeslot);
                    }
                }

                await context.SaveChangesAsync();
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception e)
            {
                return Result<Unit>.Failure(e.Message);
            }
        }


 
         public async Task<Result<Unit>> DeleteAsync(Guid id)
         {
             try
             {
                 var timetable = await context.Timetables.FindAsync(id);
                 if (timetable == null) return Result<Unit>.Failure("Timetable not found.");
 
                 context.Timetables.Remove(timetable);
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

