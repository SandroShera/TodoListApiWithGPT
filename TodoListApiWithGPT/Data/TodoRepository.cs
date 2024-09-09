using Microsoft.EntityFrameworkCore;
using TodoListApiWithGPT.Data.Interfaces;
using TodoListApiWithGPT.Models;

namespace TodoListApiWithGPT.Data
{
    public class TodoRepository : ITodoRepository
    {
        private readonly TodoContext _context;


        public TodoRepository(TodoContext context)
        {
                _context = context;
        }

        public async Task AddAsync(TodoItem item)
        {
            await _context.TodoItems.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _context.TodoItems.FindAsync(id);
            if (item != null)
            {
                _context.TodoItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.TodoItems.AnyAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<TodoItem>> GetAllAsync()
        {
            return await _context.TodoItems.ToListAsync();
        }

        public async Task<TodoItem> GetByIdAsync(int id)
        {
            return await _context.TodoItems.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task UpdateAsync(TodoItem item)
        {
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
