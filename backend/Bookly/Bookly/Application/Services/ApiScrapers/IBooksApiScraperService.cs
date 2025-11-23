namespace Bookly.Application.Services.ApiScrapers;

public interface IBooksApiScraperService
{
    Task ScrapeNextAsync();
}