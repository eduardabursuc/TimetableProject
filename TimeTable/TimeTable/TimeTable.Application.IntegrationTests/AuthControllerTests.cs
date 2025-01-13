using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.UseCases.Authentication;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace TimeTable.Application.IntegrationTests
{
    public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly ApplicationDbContext _dbContext;
        private readonly AuthorizationService authorizationService = new AuthorizationService();
        private const string BaseUrl = "/api/auth";

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
        public async Task Register_ShouldReturnOk_WhenValidCommand()
        {
            var command = new RegisterUserCommand
            {
                Email = "user@example.com",
                Password = "Password123",
                AccountType = "admin"
            };

            var response = await _client.PostAsJsonAsync($"{BaseUrl}/register", command);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var email = await response.Content.ReadAsStringAsync();
            email.Should().NotBeNull();
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenValidCredentials()
        {
            // Register first
            var registerCommand = new RegisterUserCommand
            {
                Email = "some@example.com",
                Password = "Password123",
                AccountType = "admin"
            };
            await _client.PostAsJsonAsync($"{BaseUrl}/register", registerCommand);

            // Then log in
            var loginCommand = new LoginUserCommand
            {
                Email = "some@example.com",
                Password = "Password123"
            };

            var response = await _client.PostAsJsonAsync($"{BaseUrl}/login", loginCommand);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var token = await response.Content.ReadAsStringAsync();
            token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenInvalidCredentials()
        {
            var command = new LoginUserCommand
            {
                Email = "nonexistentuser@example.com",
                Password = "WrongPassword"
            };

            var response = await _client.PostAsJsonAsync($"{BaseUrl}/login", command);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Refresh_ShouldReturnOk_WhenValidToken()
        {
            await SetAdminAuthorizationHeader();
            
            var registerCommand = new RegisterUserCommand()
            {
                Email = "user@example.com",
                Password = "Password123",
                AccountType = "admin"
            };
            await _client.PostAsJsonAsync($"{BaseUrl}/register", registerCommand);

            var refreshCommand = new RefreshTokenCommand
            {
                Email = "user@example.com"
            };

            var response = await _client.PostAsJsonAsync($"{BaseUrl}/refresh", refreshCommand);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var token = await response.Content.ReadAsStringAsync();
            token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Refresh_ShouldReturnUnauthorized_WhenInvalidToken()
        {
            var command = new RefreshTokenCommand
            {
                Email = "user@example.com"
            };

            var response = await _client.PostAsJsonAsync($"{BaseUrl}/refresh", command);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ResetPassword_ShouldReturnOk_WhenValidEmail()
        {

            var registerCommand = new RegisterUserCommand
            {
                Email = "user@example.com",
                Password = "Password123",
                AccountType = "admin"
            };  
            
            await _client.PostAsJsonAsync($"{BaseUrl}/register", registerCommand);

            
            var command = new ResetPasswordCommand
            {
                Email = "user@example.com"
            };

            var response = await _client.PostAsJsonAsync($"{BaseUrl}/resetPassword", command);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var token = await response.Content.ReadAsStringAsync();
            token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ResetPassword_ShouldReturnUnauthorized_WhenInvalidEmail()
        {
            var command = new ResetPasswordCommand
            {
                Email = "nonexistentuser@example.com"
            };

            var response = await _client.PostAsJsonAsync($"{BaseUrl}/resetPassword", command);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ValidateResetPassword_ShouldReturnOk_WhenAuthorizedAndValidCommand()
        {
            await SetAdminAuthorizationHeader();
            
            var registerCommand = new RegisterUserCommand
            {
                Email = "user@example.com",
                Password = "Password123",
                AccountType = "admin"
            };
            
            await _client.PostAsJsonAsync($"{BaseUrl}/register", registerCommand);
            
            var command = new ValidateResetPasswordCommand
            {
                Email = "user@example.com",
                NewPassword = "newPassword123",
            };

            var response = await _client.PostAsJsonAsync($"{BaseUrl}/validateResetPassword", command);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
        }
        
    }
}
