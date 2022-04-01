using Microsoft.EntityFrameworkCore;
using ProiectSoft.DAL.Configurations;
using ProiectSoft.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Location> Locations { get; set; }
        public DbSet<Shelter> Shelters { get; set; }
        public DbSet<Refugee> Refugees { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<Cases> Cases { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Donation> Donations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new LocationConfig());
            modelBuilder.ApplyConfiguration(new ShelterConfig());
            modelBuilder.ApplyConfiguration(new RefugeeConfig());
            modelBuilder.ApplyConfiguration(new OrganisationConfig());
            modelBuilder.ApplyConfiguration(new VolunteerConfig());
            modelBuilder.ApplyConfiguration(new CasesConfig());
            modelBuilder.ApplyConfiguration(new UserConfig());
            modelBuilder.ApplyConfiguration(new DonationConfig());
        }
    }
}
