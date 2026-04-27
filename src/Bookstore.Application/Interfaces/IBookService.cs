using Bookstore.Application.DTOs;

namespace Bookstore.Application.Interfaces;

public interface IBookService
{
    Task<BookDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<BookDto>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<BookDto> CreateAsync(CreateBookDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid id, UpdateBookDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
