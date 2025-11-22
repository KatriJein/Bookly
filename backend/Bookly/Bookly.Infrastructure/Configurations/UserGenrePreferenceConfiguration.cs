using Bookly.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookly.Infrastructure.Configurations;

public class UserGenrePreferenceConfiguration : IEntityTypeConfiguration<UserGenrePreference>
{
    public void Configure(EntityTypeBuilder<UserGenrePreference> builder)
    {
        builder.Property(p => p.PreferenceType).HasConversion<string>();
    }
}