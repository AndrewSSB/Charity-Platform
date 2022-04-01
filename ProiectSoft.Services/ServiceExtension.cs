using Microsoft.Extensions.DependencyInjection;
using ProiectSoft.Services.CasesServices;
using ProiectSoft.Services.OrganizationService;
using ProiectSoft.Services.OrganizationsService;
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
        }
    }
}
