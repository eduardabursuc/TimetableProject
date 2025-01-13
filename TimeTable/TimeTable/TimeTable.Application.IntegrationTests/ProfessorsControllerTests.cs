using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.DTOs;
using Application.UseCases.Commands.ProfessorCommands;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace TimeTable.Application.IntegrationTests
{
    public class ProfessorsControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly AuthorizationService authorizationService = new AuthorizationService();
        private readonly HttpClient _client;
        private readonly ApplicationDbContext _dbContext;
        private const string BaseUrl = "/api/v1/professors";

        public ProfessorsControllerTests(WebApplicationFactory<Program> factory)
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
        public async Task CreateProfessor_AsAdmin_ShouldReturnCreated()
        {
            await SetAdminAuthorizationHeader();

            // Arrange
            var command = new CreateProfessorCommand
            {
                UserEmail = "testuser@example.com",
                Name = "Professor 1",
                Email = "professor1@example.com"
            };

            // Act
            var response = await _client.PostAsJsonAsync(BaseUrl, command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task DeleteProfessor_AsAdmin_ShouldReturnNoContent()
        {
            await SetAdminAuthorizationHeader();

            // Arrange
            var createCommand = new CreateProfessorCommand
            {
                UserEmail = "testuser@example.com",
                Name = "Professor to Delete",
                Email = "deleteprofessor@example.com"
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
        public async Task UpdateProfessor_AsAdmin_ShouldReturnNoContent()
        {
            await SetAdminAuthorizationHeader();

            // Arrange
            var createCommand = new CreateProfessorCommand
            {
                UserEmail = "testuser@example.com",
                Name = "Original Professor Name",
                Email = "originalprofessor@example.com"
            };

            var createResponse = await _client.PostAsJsonAsync(BaseUrl, createCommand);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseContent = await createResponse.Content.ReadFromJsonAsync<Guid>();
            var id = responseContent;

            var updateCommand = new UpdateProfessorCommand
            {
                Id = id,
                UserEmail = "testuser@example.com",
                Name = "Updated Professor Name",
                Email = "updatedprofessor@example.com"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"{BaseUrl}/{id}", updateCommand);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task GetAllProfessors_ShouldReturnOk()
        {
            await SetAdminAuthorizationHeader();

            // Arrange
            var userEmail = "testuser@example.com";

            var createCommand1 = new CreateProfessorCommand
            {
                UserEmail = userEmail,
                Name = "Professor 1",
                Email = "prof1@example.com"
            };

            var createCommand2 = new CreateProfessorCommand
            {
                UserEmail = userEmail,
                Name = "Professor 2",
                Email = "prof2@example.com"
            };

            await _client.PostAsJsonAsync(BaseUrl, createCommand1);
            await _client.PostAsJsonAsync(BaseUrl, createCommand2);

            // Act
            var response = await _client.GetAsync($"{BaseUrl}?userEmail={userEmail}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var professors = await response.Content.ReadFromJsonAsync<IEnumerable<ProfessorDto>>();
            professors.Should().NotBeNullOrEmpty();
            professors.All(p => p.UserEmail == userEmail).Should().BeTrue();
        }

        [Fact]
        public async Task GetProfessorById_AsAdmin_ShouldReturnOk()
        {
            await SetAdminAuthorizationHeader();

            // Arrange
            var createCommand = new CreateProfessorCommand
            {
                UserEmail = "testuser@example.com",
                Name = "Professor 1",
                Email = "prof1@example.com"
            };

            var createResponse = await _client.PostAsJsonAsync(BaseUrl, createCommand);
            var responseContent = await createResponse.Content.ReadFromJsonAsync<Guid>();
            var id = responseContent;

            // Act
            var response = await _client.GetAsync($"{BaseUrl}/{id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var professor = await response.Content.ReadFromJsonAsync<ProfessorDto>();
            professor.Should().NotBeNull();
            professor.Id.Should().Be(id);
            professor.Name.Should().Be("Professor 1");
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
