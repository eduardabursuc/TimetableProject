using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.UseCases.Commands.ConstraintCommands;
using Application.UseCases.Commands.TimetableCommands;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TimeTable.Application.IntegrationTests
{
    public class ConstraintsControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly ApplicationDbContext _dbContext;
        private readonly AuthorizationService _authorizationService = new AuthorizationService();
        private readonly HttpClient _client;
        private const string BaseUrl = "/api/v1/constraints";

        public ConstraintsControllerTests(WebApplicationFactory<Program> factory)
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
            var token = await _authorizationService.GetToken("admin@example.com", 60);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        
        [Fact]
        public async Task CreateConstraint_ShouldReturnCreated()
        {
            await SetAdminAuthorizationHeader();
            
            // Arrange
            var createCommand = new CreateTimetableCommand
            {
                UserEmail = "user@example.com",
                Name = "Timetable 1",
                Events = new List<Event>(),
                Timeslots = new List<Timeslot>()
            };
            
            var createResponse = await _client.PostAsJsonAsync("/api/v1/timetables", createCommand);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var id = await createResponse.Content.ReadFromJsonAsync<Guid>();
            
            var command = new CreateConstraintCommand
            {
                ProfessorEmail = "user@gmail.com",
                TimetableId = id,
                Input = "No overlap",
                
            };

            // Act
            var response = await _client.PostAsJsonAsync(BaseUrl, command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }
        
        
        [Fact]
        public async Task DeleteConstraint_ShouldReturnNoContent()
        {
            await SetAdminAuthorizationHeader();
            
            // Arrange
            var createCommand = new CreateTimetableCommand
            {
                UserEmail = "user@example.com",
                Name = "Timetable 1",
                Events = new List<Event>(),
                Timeslots = new List<Timeslot>()
            };
            
            var createResponse = await _client.PostAsJsonAsync("/api/v1/timetables", createCommand);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var id = await createResponse.Content.ReadFromJsonAsync<Guid>();
            
            var command = new CreateConstraintCommand
            {
                ProfessorEmail = "user@gmail.com",
                TimetableId = id,
                Input = "No overlap",
                
            };
            
            var created = await _client.PostAsJsonAsync(BaseUrl, command);
            created.StatusCode.Should().Be(HttpStatusCode.Created);

            var constraintId = await created.Content.ReadFromJsonAsync<Guid>();
            
            // Act
            var response = await _client.DeleteAsync($"{BaseUrl}/{constraintId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
        
        
        [Fact]
        public async Task GetAllTimetable_ShouldReturnOk()
        {
            await SetAdminAuthorizationHeader();

            // Arrange
            var createCommand = new CreateTimetableCommand
            {
                UserEmail = "user@example.com",
                Name = "Timetable 1",
                Events = new List<Event>(),
                Timeslots = new List<Timeslot>()
            };
            
            var createResponse = await _client.PostAsJsonAsync("/api/v1/timetables", createCommand);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var id = await createResponse.Content.ReadFromJsonAsync<Guid>();
            
            var createCommand1 = new CreateConstraintCommand
            {
                ProfessorEmail = "user@gmail.com",
                TimetableId = id,
                Input = "No overlap",
                
            };

            var createCommand2 = new CreateConstraintCommand
            {
                ProfessorEmail = "user@gmail.com",
                TimetableId = id,
                Input = "Room capacity",
                
            };

            await _client.PostAsJsonAsync(BaseUrl, createCommand1);
            await _client.PostAsJsonAsync(BaseUrl, createCommand2);

            // Act
            var response = await _client.GetAsync($"{BaseUrl}?timetableId={id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var constraints = await response.Content.ReadFromJsonAsync<IEnumerable<Constraint>>();
            constraints.Should().NotBeNullOrEmpty();
            constraints.All(p => p.TimetableId == id).Should().BeTrue();
        }

        [Fact]
        public async Task GetConstraintById_AsAdmin_ShouldReturnOk()
        {
            await SetAdminAuthorizationHeader();

            // Arrange
            var create = new CreateTimetableCommand
            {
                UserEmail = "user@example.com",
                Name = "Timetable 1",
                Events = new List<Event>(),
                Timeslots = new List<Timeslot>()
            };
            
            var createResponse = await _client.PostAsJsonAsync("/api/v1/timetables", create);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var id = await createResponse.Content.ReadFromJsonAsync<Guid>();
            
            var createCommand = new CreateConstraintCommand
            {
                ProfessorEmail = "user@gmail.com",
                TimetableId = id,
                Input = "No overlap",
                
            };
            
            var created = await _client.PostAsJsonAsync(BaseUrl, createCommand);
            var cId = await created.Content.ReadFromJsonAsync<Guid>();

            // Act
            var response = await _client.GetAsync($"{BaseUrl}/{cId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var constraint = await response.Content.ReadFromJsonAsync<Constraint>();
            constraint.Should().NotBeNull();
        }

        [Fact]
        public async Task GivenNonExistingConstraint_WhenGettingConstraintById_ThenShouldReturnNotFoundResponse()
        {
            // Act
            var response = await _client.GetAsync($"{BaseUrl}/{Guid.NewGuid()}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

    }
}