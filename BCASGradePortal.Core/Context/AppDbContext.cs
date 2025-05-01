using BCASGradePortal.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BCASGradePortal.Core.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
