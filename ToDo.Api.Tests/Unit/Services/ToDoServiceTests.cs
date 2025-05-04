using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using ToDo.Api.DTOs;
using ToDo.Api.Entities;
using ToDo.Api.Repositories;
using ToDo.Api.Services;
using ToDo.Api.Tests.Helpers.TestData;

namespace ToDo.Api.Tests.Unit.Services;

public class ToDoServiceTests
{
    private readonly Mock<IToDoRepository> _mockRepository;
    private readonly Mock<IValidator<ToDoItemCreateUpdateDto>> _mockValidator;
    private readonly Mock<IValidator<int>> _mockPercentValidator;
    private readonly ToDoService _service;

    public ToDoServiceTests()
    {
        _mockRepository = new Mock<IToDoRepository>();
        _mockValidator = new Mock<IValidator<ToDoItemCreateUpdateDto>>();
        _mockPercentValidator = new Mock<IValidator<int>>();
        
        _service = new ToDoService(
            _mockRepository.Object,
            _mockValidator.Object,
            _mockPercentValidator.Object
        );
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllToDos()
    {
        // Arrange
        var todoItems = TodoTestDataFactory.GetSampleToDoItems();
        _mockRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(todoItems);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(todoItems.Count);
        // Sprawdzenie czy ID odpowiadają
        result.Select(dto => dto.Id).Should().BeEquivalentTo(todoItems.Select(item => item.Id));
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnItem()
    {
        // Arrange
        var todoItem = TodoTestDataFactory.GetValidToDoItem();
        _mockRepository.Setup(repo => repo.GetByIdAsync(todoItem.Id))
            .ReturnsAsync(todoItem);

        // Act
        var result = await _service.GetByIdAsync(todoItem.Id);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(todoItem.Id);
        result.Data.Title.Should().Be(todoItem.Title);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        int nonExistingId = 999;
        _mockRepository.Setup(repo => repo.GetByIdAsync(nonExistingId))
            .ReturnsAsync((ToDoItem?)null);

        // Act
        var result = await _service.GetByIdAsync(nonExistingId);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ServiceResponseStatus.NotFound);
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldReturnId()
    {
        // Arrange
        var createDto = TodoTestDataFactory.GetValidToDoItemCreateDto();
        int newItemId = 5;

        _mockValidator.Setup(v => v.ValidateAsync(createDto, default))
            .ReturnsAsync(new ValidationResult());

        _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<ToDoItem>()))
            .ReturnsAsync(newItemId);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ServiceResponseStatus.Success);
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(newItemId);

        _mockRepository.Verify(repo => repo.CreateAsync(It.IsAny<ToDoItem>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidData_ShouldReturnValidationErrors()
    {
        // Arrange
        var invalidDto = TodoTestDataFactory.GetInvalidToDoItemCreateDto();
        var validationResult = new ValidationResult(new List<ValidationFailure>
        {
            new("Title", "Tytuł zadania jest wymagany"),
            new("DueDate", "Data wygaśnięcia powinna być w przyszłości"),
            new("CompletionPercentage", "Procent ukończenia musi być wartością między 0 a 100")
        });

        _mockValidator.Setup(v => v.ValidateAsync(invalidDto, default))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _service.CreateAsync(invalidDto);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ServiceResponseStatus.ValidationError);
        result.IsSuccess.Should().BeFalse();
        result.ValidationErrors.Should().NotBeNull();
        result.ValidationErrors.Should().ContainKey("Title");
        result.ValidationErrors.Should().ContainKey("DueDate");
        result.ValidationErrors.Should().ContainKey("CompletionPercentage");

        _mockRepository.Verify(repo => repo.CreateAsync(It.IsAny<ToDoItem>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WithValidDataAndExistingId_ShouldReturnUpdatedItem()
    {
        // Arrange
        int existingId = 1;
        var updateDto = TodoTestDataFactory.GetValidToDoItemCreateDto();
        var existingItem = TodoTestDataFactory.GetValidToDoItem();

        _mockValidator.Setup(v => v.ValidateAsync(updateDto, default))
            .ReturnsAsync(new ValidationResult());

        _mockRepository.Setup(repo => repo.GetByIdAsync(existingId))
            .ReturnsAsync(existingItem);
        
        _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<ToDoItem>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.UpdateAsync(existingId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ServiceResponseStatus.Success);
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(existingId);
        result.Data.Title.Should().Be(updateDto.Title);

        _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<ToDoItem>()), Times.Once);
    }

    [Fact]
    public async Task MarkAsDoneAsync_WithExistingId_ShouldMarkItemAsDone()
    {
        // Arrange
        int itemId = 1;
        _mockRepository.Setup(repo => repo.MarkAsDoneAsync(itemId))
            .ReturnsAsync(true);

        // Act
        var result = await _service.MarkAsDoneAsync(itemId);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ServiceResponseStatus.Success);
        result.IsSuccess.Should().BeTrue();
        
        _mockRepository.Verify(repo => repo.MarkAsDoneAsync(itemId), Times.Once);
    }

    [Fact]
    public async Task MarkAsDoneAsync_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        int nonExistingId = 999;
        _mockRepository.Setup(repo => repo.MarkAsDoneAsync(nonExistingId))
            .ReturnsAsync(false);

        // Act
        var result = await _service.MarkAsDoneAsync(nonExistingId);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ServiceResponseStatus.NotFound);
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task SetPercentCompleteAsync_WithValidData_ShouldUpdatePercentage()
    {
        // Arrange
        int itemId = 1;
        int percentage = 75;
        
        _mockPercentValidator.Setup(v => v.ValidateAsync(percentage, default))
            .ReturnsAsync(new ValidationResult());

        _mockRepository.Setup(repo => repo.SetPercentCompleteAsync(itemId, percentage))
            .ReturnsAsync(true);

        // Act
        var result = await _service.SetPercentCompleteAsync(itemId, percentage);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ServiceResponseStatus.Success);
        result.IsSuccess.Should().BeTrue();
        
        _mockRepository.Verify(repo => repo.SetPercentCompleteAsync(itemId, percentage), Times.Once);
    }
}