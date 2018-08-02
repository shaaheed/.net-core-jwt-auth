using JwtAuthExample.Models;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthExample.Repository
{
    public class TokenDbContext : DbContext
    {

        public TokenDbContext(DbContextOptions<TokenDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; } 
        public DbSet<Token> Tokens { get; set; }

    }
}
