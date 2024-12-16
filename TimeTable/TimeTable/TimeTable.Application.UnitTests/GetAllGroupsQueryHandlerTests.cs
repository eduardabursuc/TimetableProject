using Application.DTOs;
using Application.UseCases.Queries.GroupQueries;
using Application.UseCases.QueryHandlers.GroupQueryHandlers;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using NSubstitute;
using Xunit;

namespace TimeTable.Application.UnitTests
{
    public class GetAllGroupsQueryHandlerTests
    {
        private readonly IGroupRepository _repository = Substitute.For<IGroupRepository>();
        private readonly IMapper _mapper = Substitute.For<IMapper>();

        [Fact]
        public async Task Given_GetAllGroupsQueryHandler_When_HandleIsCalled_Then_AListOfGroupsShouldBeReturned()
        {
            // Arrange
            var groups = GenerateGroupList();
            _repository.GetAllAsync(Arg.Any<string>()).Returns(Result<IEnumerable<Group>>.Success(groups));

            var query = new GetAllGroupsQuery { UserEmail = "some1@gmail.com" };
            var groupDtos = GenerateGroupDto(groups.ToList());
            _mapper.Map<List<GroupDto>>(groups).Returns(groupDtos);

            var handler = new GetAllGroupsQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(groupDtos.Count, result.Data.Count);
            Assert.Equal(groupDtos[0], result.Data[0]);
            Assert.Equal(groupDtos[1], result.Data[1]);
        }

        [Fact]
        public async Task Given_GetAllGroupsQueryHandler_When_NoGroupsInRepository_Then_EmptyListShouldBeReturned()
        {
            // Arrange
            _repository.GetAllAsync(Arg.Any<string>()).Returns(Result<IEnumerable<Group>>.Success(new List<Group>()));

            var query = new GetAllGroupsQuery { UserEmail = "some1@gmail.com" };
            var handler = new GetAllGroupsQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task Given_GetAllGroupsQueryHandler_When_MappingFails_Then_ExceptionShouldBeThrown()
        {
            // Arrange
            var groups = GenerateGroupList();
            _repository.GetAllAsync(Arg.Any<string>()).Returns(Result<IEnumerable<Group>>.Success(groups));

            _mapper.Map<List<GroupDto>>(groups).Returns(x => throw new Exception("Mapping failed"));

            var query = new GetAllGroupsQuery { UserEmail = "some1@gmail.com" };
            var handler = new GetAllGroupsQueryHandler(_repository, _mapper);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Given_GetAllGroupsQueryHandler_When_HandleIsCalled_Then_FieldsShouldMapCorrectly()
        {
            // Arrange
            var groups = GenerateGroupList();
            _repository.GetAllAsync(Arg.Any<string>()).Returns(Result<IEnumerable<Group>>.Success(groups));

            var groupDtos = GenerateGroupDto(groups.ToList());
            _mapper.Map<List<GroupDto>>(groups).Returns(groupDtos);

            var query = new GetAllGroupsQuery { UserEmail = "some1@gmail.com" };
            var handler = new GetAllGroupsQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(groupDtos[0].Id, result.Data[0].Id);
            Assert.Equal(groupDtos[0].Name, result.Data[0].Name);
        }

        private static List<Group> GenerateGroupList()
        {
            return new List<Group>
            {
                new Group
                {
                    UserEmail = "some1@gmail.com",
                    Id = Guid.NewGuid(),
                    Name = "Group 1",
                },
                new Group
                {
                    UserEmail = "some2@gmail.com",
                    Id = Guid.NewGuid(),
                    Name = "Group 2",
                }
            };
        }

        private static List<GroupDto> GenerateGroupDto(List<Group> groups)
        {
            return groups.Select(g => new GroupDto
            {
                UserEmail = g.UserEmail,
                Id = g.Id,
                Name = g.Name
            }).ToList();
        }
    }
}