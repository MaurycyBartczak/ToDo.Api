using Microsoft.EntityFrameworkCore;
using ToDo.Api.Data;
using ToDo.Api.Entities;

namespace ToDo.Api.Repositories;

/// <summary>
/// Implementacja repozytorium zadań - odpowiedzialna za operacje bazodanowe
/// </summary>
public class ToDoRepository(ToDoDbContext context) : IToDoRepository
{
    /// <summary>
    /// Pobiera wszystkie zadania z bazy danych
    /// </summary>
    /// <returns>Lista wszystkich zadań</returns>
    public async Task<List<ToDoItem>> GetAllAsync()
    {
        return await context.ToDoItems.ToListAsync();
    }

    /// <summary>
    /// Pobiera zadanie o określonym identyfikatorze
    /// </summary>
    /// <param name="id">Identyfikator zadania</param>
    /// <returns>Znalezione zadanie lub null jeśli nie istnieje</returns>
    public async Task<ToDoItem?> GetByIdAsync(int id)
    {
        return await context.ToDoItems.FindAsync(id);
    }

    /// <summary>
    /// Pobiera nadchodzące zadania w określonym przedziale dat
    /// </summary>
    /// <param name="startDate">Data początkowa</param>
    /// <param name="endDate">Data końcowa</param>
    /// <returns>Lista zadań z terminem wykonania w podanym zakresie</returns>
    /// <remarks>
    /// Zwraca tylko zadania, które nie zostały jeszcze ukończone i posortowane według daty wykonania
    /// </remarks>
    public async Task<List<ToDoItem>> GetIncomingAsync(DateTime startDate, DateTime endDate)
    {
        return await context.ToDoItems
            .Where(t => t.DueDate >= startDate && t.DueDate <= endDate && !t.IsCompleted)
            .OrderBy(t => t.DueDate)
            .ToListAsync();
    }

    /// <summary>
    /// Tworzy nowe zadanie w bazie danych
    /// </summary>
    /// <param name="toDoItem">Dane nowego zadania</param>
    /// <returns>Identyfikator utworzonego zadania</returns>
    /// <remarks>
    /// Automatycznie ustawia datę utworzenia (CreatedAt) na aktualny czas UTC
    /// </remarks>
    public async Task<int> CreateAsync(ToDoItem toDoItem)
    {
        toDoItem.CreatedAt = DateTime.UtcNow;
        context.ToDoItems.Add(toDoItem);
        await context.SaveChangesAsync();
        return toDoItem.Id;
    }

    /// <summary>
    /// Aktualizuje istniejące zadanie
    /// </summary>
    /// <param name="toDoItem">Zadanie ze zaktualizowanymi danymi</param>
    /// <returns>True jeśli aktualizacja się powiodła, false w przeciwnym razie</returns>
    /// <remarks>
    /// Automatycznie ustawia datę aktualizacji (UpdatedAt) na aktualny czas UTC.
    /// Jeśli procent ukończenia jest równy lub większy niż 100%, zadanie zostanie oznaczone jako zakończone.
    /// </remarks>
    public async Task<bool> UpdateAsync(ToDoItem toDoItem)
    {
        var existingItem = await context.ToDoItems.FindAsync(toDoItem.Id);
        if (existingItem == null)
            return false;

        // Przenosimy cały obiekt
        context.Entry(existingItem).CurrentValues.SetValues(toDoItem);
        
        // Upewniamy się, że UpdatedAt jest ustawione
        existingItem.UpdatedAt = DateTime.UtcNow;
        
        // Sprawdzamy czy zadanie powinno być oznaczone jako zakończone
        if (existingItem.CompletionPercentage >= 100)
        {
            existingItem.CompletionPercentage = 100;
            existingItem.IsCompleted = true;
        }

        return await context.SaveChangesAsync() > 0;
    }

    /// <summary>
    /// Usuwa zadanie o określonym identyfikatorze
    /// </summary>
    /// <param name="id">Identyfikator zadania</param>
    /// <returns>True jeśli usunięcie się powiodło, false w przeciwnym razie</returns>
    public async Task<bool> DeleteAsync(int id)
    {
        var toDoItem = await context.ToDoItems.FindAsync(id);
        if (toDoItem == null)
            return false;

        context.ToDoItems.Remove(toDoItem);
        return await context.SaveChangesAsync() > 0;
    }

    /// <summary>
    /// Ustawia procent ukończenia zadania
    /// </summary>
    /// <param name="id">Identyfikator zadania</param>
    /// <param name="percentComplete">Nowy procent ukończenia (0-100)</param>
    /// <returns>True jeśli operacja się powiodła, false w przeciwnym razie</returns>
    /// <remarks>
    /// Automatycznie ustawia datę aktualizacji (UpdatedAt) na aktualny czas UTC.
    /// Jeśli procent wynosi 100, zadanie zostanie także oznaczone jako zakończone.
    /// </remarks>
    public async Task<bool> SetPercentCompleteAsync(int id, int percentComplete)
    {
        var toDoItem = await context.ToDoItems.FindAsync(id);
        if (toDoItem == null)
            return false;

        toDoItem.CompletionPercentage = percentComplete;
        toDoItem.UpdatedAt = DateTime.UtcNow;

        if (percentComplete >= 100)
        {
            toDoItem.CompletionPercentage = 100;
            toDoItem.IsCompleted = true;
        }

        return await context.SaveChangesAsync() > 0;
    }

    /// <summary>
    /// Oznacza zadanie jako wykonane
    /// </summary>
    /// <param name="id">Identyfikator zadania</param>
    /// <returns>True jeśli operacja się powiodła, false w przeciwnym razie</returns>
    /// <remarks>
    /// Operacja ustawia procent ukończenia na 100% i datę aktualizacji na aktualny czas UTC
    /// </remarks>
    public async Task<bool> MarkAsDoneAsync(int id)
    {
        var toDoItem = await context.ToDoItems.FindAsync(id);
        if (toDoItem == null)
            return false;

        toDoItem.IsCompleted = true;
        toDoItem.CompletionPercentage = 100;
        toDoItem.UpdatedAt = DateTime.UtcNow;

        return await context.SaveChangesAsync() > 0;
    }
}