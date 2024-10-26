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
    public class GetToDoItemsQueryHandlerTests
    {
        private readonly IToDoItemRepository repository;
        private readonly IMapper mapper;

        public GetToDoItemsQueryHandlerTests()
        {
            repository = Substitute.For<IToDoItemRepository>();
            mapper = Substitute.For<IMapper>();
        }

        [Fact]
        public async Task Given_GetToDoItemsQueryHandler_When_HandleIsCalled_Then_AToDoListShouldBeReturned()
        {
            // Arrange
            List<ToDoItem> toDoItems = GenerateToDoList();
            repository.GetAllAsync().Returns(toDoItems);

            var query = new GetToDoItemsQuery();
            var toDoItemsDto = GenerateToDoItemDto(toDoItems);
            mapper.Map<List<ToDoItemDTO>>(toDoItems).Returns(toDoItemsDto);

            var handler = new GetToDoItemsQueryHandler(repository, mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Equal(toDoItemsDto.Count, result.Count);
            Xunit.Assert.Equal(toDoItemsDto[0], result[0]);
            Xunit.Assert.Equal(toDoItemsDto[1], result[1]);
        }
        
        [Fact]
        public async Task Given_GetToDoItemsQueryHandler_When_NoItemsInRepository_Then_EmptyListShouldBeReturned()
        {
            // Arrange
            repository.GetAllAsync().Returns(new List<ToDoItem>());
    
            var query = new GetToDoItemsQuery();
            var handler = new GetToDoItemsQueryHandler(repository, mapper);
    
            // Act
            var result = await handler.Handle(query, CancellationToken.None);
    
            // Assert
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Empty(result);
        }
        
        [Fact]
        public async Task Given_GetToDoItemsQueryHandler_When_MappingFails_Then_ExceptionShouldBeThrown()
        {
            // Arrange
            List<ToDoItem> toDoItems = GenerateToDoList();
            repository.GetAllAsync().Returns(toDoItems);

            mapper.Map<List<ToDoItemDTO>>(toDoItems).Returns(x => { throw new Exception("Mapping failed"); });

            var query = new GetToDoItemsQuery();
            var handler = new GetToDoItemsQueryHandler(repository, mapper);

            // Act & Assert
            await Xunit.Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Given_GetToDoItemsQueryHandler_When_HandleIsCalled_Then_FieldsShouldMapCorrectly()
        {
            // Arrange
            List<ToDoItem> toDoItems = GenerateToDoList();
            repository.GetAllAsync().Returns(toDoItems);
    
            var toDoItemsDto = GenerateToDoItemDto(toDoItems);
            mapper.Map<List<ToDoItemDTO>>(toDoItems).Returns(toDoItemsDto);

            var query = new GetToDoItemsQuery();
            var handler = new GetToDoItemsQueryHandler(repository, mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Xunit.Assert.Equal(toDoItemsDto[0].Id, result[0].Id);
            Xunit.Assert.Equal(toDoItemsDto[0].Description, result[0].Description);
            Xunit.Assert.Equal(toDoItemsDto[0].DueDate, result[0].DueDate);
            Xunit.Assert.Equal(toDoItemsDto[0].IsDone, result[0].IsDone);
        }
        

        private List<ToDoItem> GenerateToDoList()
        {
            return new List<ToDoItem>
            {
                new ToDoItem
                {
                    Id = Guid.NewGuid(),
                    Description = "Description for task 1",
                    DueDate = DateTime.Now.AddDays(1),
                    IsDone = false
                },
                new ToDoItem
                {
                    Id = Guid.NewGuid(),
                    Description = "Description for task 2",
                    DueDate = DateTime.Now.AddDays(2),
                    IsDone = true
                }
            };
        }

        private List<ToDoItemDTO> GenerateToDoItemDto(List<ToDoItem> toDoItems)
        {
            return new List<ToDoItemDTO>
            {
                new ToDoItemDTO
                {
                    Id = toDoItems[0].Id,
                    Description = toDoItems[0].Description,
                    DueDate = toDoItems[0].DueDate,
                    IsDone = toDoItems[0].IsDone,
                },
                new ToDoItemDTO
                {
                    Id = toDoItems[1].Id,
                    Description = toDoItems[1].Description,
                    DueDate = toDoItems[1].DueDate,
                    IsDone = toDoItems[1].IsDone,
                }
            };
        }
    }
}
