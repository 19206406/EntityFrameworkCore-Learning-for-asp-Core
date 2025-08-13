using EntityFrameworkCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Data
{
    public class FootballLeagueDbContext : DbContext
    {
        // Conección a la base de datos

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=FootballLeague_EfCore;Integrated Security=True")
                .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information)
                .EnableSensitiveDataLogging();
        }

        public DbSet<Team> Teams { get; set; }
        public DbSet<League> Leagues { get; set; }

    }
}
