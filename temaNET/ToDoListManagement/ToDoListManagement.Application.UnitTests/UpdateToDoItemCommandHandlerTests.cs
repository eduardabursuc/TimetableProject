using Application.UseCases.CommandHandlers;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using NSubstitute;
using Xunit;

namespace ToDoListManagement.Application.UnitTests
{
    public class UpdateToDoItemCommandHandlerTests
    {
        private readonly IToDoItemRepository repository;
        private readonly IMapper mapper;

        public UpdateToDoItemCommandHandlerTests()
        {
            repository = Substitute.For<IToDoItemRepository>();
            mapper = Substitute.For<IMapper>();
        }

        [Fact]
        public async Task Given_UpdateToDoItemCommandHandler_When_ItemIsUpdated_Then_ItemShouldBeUpdatedAndIdReturned()
        {
            // Arrange
            var command = new UpdateToDoItemCommand
            {
                Id = Guid.NewGuid(),
                Description = "Updated task",
                DueDate = DateTime.Now.AddDays(2),
                IsDone = true
            };

            var toDoItem = new ToDoItem
            {
                Id = command.Id,
                Description = command.Description,
                DueDate = command.DueDate,
                IsDone = command.IsDone
            };

            mapper.Map<ToDoItem>(command).Returns(toDoItem);

            var handler = new UpdateToDoItemCommandHandler(repository, mapper);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Xunit.Assert.Equal(toDoItem.Id, result);
            await repository.Received(1).UpdateAsync(toDoItem);
        }

        [Fact]
        public async Task Given_UpdateToDoItemCommandHandler_When_MappingFails_Then_ExceptionShouldBeThrown()
        {
            // Arrange
            var command = new UpdateToDoItemCommand
            {
                Id = Guid.NewGuid(),
                Description = "Updated task",
                DueDate = DateTime.Now.AddDays(2),
                IsDone = true
            };

            mapper.Map<ToDoItem>(command).Returns(x => { throw new Exception("Mapping failed"); });

            var handler = new UpdateToDoItemCommandHandler(repository, mapper);

            // Act & Assert
            await Xunit.Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Given_UpdateToDoItemCommandHandler_When_RepositoryThrowsException_Then_ExceptionShouldBeThrown()
        {
            // Arrange
            var command = new UpdateToDoItemCommand
            {
                Id = Guid.NewGuid(),
                Description = "Updated task",
                DueDate = DateTime.Now.AddDays(2),
                IsDone = true
            };

            var toDoItem = new ToDoItem
            {
                Id = command.Id,
                Description = command.Description,
                DueDate = command.DueDate,
                IsDone = command.IsDone
            };

            mapper.Map<ToDoItem>(command).Returns(toDoItem);
            repository.UpdateAsync(toDoItem).Returns(Task.FromException(new Exception("Database error")));

            var handler = new UpdateToDoItemCommandHandler(repository, mapper);

            // Act & Assert
            await Xunit.Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}