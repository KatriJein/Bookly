using Bookly.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookly.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.OwnsOne(u => u.Login, b => b
            .Property(l => l.Value)
            .HasColumnName("Login"));
        builder.OwnsOne(u => u.Email,  b => b
            .Property(e => e.Value)
            .HasColumnName("Email"));
    }
}