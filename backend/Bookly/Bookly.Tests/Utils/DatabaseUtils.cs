using Bookly.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Tests.Utils;

public static class DatabaseUtils
{
    public static BooklyDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<BooklyDbContext>()
            .UseInMemoryDatabase(databaseName: $"BooklyDb_{TestContext.CurrentContext.Test.ID}")
            .Options;
        return new BooklyDbContext(options);
    }
}