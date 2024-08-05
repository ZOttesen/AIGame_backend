namespace AIGame_backend.Controllers;

using AIGame_backend.Models;
using Microsoft.EntityFrameworkCore;

/// <summary>
///     User context.
/// </summary>
public class UserContext : DbContext
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UserContext" /> class.
    /// </summary>
    /// <param name="options">options.</param>
    public UserContext(DbContextOptions<UserContext> options)
        : base(options)
    {
    }

    /// <summary>
    ///     Gets or sets users.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    ///     On model creating.
    /// </summary>
    /// <param name="modelBuilder">modelBuilder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserGuid).HasColumnName("user_guid");
            entity.Property(e => e.Username).HasColumnName("username");
            entity.Property(e => e.FirstName).HasColumnName("first_name");
            entity.Property(e => e.LastName).HasColumnName("last_name");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        });
    }
}