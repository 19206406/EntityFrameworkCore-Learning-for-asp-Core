using EntityFrameworkCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Data.Configurations.Entities
{
    public class LeagueSeedConfiguration : IEntityTypeConfiguration<League>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<League> builder)
        {
            builder.HasData(
                new League
                {
                    Id = 20,
                    Name = "Sample League",
                }
            ); 
        }
    }
}
