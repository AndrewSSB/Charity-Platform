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
    public class ShelterConfig : IEntityTypeConfiguration<Shelter>
    {
        public void Configure(EntityTypeBuilder<Shelter> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasColumnType("nvarchar(100)")
                .HasMaxLength(100);

            builder.Property(x => x.Email)
                .HasColumnType("nvarchar(30)")
                .HasMaxLength(30)
                .IsRequired(false);

            builder.Property(x => x.Phone)
                .HasColumnType("nvarchar(15)")
                .HasMaxLength(15)
                .IsRequired(false);

            builder.HasOne(x => x.Location)
                .WithOne(x => x.Shelter)
                .HasForeignKey<Shelter>(x => x.LocationId);

            builder.HasMany(x => x.Refugees)
                .WithOne(x => x.Shelter);
        }
    }
}
