using Microsoft.EntityFrameworkCore;
using TodoListApiWithGPT.Models;

namespace TodoListApiWithGPT.Data
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options) : base(options) { }

        public DbSet<TodoItem> TodoItems { get; set; }
    }
}
