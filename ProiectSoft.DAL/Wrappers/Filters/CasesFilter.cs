using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Wrappers.Filters
{
    public class CasesFilter : PaginationFilter
    {
        public string? filterName { get; set; }
        public DateTime? filterDate { get; set; }
    }
}
