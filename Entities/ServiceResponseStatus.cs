namespace ToDo.Api.Entities;

/// <summary>
/// Reprezentuje możliwe statusy odpowiedzi z serwisu
/// </summary>
public enum ServiceResponseStatus
{
    Success = 0,
    
    ValidationError = 1,
    
    NotFound = 2,
    
    Failure = 3,
}