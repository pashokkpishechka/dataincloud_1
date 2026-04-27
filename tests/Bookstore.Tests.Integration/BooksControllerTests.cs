using Bookstore.Application.DTOs;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace Bookstore.Tests.Integration;

public class BooksControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BooksControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CRUD_Operations_ShouldWorkAsExpected()
    {
        // 1. Create a new book
        var createDto = new CreateBookDto
        {
            Title = "Integration Test Book",
            Author = "Test Author",
            Price = 19.99m,
            PublishedDate = DateTime.UtcNow
        };

        var createResponse = await _client.PostAsJsonAsync("/api/books", createDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var createdBook = await createResponse.Content.ReadFromJsonAsync<BookDto>();
        createdBook.Should().NotBeNull();
        createdBook!.Title.Should().Be("Integration Test Book");
        
        var bookId = createdBook.Id;

        // 2. Get the created book by ID
        var getResponse = await _client.GetAsync($"/api/books/{bookId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var getBook = await getResponse.Content.ReadFromJsonAsync<BookDto>();
        getBook.Should().NotBeNull();
        getBook!.Id.Should().Be(bookId);

        // 3. Update the book
        var updateDto = new UpdateBookDto
        {
            Title = "Updated Book Title",
            Author = "Test Author",
            Price = 25.00m,
            PublishedDate = DateTime.UtcNow
        };
        var putResponse = await _client.PutAsJsonAsync($"/api/books/{bookId}", updateDto);
        putResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify update
        var updatedResponse = await _client.GetAsync($"/api/books/{bookId}");
        var updatedBook = await updatedResponse.Content.ReadFromJsonAsync<BookDto>();
        updatedBook!.Title.Should().Be("Updated Book Title");

        // 4. Get Paged List
        var pagedResponse = await _client.GetAsync("/api/books?page=1&pageSize=10");
        pagedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var booksList = await pagedResponse.Content.ReadFromJsonAsync<IEnumerable<BookDto>>();
        booksList.Should().NotBeEmpty();

        // 5. Delete the book (Soft Delete)
        var deleteResponse = await _client.DeleteAsync($"/api/books/{bookId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // 6. Verify Soft Delete (Get should return NotFound)
        var getAfterDeleteResponse = await _client.GetAsync($"/api/books/{bookId}");
        getAfterDeleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
