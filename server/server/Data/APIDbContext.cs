using Microsoft.EntityFrameworkCore;
using server.Models;

// DbContext is a class that communicates with the database
namespace server.Data
{
    public class APIDbContext : DbContext
    {
        public APIDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<InstitutionData> InstitutionsData { get; set; }
    }
}
