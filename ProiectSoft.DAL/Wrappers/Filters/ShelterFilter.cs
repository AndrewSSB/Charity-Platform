using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Wrappers.Filters
{
    public class ShelterFilter : PaginationFilter
    {
        public string? name { get; set; }
        public int? availableSpace { get; set; }
        public DateTime? dateCreated { get; set; }
    }
}
