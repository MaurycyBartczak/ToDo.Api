namespace ToDo.Api.Entities;

/// <summary>
/// Generyczna klasa odpowiedzi z serwisu zawierająca status operacji, dane i informacje o błędach
/// </summary>
/// <typeparam name="T">Typ zwracanych danych</typeparam>
public class ServiceResponse<T>
{
    /// <summary>
    /// Status odpowiedzi
    /// </summary>
    public ServiceResponseStatus Status { get; set; }
    
    /// <summary>
    /// Czy operacja zakończyła się sukcesem (dla kompatybilności)
    /// </summary>
    public bool IsSuccess => Status == ServiceResponseStatus.Success;
    
    /// <summary>
    /// Czy dane przeszły walidację (dla kompatybilności)
    /// </summary>
    public bool IsValid => Status != ServiceResponseStatus.ValidationError;
    
    /// <summary>
    /// Dane zwracane w przypadku sukcesu
    /// </summary>
    public T? Data { get; set; }
    
    /// <summary>
    /// Błędy walidacji, jeśli występują
    /// </summary>
    public Dictionary<string, string[]>? ValidationErrors { get; set; }
    
    /// <summary>
    /// Komunikat związany z operacją
    /// </summary>
    public string? Message { get; set; }
    
    /// <summary>
    /// Tworzy odpowiedź sukcesu z danymi
    /// </summary>
    /// <param name="data">Dane, które mają być zwrócone</param>
    /// <param name="message">Komunikat opisujący sukces operacji</param>
    /// <returns>Obiekt ServiceResponse ze statusem Success i przekazanymi danymi</returns>
    public static ServiceResponse<T> Success(T data, string message = "Operacja wykonana pomyślnie")
    {
        return new ServiceResponse<T>
        {
            Status = ServiceResponseStatus.Success,
            Data = data,
            Message = message
        };
    }
    
    /// <summary>
    /// Tworzy odpowiedź sukcesu bez danych
    /// </summary>
    /// <param name="message">Komunikat opisujący sukces operacji</param>
    /// <returns>Obiekt ServiceResponse ze statusem Success</returns>
    public static ServiceResponse<T> Success(string message = "Operacja wykonana pomyślnie")
    {
        return new ServiceResponse<T>
        {
            Status = ServiceResponseStatus.Success,
            Message = message
        };
    }
    
    /// <summary>
    /// Tworzy odpowiedź niepowodzenia spowodowanego błędami walidacji
    /// </summary>
    /// <param name="errors">Słownik zawierający błędy walidacji, gdzie klucz to nazwa pola, a wartość to tablica błędów</param>
    /// <param name="message">Komunikat opisujący błąd walidacji</param>
    /// <returns>Obiekt ServiceResponse ze statusem ValidationError i przekazanymi błędami</returns>
    public static ServiceResponse<T> ValidationFailure(Dictionary<string, string[]> errors, string message = "Błąd walidacji")
    {
        return new ServiceResponse<T>
        {
            Status = ServiceResponseStatus.ValidationError,
            ValidationErrors = errors,
            Message = message
        };
    }
    
    /// <summary>
    /// Tworzy odpowiedź niepowodzenia operacji
    /// </summary>
    /// <param name="message">Komunikat opisujący przyczynę niepowodzenia operacji</param>
    /// <returns>Obiekt ServiceResponse ze statusem Failure</returns>
    public static ServiceResponse<T> Failure(string message = "Operacja nie powiodła się")
    {
        return new ServiceResponse<T>
        {
            Status = ServiceResponseStatus.Failure,
            Message = message
        };
    }
    
    /// <summary>
    /// Tworzy odpowiedź informującą o nieznalezieniu zasobu
    /// </summary>
    /// <param name="message">Komunikat opisujący, który zasób nie został znaleziony</param>
    /// <returns>Obiekt ServiceResponse ze statusem NotFound</returns>
    public static ServiceResponse<T> NotFound(string message = "Zasób nie został znaleziony")
    {
        return new ServiceResponse<T>
        {
            Status = ServiceResponseStatus.NotFound,
            Message = message
        };
    }
}