using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Application.DTOs;
using Application.UseCases.Commands.RoomCommands;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TimeTable.Application.IntegrationTests
{
    public class RoomsControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly AuthorizationService authorizationService = new AuthorizationService();
        private readonly HttpClient _client;
        private readonly ApplicationDbContext _dbContext;
        private const string BaseUrl = "/api/v1/rooms";
        
        public RoomsControllerTests(WebApplicationFactory<Program> factory)
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
        public async Task CreateRoom_AsAdmin_ShouldReturnCreated()
        {
            await SetAdminAuthorizationHeader();

            // Arrange
            var command = new CreateRoomCommand
            {
                UserEmail = "testuser@example.com",
                Name = "Room A",
                Capacity = 30
            };

            // Act
            var response = await _client.PostAsJsonAsync(BaseUrl, command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task GetRoomById_ShouldReturnOk()
        {
            await SetAdminAuthorizationHeader();

            // Arrange
            var command = new CreateRoomCommand
            {
                UserEmail = "testuser@example.com",
                Name = "Room A",
                Capacity = 30
            };

            var createResponse = await _client.PostAsJsonAsync(BaseUrl, command);
            var createdRoomId = JsonSerializer.Deserialize<Guid>(await createResponse.Content.ReadAsStringAsync());

            // Act
            var response = await _client.GetAsync($"{BaseUrl}/{createdRoomId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetRoomById_ShouldReturnNotFound()
        {
            await SetAdminAuthorizationHeader();

            // Act
            var response = await _client.GetAsync($"{BaseUrl}/{Guid.NewGuid()}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteRoom_AsAdmin_ShouldReturnNoContent()
        {
            await SetAdminAuthorizationHeader();

            // Arrange
            var command = new CreateRoomCommand
            {
                UserEmail = "testuser@example.com",
                Name = "Room A",
                Capacity = 30
            };

            var createResponse = await _client.PostAsJsonAsync(BaseUrl, command);
            var createdRoomId = JsonSerializer.Deserialize<Guid>(await createResponse.Content.ReadAsStringAsync());

            // Act
            var response = await _client.DeleteAsync($"{BaseUrl}/{createdRoomId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task UpdateRoom_AsAdmin_ShouldReturnOk()
        {
            await SetAdminAuthorizationHeader();

            // Arrange
            var command = new CreateRoomCommand
            {
                UserEmail = "testuser@example.com",
                Name = "Room A",
                Capacity = 30
            };

            var createResponse = await _client.PostAsJsonAsync(BaseUrl, command);
            var createdRoomId = JsonSerializer.Deserialize<Guid>(await createResponse.Content.ReadAsStringAsync());

            var updateCommand = new UpdateRoomCommand
            {
                Id = createdRoomId,
                UserEmail = "testuser@example.com",
                Name = "Updated Room A",
                Capacity = 40
            };

            // Act
            var response = await _client.PutAsJsonAsync($"{BaseUrl}/{createdRoomId}", updateCommand);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        
        [Fact]
        public async Task GetAllRooms_AsAdmin_ShouldReturnOk()
        {
            await SetAdminAuthorizationHeader();

            // Arrange
            var userEmail = "testuser@example.com";

            // Arrange
            var createCommand1 = new CreateRoomCommand
            {
                UserEmail = userEmail,
                Name = "Room A",
                Capacity = 30
            };

            var createCommand2 = new CreateRoomCommand
            {
                UserEmail = userEmail,
                Name = "Room B",
                Capacity = 50
            };

            await _client.PostAsJsonAsync(BaseUrl, createCommand1);
            await _client.PostAsJsonAsync(BaseUrl, createCommand2);

            // Act
            var response = await _client.GetAsync($"{BaseUrl}?userEmail={userEmail}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var rooms = await response.Content.ReadFromJsonAsync<IEnumerable<RoomDto>>();
            rooms.Should().NotBeNullOrEmpty();
            rooms.All(p => p.UserEmail == userEmail).Should().BeTrue();
        }

        [Fact]
        public async Task GivenNonExistingRoom_WhenGettingRoomById_ThenShouldReturnNotFoundResponse()
        {
            await SetAdminAuthorizationHeader();

            // Act
            var response = await _client.GetAsync($"{BaseUrl}/{Guid.NewGuid()}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
