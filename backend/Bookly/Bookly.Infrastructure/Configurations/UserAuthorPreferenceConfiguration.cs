using Bookly.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookly.Infrastructure.Configurations;

public class UserAuthorPreferenceConfiguration : IEntityTypeConfiguration<UserAuthorPreference>
{
    public void Configure(EntityTypeBuilder<UserAuthorPreference> builder)
    {
        builder.Property(p => p.PreferenceType).HasConversion<string>();
    }
}