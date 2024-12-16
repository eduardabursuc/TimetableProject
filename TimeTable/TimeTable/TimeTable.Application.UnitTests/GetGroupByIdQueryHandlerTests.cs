using Application.DTOs;
using Application.UseCases.Queries.GroupQueries;
using Application.UseCases.QueryHandlers.GroupQueryHandlers;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using NSubstitute;

namespace TimeTable.Application.UnitTests
{
    public class GetGroupByIdQueryHandlerTests
    {
        private readonly IGroupRepository _repository = Substitute.For<IGroupRepository>();
        private readonly IMapper _mapper = Substitute.For<IMapper>();

        [Fact]
        public async Task Given_GetGroupByIdQueryHandler_When_HandleIsCalled_Then_GroupShouldBeReturned()
        {
            // Arrange
            var group = GenerateGroup();
            _repository.GetByIdAsync(group.Id).Returns(Result<Group>.Success(group));

            var query = new GetGroupByIdQuery { Id = group.Id };
            var groupDto = GenerateGroupDto(group);
            _mapper.Map<GroupDto>(group).Returns(groupDto);

            var handler = new GetGroupByIdQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(groupDto, result.Data);
        }

        [Fact]
        public async Task Given_GetGroupByIdQueryHandler_When_GroupNotFound_Then_FailureResultShouldBeReturned()
        {
            // Arrange
            var groupId = Guid.NewGuid();
            _repository.GetByIdAsync(groupId).Returns(Result<Group>.Failure("Group not found"));

            var query = new GetGroupByIdQuery { Id = groupId };
            var handler = new GetGroupByIdQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("Group not found", result.ErrorMessage);
        }

        [Fact]
        public async Task Given_GetGroupByIdQueryHandler_When_MappingFails_Then_ExceptionShouldBeThrown()
        {
            // Arrange
            var group = GenerateGroup();
            _repository.GetByIdAsync(group.Id).Returns(Result<Group>.Success(group));

            _mapper.Map<GroupDto>(group).Returns(x => throw new Exception("Mapping failed"));

            var query = new GetGroupByIdQuery { Id = group.Id };
            var handler = new GetGroupByIdQueryHandler(_repository, _mapper);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Given_GetGroupByIdQueryHandler_When_HandleIsCalled_Then_FieldsShouldMapCorrectly()
        {
            // Arrange
            var group = GenerateGroup();
            _repository.GetByIdAsync(group.Id).Returns(Result<Group>.Success(group));

            var groupDto = GenerateGroupDto(group);
            _mapper.Map<GroupDto>(group).Returns(groupDto);

            var query = new GetGroupByIdQuery { Id = group.Id };
            var handler = new GetGroupByIdQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(groupDto.Id, result.Data.Id);
            Assert.Equal(groupDto.Name, result.Data.Name);
        }

        private static Group GenerateGroup()
        {
            return new Group
            {
                UserEmail = "some1@gmail.com",
                Id = Guid.NewGuid(),
                Name = "Group 1"
            };
        }

        private static GroupDto GenerateGroupDto(Group group)
        {
            return new GroupDto
            {
                UserEmail = group.UserEmail,
                Id = group.Id,
                Name = group.Name
            };
        }
    }
}