using Microsoft.EntityFrameworkCore;
using server.Models;

// DbContext is a class that communicates with the database
namespace server.Data
{
    public class UsersAPIDbContext : DbContext
    {
        public UsersAPIDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
