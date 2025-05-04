using System.ComponentModel.DataAnnotations;

namespace ToDo.Api.Entities;

/// <summary>
/// Model reprezentujący zadanie do wykonania w systemie ToDo
/// </summary>
public class ToDoItem
{
    /// <summary>
    /// Unikalny identyfikator zadania
    /// </summary>
    [Key] public int Id { get; set; }
    
    /// <summary>
    /// Tytuł zadania (maksymalnie 100 znaków)
    /// </summary>
    [StringLength(100)] public required string Title { get; set; }
    
    /// <summary>
    /// Szczegółowy opis zadania (maksymalnie 500 znaków)
    /// </summary>
    [StringLength(500)] public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Data, do której zadanie powinno zostać wykonane
    /// Format: RRRR-MM-DDThh:mm:ss (np. 2025-05-15T14:30:00) zgodny ze standardem ISO 8601
    /// </summary>
    public required DateTime DueDate { get; set; }
    
    /// <summary>
    /// Procent wykonania zadania (wartość od 0 do 100)
    /// </summary>
    public int CompletionPercentage { get; set; }
    
    /// <summary>
    /// Flaga określająca, czy zadanie zostało ukończone
    /// </summary>
    public bool IsCompleted { get; set; }
    
    /// <summary>
    /// Data utworzenia zadania
    /// Format: RRRR-MM-DDThh:mm:ss (np. 2025-05-15T14:30:00) zgodny ze standardem ISO 8601
    /// Przechowywana w formacie UTC
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Data ostatniej aktualizacji zadania (null jeśli nie było aktualizacji)
    /// Format: RRRR-MM-DDThh:mm:ss (np. 2025-05-15T14:30:00) zgodny ze standardem ISO 8601
    /// Przechowywana w formacie UTC
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}