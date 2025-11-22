using Bookly.Domain.Models;
using Core.Enums;
using Core.Interfaces;

namespace Bookly.Application.Functions.Books;

public class ByAgeRestrictionSearchFunction(AgeRestriction? ageRestriction) : ISearchFunction<Book>
{
    public IQueryable<Book> Apply(IQueryable<Book> items)
    {
        return ageRestriction is null 
            ? items 
            : items.Where(i => i.AgeRestriction == ageRestriction);
    }
}