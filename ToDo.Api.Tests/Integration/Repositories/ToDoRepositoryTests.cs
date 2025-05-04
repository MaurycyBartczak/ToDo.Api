using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ToDo.Api.Data;
using ToDo.Api.Entities;
using ToDo.Api.Repositories;

namespace ToDo.Api.Tests.Integration.Repositories;

public class ToDoRepositoryTests : IDisposable
{
    private readonly ToDoDbContext _context;
    private readonly ToDoRepository _repository;
    
    public ToDoRepositoryTests()
    {
        // Konfiguracja bazy danych InMemory dla testów
        var options = new DbContextOptionsBuilder<ToDoDbContext>()
            .UseInMemoryDatabase(databaseName: $"ToDoDb_{Guid.NewGuid()}")
            .Options;
            
        _context = new ToDoDbContext(options);
        _repository = new ToDoRepository(_context);
        
        // Czyszczenie bazy danych przed każdym testem
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }
    
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
    
    private async Task<List<ToDoItem>> SeedDatabase()
    {
        var items = new List<ToDoItem>
        {
            new()
            {
                Title = "Zadanie testowe 1",
                Description = "Opis zadania testowego 1",
                DueDate = DateTime.Now.AddDays(1),
                CompletionPercentage = 0,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Title = "Zadanie testowe 2",
                Description = "Opis zadania testowego 2",
                DueDate = DateTime.Now.AddDays(2),
                CompletionPercentage = 50,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Title = "Zadanie testowe 3",
                Description = "Opis zadania testowego 3",
                DueDate = DateTime.Now.AddHours(5),
                CompletionPercentage = 100,
                IsCompleted = true,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            }
        };
        
        await _context.ToDoItems.AddRangeAsync(items);
        await _context.SaveChangesAsync();
        
        return items;
    }
    
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllItems()
    {
        // Arrange
        var seededItems = await SeedDatabase();
        
        // Act
        var result = await _repository.GetAllAsync();
        
        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(seededItems.Count);
        result.Select(item => item.Title).Should().Contain(seededItems.Select(item => item.Title));
    }
    
    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnItem()
    {
        // Arrange
        var seededItems = await SeedDatabase();
        var existingItem = seededItems.First();
        
        // Act
        var result = await _repository.GetByIdAsync(existingItem.Id);
        
        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(existingItem.Id);
        result.Title.Should().Be(existingItem.Title);
    }
    
    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        var nonExistingId = 999;
        
        // Act
        var result = await _repository.GetByIdAsync(nonExistingId);
        
        // Assert
        result.Should().BeNull();
    }
    
    [Fact]
    public async Task CreateAsync_ShouldCreateItemAndReturnId()
    {
        // Arrange
        var newItem = new ToDoItem
        {
            Title = "Nowe zadanie",
            Description = "Opis nowego zadania",
            DueDate = DateTime.Now.AddDays(3),
            CompletionPercentage = 0,
            IsCompleted = false
        };
        
        // Act
        var id = await _repository.CreateAsync(newItem);
        
        // Assert
        id.Should().BeGreaterThan(0);
        
        var createdItem = await _context.ToDoItems.FindAsync(id);
        createdItem.Should().NotBeNull();
        createdItem!.Title.Should().Be(newItem.Title);
        createdItem.Description.Should().Be(newItem.Description);
        createdItem.CreatedAt.Should().NotBe(default(DateTime));
    }
    
    [Fact]
    public async Task UpdateAsync_WithExistingItem_ShouldUpdateAndReturnTrue()
    {
        // Arrange
        var seededItems = await SeedDatabase();
        var existingItem = seededItems.First();
        
        existingItem.Title = "Zaktualizowany tytuł";
        existingItem.Description = "Zaktualizowany opis";
        existingItem.CompletionPercentage = 75;
        
        // Act
        var result = await _repository.UpdateAsync(existingItem);
        
        // Assert
        result.Should().BeTrue();
        
        var updatedItem = await _context.ToDoItems.FindAsync(existingItem.Id);
        updatedItem.Should().NotBeNull();
        updatedItem!.Title.Should().Be("Zaktualizowany tytuł");
        updatedItem.Description.Should().Be("Zaktualizowany opis");
        updatedItem.CompletionPercentage.Should().Be(75);
        updatedItem.UpdatedAt.Should().NotBeNull();
    }
    
    [Fact]
    public async Task SetPercentCompleteAsync_WithExistingId_ShouldUpdatePercentage()
    {
        // Arrange
        var seededItems = await SeedDatabase();
        var existingItem = seededItems.First();
        int newPercentage = 75;
        
        // Act
        var result = await _repository.SetPercentCompleteAsync(existingItem.Id, newPercentage);
        
        // Assert
        result.Should().BeTrue();
        
        var updatedItem = await _context.ToDoItems.FindAsync(existingItem.Id);
        updatedItem.Should().NotBeNull();
        updatedItem!.CompletionPercentage.Should().Be(newPercentage);
        updatedItem.UpdatedAt.Should().NotBeNull();
    }
    
    [Fact]
    public async Task SetPercentCompleteAsync_With100Percent_ShouldMarkAsCompleted()
    {
        // Arrange
        var seededItems = await SeedDatabase();
        var existingItem = seededItems.First();
        int newPercentage = 100;
        
        // Act
        var result = await _repository.SetPercentCompleteAsync(existingItem.Id, newPercentage);
        
        // Assert
        result.Should().BeTrue();
        
        var updatedItem = await _context.ToDoItems.FindAsync(existingItem.Id);
        updatedItem.Should().NotBeNull();
        updatedItem!.CompletionPercentage.Should().Be(100);
        updatedItem.IsCompleted.Should().BeTrue();
    }
    
    [Fact]
    public async Task MarkAsDoneAsync_WithExistingId_ShouldMarkItemAsCompletedAnd100Percent()
    {
        // Arrange
        var seededItems = await SeedDatabase();
        var existingItem = seededItems.First();
        
        // Act
        var result = await _repository.MarkAsDoneAsync(existingItem.Id);
        
        // Assert
        result.Should().BeTrue();
        
        var updatedItem = await _context.ToDoItems.FindAsync(existingItem.Id);
        updatedItem.Should().NotBeNull();
        updatedItem!.IsCompleted.Should().BeTrue();
        updatedItem.CompletionPercentage.Should().Be(100);
    }
    
    [Fact]
    public async Task DeleteAsync_WithExistingId_ShouldDeleteItemAndReturnTrue()
    {
        // Arrange
        var seededItems = await SeedDatabase();
        var existingItem = seededItems.First();
        
        // Act
        var result = await _repository.DeleteAsync(existingItem.Id);
        
        // Assert
        result.Should().BeTrue();
        
        var deletedItem = await _context.ToDoItems.FindAsync(existingItem.Id);
        deletedItem.Should().BeNull();
    }
    
    [Fact]
    public async Task GetIncomingAsync_ShouldReturnItemsInDateRange()
    {
        // Arrange
        await SeedDatabase();
        
        // Ponieważ metoda GetIncomingAsync filtruje jedynie zadania, które:
        // 1. Nie są ukończone (IsCompleted = false)
        // 2. Mają termin wykonania w podanym zakresie dat
        // Tworzymy zakres dat który na pewno obejmie przynajmniej jedno zadanie z SeedDatabase
        var startDate = DateTime.Now.Date; // Używamy .Date dla spójności
        var endDate = startDate.AddDays(2); // Obejmie zadania 1 i 2 (nieukończone)
        
        // Act
        var result = await _repository.GetIncomingAsync(startDate, endDate);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThanOrEqualTo(1);
        result.Should().AllSatisfy(item => 
        {
            item.DueDate.Should().BeOnOrAfter(startDate);
            item.DueDate.Should().BeOnOrBefore(endDate);
            item.IsCompleted.Should().BeFalse(); // Sprawdzamy czy wszystkie zadania są nieukończone
        });
    }
}