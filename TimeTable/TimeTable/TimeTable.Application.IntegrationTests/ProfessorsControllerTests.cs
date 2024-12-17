using System.Net;
using System.Text.Json;
using Application.DTOs;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TimeTable.Application.IntegrationTests
{
    public class ProfessorsControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private const string BaseUrl = "/api/v1/professors";

        // Define a static JsonSerializerOptions instance to reuse across all test cases
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public ProfessorsControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    // Add appsettings.Test.json for test-specific configurations
                    config.AddJsonFile("appsettings.Test.json");
                });

                builder.ConfigureServices(services =>
                {
                    // Remove the existing DbContextOptions<ApplicationDbContext> registration
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    // Register ApplicationDbContext for testing
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        // The "UseInMemoryDatabase" flag will now control the database provider
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

                    // Build the service provider to initialize the database
                    var sp = services.BuildServiceProvider();

                    // Create a scope to obtain a reference to the database context (ApplicationDbContext)
                    using (var scope = sp.CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;
                        var db = scopedServices.GetRequiredService<ApplicationDbContext>();

                        // Ensure the database is created
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
/*
        [Fact]
        public async Task GivenProfessorsExist_WhenGettingAllProfessors_ThenShouldReturnOkResponse()
        {
            // Arrange
            var professor = new Professor
            {
                UserEmail = "some1@gmail.com",
                Id = Guid.NewGuid(),
                Name = "Professor 1",
                Email = "email@gmail.com"
            };
            var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Professors.Add(professor);
            dbContext.SaveChanges();

            // Act
            var response = await _client.GetAsync(BaseUrl);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseData = JsonSerializer.Deserialize<List<ProfessorDto>>(
                await response.Content.ReadAsStringAsync(), JsonOptions);
            responseData.Should().ContainSingle(p => p.Name == "Professor 1");
        }
*/
        [Fact]
        public async Task GivenExistingProfessor_WhenGettingProfessorById_ThenShouldReturnOkResponse()
        {
            // Arrange
            var professorId = Guid.NewGuid();
            var professor = new Professor { UserEmail = "some1@gmail.com", Id = professorId, Name = "Professor 1", Email = "email@gmail.com"};
            var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Professors.Add(professor);
            dbContext.SaveChanges();

            // Act
            var response = await _client.GetAsync($"{BaseUrl}/{professorId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseData = JsonSerializer.Deserialize<ProfessorDto>(
                await response.Content.ReadAsStringAsync(), JsonOptions);
            responseData.Should().NotBeNull();
            responseData!.Id.Should().Be(professorId);
        }

        [Fact]
        public async Task GivenNonExistingProfessor_WhenGettingProfessorById_ThenShouldReturnNotFoundResponse()
        {
            // Act
            var response = await _client.GetAsync($"{BaseUrl}/{Guid.NewGuid()}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}