namespace AIGame_backend.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public partial class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; init; }

    [Required]
    [Column("user_guid")]
    public Guid UserGuid { get; init; } = Guid.NewGuid();

    [Required]
    [Column("username")]
    public string Username { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("first_name")]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("last_name")]
    public string LastName { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("email")]
    public string Email { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("password")]
    public string Password { get; set; }

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
