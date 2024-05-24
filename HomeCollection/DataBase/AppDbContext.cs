using System.Configuration;
using HomeCollection.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeCollection.DataBase
{
    public class AppDbContext : DbContext
    {
        public DbSet<Building> Buildings { get; set; }
        public DbSet<Enterance> Enterances { get; set; }
        public DbSet<Flat> Flats { get; set; }
        public DbSet<People> Peoples { get; set; }

        public AppDbContext()
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConfigurationManager.ConnectionStrings["ConnectionSqlite"].ToString());
        }
    }
}
