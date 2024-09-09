using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoListApiWithGPT.Controllers;
using TodoListApiWithGPT.Data.Interfaces;
using TodoListApiWithGPT.Models;

namespace TodoListApiWithGPT.Tests
{
    [TestFixture]
    public class TodoControllerTests
    {
        private Mock<ITodoRepository> _mockRepository;
        private TodoController _controller;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<ITodoRepository>();
            _controller = new TodoController(_mockRepository.Object);
        }

        [Test]
        public async Task GetTodoItems_ReturnsAllItems()
        {
            // Arrange
            var todoItems = new List<TodoItem>
            {
                new TodoItem { Id = 1, Title = "Task 1", Description = "Task 1 Description", IsCompleted = false },
                new TodoItem { Id = 2, Title = "Task 2", Description = "Task 2 Description", IsCompleted = true }
            };
            _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(todoItems);

            // Act
            var result = await _controller.GetTodoItems();

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var items = okResult.Value as IEnumerable<TodoItem>;
            Assert.IsNotNull(items);
            Assert.AreEqual(todoItems.Count, items.Count());
        }

        [Test]
        public async Task GetTodoItem_ReturnsItem_WhenItemExists()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Title = "Task 1", Description = "Task 1 Description", IsCompleted = false };
            _mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(todoItem);

            // Act
            var result = await _controller.GetTodoItem(1);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var item = okResult.Value as TodoItem;
            Assert.IsNotNull(item);
            Assert.AreEqual(todoItem.Title, item.Title);
        }

        [Test]
        public async Task GetTodoItem_ReturnsNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((TodoItem)null);

            // Act
            var result = await _controller.GetTodoItem(1);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task PostTodoItem_CreatesNewItem()
        {
            // Arrange
            var newItem = new TodoItem { Id = 3, Title = "New Task", Description = "New Task Description" };
            _mockRepository.Setup(repo => repo.AddAsync(newItem)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.PostTodoItem(newItem);

            // Assert
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            var item = createdResult.Value as TodoItem;
            Assert.IsNotNull(item);
            Assert.AreEqual(newItem.Title, item.Title);
        }

        [Test]
        public async Task PutTodoItem_UpdatesExistingItem()
        {
            // Arrange
            var updatedItem = new TodoItem { Id = 1, Title = "Updated Task", Description = "Updated Task Description" };
            _mockRepository.Setup(repo => repo.ExistsAsync(1)).ReturnsAsync(true);
            _mockRepository.Setup(repo => repo.UpdateAsync(updatedItem)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.PutTodoItem(1, updatedItem);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task PutTodoItem_ReturnsNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            var updatedItem = new TodoItem { Id = 1, Title = "Updated Task", Description = "Updated Task Description" };
            _mockRepository.Setup(repo => repo.ExistsAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _controller.PutTodoItem(1, updatedItem);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task DeleteTodoItem_RemovesItem_WhenItemExists()
        {
            // Arrange
            var idToDelete = 1;
            _mockRepository.Setup(repo => repo.ExistsAsync(idToDelete)).ReturnsAsync(true);
            _mockRepository.Setup(repo => repo.DeleteAsync(idToDelete)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteTodoItem(idToDelete);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task DeleteTodoItem_ReturnsNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            var idToDelete = 1;
            _mockRepository.Setup(repo => repo.ExistsAsync(idToDelete)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteTodoItem(idToDelete);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
    }
}
