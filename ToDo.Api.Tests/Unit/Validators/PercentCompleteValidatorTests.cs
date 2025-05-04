using FluentAssertions;
using ToDo.Api.Validators;

namespace ToDo.Api.Tests.Unit.Validators;

public class PercentCompleteValidatorTests
{
    private readonly PercentCompleteValidator _validator;
    
    public PercentCompleteValidatorTests()
    {
        _validator = new PercentCompleteValidator();
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Validate_WhenPercentIsOutOfRange_ReturnsError(int percent)
    {
        // Act
        var result = _validator.Validate(percent);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("między 0 a 100"));
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(50)]
    [InlineData(100)]
    public void Validate_WhenPercentIsInRange_IsValid(int percent)
    {
        // Act
        var result = _validator.Validate(percent);
        
        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}