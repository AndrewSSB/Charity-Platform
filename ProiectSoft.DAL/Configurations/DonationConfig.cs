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
    public class DonationConfig : IEntityTypeConfiguration<Donation>
    {
        public void Configure(EntityTypeBuilder<Donation> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Organisation)
                .WithMany(x => x.Donations)
                .HasForeignKey(x => x.OrganisationId);

            builder.HasOne(x => x.User)
                .WithMany(x => x.Donations)
                .HasForeignKey(x => x.UserId);
        }
    }
}
