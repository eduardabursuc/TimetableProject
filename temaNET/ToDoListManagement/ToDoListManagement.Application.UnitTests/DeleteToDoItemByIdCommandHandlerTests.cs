using Application.UseCases.Commands;
using Application.UseCases.CommandHandlers;
using Domain.Repositories;
using NSubstitute;
using Xunit;

namespace ToDoListManagement.Application.UnitTests
{
    public class DeleteToDoItemByIdCommandHandlerTests
    {
        private readonly IToDoItemRepository repository;

        public DeleteToDoItemByIdCommandHandlerTests()
        {
            repository = Substitute.For<IToDoItemRepository>();
        }

        [Fact]
        public async Task Given_DeleteToDoItemByIdCommandHandler_When_ItemExists_Then_ItemShouldBeDeleted()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var command = new DeleteToDoItemByIdCommand { Id = itemId };

            var handler = new DeleteToDoItemByIdCommandHandler(repository);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).DeleteAsync(itemId);
        }

        [Fact]
        public async Task Given_DeleteToDoItemByIdCommandHandler_When_ItemDoesNotExist_Then_NoExceptionShouldBeThrown()
        {
            // Arrange
            var itemId = Guid.NewGuid(); // ID for a non-existing item
            var command = new DeleteToDoItemByIdCommand { Id = itemId };

            repository.DeleteAsync(itemId).Returns(Task.CompletedTask); // Simulate non-existing item behavior

            var handler = new DeleteToDoItemByIdCommandHandler(repository);

            // Act & Assert
            await handler.Handle(command, CancellationToken.None); // Should complete without exception
            await repository.Received(1).DeleteAsync(itemId);
        }

        [Fact]
        public async Task Given_DeleteToDoItemByIdCommandHandler_When_RepositoryThrowsException_Then_ExceptionShouldBeThrown()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var command = new DeleteToDoItemByIdCommand { Id = itemId };

            repository.DeleteAsync(itemId).Returns(Task.FromException(new Exception("Database error")));

            var handler = new DeleteToDoItemByIdCommandHandler(repository);

            // Act & Assert
            await Xunit.Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}