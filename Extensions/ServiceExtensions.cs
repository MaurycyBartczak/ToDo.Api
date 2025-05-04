using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using ToDo.Api.Data;
using ToDo.Api.Repositories;
using ToDo.Api.Services;
using ToDo.Api.Validators;

namespace ToDo.Api.Extensions;

/// <summary>
/// Klasa rozszerzeń do konfiguracji serwisów aplikacji
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Dodaje i konfiguruje wszystkie usługi potrzebne do działania aplikacji
    /// </summary>
    /// <param name="services">Kolekcja serwisów aplikacji</param>
    /// <param name="configuration">Konfiguracja aplikacji</param>
    /// <returns>Zaktualizowana kolekcja serwisów</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        // Dodanie kontekstu bazy danych
        services.AddDbContext<ToDoDbContext>(options =>
        {
            var serverVersion = ServerVersion.AutoDetect(connectionString);
            
            options.UseMySql(connectionString, serverVersion, mySqlOptions =>
            {
                mySqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            });
        });
        
        // Rejestracja repozytoriów
        services.AddScoped<IToDoRepository, ToDoRepository>();
        
        // Rejestracja serwisów
        services.AddScoped<IToDoService, ToDoService>();
        
        // Konfiguracja FluentValidation
        services.AddValidatorsFromAssemblyContaining<ToDoItemValidator>();
        services.AddFluentValidationAutoValidation();
        
        return services;
    }
}