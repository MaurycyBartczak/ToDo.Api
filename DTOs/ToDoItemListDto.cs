namespace ToDo.Api.DTOs;

/// <summary>
/// DTO reprezentujące skrócony widok zadania (używane w listach)
/// </summary>
public class ToDoItemListDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public int CompletionPercentage { get; set; }
    public bool IsCompleted { get; set; }
    
    // Dodatkowe informacje dla widoku listy
    public string Status => IsCompleted ? "Zakończone" : "W trakcie";
    public bool IsOverdue => !IsCompleted && DueDate < DateTime.Now;
}