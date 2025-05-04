using FluentValidation;
using ToDo.Api.Entities;

namespace ToDo.Api.Validators;

/// <summary>
/// Walidator zadań ToDo, sprawdzający poprawność danych w encji ToDoItem
/// </summary>
public class ToDoItemValidator : AbstractValidator<ToDoItem>
{
    /// <summary>
    /// Inicjalizuje nową instancję walidatora z regułami sprawdzającymi poprawność zadania
    /// </summary>
    public ToDoItemValidator()
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