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
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserName)
                .HasColumnType("nvarchar(50)")
                .HasMaxLength(50);

            builder.Property(x => x.email)
                .HasColumnType("nvarchar(50)")
                .HasMaxLength(50);

            builder.Property(x => x.Password)
                .HasColumnType("nvarchar(50)")
                .HasMaxLength(50);

            builder.Property(x => x.confirmPassword)
                .HasColumnType("nvarchar(50)")
                .HasMaxLength(50);

            builder.Property(x => x.Type)
                .HasColumnType("nvarchar(20)")
                .HasMaxLength(20);

            builder.Property(x => x.DateCreated)
                .IsRequired(false);
        }
    }
}
