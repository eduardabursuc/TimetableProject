using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.DTOs;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application.UseCases.Commands.CourseCommands;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace TimeTable.Application.IntegrationTests
{
    public class CoursesControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly ApplicationDbContext _dbContext;
        private readonly AuthorizationService authorizationService = new AuthorizationService();
        private readonly HttpClient _client;
        private const string BaseUrl = "/api/v1/courses";

        public CoursesControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.Test.json");
                });

                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }
                    
                    descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(IDbContextOptionsConfiguration<ApplicationDbContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting");
                    });
                    
                });
            });

            var scope = _factory.Services.CreateScope();
            _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            _dbContext.Database.EnsureCreated();
            _client = _factory.CreateClient();
        }

        private async Task SetAdminAuthorizationHeader()
        {
            var token = await authorizationService.GetToken("example@gmail.com", 60);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        [Fact]
        public async Task CreateCourse_AsAdmin_ShouldReturnCreated()
        {
            await SetAdminAuthorizationHeader();

            // Arrange
            var command = new CreateCourseCommand
            {
                UserEmail = "testuser@example.com",
                CourseName = "Course 101",
                Credits = 3,
                Package = "Package A",
                Semester = 1,
                Level = "Undergraduate"
            };

            // Act
            var response = await _client.PostAsJsonAsync(BaseUrl, command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task DeleteCourse_AsAdmin_ShouldReturnNoContent()
        {
            await SetAdminAuthorizationHeader();

            // Arrange
            var createCommand = new CreateCourseCommand
            {
                UserEmail = "testuser@example.com",
                CourseName = "Course to Delete",
                Credits = 3,
                Package = "Package A",
                Semester = 1,
                Level = "Undergraduate"
            };

            var createResponse = await _client.PostAsJsonAsync(BaseUrl, createCommand);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseContent = await createResponse.Content.ReadFromJsonAsync<Guid>();
            var id = responseContent;

            // Act
            var response = await _client.DeleteAsync($"{BaseUrl}/{id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task UpdateCourse_AsAdmin_ShouldReturnNoContent()
        {
            await SetAdminAuthorizationHeader();

            // Arrange
            var createCommand = new CreateCourseCommand
            {
                UserEmail = "testuser@example.com",
                CourseName = "Original Course Name",
                Credits = 3,
                Package = "Package A",
                Semester = 1,
                Level = "Undergraduate"
            };

            var createResponse = await _client.PostAsJsonAsync(BaseUrl, createCommand);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseContent = await createResponse.Content.ReadFromJsonAsync<Guid>();
            var id = responseContent;

            var updateCommand = new UpdateCourseCommand
            {
                Id = id,
                UserEmail = "testuser@example.com",
                CourseName = "Updated Course Name",
                Credits = 4,
                Package = "Package B",
                Semester = 2,
                Level = "Graduate"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"{BaseUrl}/{id}", updateCommand);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task GetAllCourses_AsAdmin_ShouldReturnOk()
        {
            await SetAdminAuthorizationHeader();

            // Arrange
            var userEmail = "testuser@example.com";

            var createCommand1 = new CreateCourseCommand
            {
                UserEmail = userEmail,
                CourseName = "Course 1",
                Credits = 3,
                Package = "Package A",
                Semester = 1,
                Level = "Undergraduate"
            };

            var createCommand2 = new CreateCourseCommand
            {
                UserEmail = userEmail,
                CourseName = "Course 2",
                Credits = 4,
                Package = "Package B",
                Semester = 2,
                Level = "Graduate"
            };

            await _client.PostAsJsonAsync(BaseUrl, createCommand1);
            await _client.PostAsJsonAsync(BaseUrl, createCommand2);

            // Act
            var response = await _client.GetAsync($"{BaseUrl}?userEmail={userEmail}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var courses = await response.Content.ReadFromJsonAsync<IEnumerable<CourseDto>>();
            courses.Should().NotBeNullOrEmpty();
            courses.All(c => c.UserEmail == userEmail).Should().BeTrue();
        }


        [Fact]
        public async Task GetCourseById_AsAdmin_ShouldReturnOk()
        {
            await SetAdminAuthorizationHeader();

            // Arrange
            var createCommand = new CreateCourseCommand
            {
                UserEmail = "test@example.com",
                CourseName = "Course 1",
                Credits = 3,
                Package = "Package A",
                Semester = 1,
                Level = "Undergraduate"
            };

            var createResponse = await _client.PostAsJsonAsync(BaseUrl, createCommand);
            var responseContent = await createResponse.Content.ReadFromJsonAsync<Guid>();
            var id = responseContent;

            // Act
            var response = await _client.GetAsync($"{BaseUrl}/{id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var course = await response.Content.ReadFromJsonAsync<CourseDto>();
            course.Should().NotBeNull();
            course.Id.Should().Be(id);
            course.CourseName.Should().Be("Course 1");
        }
    }
}
