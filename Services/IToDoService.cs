using ToDo.Api.DTOs;
using ToDo.Api.Entities;

namespace ToDo.Api.Services;

/// <summary>
/// Interfejs serwisu operacji na zadaniach ToDo
/// </summary>
public interface IToDoService
{
    /// <summary>
    /// Pobiera wszystkie zadania
    /// </summary>
    /// <returns>Lista wszystkich zadań w formacie uproszczonym do wyświetlania w listach</returns>
    Task<List<ToDoItemListDto>> GetAllAsync();

    /// <summary>
    /// Pobiera szczegóły pojedynczego zadania po ID
    /// </summary>
    /// <param name="id">ID zadania</param>
    /// <returns>Odpowiedź zawierająca szczegóły zadania lub informację o błędzie</returns>
    Task<ServiceResponse<ToDoItemDetailDto>> GetByIdAsync(int id);

    /// <summary>
    /// Pobiera nadchodzące zadania w określonym przedziale czasowym
    /// </summary>
    /// <param name="timeFrame">Przedział czasowy: "today", "tomorrow" lub "week"</param>
    /// <returns>Lista nadchodzących zadań w formacie uproszczonym</returns>
    /// <exception cref="ArgumentException">Rzucany gdy podano niepoprawny przedział czasowy</exception>
    Task<List<ToDoItemListDto>> GetIncomingAsync(string timeFrame);

    /// <summary>
    /// Tworzy nowe zadanie
    /// </summary>
    /// <param name="todo">Dane nowego zadania</param>
    /// <returns>Odpowiedź zawierająca ID utworzonego zadania lub informacje o błędach walidacji</returns>
    Task<ServiceResponse<int>> CreateAsync(ToDoItemCreateUpdateDto todo);

    /// <summary>
    /// Aktualizuje istniejące zadanie
    /// </summary>
    /// <param name="id">ID zadania do aktualizacji</param>
    /// <param name="todo">Nowe dane zadania</param>
    /// <returns>Odpowiedź zawierająca zaktualizowane zadanie lub informacje o błędach</returns>
    Task<ServiceResponse<ToDoItemDetailDto>> UpdateAsync(int id, ToDoItemCreateUpdateDto todo);

    /// <summary>
    /// Ustawia procent ukończenia zadania
    /// </summary>
    /// <param name="id">ID zadania</param>
    /// <param name="percent">Nowy procent ukończenia (0-100)</param>
    /// <returns>Odpowiedź informująca o powodzeniu lub błędach operacji</returns>
    Task<ServiceResponse<object>> SetPercentCompleteAsync(int id, int percent);

    /// <summary>
    /// Usuwa zadanie
    /// </summary>
    /// <param name="id">ID zadania do usunięcia</param>
    /// <returns>Odpowiedź informująca o powodzeniu lub niepowodzeniu usunięcia</returns>
    Task<ServiceResponse<object>> DeleteAsync(int id);

    /// <summary>
    /// Oznacza zadanie jako wykonane (100% ukończenia)
    /// </summary>
    /// <param name="id">ID zadania do oznaczenia</param>
    /// <returns>Odpowiedź informująca o powodzeniu lub niepowodzeniu operacji</returns>
    Task<ServiceResponse<object>> MarkAsDoneAsync(int id);
}