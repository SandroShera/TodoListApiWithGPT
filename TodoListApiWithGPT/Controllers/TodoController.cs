using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoListApiWithGPT.Data;
using TodoListApiWithGPT.Models;

namespace TodoListApiWithGPT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
            private readonly TodoContext _context;

            public TodoController(TodoContext context)
            {
                _context = context;
            }

            [HttpGet]
            public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
            {
                return await _context.TodoItems.ToListAsync();
            }

            [HttpGet("{id}")]
            public async Task<ActionResult<TodoItem>> GetTodoItem(int id)
            {
            // Use AsNoTracking to ensure the entity is not being tracked
            var item = await _context.TodoItems.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            return item;
        }

            [HttpPost]
            public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
            {
                await _context.TodoItems.AddAsync(todoItem);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
            }

            [HttpPut("{id}")]
            public async Task<IActionResult> PutTodoItem(int id, TodoItem item)
            {
            if (id != item.Id)
            {
                return BadRequest();
            }

            // Check if the item exists and retrieve it for updating
            var existingItem = await _context.TodoItems.FindAsync(id);
            if (existingItem == null)
            {
                return NotFound();
            }

            // Update the existing item
            existingItem.Title = item.Title;
            existingItem.Description = item.Description;
            existingItem.IsCompleted = item.IsCompleted;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteTodoItem(int id)
            {
                var todoItem = await _context.TodoItems.FindAsync(id);
                if (todoItem == null)
                {
                    return NotFound();
                }

                _context.TodoItems.Remove(todoItem);
                await _context.SaveChangesAsync();
                return NoContent();
            }

            private bool TodoItemExists(int id)
            {
                return _context.TodoItems.Any(e => e.Id == id);
            }
        }
}
