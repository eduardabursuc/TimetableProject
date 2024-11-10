using FluentAssertions;
using System.Net;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Application.DTOs;
using Application.UseCases.Commands;
using Application.UseCases.Queries;
using Domain.Common;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace TimeTable.Application.IntegrationTests
{
    public class ProfessorsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly IMediator _mediator;
        // Cache the JsonSerializerOptions instance
        private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public ProfessorsControllerTests(WebApplicationFactory<Program> factory)
        {
            _mediator = Substitute.For<IMediator>();
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton(_mediator);
                });
            }).CreateClient();
        }

        [Fact]
        public async Task GivenProfessorsExist_WhenGettingAllProfessors_ThenShouldReturnOkResponse()
        {
            // Arrange
            var professors = new List<ProfessorDto>
            {
                new ProfessorDto { Id = Guid.NewGuid(), Name = "Professor 1"},
                new ProfessorDto { Id = Guid.NewGuid(), Name = "Professor 2"}
            };
            _mediator.Send(Arg.Any<GetAllProfessorsQuery>()).Returns(Result<List<ProfessorDto>>.Success(professors));

            // Act
            var response = await _client.GetAsync("/api/professors");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseData = JsonSerializer.Deserialize<List<ProfessorDto>>(await response.Content.ReadAsStringAsync(), JsonSerializerOptions);
            responseData.Should().BeEquivalentTo(professors);
        }

        [Fact]
        public async Task GivenExistingProfessor_WhenGettingProfessorById_ThenShouldReturnOkResponse()
        {
            // Arrange
            var professorId = Guid.NewGuid();
            var professor = new ProfessorDto { Id = professorId, Name = "Professor 1" };
            _mediator.Send(Arg.Any<GetProfessorByIdQuery>()).Returns(Result<ProfessorDto>.Success(professor));

            // Act
            var response = await _client.GetAsync($"/api/professors/{professorId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseData = JsonSerializer.Deserialize<ProfessorDto>(await response.Content.ReadAsStringAsync(), JsonSerializerOptions);
            responseData.Should().BeEquivalentTo(professor);
        }

        [Fact]
        public async Task GivenNonExistingProfessor_WhenGettingProfessorById_ThenShouldReturnNotFoundResponse()
        {
            // Arrange
            var professorId = Guid.NewGuid();
            _mediator.Send(Arg.Any<GetProfessorByIdQuery>()).Returns(Result<ProfessorDto>.Failure("Professor not found"));

            // Act
            var response = await _client.GetAsync($"/api/professors/{professorId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenValidProfessorRequest_WhenCreatingProfessor_ThenShouldReturnCreatedResponse()
        {
            // Arrange
            var newProfessor = new CreateProfessorCommand { Name = "Professor 1" };
            var createdProfessorId = Guid.NewGuid();
            _mediator.Send(Arg.Any<CreateProfessorCommand>()).Returns(Result<Guid>.Success(createdProfessorId));
            var content = new StringContent(JsonSerializer.Serialize(newProfessor), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/professors", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var responseData = await response.Content.ReadAsStringAsync();
            
            // Parse the GUID string from the response
            var parsedGuid = Guid.Parse(responseData.Trim('"')); // Trimming quotes in case response is a quoted GUID string
            parsedGuid.Should().Be(createdProfessorId);
        }


        [Fact]
        public async Task GivenExistingProfessor_WhenDeletingProfessor_ThenShouldReturnNoContentResponse()
        {
            // Arrange
            var professorId = Guid.NewGuid();
            _mediator.Send(Arg.Any<DeleteProfessorCommand>()).Returns(Result<Unit>.Success(new Unit()));

            // Act
            var response = await _client.DeleteAsync($"/api/professors/{professorId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
