using EntityFrameworkCore.Data;
using EntityFrameworkCore.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace EntityFrameworkCore
{
    class Program
    {
        private static readonly FootballLeagueDbContext context = new FootballLeagueDbContext(); 

        static async Task Main(string[] args)
        {
            /*Simple Insert Operation Methods*/
            //await AddNewLeague();
            //await AddNewTeamWithLeague(); 

            // esta parte es uso de LINQ para consultar la base de datos
            // a traves de .ToList() 
            //SimpleSelectQuery(); 

            // en este ejemplo usamos filtros en las consultas
            //await QueryFilters(); 

            /*Aggregate Functions*/
            ////await AditionalExecutionMethods(); 

            /*Alternative LINQ Syntax*/
            //await AlternativeLinqSyntax(); 

            /* Perform Update */
            ////await UpdateRecord();
            //await SimpleUpdateTeamRecord(); 

            /* Perfom Delete */
            //await SimpleDelete();
            //await DeleteWithRelationship();
        }

        private static async Task SimpleDelete()
        {
            var league = await context.Leagues.FindAsync(5);
            context.Leagues.Remove(league); 

            await context.SaveChangesAsync();
        }

        private static async Task DeleteWithRelationship()
        {
            var league = await context.Leagues.FindAsync(3);
            context.Leagues.Remove(league); // Elimina la liga y sus equipos relacionados
            await context.SaveChangesAsync();
        } 

        private static async Task SimpleUpdateTeamRecord()
        {
            var team = new Team
            {
                Id = 3,
                Name = "Barcelona",
                LeagueId = 3
            }; 

            context.Teams.Update(team); // Actualiza el registro
            await context.SaveChangesAsync(); 
        }

        private static async Task GetRecord()
        {
            var league = await context.Leagues.FindAsync(3); // busca por Id
            Console.WriteLine($"{league.Id} - {league.Name}");
        }

        static async Task UpdateRecord()
        {
            // Retrieve Record 
            var league = await context.Leagues.FindAsync(3); 

            // Make Record Changes
            league.Name = "Scottish Premiership";

            // Saves Changes 
            await context.SaveChangesAsync();

            await GetRecord(); 
        }

        static async Task AlternativeLinqSyntax()
        {
            Console.WriteLine("Enter Team Name (Or Part Of): ");
            var teamName = Console.ReadLine();
            var teams = await (from i in context.Teams
                               where EF.Functions.Like(i.Name, $"%{teamName}%")
                               select i).ToListAsync();

            foreach (var team in teams)
            {
                Console.WriteLine($"{team.Id} - {team.Name}");
            }
        }

        static async Task AditionalExecutionMethods()
        {
            // busca el primer elemento de la tabla que contenga "A" en su nombre 
            //var l = context.Leagues.FirstOrDefaultAsync(x => x.Name.Contains("A"));

            var leagues = context.Leagues;
            var list = await leagues.ToListAsync();
            var first = await leagues.FirstAsync(); 
            var firstOrDefault = await leagues.FirstOrDefaultAsync();
            //var single = await leagues.SingleAsync();
            //var singleOrDefault = await leagues.SingleOrDefaultAsync(); 

            ////var count = await leagues.CountAsync();
            ////var longCount = await leagues.LongCountAsync();
            ////var min = await leagues.MinAsync();
            ////var max = await leagues.MaxAsync();

            // DbSet Method that will execute 
            var league = await context.Leagues.FindAsync(1); // busca por Id   
        }

        static async Task QueryFilters()
        {
            Console.WriteLine($"Enter League Name (Or Part Of): ");
            var leagueName = Console.ReadLine(); 

            var exactMatches = await context.Leagues.Where(x => x.Name.Equals(leagueName)).ToListAsync();
            foreach (var league in exactMatches)
            {
                Console.WriteLine($"{league.Id} - {league.Name}");
            }

            // Uso de LINQ para buscar coincidencias parciales
            //var partialMatches = await context.Leagues.Where(x => x.Name.Contains(leagueName)).ToListAsync();

            // Uso de EF.Functions.Like para buscar coincidencias parciales
            var partialMatches = await context.Leagues.Where(x => EF.Functions.Like(x.Name, $"%{leagueName}")).ToListAsync();
            foreach (var league in partialMatches)
            {
                Console.WriteLine($"{league.Id} - {league.Name}");
            }
        }

        static async Task SimpleSelectQuery() 
        {
            // smartest most efficient way to get results 
            var leagues = await context.Leagues.ToListAsync(); 
            foreach (var league in leagues)
            {
                Console.WriteLine($"{league.Id} - {league.Name}");
            }
        }

        static async Task AddTeamWithLeague(League league)
        {
            var teams = new List<Team>
            {
                new Team { Name = "Juventus", LeagueId = league.Id },
                new Team { Name = "AC Milan", LeagueId = league.Id },
                new Team { Name = "AS Roma", League = league }
            };

            await context.AddRangeAsync(teams); 
        }
    }
}