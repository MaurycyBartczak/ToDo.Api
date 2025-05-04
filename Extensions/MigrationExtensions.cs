using Microsoft.EntityFrameworkCore;

namespace ToDo.Api.Extensions;

/// <summary>
/// Klasa rozszerzeń do zarządzania migracjami bazy danych
/// </summary>
public static class MigrationExtensions
{
    /// <summary>
    /// Stosuje migracje do bazy danych z mechanizmem ponownych prób
    /// </summary>
    /// <param name="app">Aplikacja webowa</param>
    /// <remarks>
    /// Metoda próbuje wykonać migracje kilkukrotnie w przypadku niepowodzenia,
    /// stosuje (opóźnienie) między kolejnymi próbami
    /// </remarks>
    public static void ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();
        
        // Liczba prób połączenia
        const int maxRetries = 5;
        int currentRetry = 0;
        
        while (currentRetry < maxRetries)
        {
            try
            {
                logger.LogInformation("Próba migracji bazy danych... ({CurrentRetry}/{MaxRetries})", currentRetry+1, maxRetries);
                var context = services.GetRequiredService<Data.ToDoDbContext>();
                context.Database.Migrate();
                logger.LogInformation("Migracja bazy danych zakończona pomyślnie.");
                return;
            }
            catch (Exception ex)
            {
                currentRetry++;
                logger.LogError(ex, "Wystąpił błąd podczas migracji bazy danych (próba {CurrentRetry}/{MaxRetries})", currentRetry, maxRetries);
                
                if (currentRetry < maxRetries)
                {
                    // Opóźnienie przed kolejną próbą (zwiększane z każdą kolejną próbą)
                    var delay = TimeSpan.FromSeconds(Math.Pow(2, currentRetry));
                    logger.LogInformation("Oczekiwanie {Delay} sekund przed ponowną próbą...", delay.TotalSeconds);
                    Thread.Sleep(delay);
                }
                else
                {
                    logger.LogError("Osiągnięto maksymalną liczbę prób połączenia. Migracja bazy danych nie powiodła się.");
                }
            }
        }
    }
}