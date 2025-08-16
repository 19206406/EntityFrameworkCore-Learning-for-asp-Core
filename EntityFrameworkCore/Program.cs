using EntityFrameworkCore.Data;
using EntityFrameworkCore.Domain;
using EntityFrameworkCore.Domain.Models;
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

            /* Tracking vs No-Tracking*/
            //await TrackingVsNoTracking(); 

            ///* Adding OneToMany Related Records*/
            //await AddNewTeamsWithLeague();
            //await AddNewTeamWithLeagueId();
            //await AddNewLeagueWithTeams();

            ///* Adding ManyToMany Records*/
            //await AddNewMatches();

            /* Adding OneToOne Records */
            //await AddNewCoach();

            /* Including Related Data - Fager Loading */
            //await IncludeRelatedRecords();

            /* Projections to other data types or ananymous types */
            //await SelectOneProperty();
            //await AnonymousProjection();
            //await StronglyTypedProjection(); 

            /* Filtering with Related Data */
            //await FilteringWithRelatedData();

            /* Querying views */
            //await QueryView(); 

            /* Querying views */
            ////await RawSQLQuery(); 
            ///

            /* Query stored Procedures */
            ////await ExecStoredProcedure(); 

            /* RAW SQL Non-Query Commands */
            //await ExecuteNonQueryCommand(); 
        }

        async static Task ExecuteNonQueryCommand()
        {
            var teamId = 2;
            var affectedRows = await context.Database.ExecuteSqlRawAsync("exec sp_DeleteTeamById {0}", teamId);

            var teamId2 = 5; 
            var affectedRows2 = await context.Database.ExecuteSqlInterpolatedAsync($"exec sp_DeleteTeamById {teamId2}");
        }

        async static Task ExecStoredProcedure()
        {
            var teamId = 3;
            var result = await context.Coaches.FromSqlRaw("EXEC dbo.sp_GetTeamCoach {0}", teamId).ToListAsync(); 
        }

        async static Task RawSQLQuery()
        {
            //var teams1 = await context.Teams.FromSqlRaw("SELECT * FROM Teams")
            //    .Include(x => x.Coach).ToListAsync();

            var name = "AS Roma"; 
            var teams1 = await context.Teams.FromSqlRaw($"SELECT * FROM Teams WHERE name = '{name}'")
                .Include(x => x.Coach).ToListAsync();

            // Mas recomendable usar FromSqlInterpolated para evitar inyecciones SQL
            var teams2 = await context.Teams.FromSqlInterpolated($"SELECT * FROM Teams WHERE name = '{name}'").ToListAsync(); 
        }

        // esto es un ejemplo de consultar una vista creada 
        async static Task QueryView()
        {
            var details = await context.TeamsCoachesLeagues.ToListAsync(); 
        }

        async static Task FilteringWithRelatedData()
        {
            var leagues = await context.Leagues.Where(x => x.Teams.Any(x => x.Name.Contains("Bay"))).ToListAsync();
        }

        async static Task SelectOneProperty()
        {
            // solo estamos seleccionando una propiedad de la entidad 
            var teams = await context.Teams.Select(x => x.Name).ToListAsync(); 
        }

        async static Task AnonymousProjection()
        {
            var teams = await context.Teams.Include(x => x.Coach)
                .Select(
                    x => 
                    new { 
                        TeamName = x.Name,
                        CoachName = x.Coach.Name
                    }
                ).ToListAsync(); 
            
            foreach (var team in teams)
            {
                Console.WriteLine($"Team: {team.TeamName} | Coach: {team.CoachName}");
            }
        }

        async static Task StronglyTypedProjection()
        {
            var teams = await context.Teams.Include(x => x.Coach).Include(x => x.League)
                .Select(
                    x =>
                    new TeamDetail {
                        Name = x.Name,
                        CoachName = x.Coach.Name, 
                        LeagueName = x.League.Name
                    }
                ).ToListAsync();

            foreach (var team in teams)
            {
                Console.WriteLine($"Team: {team.Name} | Coach: {team.CoachName} | League: {team.LeagueName}");
            }
        }

        static async Task IncludeRelatedRecords()
        {
            // Get many related records Leagues -> Teams 
            var leagues = await context.Leagues.Include(x => x.Teams).ToListAsync();

            //Get one related record team > coah
            var team = await context.Teams
                .Include(x => x.Coach)
                .FirstOrDefaultAsync(x => x.Id == 3);

            // Get 'Grand Children' Related Record - Team --> Matches --> Home/Away Teams   
            var teamsWithMatchesAndOpponents = await context.Teams
                .Include(x => x.AwayMatches).ThenInclude(x => x.HomeTeam).ThenInclude(x => x.Coach)
                .Include(x => x.HomeMatches).ThenInclude(x => x.AwayTeam).ThenInclude(x => x.Coach)
                .FirstOrDefaultAsync(x => x.Id == 2);

            // Get Includes with filters 
            var teams = await context.Teams
                .Where(x => x.HomeMatches == null)
                .Include(x => x.Coach)
                .ToListAsync(); 
        }

        private static async Task TrackingVsNoTracking()
        {
            var withTracking = await context.Teams.FirstOrDefaultAsync(x => x.Id == 2);
            var withNoTracking = await context.Teams.AsNoTracking().FirstOrDefaultAsync(x => x.Id == 4);

            withTracking.Name = "Inter Milan";
            withNoTracking.Name = "Rivoli United";

            var entriesBeforeSave = context.ChangeTracker.Entries();

            await context.SaveChangesAsync();

            var entriesAfterSave = context.ChangeTracker.Entries(); 
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

        static async Task AddNewTeamsWithLeague()
        {
            var league = new League { Name = "Bundesliga" };
            var team = new Team { Name = "Bayern Munich", League = league };
            await context.AddAsync(team);
            await context.SaveChangesAsync(); 
        }

        static async Task AddNewTeamWithLeagueId()
        {
            var team = new Team { Name = "Bayern Munich", LeagueId = 4 };
            await context.AddAsync(team); 
            await context.SaveChangesAsync();
        }
    
        static async Task AddNewLeagueWithTeams()
        {
            var teams = new List<Team>
            {
                new Team
                {
                    Name = "Rivoli United"
                },
                new Team
                {
                    Name = "Waterhouse FC"
                },
            };

            var league = new League { Name = "CIFA", Teams = teams };
            await context.AddAsync(league);
            await context.SaveChangesAsync(); 

        }
        
        static async Task AddNewMatches()
        {
            var matches = new List<Match>
            {
                new Match
                {
                    AwayTeamId = 4, HomeTeamId = 2, Date = new DateTime(2023, 10, 1),
                },
                new Match
                {
                    AwayTeamId = 5, HomeTeamId = 6, Date = new DateTime(2023, 10, 2),
                },
            }; 

            await context.AddRangeAsync(matches);
            await context.SaveChangesAsync(); 
        }
    
        static async Task AddNewCoach()
        {
            var coach1 = new Coach { Name = "Jose Mourinho", TeamId = 2 };
            await context.AddAsync(coach1); 

            var coach2 = new Coach { Name = "Antonio Conte" };
            await context.AddAsync(coach2);

            await context.SaveChangesAsync();
        }
    }
}