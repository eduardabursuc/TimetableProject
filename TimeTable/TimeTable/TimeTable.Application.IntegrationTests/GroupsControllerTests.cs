using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Application.DTOs;
using Application.UseCases.Commands.GroupCommands;
using Domain.Repositories;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace TimeTable.Application.IntegrationTests
{
    public class GroupsControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly AuthorizationService authorizationService = new AuthorizationService();
        private readonly ApplicationDbContext _dbContext;
        private readonly HttpClient _client;
        private const string BaseUrl = "/api/v1/groups";

        public GroupsControllerTests(WebApplicationFactory<Program> factory)
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
        public async Task CreateGroup_AsAdmin_ShouldReturnCreated()
        {
            await SetAdminAuthorizationHeader();

            // Arrange
            var command = new CreateGroupCommand
            {
                UserEmail = "testuser@example.com",
                Name = "Group 1"
            };

            // Act
            var response = await _client.PostAsJsonAsync(BaseUrl, command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task DeleteGroup_AsAdmin_ShouldReturnNoContent()
        {
            await SetAdminAuthorizationHeader();

            // Arrange
            var createCommand = new CreateGroupCommand
            {
                UserEmail = "testuser@example.com",
                Name = "Group to Delete"
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
        public async Task UpdateGroup_AsAdmin_ShouldReturnOk()
        {
            await SetAdminAuthorizationHeader();

            // Arrange
            var createCommand = new CreateGroupCommand
            {
                UserEmail = "testuser@example.com",
                Name = "Original Group Name"
            };

            var createResponse = await _client.PostAsJsonAsync(BaseUrl, createCommand);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseContent = await createResponse.Content.ReadFromJsonAsync<Guid>();
            var id = responseContent;

            var updateCommand = new UpdateGroupCommand
            {
                Id = id,
                UserEmail = "testuser@example.com",
                Name = "Updated Group Name"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"{BaseUrl}/{id}", updateCommand);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetAllGroups_AsAdmin_ShouldReturnOk()
        {
            await SetAdminAuthorizationHeader();

            // Arrange
            var userEmail = "testuser@example.com";

            var createCommand1 = new CreateGroupCommand
            {
                UserEmail = userEmail,
                Name = "Group 1"
            };

            var createCommand2 = new CreateGroupCommand
            {
                UserEmail = userEmail,
                Name = "Group 2"
            };

            await _client.PostAsJsonAsync(BaseUrl, createCommand1);
            await _client.PostAsJsonAsync(BaseUrl, createCommand2);

            // Act
            var response = await _client.GetAsync($"{BaseUrl}?userEmail={userEmail}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var groups = await response.Content.ReadFromJsonAsync<IEnumerable<GroupDto>>();
            groups.Should().NotBeNullOrEmpty();
            groups.Should().HaveCount(2);
            groups.All(g => g.UserEmail == userEmail).Should().BeTrue();
        }

        [Fact]
        public async Task GetGroupById_AsAdmin_ShouldReturnOk()
        {
            await SetAdminAuthorizationHeader();

            // Arrange
            var createCommand = new CreateGroupCommand
            {
                UserEmail = "testuser@example.com",
                Name = "Group 1"
            };

            var createResponse = await _client.PostAsJsonAsync(BaseUrl, createCommand);
            var responseContent = await createResponse.Content.ReadFromJsonAsync<Guid>();
            var id = responseContent;

            // Act
            var response = await _client.GetAsync($"{BaseUrl}/{id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var group = await response.Content.ReadFromJsonAsync<GroupDto>();
            group.Should().NotBeNull();
            group.Id.Should().Be(id);
            group.Name.Should().Be("Group 1");
        }
    }
}
