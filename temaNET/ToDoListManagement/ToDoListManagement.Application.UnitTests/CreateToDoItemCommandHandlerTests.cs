using Application.UseCases.Commands;
using Application.UseCases.CommandHandlers;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using NSubstitute;
using Xunit;

namespace ToDoListManagement.Application.UnitTests
{
    public class CreateToDoItemCommandHandlerTests
    {
        private readonly IToDoItemRepository repository;
        private readonly IMapper mapper;

        public CreateToDoItemCommandHandlerTests()
        {
            repository = Substitute.For<IToDoItemRepository>();
            mapper = Substitute.For<IMapper>();
        }

        [Fact]
        public async Task Given_CreateToDoItemCommandHandler_When_HandleIsCalled_Then_ItemShouldBeCreated()
        {
            // Arrange
            var command = new CreateToDoItemCommand
            {
                Description = "Sample task",
                DueDate = DateTime.Now.AddDays(1),
                IsDone = false
            };
            
            var toDoItem = new ToDoItem
            {
                Id = Guid.NewGuid(),
                Description = command.Description,
                DueDate = command.DueDate,
                IsDone = command.IsDone
            };

            mapper.Map<ToDoItem>(command).Returns(toDoItem);
            repository.AddAsync(toDoItem).Returns(toDoItem.Id);

            var handler = new CreateToDoItemCommandHandler(repository, mapper);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Xunit.Assert.Equal(toDoItem.Id, result);
            await repository.Received(1).AddAsync(toDoItem);
        }

        [Fact]
        public async Task Given_CreateToDoItemCommandHandler_When_MappingFails_Then_ExceptionShouldBeThrown()
        {
            // Arrange
            var command = new CreateToDoItemCommand
            {
                Description = "Sample task",
                DueDate = DateTime.Now.AddDays(1),
                IsDone = false
            };

            mapper.Map<ToDoItem>(command).Returns(x => { throw new Exception("Mapping failed"); });

            var handler = new CreateToDoItemCommandHandler(repository, mapper);

            // Act & Assert
            await Xunit.Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Given_CreateToDoItemCommandHandler_When_RepositoryThrowsException_Then_ExceptionShouldBeThrown()
        {
            // Arrange
            var command = new CreateToDoItemCommand
            {
                Description = "Sample task",
                DueDate = DateTime.Now.AddDays(1),
                IsDone = false
            };

            var toDoItem = new ToDoItem
            {
                Id = Guid.NewGuid(),
                Description = command.Description,
                DueDate = command.DueDate,
                IsDone = command.IsDone
            };

            mapper.Map<ToDoItem>(command).Returns(toDoItem);
            repository.AddAsync(toDoItem).Returns(Task.FromException<Guid>(new Exception("Database error")));

            var handler = new CreateToDoItemCommandHandler(repository, mapper);

            // Act & Assert
            await Xunit.Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}