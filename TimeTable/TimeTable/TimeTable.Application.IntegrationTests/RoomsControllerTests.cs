using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Application.UseCases.Commands.RoomCommands;
using Domain.Common;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace TimeTable.Application.IntegrationTests
{
    public class RoomsControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private const string BaseUrl = "/api/v1/rooms";

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

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

                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        var serviceProvider = services.BuildServiceProvider();
                        var config = serviceProvider.GetRequiredService<IConfiguration>();
                        var useInMemory = config.GetValue<bool>("UseInMemoryDatabase");

                        if (useInMemory)
                        {
                            options.UseInMemoryDatabase("TestDatabase");
                        }
                        else
                        {
                            options.UseNpgsql(
                                config.GetConnectionString("DefaultConnection"),
                                b => b.MigrationsAssembly("Infrastructure"));
                        }
                    });

                    var sp = services.BuildServiceProvider();

                    using (var scope = sp.CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;
                        var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                        db.Database.EnsureCreated();
                    }
                });
            });

            _client = _factory.CreateClient();
        }

        public void Dispose()
        {
            var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.EnsureDeleted();
            dbContext.Dispose();

            GC.SuppressFinalize(this);
        }

        [Fact]
        public async Task CreateRoom_ShouldReturnCreated()
        {
            var command = new CreateRoomCommand
            {
                UserEmail = "testuser@example.com",
                Name = "Room A",
                Capacity = 30
            };

            var response = await _client.PostAsJsonAsync(BaseUrl, command);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task GetRoomById_ShouldReturnOk()
        {
            var command = new CreateRoomCommand
            {
                UserEmail = "testuser@example.com",
                Name = "Room A",
                Capacity = 30
            };

            var createResponse = await _client.PostAsJsonAsync(BaseUrl, command);
            var createdRoomId = JsonSerializer.Deserialize<Guid>(await createResponse.Content.ReadAsStringAsync());

            var response = await _client.GetAsync($"{BaseUrl}/{createdRoomId}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetRoomById_ShouldReturnNotFound()
        {
            var id = Guid.NewGuid();
            var response = await _client.GetAsync($"{BaseUrl}/{id}");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteRoom_ShouldReturnNoContent()
        {
            var command = new CreateRoomCommand
            {
                UserEmail = "testuser@example.com",
                Name = "Room A",
                Capacity = 30
            };

            var createResponse = await _client.PostAsJsonAsync(BaseUrl, command);
            var createdRoomId = JsonSerializer.Deserialize<Guid>(await createResponse.Content.ReadAsStringAsync());

            var response = await _client.DeleteAsync($"{BaseUrl}/{createdRoomId}");
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}