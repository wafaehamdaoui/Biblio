using Biblio.Models;
using Microsoft.EntityFrameworkCore;

namespace Biblio.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Book> Books { get; set; }
        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<Reservation> Resevations { get; set; }
        public DbSet<UserLogin> UserLogin { get; set; }
    }
}
