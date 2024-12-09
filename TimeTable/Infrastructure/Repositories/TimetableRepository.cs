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
                var existingTimetable = await _context.Timetables
                    .Include(t => t.Events)
                    .ThenInclude(e => e.Timeslot)
                    .FirstOrDefaultAsync(t => t.Id == timetable.Id);

                if (existingTimetable == null)
                    return Result<Unit>.Failure($"Timetable with ID '{timetable.Id}' not found.");

                // Update main timetable details
                _context.Entry(existingTimetable).CurrentValues.SetValues(timetable);

                // Sync Events
                foreach (var incomingEvent in timetable.Events)
                {
                    var existingEvent = existingTimetable.Events
                        .FirstOrDefault(e => e.Id == incomingEvent.Id);

                    if (existingEvent == null)
                    {
                        existingTimetable.Events.Add(incomingEvent);
                    }
                    else
                    {
                        _context.Entry(existingEvent).CurrentValues.SetValues(incomingEvent);

                        if (incomingEvent.Timeslot != null)
                        {
                            if (existingEvent.Timeslot == null)
                            {
                                existingEvent.Timeslot = incomingEvent.Timeslot;
                            }
                            else
                            {
                                _context.Entry(existingEvent.Timeslot).CurrentValues.SetValues(incomingEvent.Timeslot);
                            }
                        }
                    }
                }

                // Remove deleted events
                foreach (var existingEvent in existingTimetable.Events.ToList())
                {
                    if (timetable.Events.All(e => e.Id != existingEvent.Id))
                    {
                        existingTimetable.Events.Remove(existingEvent);
                    }
                }

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
    }
}
