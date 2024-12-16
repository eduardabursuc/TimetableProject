// using System.Net;
// using System.Net.Http.Json;
// using System.Text.Json;
// using Application.UseCases.Commands.ConstraintCommands;
// using Domain.Common;
// using Domain.Entities;
// using FluentAssertions;
// using Infrastructure.Persistence;
// using Microsoft.AspNetCore.Mvc.Testing;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using Xunit;
//
// namespace TimeTable.Application.IntegrationTests
// {
//     public class ConstraintsControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
//     {
//         private readonly WebApplicationFactory<Program> _factory;
//         private readonly HttpClient _client;
//         private const string BaseUrl = "/api/v1/constraints";
//
//         private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
//         {
//             PropertyNameCaseInsensitive = true
//         };
//
//         public ConstraintsControllerTests(WebApplicationFactory<Program> factory)
//         {
//             _factory = factory.WithWebHostBuilder(builder =>
//             {
//                 builder.ConfigureAppConfiguration((context, config) =>
//                 {
//                     config.AddJsonFile("appsettings.Test.json");
//                 });
//
//                 builder.ConfigureServices(services =>
//                 {
//                     var descriptor = services.SingleOrDefault(
//                         d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
//
//                     if (descriptor != null)
//                     {
//                         services.Remove(descriptor);
//                     }
//
//                     services.AddDbContext<ApplicationDbContext>(options =>
//                     {
//                         var serviceProvider = services.BuildServiceProvider();
//                         var config = serviceProvider.GetRequiredService<IConfiguration>();
//                         var useInMemory = config.GetValue<bool>("UseInMemoryDatabase");
//
//                         if (useInMemory)
//                         {
//                             options.UseInMemoryDatabase("TestDatabase");
//                         }
//                         else
//                         {
//                             options.UseNpgsql(
//                                 config.GetConnectionString("DefaultConnection"),
//                                 b => b.MigrationsAssembly("Infrastructure"));
//                         }
//                     });
//
//                     var sp = services.BuildServiceProvider();
//
//                     using (var scope = sp.CreateScope())
//                     {
//                         var scopedServices = scope.ServiceProvider;
//                         var db = scopedServices.GetRequiredService<ApplicationDbContext>();
//                         db.Database.EnsureCreated();
//                     }
//                 });
//             });
//
//             _client = _factory.CreateClient();
//         }
//
//         public void Dispose()
//         {
//             var scope = _factory.Services.CreateScope();
//             var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//             dbContext.Database.EnsureDeleted();
//             dbContext.Dispose();
//
//             GC.SuppressFinalize(this);
//         }
//
//         [Fact]
//         public async Task CreateConstraint_ShouldReturnCreated()
//         {
//             var command = new CreateConstraintCommand
//             {
//                 Type = ConstraintType.HARD_NO_OVERLAP,
//                 ProfessorId = Guid.NewGuid(),
//                 CourseName = "Course 101",
//                 RoomName = "Room A",
//                 GroupName = "Group 1",
//                 Day = "Monday",
//                 Time = "08:00-10:00"
//             };
//
//             var response = await _client.PostAsJsonAsync(BaseUrl, command);
//             response.StatusCode.Should().Be(HttpStatusCode.Created);
//         }
//
//         [Fact]
//         public async Task GetConstraintById_ShouldReturnOk()
//         {
//             var id = Guid.NewGuid();
//             var command = new CreateConstraintCommand
//             {
//                 Type = ConstraintType.HARD_NO_OVERLAP,
//                 ProfessorId = Guid.NewGuid(),
//                 CourseName = "Course 101",
//                 RoomName = "Room A",
//                 GroupName = "Group 1",
//                 Day = "Monday",
//                 Time = "08:00-10:00"
//             };
//
//             await _client.PostAsJsonAsync(BaseUrl, command);
//
//             var response = await _client.GetAsync($"{BaseUrl}/{id}");
//             response.StatusCode.Should().Be(HttpStatusCode.OK);
//         }
//
//         [Fact]
//         public async Task DeleteConstraint_ShouldReturnNoContent()
//         {
//             var id = Guid.NewGuid();
//             var command = new CreateConstraintCommand
//             {
//                 Type = ConstraintType.HARD_NO_OVERLAP,
//                 ProfessorId = Guid.NewGuid(),
//                 CourseName = "Course 101",
//                 RoomName = "Room A",
//                 GroupName = "Group 1",
//                 Day = "Monday",
//                 Time = "08:00-10:00"
//             };
//
//             await _client.PostAsJsonAsync(BaseUrl, command);
//
//             var response = await _client.DeleteAsync($"{BaseUrl}/{id}");
//             response.StatusCode.Should().Be(HttpStatusCode.NoContent);
//         }
//     }
// }