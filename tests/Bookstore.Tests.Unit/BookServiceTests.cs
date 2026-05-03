using Bookstore.Application.DTOs;
using Bookstore.Application.Services;
using Bookstore.Domain.Entities;
using Bookstore.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Bookstore.Tests.Unit;

public class BookServiceTests
{
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly BookService _sut;

    public BookServiceTests()
    {
        _bookRepositoryMock = new Mock<IBookRepository>();
        _sut = new BookService(_bookRepositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnBookDto_WhenBookExists()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var book = new Book { Id = bookId, Title = "Test Book" };
        _bookRepositoryMock.Setup(x => x.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        // Act
        var result = await _sut.GetByIdAsync(bookId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(bookId);
        result.Title.Should().Be("Test Book");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenBookDoesNotExist()
    {
        // Arrange
        _bookRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        // Act
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        Func<Task> act = async () => await _sut.GetByIdAsync(Guid.Empty);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("id");
    }

    [Fact]
    public async Task GetPagedAsync_ShouldThrowArgumentOutOfRangeException_WhenPageIsInvalid()
    {
        Func<Task> act = async () => await _sut.GetPagedAsync(0, 10);

        await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
            .WithParameterName("page");
    }

    [Fact]
    public async Task GetPagedAsync_ShouldThrowArgumentOutOfRangeException_WhenPageSizeIsInvalid()
    {
        Func<Task> act = async () => await _sut.GetPagedAsync(1, 0);

        await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
            .WithParameterName("pageSize");
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnCreatedBookDto()
    {
        // Arrange
        var dto = new CreateBookDto { Title = "New Book", Author = "Author", Price = 10, PublishedDate = DateTime.UtcNow };

        // Act
        var result = await _sut.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(dto.Title);
        _bookRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenBookDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new UpdateBookDto { Title = "Updated" };
        _bookRepositoryMock.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        // Act
        var result = await _sut.UpdateAsync(id, dto);

        // Assert
        result.Should().BeFalse();
        _bookRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        var dto = new UpdateBookDto { Title = "Updated" };

        Func<Task> act = async () => await _sut.UpdateAsync(Guid.Empty, dto);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("id");
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnTrue_WhenBookIsUpdated()
    {
        // Arrange
        var id = Guid.NewGuid();
        var book = new Book { Id = id, Title = "Old Title" };
        var dto = new UpdateBookDto { Title = "New Title", Author = "Author", Price = 20, PublishedDate = DateTime.UtcNow };
        
        _bookRepositoryMock.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        // Act
        var result = await _sut.UpdateAsync(id, dto);

        // Assert
        result.Should().BeTrue();
        book.Title.Should().Be(dto.Title);
        _bookRepositoryMock.Verify(x => x.UpdateAsync(book, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenBookDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _bookRepositoryMock.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        // Act
        var result = await _sut.DeleteAsync(id);

        // Assert
        result.Should().BeFalse();
        _bookRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        Func<Task> act = async () => await _sut.DeleteAsync(Guid.Empty);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("id");
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenBookExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var book = new Book { Id = id };
        
        _bookRepositoryMock.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        // Act
        var result = await _sut.DeleteAsync(id);

        // Assert
        result.Should().BeTrue();
        _bookRepositoryMock.Verify(x => x.DeleteAsync(book, It.IsAny<CancellationToken>()), Times.Once);
    }
}
