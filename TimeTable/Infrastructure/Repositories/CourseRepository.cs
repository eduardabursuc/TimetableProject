using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDbContext context;

        public CourseRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Result<string>> AddAsync(Course course)
        {
            try
            {
                await context.Courses.AddAsync(course);
                await context.SaveChangesAsync();
                return Result<string>.Success(course.CourseName);
            }
            catch (Exception e)
            {
                return Result<string>.Failure(e.Message);
            }
        }

        public async Task<Result<IEnumerable<Course>>> GetAllAsync()
        {
            try
            {
                var courses = await context.Courses.ToListAsync();
                return Result<IEnumerable<Course>>.Success(courses);
            }
            catch (Exception e)
            {
                return Result<IEnumerable<Course>>.Failure(e.Message);
            }
        }

        public async Task<Result<Course>> GetByNameAsync(string id)
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

        public async Task<Result<string>> UpdateAsync(Course course)
        {
            try
            {
                context.Entry(course).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Result<string>.Success(course.CourseName);
            }
            catch (Exception e)
            {
                return Result<string>.Failure(e.Message);
            }
        }

        public async Task<Result<string>> DeleteAsync(string id)
        {
            try
            {
                var course = await context.Courses.FindAsync(id);
                if (course == null) return Result<string>.Failure("Course not found.");

                context.Courses.Remove(course);
                await context.SaveChangesAsync();
                return Result<string>.Success(course.CourseName);
            }
            catch (Exception e)
            {
                return Result<string>.Failure(e.Message);
            }
        }
    }
}