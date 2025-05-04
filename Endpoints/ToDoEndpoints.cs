using ToDo.Api.DTOs;
using ToDo.Api.Entities;
using ToDo.Api.Services;

namespace ToDo.Api.Endpoints;

/// <summary>
/// Klasa rozszerzająca zawierająca definicje endpointów API dla zadań ToDo
/// </summary>
public static class ToDoEndpoints
{
    /// <summary>
    /// Rejestruje wszystkie endpointy API związane z zadaniami ToDo
    /// </summary>
    /// <param name="app">Aplikacja webowa</param>
    /// <returns>Grupa tras API dla zadań ToDo</returns>
    public static RouteGroupBuilder MapToDoEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/todos");

        // 1. Pobierz wszystkie zadania
        group.MapGet("/", GetAllTodosAsync)
            .WithSummary("Pobierz wszystkie zadania")
            .WithName("GetAllTodos");

        // 2. Pobierz konkretne zadanie
        group.MapGet("/{id}", GetTodoByIdAsync)
            .WithSummary("Pobierz zadanie po ID")
            .WithName("GetTodoById");

        // 3. Pobierz nadchodzące zadania (dziś / jutro / bieżący tydzień)
        group.MapGet("/incoming/{timeFrame}", GetIncomingTodosAsync)
            .WithSummary("Pobierz nadchodzące zadania")
            .WithName("GetIncomingTodos");

        // 4. Utwórz nowe zadanie
        group.MapPost("/", CreateTodoAsync)
            .WithSummary("Utwórz nowe zadanie")
            .WithName("CreateTodo");

        // 5. Zaktualizuj zadanie
        group.MapPut("/{id}", UpdateTodoAsync)
            .WithSummary("Zaktualizuj zadanie")
            .WithName("UpdateTodo");

        // 6. Ustaw procent ukończenia zadania
        group.MapPatch("/{id}/percent/{percent}", SetTodoPercentCompleteAsync)
            .WithSummary("Ustaw procent ukończenia zadania")
            .WithName("SetTodoPercentComplete");

        // 7. Usuń zadanie
        group.MapDelete("/{id}", DeleteTodoAsync)
            .WithSummary("Usuń zadanie")
            .WithName("DeleteTodo");

        // 8. Oznacz zadanie jako wykonane
        group.MapPatch("/{id}/done", MarkTodoAsDoneAsync)
            .WithSummary("Oznacz zadanie jako wykonane")
            .WithName("MarkTodoAsDone");

        return group;
    }

    /// <summary>
    /// Obsługuje żądanie pobrania wszystkich zadań
    /// </summary>
    /// <param name="service">Serwis operacji na zadaniach</param>
    /// <returns>Lista wszystkich zadań</returns>
    private static async Task<IResult> GetAllTodosAsync(IToDoService service)
    {
        var todos = await service.GetAllAsync();
        return Results.Ok(todos);
    }
    
    /// <summary>
    /// Obsługuje żądanie pobrania zadania po ID
    /// </summary>
    /// <param name="id">ID zadania</param>
    /// <param name="service">Serwis operacji na zadaniach</param>
    /// <returns>Zadanie jeśli istnieje, w przeciwnym razie kod 404 NotFound</returns>
    private static async Task<IResult> GetTodoByIdAsync(int id, IToDoService service)
    {
        var response = await service.GetByIdAsync(id);
        
        if (response.Status == ServiceResponseStatus.NotFound)
        {
            return Results.NotFound(response.Message);
        }
        
        return Results.Ok(response.Data);
    }
    
    /// <summary>
    /// Obsługuje żądanie pobrania nadchodzących zadań w określonym przedziale czasowym
    /// </summary>
    /// <param name="timeFrame">Przedział czasowy (today, tomorrow, week)</param>
    /// <param name="service">Serwis operacji na zadaniach</param>
    /// <returns>Lista nadchodzących zadań lub kod 400 BadRequest jeśli podano nieprawidłowy przedział</returns>
    private static async Task<IResult> GetIncomingTodosAsync(string timeFrame, IToDoService service)
    {
        try
        {
            var todos = await service.GetIncomingAsync(timeFrame);
            return Results.Ok(todos);
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
    
    /// <summary>
    /// Obsługuje żądanie utworzenia nowego zadania
    /// </summary>
    /// <param name="todoDto">Dane nowego zadania</param>
    /// <param name="service">Serwis operacji na zadaniach</param>
    /// <returns>Kod 201 Created z lokalizacją nowego zasobu lub błędy walidacji</returns>
    private static async Task<IResult> CreateTodoAsync(ToDoItemCreateUpdateDto todoDto, IToDoService service)
    {
        var response = await service.CreateAsync(todoDto);
        
        if (response.Status == ServiceResponseStatus.ValidationError && response.ValidationErrors != null)
        {
            return Results.ValidationProblem(response.ValidationErrors);
        }
        
        return Results.Created($"/api/todos/{response.Data}", todoDto);
    }
    
    /// <summary>
    /// Obsługuje żądanie aktualizacji istniejącego zadania
    /// </summary>
    /// <param name="id">ID zadania do aktualizacji</param>
    /// <param name="todoDto">Nowe dane zadania</param>
    /// <param name="service">Serwis operacji na zadaniach</param>
    /// <returns>Zaktualizowane zadanie lub odpowiedni kod błędu</returns>
    private static async Task<IResult> UpdateTodoAsync(int id, ToDoItemCreateUpdateDto todoDto, IToDoService service)
    {
        var response = await service.UpdateAsync(id, todoDto);
        
        return response.Status switch
        {
            ServiceResponseStatus.ValidationError when response.ValidationErrors != null => 
                Results.ValidationProblem(response.ValidationErrors),
                
            ServiceResponseStatus.NotFound => 
                Results.NotFound(response.Message),
                
            ServiceResponseStatus.Failure => 
                Results.BadRequest(response.Message),
                
            _ => Results.Ok(response.Data)
        };
    }
    
    /// <summary>
    /// Obsługuje żądanie ustawienia procentu ukończenia zadania
    /// </summary>
    /// <param name="id">ID zadania</param>
    /// <param name="percent">Nowy procent ukończenia (0-100)</param>
    /// <param name="service">Serwis operacji na zadaniach</param>
    /// <returns>Potwierdzenie zmiany lub kod błędu</returns>
    private static async Task<IResult> SetTodoPercentCompleteAsync(int id, int percent, IToDoService service)
    {
        var response = await service.SetPercentCompleteAsync(id, percent);
        
        return response.Status switch
        {
            ServiceResponseStatus.ValidationError when response.ValidationErrors != null => 
                Results.ValidationProblem(response.ValidationErrors),
                
            ServiceResponseStatus.NotFound => 
                Results.NotFound(response.Message),
                
            _ => Results.Ok(response.Message)
        };
    }
    
    /// <summary>
    /// Obsługuje żądanie usunięcia zadania
    /// </summary>
    /// <param name="id">ID zadania do usunięcia</param>
    /// <param name="service">Serwis operacji na zadaniach</param>
    /// <returns>Kod 204 NoContent jeśli usunięto, lub 404 NotFound jeśli nie znaleziono</returns>
    private static async Task<IResult> DeleteTodoAsync(int id, IToDoService service)
    {
        var response = await service.DeleteAsync(id);
        
        if (response.Status == ServiceResponseStatus.NotFound)
        {
            return Results.NotFound(response.Message);
        }

        return Results.NoContent();
    }
    
    /// <summary>
    /// Obsługuje żądanie oznaczenia zadania jako wykonane
    /// </summary>
    /// <param name="id">ID zadania</param>
    /// <param name="service">Serwis operacji na zadaniach</param>
    /// <returns>Potwierdzenie zmiany lub kod 404 NotFound</returns>
    private static async Task<IResult> MarkTodoAsDoneAsync(int id, IToDoService service)
    {
        var response = await service.MarkAsDoneAsync(id);
        
        if (response.Status == ServiceResponseStatus.NotFound)
        {
            return Results.NotFound(response.Message);
        }

        return Results.Ok(response.Message);
    }
}