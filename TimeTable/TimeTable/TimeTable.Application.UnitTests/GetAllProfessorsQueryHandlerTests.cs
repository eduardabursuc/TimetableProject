using Application.DTOs;
using Application.UseCases.Queries.ProfessorQueries;
using Application.UseCases.QueryHandlers.ProfessorQueryHandlers;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using NSubstitute;
using Xunit;

namespace TimeTable.Application.UnitTests
{
    public class GetAllProfessorsQueryHandlerTests
    {
        private readonly IProfessorRepository _repository = Substitute.For<IProfessorRepository>();
        private readonly IMapper _mapper = Substitute.For<IMapper>();

        [Fact]
        public async Task Given_GetAllProfessorsQueryHandler_When_HandleIsCalled_Then_AListOfProfessorsShouldBeReturned()
        {
            // Arrange
            var professors = GenerateProfessorList();
            _repository.GetAllAsync(Arg.Any<string>()).Returns(Result<IEnumerable<Professor>>.Success(professors));

            var query = new GetAllProfessorsQuery { UserEmail = "some1@gmail.com" };
            var professorDtos = GenerateProfessorDto(professors.ToList());
            _mapper.Map<List<ProfessorDto>>(professors).Returns(professorDtos);

            var handler = new GetAllProfessorsQueryHandler(_repository, _mapper);

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
            _repository.GetAllAsync(Arg.Any<string>()).Returns(Result<IEnumerable<Professor>>.Success(new List<Professor>()));

            var query = new GetAllProfessorsQuery { UserEmail = "some1@gmail.com" };
            var handler = new GetAllProfessorsQueryHandler(_repository, _mapper);

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
            var professors = GenerateProfessorList();
            _repository.GetAllAsync(Arg.Any<string>()).Returns(Result<IEnumerable<Professor>>.Success(professors));

            _mapper.Map<List<ProfessorDto>>(professors).Returns(x => throw new Exception("Mapping failed"));

            var query = new GetAllProfessorsQuery { UserEmail = "some1@gmail.com" };
            var handler = new GetAllProfessorsQueryHandler(_repository, _mapper);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Given_GetAllProfessorsQueryHandler_When_HandleIsCalled_Then_FieldsShouldMapCorrectly()
        {
            // Arrange
            var professors = GenerateProfessorList();
            _repository.GetAllAsync(Arg.Any<string>()).Returns(Result<IEnumerable<Professor>>.Success(professors));

            var professorDtos = GenerateProfessorDto(professors.ToList());
            _mapper.Map<List<ProfessorDto>>(professors).Returns(professorDtos);

            var query = new GetAllProfessorsQuery { UserEmail = "some1@gmail.com" };
            var handler = new GetAllProfessorsQueryHandler(_repository, _mapper);

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
                    UserEmail = "some1@gmail.com",
                    Id = Guid.NewGuid(),
                    Name = "Professor 1",
                },
                new Professor
                {
                    UserEmail = "some2@gmail.com",
                    Id = Guid.NewGuid(),
                    Name = "Professor 2",
                }
            };
        }

        private static List<ProfessorDto> GenerateProfessorDto(List<Professor> professors)
        {
            return professors.Select(p => new ProfessorDto
            {
                UserEmail = p.UserEmail,
                Id = p.Id,
                Name = p.Name
            }).ToList();
        }
    }
}