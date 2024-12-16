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
    public class GetAllRoomsQueryHandlerTests
    {
        private readonly IRoomRepository _repository = Substitute.For<IRoomRepository>();
        private readonly IMapper _mapper = Substitute.For<IMapper>();

        [Fact]
        public async Task Given_GetAllRoomsQueryHandler_When_HandleIsCalled_Then_AListOfRoomsShouldBeReturned()
        {
            // Arrange
            var rooms = GenerateRoomList();
            _repository.GetAllAsync(Arg.Any<string>()).Returns(Result<IEnumerable<Room>>.Success(rooms));

            var query = new GetAllRoomsQuery { UserEmail = "some1@gmail.com" };
            var roomDtos = GenerateRoomDto(rooms.ToList());
            _mapper.Map<List<RoomDto>>(rooms).Returns(roomDtos);

            var handler = new GetAllRoomsQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(roomDtos.Count, result.Data.Count);
            Assert.Equal(roomDtos[0], result.Data[0]);
            Assert.Equal(roomDtos[1], result.Data[1]);
        }

        [Fact]
        public async Task Given_GetAllRoomsQueryHandler_When_NoRoomsInRepository_Then_EmptyListShouldBeReturned()
        {
            // Arrange
            _repository.GetAllAsync(Arg.Any<string>()).Returns(Result<IEnumerable<Room>>.Success(new List<Room>()));

            var query = new GetAllRoomsQuery { UserEmail = "some1@gmail.com" };
            var handler = new GetAllRoomsQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task Given_GetAllRoomsQueryHandler_When_MappingFails_Then_ExceptionShouldBeThrown()
        {
            // Arrange
            var rooms = GenerateRoomList();
            _repository.GetAllAsync(Arg.Any<string>()).Returns(Result<IEnumerable<Room>>.Success(rooms));

            _mapper.Map<List<RoomDto>>(rooms).Returns(x => throw new Exception("Mapping failed"));

            var query = new GetAllRoomsQuery { UserEmail = "some1@gmail.com" };
            var handler = new GetAllRoomsQueryHandler(_repository, _mapper);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Given_GetAllRoomsQueryHandler_When_HandleIsCalled_Then_FieldsShouldMapCorrectly()
        {
            // Arrange
            var rooms = GenerateRoomList();
            _repository.GetAllAsync(Arg.Any<string>()).Returns(Result<IEnumerable<Room>>.Success(rooms));

            var roomDtos = GenerateRoomDto(rooms.ToList());
            _mapper.Map<List<RoomDto>>(rooms).Returns(roomDtos);

            var query = new GetAllRoomsQuery { UserEmail = "some1@gmail.com" };
            var handler = new GetAllRoomsQueryHandler(_repository, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(roomDtos[0].Id, result.Data[0].Id);
            Assert.Equal(roomDtos[0].Name, result.Data[0].Name);
            Assert.Equal(roomDtos[0].Capacity, result.Data[0].Capacity);
        }

        private static List<Room> GenerateRoomList()
        {
            return new List<Room>
            {
                new Room
                {
                    UserEmail = "some1@gmail.com",
                    Id = Guid.NewGuid(),
                    Name = "Room 1",
                    Capacity = 30
                },
                new Room
                {
                    UserEmail = "some2@gmail.com",
                    Id = Guid.NewGuid(),
                    Name = "Room 2",
                    Capacity = 40
                }
            };
        }

        private static List<RoomDto> GenerateRoomDto(List<Room> rooms)
        {
            return rooms.Select(r => new RoomDto
            {
                UserEmail = r.UserEmail,
                Id = r.Id,
                Name = r.Name,
                Capacity = r.Capacity
            }).ToList();
        }
    }
}