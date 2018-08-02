using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace JwtAuthExample.Models
{
    [Table("Token", Schema = "users")]
    public class Token
    {
        public long Id { get; set; }

        [ForeignKey("UserId")]
        public long UserId { get; set; }
        public User User { get; set; }

        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiredIn { get; set; }

    }
}
