using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.UseCases.Commands.TimetableCommands;
using Application.DTOs;
using Application.UseCases.Authentication;
using Application.UseCases.Commands.CourseCommands;
using Application.UseCases.Commands.GroupCommands;
using Application.UseCases.Commands.ProfessorCommands;
using Application.UseCases.Commands.RoomCommands;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TimeTable.Application.IntegrationTests
{
    public class TimetablesControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly AuthorizationService authorizationService = new AuthorizationService();
        private readonly ApplicationDbContext _dbContext;
        private const string BaseUrl = "/api/v1/timetables";

        public TimetablesControllerTests(WebApplicationFactory<Program> factory)
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

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private async Task SetAdminAuthorizationHeader()
        {
            var token = await authorizationService.GetToken("example@gmail.com", 60);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        [Fact]
        public async Task CreateTimetable_ShouldReturnCreated()
        {
            await SetAdminAuthorizationHeader();

            // Arrange
            var command = new CreateTimetableCommand
            {
                UserEmail = "testuser@example.com",
                Name = "Timetable 1",
                Events = new List<Event>(),
                Timeslots = new List<Timeslot>()
            };

            // Act
            var response = await _client.PostAsJsonAsync(BaseUrl, command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task GetTimetableById_ShouldReturnOk()
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

            var createResponse = await _client.PostAsJsonAsync(BaseUrl, createCommand);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var id = await createResponse.Content.ReadFromJsonAsync<Guid>();

            // Act
            var response = await _client.GetAsync($"{BaseUrl}/{id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetTimetableById_ShouldReturnNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            // Act
            var response = await _client.GetAsync($"{BaseUrl}/{id}");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateTimetable_AsAdmin_ShouldReturnNoContent()
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

            var createResponse = await _client.PostAsJsonAsync(BaseUrl, createCommand);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseContent = await createResponse.Content.ReadFromJsonAsync<Guid>();
            var id = responseContent;

            var updateCommand = new UpdateTimetableCommand
            {
                Id = id,
                Name = "Timetable 1",
                Events = new List<Event>(),
                CreatedAt = new DateTime(),
                IsPublic = true
            };

            // Act
            var response = await _client.PutAsJsonAsync($"{BaseUrl}/{id}", updateCommand);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }


        [Fact]
        public async Task DeleteTimetable_AsAdmin_ShouldReturnNoContent()
        {
            await SetAdminAuthorizationHeader();

            // Arrange
            var command = new CreateTimetableCommand
            {
                UserEmail = "testuser@example.com",
                Name = "Timetable 1",
                Events = new List<Event>(),
                Timeslots = new List<Timeslot>()
            };

            var createResponse = await _client.PostAsJsonAsync(BaseUrl, command);
            var id = await createResponse.Content.ReadFromJsonAsync<Guid>();

            // Act
            var response = await _client.DeleteAsync($"{BaseUrl}/{id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task GetAllTimetables_ShouldReturnOk()
        {
            await SetAdminAuthorizationHeader();

            // Arrange
            var userEmail = "testuser@example.com";

            var createCommand1 = new CreateTimetableCommand
            {
                UserEmail = userEmail,
                Name = "Timetable 1",
                Events = new List<Event>(),
                Timeslots = new List<Timeslot>()
            };

            var createCommand2 = new CreateTimetableCommand
            {
                UserEmail = userEmail,
                Name = "Timetable 2",
                Events = new List<Event>(),
                Timeslots = new List<Timeslot>()
            };

            await _client.PostAsJsonAsync(BaseUrl, createCommand1);
            await _client.PostAsJsonAsync(BaseUrl, createCommand2);

            // Act
            var response = await _client.GetAsync($"{BaseUrl}?userEmail={userEmail}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var timetables = await response.Content.ReadFromJsonAsync<IEnumerable<TimetableDto>>();
            timetables.Should().NotBeNullOrEmpty();
            timetables.All(p => p.UserEmail == userEmail).Should().BeTrue();
        }


        [Fact]
        public async Task GetAllForProfessor_ShouldReturnOk()
        {
            await SetAdminAuthorizationHeader();

            var userEmail = "testuser@example.com";

            var registerCommand = new RegisterUserCommand
            {
                Email = userEmail,
                Password = "Password123",
                AccountType = "admin"
            };

            var register = await _client.PostAsJsonAsync($"/api/auth/register", registerCommand);
            register.StatusCode.Should().Be(HttpStatusCode.OK);

            var profEmail = "johndoe@example.com";

            // Arrange: Create a professor
            var createProfessorCommand = new CreateProfessorCommand
            {
                UserEmail = userEmail,
                Name = "Dr. John Doe",
                Email = profEmail
            };

            var professorResponse = await _client.PostAsJsonAsync("/api/v1/professors", createProfessorCommand);
            professorResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var profId = await professorResponse.Content.ReadFromJsonAsync<Guid>();

            var createCommand1 = new CreateTimetableCommand
            {
                UserEmail = userEmail,
                Name = "Timetable 1",
                Events =
                [
                    new Event
                    {
                        EventName = "laboratory",
                        CourseId = new Guid(),
                        ProfessorId = profId,
                        GroupId = new Guid(),
                        Duration = 1
                    }
                ],
                Timeslots = new List<Timeslot>()
            };

            var createCommand2 = new CreateTimetableCommand
            {
                UserEmail = userEmail,
                Name = "Timetable 2",
                Events = new List<Event>(),
                Timeslots = new List<Timeslot>()
            };

            // Act: Create timetables
            await _client.PostAsJsonAsync(BaseUrl, createCommand1);
            await _client.PostAsJsonAsync(BaseUrl, createCommand2);

            // Act: Retrieve timetables
            var response = await _client.GetAsync($"{BaseUrl}/forProfessor?professorEmail={profEmail}");

            // Assert: Check response status and timetables
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var timetables = await response.Content.ReadFromJsonAsync<IEnumerable<TimetableDto>>();
            //timetables.Should().NotBeNullOrEmpty();
        }
/*
        [Fact]
        public async Task UpdateAsync_ShouldUpdateTimetableAndEventsCorrectly()
        {
            await SetAdminAuthorizationHeader();
            
            // Arrange
            var email = "user@example.com";
            var eventId = Guid.NewGuid();
            
            var regCommand = new RegisterUserCommand
            {
                Email = email,
                Password = "Password123",
                AccountType = "admin"
            };

            var reg = await _client.PostAsJsonAsync("api/auth/register", regCommand);

            reg.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var prof = new CreateProfessorCommand
            {
                UserEmail = email,
                Name = "Professor 1",
                Email = "professor1@example.com"
            };
            
            var profRes = await _client.PostAsJsonAsync("api/v1/professors", prof);
            profRes.StatusCode.Should().Be(HttpStatusCode.Created);
            var profId = await profRes.Content.ReadFromJsonAsync<Guid>();
            
            var course = new CreateCourseCommand
            {
                UserEmail = email,
                CourseName = "Some",
                Package = "comp",
                Level = "license",
                Semester = 1,
                Credits = 5
            };
            
            var courseRes = await _client.PostAsJsonAsync("api/v1/courses", course);
            courseRes.StatusCode.Should().Be(HttpStatusCode.Created);
            var courseId = await courseRes.Content.ReadFromJsonAsync<Guid>();
            
            var room = new CreateRoomCommand
            {
                UserEmail = email,
                Name = "C2",
                Capacity = 100
            };
            
            var roomRes = await _client.PostAsJsonAsync("api/v1/rooms", room);
            roomRes.StatusCode.Should().Be(HttpStatusCode.Created);
            var roomId = await roomRes.Content.ReadFromJsonAsync<Guid>();
            
            var group = new CreateGroupCommand
            {
                UserEmail = email,
                Name = "1A1"
            };
            
            var groupRes = await _client.PostAsJsonAsync("api/v1/groups", group);
            groupRes.StatusCode.Should().Be(HttpStatusCode.Created);
            var groupId = await groupRes.Content.ReadFromJsonAsync<Guid>();

            // Arrange
            var createCommand = new CreateTimetableCommand
            {
                UserEmail = "test@gmail.com",
                Name = "Initial Timetable",
                Events = new List<Event>
                {
                    new Event
                    {
                        Id = eventId,
                        EventName = "seminary",
                        Duration = 2,
                        CourseId = courseId,
                        RoomId = roomId,
                        ProfessorId = profId,
                        GroupId = groupId
                    }
                },
                Timeslots = [ new Timeslot { Day = "Monday", Time = "08:00 - 20:00" }, new Timeslot { Day = "Tuesday", Time = "08:00 - 20:00" }]
            };

            var createResponse = await _client.PostAsJsonAsync(BaseUrl, createCommand);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseContent = await createResponse.Content.ReadFromJsonAsync<Guid>();
            var id = responseContent;


            var updatedCommand = new UpdateTimetableCommand
            {
                Id = id,
                Name = "Updated Timetable",
                IsPublic = true,
                Events = new List<Event>
                {
                    new Event
                    {
                        Id = eventId, // Update existing event
                        EventName = "laboratory",
                        Duration = 3,
                        CourseId = courseId,
                        RoomId = roomId,
                        ProfessorId = profId,
                        GroupId = groupId,
                        Timeslot = new Timeslot
                        {
                            Day = "Tuesday",
                            Time = "10:00 - 12:00"
                        }
                    },
                    new Event
                    {
                        Id = Guid.NewGuid(), // New event
                        EventName = "course",
                        Duration = 2,
                        CourseId = courseId,
                        RoomId = roomId,
                        ProfessorId = profId,
                        GroupId = groupId,
                    }
                },
                CreatedAt = new DateTime()
            };

            // Act
            var response = await _client.PutAsJsonAsync($"{BaseUrl}/{id}", updatedCommand);
            

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var savedTimetable = await _dbContext.Timetables
                .Include(t => t.Events)
                .ThenInclude(e => e.Timeslot)
                .FirstOrDefaultAsync(t => t.Id == id);

            savedTimetable.Should().NotBeNull();
            savedTimetable!.Name.Should().Be("Updated Timetable");
            savedTimetable.IsPublic.Should().BeTrue();

            savedTimetable.Events.Count.Should().Be(2); // 1 updated + 1 new event

            var updatedEvent = savedTimetable.Events.First(e => e.Id == eventId);
            updatedEvent.EventName.Should().Be("laboratory");
            updatedEvent.Duration.Should().Be(3);
            updatedEvent.Timeslot.Day.Should().Be("Tuesday");
            updatedEvent.Timeslot.Time.Should().Be("10:00 - 12:00");

            var newEvent = savedTimetable.Events.First(e => e.EventName == "Physics Class");
            newEvent.Timeslot.Day.Should().Be("Monday");
            newEvent.Timeslot.Time.Should().Be("10:00 - 12:00");
        }
 */   }

}