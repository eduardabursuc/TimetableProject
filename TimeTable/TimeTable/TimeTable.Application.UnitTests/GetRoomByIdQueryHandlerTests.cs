using Application.DTOs;
using Application.UseCases.Queries.RoomQueries;
using Application.UseCases.QueryHandlers.RoomQueryHandlers;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using NSubstitute;
using Xunit;

namespace TimeTable.Application.UnitTests
{
    public class GetRoomByIdQueryHandlerTests
    {
        private readonly IRoomRepository _repository = Substitute.For<IRoomRepository>();
        private readonly IMapper _mapper = Substitute.For<IMapper>();

        [Fact]
        public async Task Given_GetRoomByIdQueryHandler_When_HandleIsCalled_Then_RoomShouldBeReturned()
        {
            // Arrange
            var room = GenerateRoom();
            _repository.GetByIdAsync(room.Id).Returns(Result<Room>.Success(room));

            var query = new GetRoomByIdQuery { Id = room.Id };
            var roomDto = GenerateRoomDto(room);
            _mapper.Map<RoomDto>(room).Returns(roomDto);

            var handler = new GetRoomByIdQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(roomDto, result.Data);
        }

        [Fact]
        public async Task Given_GetRoomByIdQueryHandler_When_RoomNotFound_Then_FailureResultShouldBeReturned()
        {
            // Arrange
            var roomId = Guid.NewGuid();
            _repository.GetByIdAsync(roomId).Returns(Result<Room>.Failure("Room not found"));

            var query = new GetRoomByIdQuery { Id = roomId };
            var handler = new GetRoomByIdQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("Room not found", result.ErrorMessage);
        }

        [Fact]
        public async Task Given_GetRoomByIdQueryHandler_When_MappingFails_Then_ExceptionShouldBeThrown()
        {
            // Arrange
            var room = GenerateRoom();
            _repository.GetByIdAsync(room.Id).Returns(Result<Room>.Success(room));

            _mapper.Map<RoomDto>(room).Returns(x => throw new Exception("Mapping failed"));

            var query = new GetRoomByIdQuery { Id = room.Id };
            var handler = new GetRoomByIdQueryHandler(_repository, _mapper);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Given_GetRoomByIdQueryHandler_When_HandleIsCalled_Then_FieldsShouldMapCorrectly()
        {
            // Arrange
            var room = GenerateRoom();
            _repository.GetByIdAsync(room.Id).Returns(Result<Room>.Success(room));

            var roomDto = GenerateRoomDto(room);
            _mapper.Map<RoomDto>(room).Returns(roomDto);

            var query = new GetRoomByIdQuery { Id = room.Id };
            var handler = new GetRoomByIdQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(roomDto.Id, result.Data.Id);
            Assert.Equal(roomDto.Name, result.Data.Name);
            Assert.Equal(roomDto.Capacity, result.Data.Capacity);
        }

        private static Room GenerateRoom()
        {
            return new Room
            {
                UserEmail = "some1@gmail.com",
                Id = Guid.NewGuid(),
                Name = "Room 1",
                Capacity = 30
            };
        }

        private static RoomDto GenerateRoomDto(Room room)
        {
            return new RoomDto
            {
                UserEmail = room.UserEmail,
                Id = room.Id,
                Name = room.Name,
                Capacity = room.Capacity
            };
        }
    }
}