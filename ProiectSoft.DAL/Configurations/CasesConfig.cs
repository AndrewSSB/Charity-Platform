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
    public class CasesConfig : IEntityTypeConfiguration<Cases>
    {
        public void Configure(EntityTypeBuilder<Cases> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.caseName)
                .HasColumnType("nvarchar(50)")
                .HasMaxLength(50);

            builder.Property(x => x.caseDetails)
                .HasColumnType("nvarchar(400)")
                .HasMaxLength(400);

            builder.Property(x => x.startDate)
                .IsRequired(false);

            builder.Property(x => x.endDate)
                .IsRequired(false);

            builder.HasMany(x => x.Organisations)
                .WithOne(x => x.Case)
                .HasForeignKey(x => x.CasesId);

            builder.Property(x=>x.DateCreated)
                .IsRequired(false);
        }
    }
}
