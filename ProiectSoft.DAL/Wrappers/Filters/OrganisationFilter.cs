using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Wrappers.Filters
{
    public class OrganisationFilter : PaginationFilter
    {
        public string? Name { get; set; }
        public string? email { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
