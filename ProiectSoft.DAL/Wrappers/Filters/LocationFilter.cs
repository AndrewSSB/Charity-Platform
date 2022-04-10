using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Wrappers.Filters
{
    public class LocationFilter : PaginationFilter
    {
        public string? county { get; set; }
        public string? city { get; set; }
        public string? street { get; set; }
    }
}
