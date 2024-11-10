using Application.DTOs;
using Application.UseCases.Queries;
using Application.UseCases.QueryHandlers;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using NSubstitute;

namespace Application.UnitTests
{
    public class GetProfessorByIdQueryHandlerTests
    {
        private readonly IProfessorRepository repository;
        private readonly IMapper mapper;

        public GetProfessorByIdQueryHandlerTests()
        {
            repository = Substitute.For<IProfessorRepository>();
            mapper = Substitute.For<IMapper>();
        }

        [Fact]
        public async Task Given_GetProfessorByIdQueryHandler_When_HandleIsCalled_Then_ProfessorShouldBeReturned()
        {
            // Arrange
            var professor = GenerateProfessor();
            repository.GetByIdAsync(professor.Id).Returns(Result<Professor>.Success(professor));

            var query = new GetProfessorByIdQuery { Id = professor.Id };
            var professorDto = GenerateProfessorDto(professor);
            mapper.Map<ProfessorDto>(professor).Returns(professorDto);

            var handler = new GetProfessorByIdQueryHandler(repository, mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(professorDto, result.Data);
        }

        [Fact]
        public async Task Given_GetProfessorByIdQueryHandler_When_ProfessorNotFound_Then_FailureResultShouldBeReturned()
        {
            // Arrange
            var professorId = Guid.NewGuid();
            repository.GetByIdAsync(professorId).Returns(Result<Professor>.Failure("Professor not found"));

            var query = new GetProfessorByIdQuery { Id = professorId };
            var handler = new GetProfessorByIdQueryHandler(repository, mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("Professor not found", result.ErrorMessage);
        }

        [Fact]
        public async Task Given_GetProfessorByIdQueryHandler_When_MappingFails_Then_ExceptionShouldBeThrown()
        {
            // Arrange
            var professor = GenerateProfessor();
            repository.GetByIdAsync(professor.Id).Returns(Result<Professor>.Success(professor));

            mapper.Map<ProfessorDto>(professor).Returns(x => { throw new Exception("Mapping failed"); });

            var query = new GetProfessorByIdQuery { Id = professor.Id };
            var handler = new GetProfessorByIdQueryHandler(repository, mapper);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Given_GetProfessorByIdQueryHandler_When_HandleIsCalled_Then_FieldsShouldMapCorrectly()
        {
            // Arrange
            var professor = GenerateProfessor();
            repository.GetByIdAsync(professor.Id).Returns(Result<Professor>.Success(professor));

            var professorDto = GenerateProfessorDto(professor);
            mapper.Map<ProfessorDto>(professor).Returns(professorDto);

            var query = new GetProfessorByIdQuery { Id = professor.Id };
            var handler = new GetProfessorByIdQueryHandler(repository, mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(professorDto.Id, result.Data.Id);
            Assert.Equal(professorDto.Name, result.Data.Name);
        }

        private static Professor GenerateProfessor()
        {
            return new Professor
            {
                Id = Guid.NewGuid(),
                Name = "Professor 1"
            };
        }

        private static ProfessorDto GenerateProfessorDto(Professor professor)
        {
            return new ProfessorDto
            {
                Id = professor.Id,
                Name = professor.Name
            };
        }
    }
}