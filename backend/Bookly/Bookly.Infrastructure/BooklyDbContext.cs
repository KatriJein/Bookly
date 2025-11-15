using Bookly.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Infrastructure;

public class BooklyDbContext(DbContextOptions<BooklyDbContext> options) : DbContext(options)
{
    public DbSet<Book> Books { get; set; }
    public DbSet<BookCollection> BookCollections { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Publisher> Publishers { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<ScrapingTaskState> ScrapingTaskStates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BooklyDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}