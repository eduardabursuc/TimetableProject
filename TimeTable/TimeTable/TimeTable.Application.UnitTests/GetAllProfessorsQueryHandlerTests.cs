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
    public class GetAllProfessorsQueryHandlerTests
    {
        private readonly IProfessorRepository repository;
        private readonly IMapper mapper;

        public GetAllProfessorsQueryHandlerTests()
        {
            repository = Substitute.For<IProfessorRepository>();
            mapper = Substitute.For<IMapper>();
        }

        [Fact]
        public async Task Given_GetAllProfessorsQueryHandler_When_HandleIsCalled_Then_AListOfProfessorsShouldBeReturned()
        {
            // Arrange
            List<Professor> professors = GenerateProfessorList();
            repository.GetAllAsync().Returns(Result<IEnumerable<Professor>>.Success(professors));

            var query = new GetAllProfessorsQuery();
            var professorDtos = GenerateProfessorDto(professors);
            mapper.Map<List<ProfessorDto>>(professors).Returns(professorDtos);

            var handler = new GetAllProfessorsQueryHandler(repository, mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(professorDtos.Count, result.Data.Count);
            Assert.Equal(professorDtos[0], result.Data[0]);
            Assert.Equal(professorDtos[1], result.Data[1]);
        }

        [Fact]
        public async Task Given_GetAllProfessorsQueryHandler_When_NoProfessorsInRepository_Then_EmptyListShouldBeReturned()
        {
            // Arrange
            repository.GetAllAsync().Returns(Result<IEnumerable<Professor>>.Success(new List<Professor>()));

            var query = new GetAllProfessorsQuery();
            var handler = new GetAllProfessorsQueryHandler(repository, mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task Given_GetAllProfessorsQueryHandler_When_MappingFails_Then_ExceptionShouldBeThrown()
        {
            // Arrange
            List<Professor> professors = GenerateProfessorList();
            repository.GetAllAsync().Returns(Result<IEnumerable<Professor>>.Success(professors));

            mapper.Map<List<ProfessorDto>>(professors).Returns(x => { throw new Exception("Mapping failed"); });

            var query = new GetAllProfessorsQuery();
            var handler = new GetAllProfessorsQueryHandler(repository, mapper);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Given_GetAllProfessorsQueryHandler_When_HandleIsCalled_Then_FieldsShouldMapCorrectly()
        {
            // Arrange
            List<Professor> professors = GenerateProfessorList();
            repository.GetAllAsync().Returns(Result<IEnumerable<Professor>>.Success(professors));

            var professorDtos = GenerateProfessorDto(professors);
            mapper.Map<List<ProfessorDto>>(professors).Returns(professorDtos);

            var query = new GetAllProfessorsQuery();
            var handler = new GetAllProfessorsQueryHandler(repository, mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(professorDtos[0].Id, result.Data[0].Id);
            Assert.Equal(professorDtos[0].Name, result.Data[0].Name);
        }

        private static List<Professor> GenerateProfessorList()
        {
            return new List<Professor>
            {
                new Professor
                {
                    Id = Guid.NewGuid(),
                    Name = "Professor 1",
                },
                new Professor
                {
                    Id = Guid.NewGuid(),
                    Name = "Professor 2",
                }
            };
        }

        private static List<ProfessorDto> GenerateProfessorDto(List<Professor> professors)
        {
            return new List<ProfessorDto>
            {
                new ProfessorDto
                {
                    Id = professors[0].Id,
                    Name = professors[0].Name
                },
                new ProfessorDto
                {
                    Id = professors[1].Id,
                    Name = professors[1].Name
                }
            };
        }
    }
}