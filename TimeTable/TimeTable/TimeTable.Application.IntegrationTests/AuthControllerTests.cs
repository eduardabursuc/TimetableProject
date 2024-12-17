using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Application.UseCases.Authentication;
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
    public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public AuthControllerTests(WebApplicationFactory<Program> factory)
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
        public async Task Register_ShouldReturnOk()
        {
            var command = new RegisterUserCommand
            {
                Email = "testuser@example.com",
                Password = "Test@123",
                AccountType = "user"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/register", command);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
/*
        [Fact]
        public async Task Login_ShouldReturnOk()
        {
            var registerCommand = new RegisterUserCommand
            {
                Email = "testuser@example.com",
                Password = "Test@123",
                AccountType = "user"
            };
            await _client.PostAsJsonAsync("/api/auth/register", registerCommand);

            var loginCommand = new LoginUserCommand
            {
                Email = "testuser@example.com",
                Password = "Test@123"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/login", loginCommand);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseData = JsonSerializer.Deserialize<Result<string>>(
                await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            responseData.Should().NotBeNull();
            responseData!.IsSuccess.Should().BeTrue();
            responseData.Data.Should().NotBeNullOrEmpty();
        }
        
*/
    }
}