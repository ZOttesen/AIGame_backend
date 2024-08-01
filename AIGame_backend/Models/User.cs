using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIGame_backend.Models;

public partial class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }
    
    [Required]
    public Guid user_guid { get; set; }
        
    [Required]
    [MaxLength(255)]
    public string first_name { get; set; }
        
    [Required]
    [MaxLength(255)]
    public string last_name { get; set; }
        
    [Required]
    [MaxLength(255)]
    public string email { get; set; }
        
    [Required]
    [MaxLength(255)]
    public string password { get; set; }
        
    [Required]
    public DateTime created_at { get; set; }
    
    public User()
    {
        user_guid = Guid.NewGuid();
        created_at = DateTime.UtcNow;
    }
        
}
