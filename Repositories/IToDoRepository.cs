using ToDo.Api.Entities;

namespace ToDo.Api.Repositories;

/// <summary>
/// Interfejs repozytorium zadań - odpowiedzialny za operacje bazodanowe
/// </summary>
public interface IToDoRepository
{
    /// <summary>
    /// Pobiera wszystkie zadania z bazy danych
    /// </summary>
    /// <returns>Lista wszystkich zadań</returns>
    Task<List<ToDoItem>> GetAllAsync();
    
    /// <summary>
    /// Pobiera zadanie o określonym identyfikatorze
    /// </summary>
    /// <param name="id">Identyfikator zadania</param>
    /// <returns>Znalezione zadanie lub null jeśli nie istnieje</returns>
    Task<ToDoItem?> GetByIdAsync(int id);
    
    /// <summary>
    /// Pobiera nadchodzące zadania w określonym przedziale dat
    /// </summary>
    /// <param name="startDate">Data początkowa</param>
    /// <param name="endDate">Data końcowa</param>
    /// <returns>Lista zadań z terminem wykonania w podanym zakresie</returns>
    Task<List<ToDoItem>> GetIncomingAsync(DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// Tworzy nowe zadanie w bazie danych
    /// </summary>
    /// <param name="toDoItem">Dane nowego zadania</param>
    /// <returns>Identyfikator utworzonego zadania</returns>
    Task<int> CreateAsync(ToDoItem toDoItem);
    
    /// <summary>
    /// Aktualizuje istniejące zadanie
    /// </summary>
    /// <param name="toDoItem">Zadanie ze zaktualizowanymi danymi</param>
    /// <returns>True jeśli aktualizacja się powiodła, false w przeciwnym razie</returns>
    Task<bool> UpdateAsync(ToDoItem toDoItem);
    
    /// <summary>
    /// Usuwa zadanie o określonym identyfikatorze
    /// </summary>
    /// <param name="id">Identyfikator zadania</param>
    /// <returns>True jeśli usunięcie się powiodło, false w przeciwnym razie</returns>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// Ustawia procent ukończenia zadania
    /// </summary>
    /// <param name="id">Identyfikator zadania</param>
    /// <param name="percentComplete">Nowy procent ukończenia (0-100)</param>
    /// <returns>True jeśli operacja się powiodła, false w przeciwnym razie</returns>
    /// <remarks>Jeśli procent wynosi 100, zadanie zostanie także oznaczone jako zakończone</remarks>
    Task<bool> SetPercentCompleteAsync(int id, int percentComplete);
    
    /// <summary>
    /// Oznacza zadanie jako wykonane
    /// </summary>
    /// <param name="id">Identyfikator zadania</param>
    /// <returns>True jeśli operacja się powiodła, false w przeciwnym razie</returns>
    /// <remarks>Operacja ustawia również procent ukończenia na 100%</remarks>
    Task<bool> MarkAsDoneAsync(int id);
}