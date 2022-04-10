using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Wrappers.Filters
{
    public class RefugeesFilter : PaginationFilter
    {
        public string? lastName { get; set; }
        public int? age { get; set; }
        public string? flag { get; set; }
    }
}
