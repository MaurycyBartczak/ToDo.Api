using Microsoft.EntityFrameworkCore;
using ToDo.Api.Entities;

namespace ToDo.Api.Data;

/// <summary>
/// Kontekst bazy danych aplikacji ToDo
/// Odpowiada za połączenie z bazą danych i konfigurację modeli
/// </summary>
public class ToDoDbContext : DbContext
{
    /// <summary>
    /// Konstruktor kontekstu bazy danych
    /// </summary>
    /// <param name="options">Opcje konfiguracyjne dla kontekstu bazy danych</param>
    public ToDoDbContext(DbContextOptions<ToDoDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Kolekcja zadań przechowywana w bazie danych
    /// </summary>
    public DbSet<ToDoItem> ToDoItems { get; set; } = null!;

    /// <summary>
    /// Konfiguruje model bazy danych przy jej tworzeniu
    /// </summary>
    /// <param name="modelBuilder">Builder modelu bazy danych</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ToDoItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CompletionPercentage).HasDefaultValue(0);
            entity.Property(e => e.IsCompleted).HasDefaultValue(false);
        });
    }
}