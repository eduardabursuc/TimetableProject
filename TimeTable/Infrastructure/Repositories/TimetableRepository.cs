using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class TimetableRepository : ITimetableRepository
    {
        private readonly ApplicationDbContext _context;

        public TimetableRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> AddAsync(Timetable timetable)
        {
            try
            {
                await _context.Timetables.AddAsync(timetable);
                await _context.SaveChangesAsync();
                return Result<Guid>.Success(timetable.Id);
            }
            catch (Exception e)
            {
                return Result<Guid>.Failure(e.Message);
            }
        }

        public async Task<Result<IEnumerable<Timetable>>> GetAllAsync(string userEmail)
        {
            try
            {
                var timetables = await _context.Timetables
                    .Where(t => t.UserEmail == userEmail)
                    .ToListAsync();

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
                var timetable = await _context.Timetables
                    .Include(t => t.Events)
                    .ThenInclude(e => e.Timeslot)
                    .FirstOrDefaultAsync(t => t.Id == id);

                return timetable == null
                    ? Result<Timetable>.Failure("Timetable not found.")
                    : Result<Timetable>.Success(timetable);
            }
            catch (Exception e)
            {
                return Result<Timetable>.Failure(e.Message);
            }
        }

        public async Task<Result<Timetable>> GetByGroupAsync(Guid id, Guid groupId)
        {
            try
            {
                var timetable = await _context.Timetables
                    .Include(t => t.Events)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (timetable == null)
                    return Result<Timetable>.Failure("Timetable not found.");

                timetable.Events = timetable.Events
                    .Where(e => e.GroupId == groupId)
                    .ToList();

                return timetable.Events.Any()
                    ? Result<Timetable>.Success(timetable)
                    : Result<Timetable>.Failure("No records found.");
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
                var timetable = await _context.Timetables
                    .Include(t => t.Events)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (timetable == null)
                    return Result<Timetable>.Failure("Timetable not found.");

                timetable.Events = timetable.Events
                    .Where(e => e.ProfessorId == professorId)
                    .ToList();

                return timetable.Events.Any()
                    ? Result<Timetable>.Success(timetable)
                    : Result<Timetable>.Failure("No records found.");
            }
            catch (Exception e)
            {
                return Result<Timetable>.Failure(e.Message);
            }
        }

        public async Task<Result<Timetable>> GetByRoomAsync(Guid id, Guid roomId)
        {
            try
            {
                var timetable = await _context.Timetables
                    .Include(t => t.Events)
                    .ThenInclude(e => e.Timeslot)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (timetable == null)
                    return Result<Timetable>.Failure("Timetable not found.");

                timetable.Events = timetable.Events
                    .Where(e => e?.RoomId == roomId)
                    .ToList();

                return timetable.Events.Any()
                    ? Result<Timetable>.Success(timetable)
                    : Result<Timetable>.Failure("No records found.");
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
                // Fetch the existing timetable with its related entities
                var existingTimetable = await _context.Timetables
                    .Include(t => t.Events)
                    .ThenInclude(e => e.Timeslot)
                    .FirstOrDefaultAsync(t => t.Id == timetable.Id);

                if (existingTimetable == null)
                    return Result<Unit>.Failure($"Timetable with ID '{timetable.Id}' not found.");

                // 1. Update the main Timetable fields manually
                existingTimetable.Name = timetable.Name;
                existingTimetable.IsPublic = timetable.IsPublic;

                // 2. Update or add Events manually
                foreach (var incomingEvent in timetable.Events)
                {
                    var existingEvent = existingTimetable.Events
                        .FirstOrDefault(e => e.Id == incomingEvent.Id);

                    if (existingEvent == null)
                    {
                        // If this event doesn't exist, add it
                        existingTimetable.Events.Add(incomingEvent);
                    }
                    else
                    {
                        // Manually update the existing Event
                        existingEvent.EventName = incomingEvent.EventName;
                        existingEvent.CourseId = incomingEvent.CourseId;
                        existingEvent.RoomId = incomingEvent.RoomId;
                        existingEvent.ProfessorId = incomingEvent.ProfessorId;
                        existingEvent.Duration = incomingEvent.Duration;
                        existingEvent.GroupId = incomingEvent.GroupId;
                        existingEvent.IsEven = incomingEvent.IsEven;

                        // Manually update the Timeslot of the event
                        if (incomingEvent.Timeslot != null)
                        {
                            if (existingEvent.Timeslot == null)
                            {
                                // If there's no Timeslot, set it
                                existingEvent.Timeslot = incomingEvent.Timeslot;
                            }
                            else
                            {
                                // Update the existing Timeslot
                                existingEvent.Timeslot.Day = incomingEvent.Timeslot.Day;
                                existingEvent.Timeslot.Time = incomingEvent.Timeslot.Time;
                            }
                        }
                    }
                }

                // 3. Manually remove Events that should no longer exist (if any)
                var eventsToRemove = existingTimetable.Events
                    .Where(e => timetable.Events.All(te => te.Id != e.Id))
                    .ToList();

                foreach (var eventToRemove in eventsToRemove)
                {
                    // Mark for deletion manually
                    _context.Entry(eventToRemove).State = EntityState.Deleted;
                }

                // 4. Save changes after all updates
                await _context.SaveChangesAsync();

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
                var timetable = await _context.Timetables.FindAsync(id);
                if (timetable == null)
                    return Result<Unit>.Failure("Timetable not found.");

                _context.Timetables.Remove(timetable);
                await _context.SaveChangesAsync();
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception e)
            {
                return Result<Unit>.Failure(e.Message);
            }
        }

        public async Task<Result<IEnumerable<Timetable>>> GetAllForProfessorAsync(string professorEmail)
        {
            try
            {
                var timetables = await _context.Timetables
                    .Include(t => t.Events) // Include the Events navigation property
                    .Where(t => t.IsPublic && t.Events.Any(e => 
                        _context.Professors.Any(p => p.Id == e.ProfessorId && p.Email == professorEmail)))
                    .ToListAsync();

                return Result<IEnumerable<Timetable>>.Success(timetables);
            }
            catch (Exception ex)
            {
                // Log the exception
                return Result<IEnumerable<Timetable>>.Failure($"An error occurred while retrieving timetables: {ex.Message}");
            }
        }

    }
}
