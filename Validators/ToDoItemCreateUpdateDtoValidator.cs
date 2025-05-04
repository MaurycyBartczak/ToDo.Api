using FluentValidation;
using ToDo.Api.DTOs;

namespace ToDo.Api.Validators;

/// <summary>
/// Walidator dla DTO służącego do tworzenia i aktualizacji zadań ToDo
/// </summary>
public class ToDoItemCreateUpdateDtoValidator : AbstractValidator<ToDoItemCreateUpdateDto>
{
    /// <summary>
    /// Inicjalizuje nową instancję walidatora z regułami dla DTO zadania
    /// </summary>
    public ToDoItemCreateUpdateDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Tytuł zadania jest wymagany")
            .MaximumLength(100).WithMessage("Tytuł nie może przekraczać 100 znaków");
        
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Opis nie może przekraczać 500 znaków");
        
        RuleFor(x => x.DueDate)
            .NotEmpty().WithMessage("Data wygaśnięcia zadania jest wymagana")
            .GreaterThan(DateTime.Now).WithMessage("Data wygaśnięcia powinna być w przyszłości");
        
        RuleFor(x => x.CompletionPercentage)
            .InclusiveBetween(0, 100).WithMessage("Procent ukończenia musi być wartością między 0 a 100");
    }
}