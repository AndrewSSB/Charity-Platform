using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectSoft.DAL.Wrappers.Filters
{
    public class UserFilter : PaginationFilter
    {
        public string? userName { get; set; }
        public string? firstName { get; set; }
        public DateTime? dateCreated { get; set; }
    }
}
