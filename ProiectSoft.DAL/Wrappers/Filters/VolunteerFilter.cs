using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Wrappers.Filters
{
    public class VolunteerFilter : PaginationFilter
    {
        public string? name { get; set; }
        public string? lastName { get; set; }
        public string? position { get; set; }
        public DateTime? dateCreated { get; set; }
    }
}
