using Bookstore.Application.DTOs;
using Bookstore.Application.Validators;
using FluentValidation.TestHelper;

namespace Bookstore.Tests.Unit;

public class BookValidatorsTests
{
    private readonly CreateBookDtoValidator _createValidator;
    private readonly UpdateBookDtoValidator _updateValidator;

    public BookValidatorsTests()
    {
        _createValidator = new CreateBookDtoValidator();
        _updateValidator = new UpdateBookDtoValidator();
    }

    [Fact]
    public void CreateValidator_ShouldHaveError_WhenTitleIsEmpty() // Negative
    {
        var dto = new CreateBookDto { Title = string.Empty };
        var result = _createValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void CreateValidator_ShouldHaveError_WhenTitleExceedsMaxLength() // Boundary
    {
        var dto = new CreateBookDto { Title = new string('A', 201) };
        var result = _createValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void CreateValidator_ShouldNotHaveError_WhenTitleIsAtMaxLength() // Boundary
    {
        var dto = new CreateBookDto { Title = new string('A', 200), Author = "Valid", Price = 10, PublishedDate = DateTime.UtcNow };
        var result = _createValidator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void CreateValidator_ShouldHaveError_WhenPriceIsZero() // Boundary / Negative
    {
        var dto = new CreateBookDto { Price = 0 };
        var result = _createValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void CreateValidator_ShouldNotHaveError_WhenPriceIsGreaterThanZero() // Positive
    {
        var dto = new CreateBookDto { Title = "Title", Author = "Valid", Price = 0.01m, PublishedDate = DateTime.UtcNow };
        var result = _createValidator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void CreateValidator_ShouldHaveError_WhenDateIsInFuture() // Boundary / Negative
    {
        var dto = new CreateBookDto { PublishedDate = DateTime.UtcNow.AddDays(1) };
        var result = _createValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.PublishedDate);
    }

    [Fact]
    public void CreateValidator_ShouldNotHaveError_WhenDateIsPastOrPresent() // Positive
    {
        var dto = new CreateBookDto { Title = "Title", Author = "Valid", Price = 10, PublishedDate = DateTime.UtcNow.AddMinutes(-1) };
        var result = _createValidator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.PublishedDate);
    }

    [Fact]
    public void UpdateValidator_ShouldHaveError_WhenAuthorIsEmpty() // Negative
    {
        var dto = new UpdateBookDto { Author = string.Empty };
        var result = _updateValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Author);
    }

    [Fact]
    public void UpdateValidator_ShouldHaveError_WhenAuthorExceedsMaxLength() // Boundary
    {
        var dto = new UpdateBookDto { Author = new string('A', 101) };
        var result = _updateValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Author);
    }
}
