using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class ToDoItemRepository : IToDoItemRepository
    {
        private readonly ApplicationDbContext context;

        public ToDoItemRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Guid> AddAsync(ToDoItem toDoItem)
        {
            await context.ToDoItems.AddAsync(toDoItem);
            await context.SaveChangesAsync();
            return toDoItem.Id;
        }

        public async Task DeleteAsync(Guid id)
        {
            var toDoItem = context.ToDoItems.FirstOrDefault(x => x.Id == id);
            if (toDoItem != null)
            {
                context.ToDoItems.Remove(toDoItem);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ToDoItem>> GetAllAsync()
        {
            return await context.ToDoItems.ToListAsync();
        }

        public async Task<ToDoItem> GetByIdAsync(Guid id)
        {
            return await context.ToDoItems.FindAsync(id);
        }

        public async Task UpdateAsync(ToDoItem toDoItem)
        {
            context.Entry(toDoItem).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
    }
}
