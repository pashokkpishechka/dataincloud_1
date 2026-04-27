using Bookstore.Application.DTOs;
using Bookstore.Application.Interfaces;
using Bookstore.Domain.Entities;
using Bookstore.Domain.Interfaces;

namespace Bookstore.Application.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _repository;

    public BookService(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<BookDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var book = await _repository.GetByIdAsync(id, cancellationToken);
        if (book == null) return null;

        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Price = book.Price,
            PublishedDate = book.PublishedDate
        };
    }

    public async Task<IEnumerable<BookDto>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var books = await _repository.GetPagedAsync(page, pageSize, cancellationToken);
        return books.Select(book => new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Price = book.Price,
            PublishedDate = book.PublishedDate
        });
    }

    public async Task<BookDto> CreateAsync(CreateBookDto dto, CancellationToken cancellationToken = default)
    {
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Author = dto.Author,
            Price = dto.Price,
            PublishedDate = dto.PublishedDate,
            IsDeleted = false
        };

        await _repository.AddAsync(book, cancellationToken);

        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Price = book.Price,
            PublishedDate = book.PublishedDate
        };
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateBookDto dto, CancellationToken cancellationToken = default)
    {
        var book = await _repository.GetByIdAsync(id, cancellationToken);
        if (book == null) return false;

        book.Title = dto.Title;
        book.Author = dto.Author;
        book.Price = dto.Price;
        book.PublishedDate = dto.PublishedDate;

        await _repository.UpdateAsync(book, cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var book = await _repository.GetByIdAsync(id, cancellationToken);
        if (book == null) return false;

        await _repository.DeleteAsync(book, cancellationToken);
        return true;
    }
}
