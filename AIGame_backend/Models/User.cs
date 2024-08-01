namespace AIGame_backend.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public partial class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("user_guid")]
    public Guid UserGuid { get; set; }

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
    public DateTime CreatedAt { get; set; }

    public User()
    {
        UserGuid = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
}
