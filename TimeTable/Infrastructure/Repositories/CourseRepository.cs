using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CourseRepository(ApplicationDbContext context) : ICourseRepository
    {
        public async Task<Result<Guid>> AddAsync(Course course)
        {
            try
            {
                await context.Courses.AddAsync(course);
                await context.SaveChangesAsync();
                return Result<Guid>.Success(course.Id);
            }
            catch (Exception e)
            {
                return Result<Guid>.Failure(e.Message);
            }
        }

        public async Task<Result<IEnumerable<Course>>> GetAllAsync(string userEmail)
        {
            try
            {
                // Query courses where UserEmail matches the provided email
                var courses = await context.Courses
                    .Where(c => c.UserEmail == userEmail)
                    .ToListAsync();

                return Result<IEnumerable<Course>>.Success(courses);
            }
            catch (Exception e)
            {
                return Result<IEnumerable<Course>>.Failure(e.Message);
            }
        }


        public async Task<Result<Course>> GetByIdAsync(Guid id)
        {
            try
            {
                var course = await context.Courses.FindAsync(id);
                return course == null ? Result<Course>.Failure("Course not found.") : Result<Course>.Success(course);
            }
            catch (Exception e)
            {
                return Result<Course>.Failure(e.Message);
            }
        }

        public async Task<Result<Guid>> UpdateAsync(Course course)
        {
            try
            {
                context.Entry(course).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Result<Guid>.Success(course.Id);
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
                var course = await context.Courses.FindAsync(id);
                if (course == null) return Result<Unit>.Failure("Course not found.");

                context.Courses.Remove(course);
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