using FluentValidation;

namespace ToDo.Api.Validators;

/// <summary>
/// Walidator dla wartości procentowej ukończenia zadania
/// </summary>
public class PercentCompleteValidator : AbstractValidator<int>
{
    /// <summary>
    /// Inicjalizuje nową instancję walidatora sprawdzającego poprawność procentu ukończenia
    /// </summary>
    /// <remarks>
    /// Sprawdza, czy wartość procentowa mieści się w zakresie od 0 do 100
    /// </remarks>
    public PercentCompleteValidator()
    {
        RuleFor(x => x)
            .InclusiveBetween(0, 100)
            .WithMessage("Procent ukończenia musi być wartością między 0 a 100.");
    }
}