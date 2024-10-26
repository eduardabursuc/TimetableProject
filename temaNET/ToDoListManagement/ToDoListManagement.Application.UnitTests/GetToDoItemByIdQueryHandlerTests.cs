using Application.DTOs;
using NSubstitute;
using Xunit;
using Domain.Entities;
using Domain.Repositories;
using Application.UseCases.Queries;
using Application.UseCases.QueryHandlers;
using AutoMapper;

namespace ToDoListManagement.Application.UnitTests
{
    public class GetToDoItemByIdQueryHandlerTests
    {
        private readonly IToDoItemRepository repository;
        private readonly IMapper mapper;

        public GetToDoItemByIdQueryHandlerTests()
        {
            repository = Substitute.For<IToDoItemRepository>();
            mapper = Substitute.For<IMapper>();
        }

        [Fact]
        public async Task Given_GetToDoItemByIdQueryHandler_When_ItemExists_Then_ItemShouldBeReturned()
        {
            // Arrange
            var toDoItem = new ToDoItem
            {
                Id = Guid.NewGuid(),
                Description = "Sample task",
                DueDate = DateTime.Now.AddDays(1),
                IsDone = false
            };
            var toDoItemDto = new ToDoItemDTO
            {
                Id = toDoItem.Id,
                Description = toDoItem.Description,
                DueDate = toDoItem.DueDate,
                IsDone = toDoItem.IsDone
            };

            repository.GetByIdAsync(toDoItem.Id).Returns(Task.FromResult(toDoItem));
            mapper.Map<ToDoItemDTO>(toDoItem).Returns(toDoItemDto);

            var query = new GetToDoItemByIdQuery { Id = toDoItem.Id };
            var handler = new GetToDoItemByIdQueryHandler(repository, mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Equal(toDoItemDto.Id, result.Id);
            Xunit.Assert.Equal(toDoItemDto.Description, result.Description);
        }

        [Fact]
        public async Task Given_GetToDoItemByIdQueryHandler_When_ItemDoesNotExist_Then_NullShouldBeReturned()
        {
            // Arrange
            var itemId = Guid.NewGuid(); // ID for a non-existing item
            repository.GetByIdAsync(itemId).Returns(Task.FromResult<ToDoItem>(null)); // Returns null for non-existing item

            var query = new GetToDoItemByIdQuery { Id = itemId };
            var handler = new GetToDoItemByIdQueryHandler(repository, mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Xunit.Assert.Null(result); // Expecting null since the item does not exist
        }

        [Fact]
        public async Task Given_GetToDoItemByIdQueryHandler_When_MappingFails_Then_ExceptionShouldBeThrown()
        {
            // Arrange
            var toDoItem = new ToDoItem
            {
                Id = Guid.NewGuid(),
                Description = "Sample task",
                DueDate = DateTime.Now.AddDays(1),
                IsDone = false
            };

            repository.GetByIdAsync(toDoItem.Id).Returns(Task.FromResult(toDoItem));
            mapper.Map<ToDoItemDTO>(toDoItem).Returns(x => { throw new Exception("Mapping failed"); });

            var query = new GetToDoItemByIdQuery { Id = toDoItem.Id };
            var handler = new GetToDoItemByIdQueryHandler(repository, mapper);

            // Act & Assert
            await Xunit.Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Given_GetToDoItemByIdQueryHandler_When_RepositoryThrowsException_Then_ExceptionShouldBeThrown()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            repository.GetByIdAsync(itemId).Returns(Task.FromException<ToDoItem>(new Exception("Database error")));

            var query = new GetToDoItemByIdQuery { Id = itemId };
            var handler = new GetToDoItemByIdQueryHandler(repository, mapper);

            // Act & Assert
            await Xunit.Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
        }
    }
}
