using Microsoft.EntityFrameworkCore;
using server.Models;
using server.ViewModels;

// DbContext is a class that communicates with the database
namespace server.Data
{
    public class APIDbContext : DbContext
    {
        public APIDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<InstitutionData> InstitutionsData { get; set; }

        public DbSet<StaffFormInput> StaffFormInputs { get; set; }

        public DbSet<StudentFormInput> StudentFormInputs { get; set; }

        public DbSet<StudentAlgoResult> StudentAlgoResults { get; set; }
    }
}
