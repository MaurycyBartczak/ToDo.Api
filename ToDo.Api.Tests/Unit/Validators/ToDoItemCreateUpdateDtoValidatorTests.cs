using FluentAssertions;
using ToDo.Api.DTOs;
using ToDo.Api.Validators;

namespace ToDo.Api.Tests.Unit.Validators;

public class ToDoItemCreateUpdateDtoValidatorTests
{
    private readonly ToDoItemCreateUpdateDtoValidator _validator;
    
    public ToDoItemCreateUpdateDtoValidatorTests()
    {
        _validator = new ToDoItemCreateUpdateDtoValidator();
    }
    
    [Fact]
    public void Validate_WhenTitleIsEmpty_ReturnsValidationError()
    {
        // Arrange
        var dto = new ToDoItemCreateUpdateDto
        {
            Title = "",
            Description = "Opis zadania",
            DueDate = DateTime.Now.AddDays(1),
            CompletionPercentage = 0
        };
        
        // Act
        var result = _validator.Validate(dto);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title" && e.ErrorMessage.Contains("wymagany"));
    }
    
    [Fact]
    public void Validate_WhenTitleIsTooLong_ReturnsValidationError()
    {
        // Arrange
        var dto = new ToDoItemCreateUpdateDto
        {
            Title = new string('A', 101), // 101 znaków
            Description = "Opis zadania",
            DueDate = DateTime.Now.AddDays(1),
            CompletionPercentage = 0
        };
        
        // Act
        var result = _validator.Validate(dto);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title" && e.ErrorMessage.Contains("100 znaków"));
    }
    
    [Fact]
    public void Validate_WhenDescriptionIsTooLong_ReturnsValidationError()
    {
        // Arrange
        var dto = new ToDoItemCreateUpdateDto
        {
            Title = "Tytuł zadania",
            Description = new string('A', 501), // 501 znaków
            DueDate = DateTime.Now.AddDays(1),
            CompletionPercentage = 0
        };
        
        // Act
        var result = _validator.Validate(dto);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description" && e.ErrorMessage.Contains("500 znaków"));
    }
    
    [Fact]
    public void Validate_WhenDueDateIsInPast_ReturnsValidationError()
    {
        // Arrange
        var dto = new ToDoItemCreateUpdateDto
        {
            Title = "Tytuł zadania",
            Description = "Opis zadania",
            DueDate = DateTime.Now.AddDays(-1), // Wczoraj
            CompletionPercentage = 0
        };
        
        // Act
        var result = _validator.Validate(dto);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DueDate" && e.ErrorMessage.Contains("w przyszłości"));
    }
    
    [Theory]
    [InlineData(-1)]   // Poniżej minimum
    [InlineData(101)]  // Powyżej maksimum
    public void Validate_WhenCompletionPercentageIsOutOfRange_ReturnsValidationError(int percentage)
    {
        // Arrange
        var dto = new ToDoItemCreateUpdateDto
        {
            Title = "Tytuł zadania",
            Description = "Opis zadania", 
            DueDate = DateTime.Now.AddDays(1),
            CompletionPercentage = percentage
        };
        
        // Act
        var result = _validator.Validate(dto);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CompletionPercentage" && e.ErrorMessage.Contains("między 0 a 100"));
    }
    
    [Fact]
    public void Validate_WithValidData_IsValid()
    {
        // Arrange
        var dto = new ToDoItemCreateUpdateDto
        {
            Title = "Poprawny tytuł",
            Description = "Poprawny opis",
            DueDate = DateTime.Now.AddDays(1),
            CompletionPercentage = 50
        };
        
        // Act
        var result = _validator.Validate(dto);
        
        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}