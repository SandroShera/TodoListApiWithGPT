using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoListApiWithGPT.Controllers;
using TodoListApiWithGPT.Data;
using TodoListApiWithGPT.Models;

namespace TodoListApiWithGPT.Tests
{
    [TestFixture]
    internal class TodoControllerTests
    {
        private TodoController _controller;
        private TodoContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TodoContext>()
                          .UseInMemoryDatabase(databaseName: "TodoDbWithGPT")
                          .Options;

            _context = new TodoContext(options);

            // Seed data for testing
            _context.TodoItems.Add(new TodoItem { Id = 1, Title = "Task 1", Description = "Task 1 Description", IsCompleted = false });
            _context.TodoItems.Add(new TodoItem { Id = 2, Title = "Task 2", Description = "Task 2 Description", IsCompleted = true });
            _context.TodoItems.Add(new TodoItem { Id = 3, Title = "Task 3", Description = "Task 3 Description", IsCompleted = false });
            _context.TodoItems.Add(new TodoItem { Id = 4, Title = "Task 4", Description = "Task 4 Description", IsCompleted = true });
            _context.SaveChanges();

            _controller = new TodoController(_context);
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetTodoItems_ReturnsAllItems()
        {
            // Act
            var result = await _controller.GetTodoItems();

            // Assert
            Assert.IsInstanceOf<ActionResult<IEnumerable<TodoItem>>>(result);
            var items = result.Value as List<TodoItem>;
            Assert.IsNotNull(items, "Expected a non-null result");
            Assert.That(items.Count, Is.EqualTo(4));
        }

        [Test]
        public async Task GetTodoItem_ReturnsItem_WhenItemExists()
        {
            // Act
            var result = await _controller.GetTodoItem(1);

            // Assert
            Assert.IsInstanceOf<ActionResult<TodoItem>>(result);
            var item = result.Value as TodoItem;
            Assert.IsNotNull(item);
            Assert.That(item.Title, Is.EqualTo("Task 1"));
        }

        [Test]
        public async Task PostTodoItem_AddsNewItem()
        {
            // Arrange
            var newItem = new TodoItem { Id = 5, Title = "New Task", Description = "New Task Description" };

            // Act
            var result = await _controller.PostTodoItem(newItem);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);
            var createdItem = (result.Result as CreatedAtActionResult).Value as TodoItem;
            Assert.IsNotNull(createdItem);
            Assert.That(createdItem.Title, Is.EqualTo(newItem.Title));

            // Ensure it's added to the in-memory database
            var items = await _controller.GetTodoItems();
            Assert.That(items.Value.Count(), Is.EqualTo(5));
        }

        [Test]
        public async Task PutTodoItem_UpdatesExistingItem()
        {
            // Arrange
            var updatedItem = new TodoItem { Id = 3, Title = "Updated Task", Description = "Updated Task Description", IsCompleted = true };

            // Act
            var result = await _controller.PutTodoItem(3, updatedItem);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);

            // Ensure the update took place in the in-memory database
            var item = await _controller.GetTodoItem(3);
            Assert.That(item.Value.Title, Is.EqualTo("Updated Task"));
        }

        [Test]
        public async Task DeleteTodoItem_RemovesItem_WhenItemExists()
        {
            // Act
            var result = await _controller.DeleteTodoItem(1);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);

            // Ensure it's removed from the in-memory database
            var items = await _controller.GetTodoItems();
            Assert.That(items.Value.Count(), Is.EqualTo(3));
        }

        [Test]
        public async Task DeleteTodoItem_ReturnsNotFound_WhenItemDoesNotExist()
        {
            // Act
            var result = await _controller.DeleteTodoItem(99);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
    }
}
