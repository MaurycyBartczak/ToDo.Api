using ToDo.Api.DTOs;
using ToDo.Api.Entities;

namespace ToDo.Api.Mappers;

/// <summary>
/// Klasa odpowiedzialna za mapowanie obiektów związanych z zadaniami
/// </summary>
public static class ToDoMapper
{
    /// <summary>
    /// Mapuje model na szczegółowe DTO
    /// </summary>
    public static ToDoItemDetailDto ToDetailDto(this ToDoItem item)
    {
        return new ToDoItemDetailDto
        {
            Id = item.Id,
            Title = item.Title,
            Description = item.Description,
            DueDate = item.DueDate,
            CompletionPercentage = item.CompletionPercentage,
            IsCompleted = item.IsCompleted,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }
    
    /// <summary>
    /// Mapuje model na uproszczone DTO (dla list)
    /// </summary>
    public static ToDoItemListDto ToListDto(this ToDoItem item)
    {
        return new ToDoItemListDto
        {
            Id = item.Id,
            Title = item.Title,
            DueDate = item.DueDate,
            CompletionPercentage = item.CompletionPercentage,
            IsCompleted = item.IsCompleted
        };
    }
    
    /// <summary>
    /// Mapuje DTO utworzenia/aktualizacji na model
    /// </summary>
    public static ToDoItem ToEntity(this ToDoItemCreateUpdateDto dto)
    {
        return new ToDoItem
        {
            Title = dto.Title,
            Description = dto.Description,
            DueDate = dto.DueDate,
            CompletionPercentage = dto.CompletionPercentage
        };
    }
    
    /// <summary>
    /// Aktualizuje istniejący model na podstawie DTO
    /// </summary>
    public static void UpdateFromDto(this ToDoItem entity, ToDoItemCreateUpdateDto dto)
    {
        entity.Title = dto.Title;
        entity.Description = dto.Description;
        entity.DueDate = dto.DueDate;
        entity.CompletionPercentage = dto.CompletionPercentage;
        entity.UpdatedAt = DateTime.UtcNow;
        
        // Aktualizacja statusu zakończenia w zależności od procentu
        if (dto.CompletionPercentage >= 100)
        {
            entity.IsCompleted = true;
            entity.CompletionPercentage = 100;
        }
    }
    
    /// <summary>
    /// Mapuje listę modeli na listę uproszczonych DTO
    /// </summary>
    public static List<ToDoItemListDto> ToListDtos(this IEnumerable<ToDoItem> items)
    {
        return items.Select(item => item.ToListDto()).ToList();
    }
}