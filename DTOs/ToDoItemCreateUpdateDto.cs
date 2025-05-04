namespace ToDo.Api.DTOs;

/// <summary>
/// DTO używane przy tworzeniu i aktualizacji zadania
/// </summary>
public class ToDoItemCreateUpdateDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public int CompletionPercentage { get; set; }
}