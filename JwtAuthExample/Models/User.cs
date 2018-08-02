using System.ComponentModel.DataAnnotations.Schema;

namespace JwtAuthExample.Models
{
    [Table("User", Schema = "users")]
    public class User
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
