using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProiectSoft.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Configurations
{
    public class OrganisationConfig : IEntityTypeConfiguration<Organisation>
    {
        public void Configure(EntityTypeBuilder<Organisation> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasColumnType("nvarchar(50)")
                .HasMaxLength(50);

            builder.Property(x => x.Email)
                .HasColumnType("nvarchar(30)")
                .HasMaxLength(30);

            builder.Property(x => x.Phone)
                .HasColumnType("nvarchar(15)")
                .HasMaxLength(15);

            builder.Property(x => x.Details)
                .HasColumnType("nvarchar(300)")
                .HasMaxLength(300);

            builder.HasMany(x => x.Shelters)
                .WithOne(x => x.Organisation)
                .HasForeignKey(x => x.OrganisationId);

            builder.HasMany(x => x.Volunteers)
                .WithOne(x => x.Organisation)
                .HasForeignKey(x => x.OrganisationId);
        }
    }
}
