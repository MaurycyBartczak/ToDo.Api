using FluentValidation;
using ToDo.Api.DTOs;
using ToDo.Api.Entities;
using ToDo.Api.Mappers;
using ToDo.Api.Repositories;

namespace ToDo.Api.Services;

/// <summary>
/// Implementacja serwisu operacji na zadaniach ToDo
/// </summary>
public class ToDoService(
    IToDoRepository repository,
    IValidator<ToDoItemCreateUpdateDto> validator,
    IValidator<int> percentValidator) : IToDoService
{
    /// <summary>
    /// Pobiera wszystkie zadania
    /// </summary>
    /// <returns>Lista wszystkich zadań w formacie uproszczonym do wyświetlania w listach</returns>
    public async Task<List<ToDoItemListDto>> GetAllAsync()
    {
        var items = await repository.GetAllAsync();
        return items.ToListDtos();
    }

    /// <summary>
    /// Pobiera szczegóły pojedynczego zadania po ID
    /// </summary>
    /// <param name="id">ID zadania</param>
    /// <returns>Odpowiedź zawierająca szczegóły zadania lub informację o błędzie</returns>
    public async Task<ServiceResponse<ToDoItemDetailDto>> GetByIdAsync(int id)
    {
        var todo = await repository.GetByIdAsync(id);
        if (todo == null)
        {
            return ServiceResponse<ToDoItemDetailDto>.NotFound($"Zadanie o ID {id} nie zostało znalezione.");
        }

        return ServiceResponse<ToDoItemDetailDto>.Success(todo.ToDetailDto());
    }

    /// <summary>
    /// Pobiera nadchodzące zadania w określonym przedziale czasowym
    /// </summary>
    /// <param name="timeFrame">Przedział czasowy: "today", "tomorrow" lub "week"</param>
    /// <returns>Lista nadchodzących zadań w formacie uproszczonym</returns>
    /// <exception cref="ArgumentException">Rzucany gdy podano niepoprawny przedział czasowy</exception>
    public async Task<List<ToDoItemListDto>> GetIncomingAsync(string timeFrame)
    {
        DateTime startDate = DateTime.Today;
        DateTime endDate;

        switch (timeFrame.ToLower())
        {
            case "today":
                endDate = startDate.AddDays(1).AddSeconds(-1);
                break;
            case "tomorrow":
                startDate = startDate.AddDays(1);
                endDate = startDate.AddDays(1).AddSeconds(-1);
                break;
            case "week":
                endDate = startDate.AddDays(7).AddSeconds(-1);
                break;
            default:
                throw new ArgumentException("Dostępne opcje: today, tomorrow, week", nameof(timeFrame));
        }

        var items = await repository.GetIncomingAsync(startDate, endDate);
        return items.ToListDtos();
    }

    /// <summary>
    /// Tworzy nowe zadanie
    /// </summary>
    /// <param name="todoDto">Dane nowego zadania</param>
    /// <returns>Odpowiedź zawierająca ID utworzonego zadania lub informacje o błędach walidacji</returns>
    public async Task<ServiceResponse<int>> CreateAsync(ToDoItemCreateUpdateDto todoDto)
    {
        var validationResult = await validator.ValidateAsync(todoDto);
        if (!validationResult.IsValid)
        {
            var errors = new Dictionary<string, string[]>(validationResult.ToDictionary());
            return ServiceResponse<int>.ValidationFailure(errors);
        }

        var todoEntity = todoDto.ToEntity();
        var id = await repository.CreateAsync(todoEntity);
        return ServiceResponse<int>.Success(id, "Zadanie zostało utworzone pomyślnie.");
    }

    /// <summary>
    /// Aktualizuje istniejące zadanie
    /// </summary>
    /// <param name="id">ID zadania do aktualizacji</param>
    /// <param name="todoDto">Nowe dane zadania</param>
    /// <returns>Odpowiedź zawierająca zaktualizowane zadanie lub informacje o błędach</returns>
    public async Task<ServiceResponse<ToDoItemDetailDto>> UpdateAsync(int id, ToDoItemCreateUpdateDto todoDto)
    {
        var validationResult = await validator.ValidateAsync(todoDto);
        if (!validationResult.IsValid)
        {
            var errors = new Dictionary<string, string[]>(validationResult.ToDictionary());
            return ServiceResponse<ToDoItemDetailDto>.ValidationFailure(errors);
        }

        var existingEntity = await repository.GetByIdAsync(id);
        if (existingEntity == null)
        {
            return ServiceResponse<ToDoItemDetailDto>.NotFound($"Zadanie o ID {id} nie zostało znalezione.");
        }

        // Aktualizacja istniejącego obiektu
        existingEntity.UpdateFromDto(todoDto);
        var result = await repository.UpdateAsync(existingEntity);
        
        if (!result)
        {
            return ServiceResponse<ToDoItemDetailDto>.Failure("Nie udało się zapisać zmian w bazie danych.");
        }
        
        return ServiceResponse<ToDoItemDetailDto>.Success(existingEntity.ToDetailDto(), "Zadanie zostało zaktualizowane pomyślnie.");
    }

    /// <summary>
    /// Ustawia procent ukończenia zadania
    /// </summary>
    /// <param name="id">ID zadania</param>
    /// <param name="percent">Nowy procent ukończenia (0-100)</param>
    /// <returns>Odpowiedź informująca o powodzeniu lub błędach operacji</returns>
    public async Task<ServiceResponse<object>> SetPercentCompleteAsync(int id, int percent)
    {
        var validationResult = await percentValidator.ValidateAsync(percent);
        if (!validationResult.IsValid)
        {
            var errors = new Dictionary<string, string[]>(validationResult.ToDictionary());
            return ServiceResponse<object>.ValidationFailure(errors);
        }

        var result = await repository.SetPercentCompleteAsync(id, percent);
        
        if (!result)
        {
            return ServiceResponse<object>.NotFound($"Zadanie o ID {id} nie zostało znalezione.");
        }
        
        return ServiceResponse<object>.Success("Procent ukończenia zadania został zmieniony pomyślnie.");
    }

    /// <summary>
    /// Usuwa zadanie
    /// </summary>
    /// <param name="id">ID zadania do usunięcia</param>
    /// <returns>Odpowiedź informująca o powodzeniu lub niepowodzeniu usunięcia</returns>
    public async Task<ServiceResponse<object>> DeleteAsync(int id)
    {
        var result = await repository.DeleteAsync(id);
        
        if (!result)
        {
            return ServiceResponse<object>.NotFound($"Zadanie o ID {id} nie zostało znalezione.");
        }
        
        return ServiceResponse<object>.Success("Zadanie zostało usunięte pomyślnie.");
    }

    /// <summary>
    /// Oznacza zadanie jako wykonane (100% ukończenia)
    /// </summary>
    /// <param name="id">ID zadania do oznaczenia</param>
    /// <returns>Odpowiedź informująca o powodzeniu lub niepowodzeniu operacji</returns>
    public async Task<ServiceResponse<object>> MarkAsDoneAsync(int id)
    {
        var result = await repository.MarkAsDoneAsync(id);
        
        if (!result)
        {
            return ServiceResponse<object>.NotFound($"Zadanie o ID {id} nie zostało znalezione.");
        }
        
        return ServiceResponse<object>.Success("Zadanie zostało oznaczone jako wykonane.");
    }
}