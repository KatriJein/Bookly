using Bookly.Application.Functions.Books;
using Bookly.Application.Mappers;
using Bookly.Domain.Models;
using Bookly.Extensions;
using Bookly.Infrastructure;
using Core.Dto.Book;
using Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.Books;

public class GetAllBooksHandler(BooklyDbContext booklyDbContext) : IRequestHandler<GetAllBooksQuery, List<GetShortBookDto>>
{
    public async Task<List<GetShortBookDto>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
    {
        var booksWithIncludedInfo = booklyDbContext.Books
            .Include(b => b.Genres)
            .Include(b => b.Authors)
            .Include(b => b.Publisher);
        var searchFunctions = CreateSearchFunctionsBasedOnIncomingSearchSettings(request.BookSearchSettingsDto);
        var sortingFunction = CreateSortingFunctionBasedOnIncomingSortingSettings(request.BookSearchSettingsDto);
        var books = await booksWithIncludedInfo
            .ApplySearchFunctionsForItems(searchFunctions)
            .ApplySortingForItems(sortingFunction)
            .RetrieveNextPage(request.BookSearchSettingsDto.Page, request.BookSearchSettingsDto.Limit)
            .ToListAsync(cancellationToken);
        return books.Select(BookMapper.MapBookToShortBookDto).ToList();
    }

    private static List<ISearchFunction<Book>> CreateSearchFunctionsBasedOnIncomingSearchSettings(BookSearchSettingsDto bookSearchSettingsDto)
    {
        var functions = new List<ISearchFunction<Book>>
        {
            new ByTitleSearchFunction(bookSearchSettingsDto.SearchByTitle),
            new ByPublisherSearchFunction(bookSearchSettingsDto.SearchByPublisher),
            new ByAuthorsSearchFunction(bookSearchSettingsDto.SearchByAuthors),
            new ByGenresSearchFunction(bookSearchSettingsDto.SearchByGenres),
            new ByRatingSearchFunction(bookSearchSettingsDto.SearchByRating),
            new ByVolumeSizeSearchFunction(bookSearchSettingsDto.SearchByVolumeSizePreference)
        };
        return functions;
    }

    private static ISortingFunction<Book> CreateSortingFunctionBasedOnIncomingSortingSettings(
        BookSearchSettingsDto bookSearchSettingsDto)
    {
        return new BooksSortingFunction(bookSearchSettingsDto.BooksOrderOption);
    }
}

public record GetAllBooksQuery(BookSearchSettingsDto BookSearchSettingsDto) : IRequest<List<GetShortBookDto>>;