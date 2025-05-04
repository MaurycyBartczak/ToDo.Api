namespace ToDo.Api.DTOs;

/// <summary>
/// DTO reprezentujące szczegółowy widok zadania
/// </summary>
public class ToDoItemDetailDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public int CompletionPercentage { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Dodatkowe informacje dla widoku szczegółowego
    public string Status => IsCompleted ? "Zakończone" : "W trakcie";
    public int DaysRemaining => IsCompleted ? 0 : (int)Math.Ceiling((DueDate - DateTime.Today).TotalDays);
    public bool IsOverdue => !IsCompleted && DueDate < DateTime.Now;
}