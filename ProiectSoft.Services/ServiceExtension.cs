using Microsoft.Extensions.DependencyInjection;
using ProiectSoft.Services.CasesServices;
using ProiectSoft.Services.DonationsServices;
using ProiectSoft.Services.LocationsServices;
using ProiectSoft.Services.OrganizationService;
using ProiectSoft.Services.OrganizationsService;
using ProiectSoft.Services.RefugeesServices;
using ProiectSoft.Services.SheltersServices;
using ProiectSoft.Services.UsersServices;
using ProiectSoft.Services.VolunteersServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.Services
{
    public static class ServiceExtension
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<ICasesServices, CasesService>();
            services.AddTransient<IOrganisationService, OrganisationService>();
            services.AddTransient<ILocationServices, LocationServices>();
            services.AddTransient<IShelterServices, ShelterServices>();
            services.AddTransient<IRefugeeServices, RefugeeServices>();
            services.AddTransient<IVolunteerServices, VolunteerServices>();
            services.AddTransient<IUserServices, UserServices>();
            services.AddTransient<IDonationServices, DonationServices>();
        }
    }
}
