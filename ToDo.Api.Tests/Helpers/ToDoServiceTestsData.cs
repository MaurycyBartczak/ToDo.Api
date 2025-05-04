using ToDo.Api.DTOs;
using ToDo.Api.Entities;
namespace ToDo.Api.Tests.Helpers.TestData;

/// <summary>
/// Klasa pomocnicza do testów dostarczająca przykładowe dane testowe
/// </summary>
public static class TodoTestDataFactory
{
    public static ToDoItem GetValidToDoItem()
    {
        return new ToDoItem
        {
            Id = 1,
            Title = "Testowe zadanie",
            Description = "Opis testowego zadania",
            DueDate = DateTime.Now.AddDays(5),
            CompletionPercentage = 0,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };
    }
    
    public static List<ToDoItem> GetSampleToDoItems()
    {
        return new List<ToDoItem>
        {
            new()
            {
                Id = 1,
                Title = "Zadanie 1",
                Description = "Opis zadania 1",
                DueDate = DateTime.Now.AddDays(2),
                CompletionPercentage = 25,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = null
            },
            new()
            {
                Id = 2,
                Title = "Zadanie 2",
                Description = "Opis zadania 2",
                DueDate = DateTime.Now.AddDays(1),
                CompletionPercentage = 50,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = 3,
                Title = "Zadanie 3",
                Description = "Opis zadania 3",
                DueDate = DateTime.Now.AddHours(5),
                CompletionPercentage = 100,
                IsCompleted = true,
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                UpdatedAt = DateTime.UtcNow
            }
        };
    }
    
    public static ToDoItemCreateUpdateDto GetValidToDoItemCreateDto()
    {
        return new ToDoItemCreateUpdateDto
        {
            Title = "Nowe zadanie",
            Description = "Opis nowego zadania",
            DueDate = DateTime.Now.AddDays(5),
            CompletionPercentage = 0
        };
    }
    
    public static ToDoItemCreateUpdateDto GetInvalidToDoItemCreateDto()
    {
        return new ToDoItemCreateUpdateDto
        {
            Title = "", // Puste - niepoprawne
            Description = "Opis zadania",
            DueDate = DateTime.Now.AddDays(-1), // W przeszłości - niepoprawne
            CompletionPercentage = 101 // Poza zakresem - niepoprawne
        };
    }
}