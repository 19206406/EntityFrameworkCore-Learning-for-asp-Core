using EntityFrameworkCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Data.Configurations.Entities
{
    public class TeamSeedConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder.HasData(
                new Team
                {
                    Id = 20,
                    Name = "Sebastian Urrego - Sample Team", 
                    LeagueId = 20,
                },
                new Team
                {
                    Id = 21,
                    Name = "Sebastian Urrego - Sample Team", 
                    LeagueId = 20
                },
                new Team
                {
                    Id = 22,
                    Name = "Sebastian Urrego - Sample Team", 
                    LeagueId = 20
                }
            );
        }
    }
}
